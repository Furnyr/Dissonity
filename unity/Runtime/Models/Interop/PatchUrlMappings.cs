using System;
using Dissonity.Models.Builders;
using Newtonsoft.Json;

namespace Dissonity.Models.Interop
{
    [Serializable]
    internal class PatchUrlMappings
    {
        [JsonProperty("mappings", NullValueHandling = NullValueHandling.Include)]
        public MappingBuilder[] Mappings { get; set; }

        [JsonProperty("config")]
        public PatchUrlMappingsConfigBuilder Config { get; set; }
    }
}