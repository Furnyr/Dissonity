using System;
using Newtonsoft.Json;

namespace Dissonity.Models.Builders
{
    [Serializable]
    public class MappingBuilder
    {
        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("target")]
        public string Target { get; set; }
    }
}