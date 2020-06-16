using System.Collections.Generic;
using Neo4jClient.Extension.Cypher.Attributes;
using Newtonsoft.Json;

namespace LinkedData.Data.Models
{
    [JsonObject]
    public class Protein
    {
        public int Id { get; set; }

        [JsonProperty("name")]
        [CypherMerge]
        [CypherMatch]
        [CypherMergeOnCreate]
        [CypherMergeOnMatch]
        public string Name { get; set; }

        [JsonProperty("code")]
        [CypherMerge]
        [CypherMatch]
        [CypherMergeOnCreate]
        [CypherMergeOnMatch]
        public string Code { get; set; }

        [JsonProperty("full_name")]
        [CypherMerge]
        [CypherMatch]
        [CypherMergeOnCreate]
        [CypherMergeOnMatch]
        public string FullName { get; set; }
    }
}