namespace LinkedData.Data.Models
{
    using Neo4jClient.Extension.Cypher;
    using Neo4jClient.Extension.Cypher.Attributes;

    [CypherLabel(Name = "sequence")]
    public class ProteinSequenceRelationship : BaseRelationship
    {
        public ProteinSequenceRelationship()
            : base(typeof(Protein).Name.ToLower(), typeof(Sequence).Name.ToLower())
        { }
    }
}