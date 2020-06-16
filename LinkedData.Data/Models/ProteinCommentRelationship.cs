using Neo4jClient.Extension.Cypher;
using Neo4jClient.Extension.Cypher.Attributes;

namespace LinkedData.Data.Models
{
    [CypherLabel(Name = "comment")]
    public class ProteinCommentRelationship : BaseRelationship
    {
        public ProteinCommentRelationship()
            : base(typeof(Protein).Name.ToLower(), typeof(Comment).Name.ToLower())
        { }
    }
}