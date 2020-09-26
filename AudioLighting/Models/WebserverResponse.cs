using Newtonsoft.Json;
using System.Collections.Generic;

namespace AudioLighting.Models
{
    public partial class WebserverResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public Value? Value { get; set; }

        [JsonProperty("min", NullValueHandling = NullValueHandling.Ignore)]
        public long? Min { get; set; }

        [JsonProperty("max", NullValueHandling = NullValueHandling.Ignore)]
        public long? Max { get; set; }

        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Options { get; set; }
    }
}
