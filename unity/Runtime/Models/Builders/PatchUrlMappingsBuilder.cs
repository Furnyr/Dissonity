using System;
using Newtonsoft.Json;

namespace Dissonity.Models.Builders
{
    [Serializable]
    public class PatchUrlMappingsConfigBuilder
    {
        #nullable enable annotations

        [JsonProperty("patchFetch")]
        public bool? PatchFetch { get; set; }

        [JsonProperty("patchWebSocket")]
        public bool? PatchWebSocket { get; set; }

        [JsonProperty("patchXhr")]
        public bool? PatchXhr { get; set; }

        [JsonProperty("patchSrcAttributes")]
        public bool? PatchSrcAttributes { get; set; }
    }
}