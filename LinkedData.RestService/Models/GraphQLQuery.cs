using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LinkedData.RestService.Models
{
    public class GraphQLQuery
    {
        [JsonProperty("operation_name")]
        public string OperationName { get; set; }
        
        [JsonProperty("named_query")]
        public string NamedQuery { get; set; }

        [JsonProperty("query")]
        public string Query { get; set; }
        
        [JsonProperty("variables")]
        public JObject Variables { get; set; } //https://github.com/graphql-dotnet/graphql-dotnet/issues/389
    }
}