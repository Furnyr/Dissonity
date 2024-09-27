using System;
using Dissonity.Commands.Responses;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal class OpenExternalLink : FrameCommand<NoResponse>
    {
        internal override string Command => DiscordCommandType.OpenExternalLink;

        [JsonProperty("url")]
        public string Url { get; set; }

        public OpenExternalLink(string url)
        {
            Url = url;
        }
    }
}