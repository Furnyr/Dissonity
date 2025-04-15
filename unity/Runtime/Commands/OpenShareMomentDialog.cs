using System;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal class OpenShareMomentDialog : FrameCommand
    {
        internal override string Command => DiscordCommandType.OpenShareMomentDialog;
    
        [JsonProperty("mediaUrl")]
        public string MediaUrl { get; set; }

        public OpenShareMomentDialog(string mediaUrl)
        {
            MediaUrl = mediaUrl;
        }
    }
}