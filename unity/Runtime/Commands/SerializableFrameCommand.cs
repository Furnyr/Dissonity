using System;
using Dissonity.Events;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal class SerializableFrameCommand<TResponse> where TResponse : DiscordEvent
    {   
        [JsonProperty("cmd")]
        public string Command { get; }
        
        [JsonProperty("nonce")]
        public Guid Nonce { get; }
        
        [JsonProperty("args")]
        public object Payload { get; }

        public SerializableFrameCommand(FrameCommand<TResponse> frameCommand)
        {
            Command = frameCommand.Command;
            Nonce = (Guid) frameCommand.Guid;
            Payload = frameCommand;
        }
    }
}