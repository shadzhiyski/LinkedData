using Neo4jClient.Extension.Cypher;
using Neo4jClient.Extension.Cypher.Attributes;

namespace LinkedData.Data.Models
{
    [CypherLabel(Name = "taxon")]
    public class ProteinTaxonRelationship : BaseRelationship
    {
        public ProteinTaxonRelationship()
            : base("protein", "taxon")
        { }
    }
}