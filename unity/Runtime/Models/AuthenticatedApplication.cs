using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class AuthenticatedApplication
    {
        #nullable enable annotations

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("icon")]
        public string? Icon { get; set; }

        [JsonProperty("rpc_origins")]
        public string? RpcOrigins { get; set; }
    }
}