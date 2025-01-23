
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dissonity.Models.Interop
{
    /// <summary>
    /// Message sent from the JS layer, not the RPC protocol.
    /// </summary>
    [Serializable]
    internal class HiRpcMessage
    {
        #nullable enable annotations
        
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        [JsonProperty("opening")]
        public bool? Opening { get; set; }
    }
}