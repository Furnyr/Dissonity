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

        [JsonProperty("referrer_id", NullValueHandling = NullValueHandling.Ignore)]
        public string? ReferrerId { get; set; }

        public ShareLink(string message, string? customId, string? referrerId)
        {
            Message = message;
            CustomId = customId;
            ReferrerId = referrerId;
        }
    }
}