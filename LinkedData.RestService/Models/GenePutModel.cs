namespace LinkedData.RestService.Models
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    [JsonObject]
    public class GenePutModel
    {
        [Required]
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}