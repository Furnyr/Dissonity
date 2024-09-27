using System;
using Dissonity.Commands.Responses;
using Dissonity.Models.Interop;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal class Subscribe : DiscordCommand<SubscribeResponse>
    {
        #nullable enable annotations

        internal override Opcode Opcode => Opcode.Frame;
     
        [JsonProperty("evt")]
        public string Event { get; set; }
        
        [JsonProperty("nonce")] 
        internal Guid? Nonce => Guid;

        [JsonProperty("cmd")]
        public string Command => DiscordCommandType.Subscribe;

        [JsonProperty("args")] 
        public object? Args { get; set; }


        public Subscribe(string evt)
        {
            Event = evt;
        }
    }
}