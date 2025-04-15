using System;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal class GetUser : FrameCommand
    {
        internal override string Command => DiscordCommandType.GetUser;
     
        [JsonProperty("id")]
        public string Id { get; set; }

        public GetUser(string id)
        {
            Id = id;
        }
    }
}