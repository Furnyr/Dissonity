using System;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Events
{
    [Serializable]
    internal class OrientationUpdate : DiscordEvent
    {
        [JsonProperty("data")]
        new public OrientationUpdateData Data { get; set; }
    }
    
    [Serializable]
    internal class OrientationUpdateData
    {
        [JsonProperty("screen_orientation")]
        public OrientationType ScreenOrientation { get; set; }
    }
}