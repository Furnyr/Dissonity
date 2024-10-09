using System;
using Dissonity.Models.Interop;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal class Unsubscribe : DiscordCommand
    {
        #nullable enable annotations

        internal override Opcode Opcode => Opcode.Frame;

        [JsonProperty("evt")]
        public string Event { get; set; }
        
        [JsonProperty("nonce")] 
        internal Guid? Nonce => Guid;
        
        [JsonProperty("cmd")]
        public string Command => DiscordCommandType.Unsubscribe;
        
        [JsonProperty("args")] 
        public object? Args { get; set; }

     
        public Unsubscribe(string evt)
        {
            Event = evt;
        }
    }
}