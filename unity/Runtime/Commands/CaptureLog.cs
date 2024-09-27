using System;
using Dissonity.Commands.Responses;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal class CaptureLog : FrameCommand<NoResponse>
    {
        internal override string Command => DiscordCommandType.CaptureLog;

        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("level")]
        public string Level { get; set; }

        public CaptureLog(string level, string message)
        {
            Message = message;
            Level = level;
        }
    }
}