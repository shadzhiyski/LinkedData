using System;
using System.Linq;
using LinkedData.Data.Models;
using Neo4jClient;
using Neo4jClient.Extension.Cypher;
using Newtonsoft.Json.Linq;

namespace LinkedData.Data.External
{
    public class Neo4jDataService
    {
        public static readonly string ServerAddress = "bolt://localhost:7687";

        private static readonly string User = "neo4j";
        
        private static readonly string Password = "neo4j123";

        public Neo4jDataService()
        {
            Client = new BoltGraphClient(new Uri(ServerAddress),
                User, Password);

            Client.Connect();
        }
        
        public BoltGraphClient Client { get; private set; }

        public void Init()
        {
            FluentConfig.Config()
                .With<Gene>("Gene")
                .MergeOnCreate(gene => gene.Name)
                .MergeOnMatch(gene => gene.Name)
                .MergeOnMatchOrCreate(gene => gene.Name)
                .Set();
            
            FluentConfig.Config()
                .With<Variation>("Variation")
                .MergeOnCreate(variation => variation.Location)
                .MergeOnCreate(variation => variation.Description)
                .MergeOnMatch(variation => variation.Location)
                .MergeOnMatch(variation => variation.Description)
                .MergeOnMatchOrCreate(variation => variation.Location)
                .MergeOnMatchOrCreate(variation => variation.Description)
                .Set();

            FluentConfig.Config()
                .With<Protein>("Protein")
                .MergeOnCreate(protein => protein.Name)
                .MergeOnCreate(protein => protein.FullName)
                .MergeOnCreate(protein => protein.Code)
                .MergeOnMatch(protein => protein.Name)
                .MergeOnMatch(protein => protein.FullName)
                .MergeOnMatch(protein => protein.Code)
                .MergeOnMatchOrCreate(protein => protein.Name)
                .MergeOnMatchOrCreate(protein => protein.FullName)
                .MergeOnMatchOrCreate(protein => protein.Code)
                .Set();

            FluentConfig.Config()
                .With<Comment>("Comment")
                .MergeOnCreate(comment => comment.Text)
                .MergeOnCreate(comment => comment.Type)
                .MergeOnMatch(comment => comment.Text)
                .MergeOnMatch(comment => comment.Type)
                .MergeOnMatchOrCreate(comment => comment.Text)
                .MergeOnMatchOrCreate(comment => comment.Type)
                .Set();

            FluentConfig.Config()
                .With<Taxon>("Taxon")
                .MergeOnCreate(taxon => taxon.Name)
                .MergeOnMatch(taxon => taxon.Name)
                .MergeOnMatchOrCreate(taxon => taxon.Name)
                .Set();

            FluentConfig.Config()
                .With<Sequence>("Sequence")
                .MergeOnCreate(sequence => sequence.Content)
                .MergeOnMatch(sequence => sequence.Content)
                .MergeOnMatchOrCreate(sequence => sequence.Content)
                .Set();
        }

        public void LinkGeneWithProteins(string geneName)
        {
            string requestInfo;
            var result = UniProtDataService.Instance.MapIds("GENENAME", "ID", new string[] { geneName }, out requestInfo);
            var client = new GraphClient(new Uri(ServerAddress), "neo4j", Password);

            client.Connect();

            var gene = client.Cypher.Match("(g:Gene)")
                .Where(string.Format("g.Name = \"{0}\"", geneName))
                .Return(g => g.As<Gene>())
                .Results.FirstOrDefault();
            if (gene == null)
            {
                gene = new Gene() { Name = geneName };
                client.Cypher.Create("(g:Gene{newGene})")
                    .WithParam("newGene", gene)
                    .ExecuteWithoutResults();
            }

            foreach (var proteinId in result.Select(kv => kv.Value))
            {
                var protein = new Protein() { Name = proteinId };
                client.Cypher.Create("(g:Protein{newProtein})")
                    .WithParam("newProtein", protein)
                    .ExecuteWithoutResults();

                client.Cypher.Match("(g:Gene), (p:Protein)")
                    .Where(string.Format("g.name = \"{0}\"", geneName))
                    .AndWhere(string.Format("p.id = \"{0}\"", proteinId))
                    .Create("(g)-[rel:protein]->(p)")
                    .ExecuteWithoutResults();
            }
        }
    }
}