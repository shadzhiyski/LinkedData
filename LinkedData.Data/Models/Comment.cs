using Neo4jClient.Extension.Cypher.Attributes;
using Newtonsoft.Json;

namespace LinkedData.Data.Models
{
    [JsonObject]
    public class Comment
    {
        public int Id { get; set; }
        
        [JsonProperty("text")]
        [CypherMerge]
        [CypherMatch]
        [CypherMergeOnCreate]
        [CypherMergeOnMatch]
        public string Text { get; set; }

        [JsonProperty("type")]
        [CypherMerge]
        [CypherMatch]
        [CypherMergeOnCreate]
        [CypherMergeOnMatch]
        public string Type { get; set; }
    }
}