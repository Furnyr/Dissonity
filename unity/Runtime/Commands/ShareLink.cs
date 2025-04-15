using System;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal class ShareLink : FrameCommand
    {
        #nullable enable
        
        internal override string Command => DiscordCommandType.ShareLink;

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("custom_id", NullValueHandling = NullValueHandling.Ignore)]
        public string? CustomId { get; set; }

        [JsonProperty("link_id", NullValueHandling = NullValueHandling.Ignore)]
        public string? LinkId { get; set; }

        public ShareLink(string message, string? customId, string? linkId)
        {
            Message = message;
            CustomId = customId;
            LinkId = linkId;
        }
    }
}