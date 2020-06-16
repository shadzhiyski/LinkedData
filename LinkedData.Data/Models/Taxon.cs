using Neo4jClient.Extension.Cypher.Attributes;
using Newtonsoft.Json;

namespace LinkedData.Data.Models
{
    [JsonObject]
    public class Taxon
    {
        public int Id { get; set; }
        
        [JsonProperty("name")]
        [CypherMerge]
        [CypherMatch]
        [CypherMergeOnCreate]
        [CypherMergeOnMatch]
        public string Name { get; set; }
    }
}