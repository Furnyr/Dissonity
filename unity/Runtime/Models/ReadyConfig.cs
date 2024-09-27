using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class ReadyConfig
    {
        #nullable enable annotations

        [JsonProperty("cdn_host")]
        public string? CdnHost { get; set; }

        [JsonProperty("api_endpoint")]
        public string ApiEndpoint { get; set; }

        [JsonProperty("environment")]
        public string Environment { get; set; }
    }
}