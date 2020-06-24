using System;
namespace LinkedData.Data.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using LinkedData.Data.External;
    using LinkedData.Data.Models;
    using Neo4jClient.Extension.Cypher;

    public class GenesRepository : BaseRepository<Gene>
    {
        public EbiDataService EbiDataService { get; }

        public EnsemblDataService EnsemblDataService { get; }

        public GenesRepository(
            Neo4jDataService neo4jService,
            EbiDataService ebiDataService,
            EnsemblDataService ensemblDataService)
            :base("Gene", neo4jService)
        {
            EbiDataService = ebiDataService;
            EnsemblDataService = ensemblDataService;
        }

        public IEnumerable<(string Gene, IEnumerable<Variation> Variations, IEnumerable<Protein> Proteins)> GetTree()
        {
            var client = _neo4jService.Client;

            var searchedItems = client.Cypher.MatchRelationship(new GeneVariationRelationship())
                .MatchRelationship(new GeneProteinRelationship())
                .Return((gene, variation, protein) => new 
                { 
                    Gene = gene.As<Gene>(),
                    Variations = variation.CollectAsDistinct<Variation>(),
                    Proteins = protein.CollectAsDistinct<Protein>()
                })
                .Results.Select(d => (d.Gene.Name, d.Variations, d.Proteins));
                
            return searchedItems;
        }

        public IEnumerable<(string Gene, IEnumerable<Variation> Variations, IEnumerable<Protein> Proteins)> GetTree(Expression<Func<Gene, bool>> filter)
        {
            var client = _neo4jService.Client;

            var searchedItems = client.Cypher.MatchRelationship(new GeneVariationRelationship())
                .MatchRelationship(new GeneProteinRelationship())
                .Where(filter)
                .Return((gene, variation, protein) => new 
                {
                    Gene = gene.As<Gene>(),
                    Variations = variation.CollectAsDistinct<Variation>(),
                    Proteins = protein.CollectAsDistinct<Protein>()
                })
                .Results.Select(d => (Gene: d.Gene.Name, d.Variations, d.Proteins));
            
            return searchedItems;
        }

        public override void PutRelated(Gene item)
        {
            PutProteinsAndRelatedData(item.Name);

            PutVariations(item.Name);
        }

        private void PutProteinsAndRelatedData(string geneName)
        {
            var gene = new Gene()
            {
                Name = geneName
            };

            var relatedData = EbiDataService.GetGeneRelatedData(geneName);
            foreach (var relatedItem in relatedData)
            {
                if (relatedItem["id"] == null
                    || relatedItem["accession"] == null
                    || relatedItem["protein"] == null)
                {
                    continue;
                }

                var protein = new Protein()
                { 
                    Name = relatedItem["id"].ToString(), 
                    Code = relatedItem["accession"].ToString(),
                    FullName = relatedItem["protein"]
                        ?["submittedName"]?.FirstOrDefault()
                        ?["fullName"]
                        ?["value"]
                        ?.ToString()
                        ?? string.Empty
                };

                _neo4jService.Client.Cypher.MergeEntity(gene)
                    .MergeEntity(protein)
                    .MergeRelationship(new GeneProteinRelationship())
                    .ExecuteWithoutResults();
                
                var relatedSequences = new Sequence[]
                { 
                    new Sequence() 
                    { 
                        Content = relatedItem["sequence"]["sequence"].ToString()
                    }
                };
                var relatedTaxons = relatedItem["organism"]["lineage"].Select(jt => 
                    new Taxon() 
                    {
                        Name = jt.ToString()
                    }
                );
                var relatedComments = relatedItem["comments"] != null ? relatedItem["comments"].Select(jt => 
                    jt["text"] != null ? jt["text"].Select(jtt =>
                        new Comment() 
                        {
                            Text = jtt["value"].ToString(),
                            Type = jt["type"].ToString()
                        }) : new Comment[] {}
                ).SelectMany(c => c) : new Comment[] {};

                foreach (var sequence in relatedSequences)
                {
                    _neo4jService.Client.Cypher.MergeEntity(protein)
                        .MergeEntity(sequence)
                        .MergeRelationship(new ProteinSequenceRelationship())
                        .ExecuteWithoutResults();
                }

                foreach (var taxon in relatedTaxons)
                {
                    _neo4jService.Client.Cypher.MergeEntity(protein)
                        .MergeEntity(taxon)
                        .MergeRelationship(new ProteinTaxonRelationship())
                        .ExecuteWithoutResults();
                }

                foreach (var comment in relatedComments)
                {
                    _neo4jService.Client.Cypher.MergeEntity(protein)
                        .MergeEntity(comment)
                        .MergeRelationship(new ProteinCommentRelationship())
                        .ExecuteWithoutResults();
                }
            }
        }

        private async void PutVariations(string geneName)
        {
            var gene = new Gene()
            {
                Name = geneName
            };

            var variationsJsonArray = await EnsemblDataService.GetGeneVariationsAsync(geneName);
            foreach (var variationJson in variationsJsonArray)
            {
                var variation = new Variation()
                { 
                    Location = variationJson["location"].ToString(),
                    Description = variationJson["description"].ToString()
                };

                _neo4jService.Client.Cypher.MergeEntity(gene)
                    .MergeEntity(variation)
                    .MergeRelationship(new GeneVariationRelationship())
                    .ExecuteWithoutResults();
            }
        }
    }
}