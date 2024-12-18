
using System;
using Newtonsoft.Json;

namespace Dissonity.Models.Interop
{
    /// <summary>
    /// Message sent from the JS level, not the RPC protocol.
    /// </summary>
    [Serializable]
    internal class HiRpcMessage
    {
        #nullable enable annotations
        
        [JsonProperty("action_code")]
        public ActionCode ActionCode { get; set; }

        [JsonProperty("data")]
        public HiRpcData Data { get; set; }
    }
}