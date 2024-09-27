using System;
using Dissonity.Commands.Responses;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal class GetChannel : FrameCommand<GetChannelResponse>
    {
        internal override string Command => DiscordCommandType.GetChannel;
     
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        public GetChannel(string channelId)
        {
            ChannelId = channelId;
        }
    }
}