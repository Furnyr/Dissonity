using System;
using Dissonity.Models.Interop;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    /// <summary>
    /// Old implementation, now the handshake is handled by the RpcBridge. <br/> <br/>
    /// Still keeping this file around just in case
    /// </summary>
    [Serializable]
    internal class HandshakeCommand : DiscordCommand
    {
        internal override Opcode Opcode => Opcode.Handshake;

        internal override Guid? Guid => null;

        [JsonProperty("v")]
        public int Version { get; set; }

        [JsonProperty("encoding")]
        public string Encoding { get; set; }
        
        [JsonProperty("client_id")] 
        public string ClientId { get; set; }
        
        [JsonProperty("frame_id")]
        public string FrameId { get; set; }

        public HandshakeCommand(int version, string encoding, string clientId, string frameId)
        {
            Version = version;
            Encoding = encoding;
            ClientId = clientId;
            FrameId = frameId;
        }
    }
}