using Neo4jClient.Extension.Cypher.Attributes;
using Neo4jClient.Extension.Cypher;

namespace LinkedData.Data.Models
{
    [CypherLabel(Name = "protein")]
    public class GeneProteinRelationship : BaseRelationship
    {
        public GeneProteinRelationship()
            : base(typeof(Gene).Name.ToLower(), typeof(Protein).Name.ToLower())
        { }
    }
}