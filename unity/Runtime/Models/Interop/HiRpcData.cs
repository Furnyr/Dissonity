
using System;
using Newtonsoft.Json;

namespace Dissonity.Models.Interop
{
    /// <summary>
    /// Data received within rpc_message.data in a normal connection.
    /// </summary>
    [Serializable]
    internal class HiRpcData
    {
        #nullable enable annotations

        [JsonProperty("channel")]
        public string? Channel { get; set; }

        [JsonProperty("hash")]
        public string? Hash { get; set; }
        
        [JsonProperty("nonce")]
        public string? Nonce { get; set; }

        [JsonProperty("raw_multi_event")]
        public RawMultiEvent? RawMultiEvent { get; set; }

        [JsonProperty("query")]
        public string? Query { get; set; }

        [JsonProperty("formatted_price")]
        public string? FormattedPrice { get; set; }
    }
}