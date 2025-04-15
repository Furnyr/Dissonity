using System;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Events
{
    [Serializable]
    internal class ActivityLayoutModeUpdate : DiscordEvent
    {
        [JsonProperty("data")]
        new public ActivityLayoutModeUpdateData Data { get; set; }
    }

    [Serializable]
    internal class ActivityLayoutModeUpdateData
    {
        [JsonProperty("layout_mode")]
        public LayoutModeType LayoutMode { get; set; }
    }
}