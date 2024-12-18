
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dissonity.Models.Interop
{
    /// <summary>
    /// Data received from hiRPC.
    /// </summary>
    [Serializable]
    internal class AppPayload
    {
        #nullable enable annotations
        
        [JsonProperty("hirpc_state")]
        public StateCode HiRpcState { get; set; }

        /// <summary>
        /// Represents data received from Discord.
        /// <code>
        /// [Opcode, Payload]
        /// </code>
        /// </summary>
        [JsonProperty("rpc_message")]
        public object[]? RpcMessage { get; set; }

        [JsonProperty("hirpc_message")]
        public HiRpcMessage? HiRpcMessage { get; set; }
    }
}