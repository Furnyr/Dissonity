using System;
using Dissonity.Commands.Responses;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal class SetConfig : FrameCommand<SetConfigResponse>
    {
        internal override string Command => DiscordCommandType.SetActivity;

        [JsonProperty("use_interactive_pip")]
        public bool UseInteractivePip { get; set; }

        public SetConfig(bool useInteractivePip)
        {
            UseInteractivePip = useInteractivePip;
        }
    }
}