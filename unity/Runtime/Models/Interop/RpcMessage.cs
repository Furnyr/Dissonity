using System;
using Newtonsoft.Json;

namespace Dissonity.Models.Interop
{
    [Serializable]
    internal class RpcMessage
    {
        /// <summary>
        /// Represents data received from Discord.
        /// <code>
        /// [Opcode, Payload]
        /// </code>
        /// </summary>
        [JsonProperty("data")]
        public object[] Data { get; set; }
    }
}