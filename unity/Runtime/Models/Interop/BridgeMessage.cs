
using System;
using Newtonsoft.Json;

namespace Dissonity.Models.Interop
{
    /// <summary>
    /// Data sent to hiRPC.
    /// </summary>
    [Serializable]
    internal class BridgeMessage
    {
        #nullable enable annotations
        
        [JsonProperty("nonce")]
        public string? Nonce { get; set; }

        [JsonProperty("stringified_data")]
        public string? StringifiedData { get; set; }
    }
}