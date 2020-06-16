using GraphQL.Types;
using LinkedData.Data.Models;
using Neo4jClient.Extension.Cypher.Attributes;

namespace LinkedData.RestService.Models.GraphQL
{
    public class GeneType : ObjectGraphType<Gene>
    {
        public GeneType()
        {
            Field(g => g.Name);
        }
    }
}