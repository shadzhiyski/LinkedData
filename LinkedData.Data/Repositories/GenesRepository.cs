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
        public GenesRepository(Neo4jDataService neo4jService)
            :base("Gene", neo4jService)
        { }

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
            var relatedData = EbiDataService.Instance.GetGeneRelatedData(item.Name);
            
            foreach (var relatedItem in relatedData)
            {
                var protein = new Protein()
                { 
                    Name = relatedItem["id"].ToString(), 
                    Code = relatedItem["accession"].ToString(),
                    FullName = relatedItem["protein"]
                        ?["submittedName"].FirstOrDefault()
                        ?["fullName"]
                        ?["value"].ToString()
                };

                var relatedGenes = relatedItem["gene"].Select(jt => 
                    new Gene() 
                    { 
                        Name = jt["name"]["value"].ToString()
                    }
                );
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

                foreach (var gene in relatedGenes)
                {
                    var t = _neo4jService.Client.Cypher.MergeEntity(gene)
                        .MergeEntity(protein)
                        .Merge("(gene)-[rel:protein]->(protein)")
                        .MergeRelationship(new GeneProteinRelationship()).Query.DebugQueryText;
                    _neo4jService.Client.Cypher.MergeEntity(gene)
                        .MergeEntity(protein)
                        //.Merge("(gene)-[rel:protein]->(protein)")
                        .MergeRelationship(new GeneProteinRelationship())
                        .ExecuteWithoutResults();
                }

                foreach (var sequence in relatedSequences)
                {
                    _neo4jService.Client.Cypher.MergeEntity(protein)
                        .MergeEntity(sequence)
                        //.Merge("(protein)-[rel:sequence]->(sequence)")
                        .MergeRelationship(new ProteinSequenceRelationship())
                        .ExecuteWithoutResults();
                }

                foreach (var taxon in relatedTaxons)
                {
                    _neo4jService.Client.Cypher.MergeEntity(protein)
                        .MergeEntity(taxon)
                        //.Merge("(protein)-[rel:taxon]->(taxon)")
                        .MergeRelationship(new ProteinTaxonRelationship())
                        .ExecuteWithoutResults();
                }

                foreach (var comment in relatedComments)
                {
                    _neo4jService.Client.Cypher.MergeEntity(protein)
                        .MergeEntity(comment)
                        //.Merge("(protein)-[rel:comment]->(comment)")
                        .MergeRelationship(new ProteinCommentRelationship())
                        .ExecuteWithoutResults();
                }
            }
        }
    }
}