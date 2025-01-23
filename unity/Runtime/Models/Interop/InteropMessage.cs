
using System;
using Newtonsoft.Json;

namespace Dissonity.Models.Interop
{
    /// <summary>
    /// Data received from outside Unity, through hiRPC. <br/> <br/>
    /// The opposite of <c> BridgeMessage </c>.
    /// </summary>
    [Serializable]
    internal class InteropMessage
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