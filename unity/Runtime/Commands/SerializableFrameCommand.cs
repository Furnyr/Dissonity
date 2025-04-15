using System;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal class SerializableFrameCommand
    {   
        [JsonProperty("cmd")]
        public string Command { get; }
        
        [JsonProperty("nonce")]
        public Guid Nonce { get; }
        
        [JsonProperty("args")]
        public object Payload { get; }

        public SerializableFrameCommand(FrameCommand frameCommand)
        {
            Command = frameCommand.Command;
            Nonce = frameCommand.Guid;
            Payload = frameCommand;
        }
    }
}