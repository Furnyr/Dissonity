using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class Application
    {
        #nullable enable annotations

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("icon")]
        public string? Icon { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("rpc_origins")]
        public string? RpcOrigins { get; set; }
    }
}