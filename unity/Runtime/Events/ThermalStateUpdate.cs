using System;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Events
{
    [Serializable]
    internal class ThermalStateUpdate : DiscordEvent
    {
        [JsonProperty("data")]
        new public ThermalStateUpdateData Data { get; set; }
    }

    [Serializable]
    internal class ThermalStateUpdateData
    {
        [JsonProperty("thermal_state")]
        public ThermalStateType ThermalState { get; set; }
    }
}