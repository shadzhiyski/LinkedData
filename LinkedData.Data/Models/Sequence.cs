using Neo4jClient.Extension.Cypher.Attributes;
using Newtonsoft.Json;

namespace LinkedData.Data.Models
{
    [JsonObject]
    public class Sequence
    {
        public int Id { get; set; }
        
        [JsonProperty("content")]
        [CypherMerge]
        [CypherMatch]
        [CypherMergeOnCreate]
        [CypherMergeOnMatch]
        public string Content { get; set; }
    }
}