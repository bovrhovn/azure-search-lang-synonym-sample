using System;
using System.Text.Json.Serialization;

namespace ASDemo.Console
{
    public class SearchModel
    {
        [JsonPropertyName("uniqueId")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("polishName")]
        public string PolishName { get; set; }

        [JsonPropertyName("updated")]
        public DateTime Updated { get; set; }

        [JsonPropertyName("keyPhrases")]
        public string[] KeyPhrases { get; set; }
    }
}