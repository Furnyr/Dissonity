
using System;
using Newtonsoft.Json;

namespace Dissonity.Models.Interop
{
    [Serializable]
    internal class BridgeMessage<T>
    {
        #nullable enable annotations
        
        [JsonProperty("nonce")]
        public string? Nonce { get; set; }

        [JsonProperty("payload")]
        public T Payload { get; set; }
    }
}