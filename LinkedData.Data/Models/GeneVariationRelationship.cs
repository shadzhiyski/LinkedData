using Neo4jClient.Extension.Cypher.Attributes;
using Neo4jClient.Extension.Cypher;

namespace LinkedData.Data.Models
{
    [CypherLabel(Name = "variation")]
    public class GeneVariationRelationship : BaseRelationship
    {
        public GeneVariationRelationship()
            : base(typeof(Gene).Name.ToLower(), typeof(Variation).Name.ToLower())
        { }
    }
}