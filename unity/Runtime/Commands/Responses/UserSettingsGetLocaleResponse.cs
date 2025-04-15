using System;
using Dissonity.Events;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    [Serializable]
    internal class UserSettingsGetLocaleResponse : DiscordEvent
    {
        [JsonProperty("data")]
        new public UserSettingsGetLocaleData Data { get; set; }
    }

    [Serializable]
    public class UserSettingsGetLocaleData
    {
        [JsonProperty("locale")]
        public string Locale { get; set; }
    }
}