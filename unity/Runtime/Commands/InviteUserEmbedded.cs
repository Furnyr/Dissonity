using System;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal class InviteUserEmbedded : FrameCommand
    {
        #nullable enable annotations
        
        internal override string Command => DiscordCommandType.InviteUserEmbedded;
     
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public string? Content { get; set; }

        public InviteUserEmbedded(string userId, string? content)
        {
            UserId = userId;
            Content = content;
        }
    }
}