using System;
using Dissonity.Commands.Responses;
using Dissonity.Models.Interop;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal class Close : DiscordCommand
    {
        internal override Opcode Opcode => Opcode.Close;

        [JsonProperty("code")]
        public RpcCloseCode Code { get; set; }
        
        [JsonProperty("message")] 
        public string Message { get; set; }

        [JsonProperty("nonce")] 
        public Guid Nonce => Guid;

        public Close(RpcCloseCode code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}