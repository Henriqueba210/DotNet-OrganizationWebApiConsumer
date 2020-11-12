using System.Text.Json.Serialization;

namespace Consumer.Api.Models
{
    public class Repository
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}