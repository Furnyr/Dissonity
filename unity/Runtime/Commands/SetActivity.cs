using System;
using Dissonity.Commands.Responses;
using Dissonity.Models.Builders;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal class SetActivity : FrameCommand<SetActivityResponse>
    {
        internal override string Command => DiscordCommandType.SetActivity;

        [JsonProperty("activity")]
        public ActivityBuilder Activity { get; set; }

        public SetActivity(ActivityBuilder activity)
        {
            Activity = activity;
        }
    }
}