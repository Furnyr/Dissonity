using System;
using Dissonity.Events;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    [Serializable]
    internal class AuthenticateResponse : DiscordEvent
    {
        [JsonProperty("data")]
        new public AuthenticateData Data { get; set; }
    }

    [Serializable]
    public class AuthenticateData
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("user")]
        public AuthenticatedUser User { get; set; }

        [JsonProperty("scopes")]
        public string[] Scopes { get; set; } = new string[0];

        [JsonProperty("expires")]
        public string Expires { get; set; }
        
        [JsonProperty("application")]
        public Application Application { get; set; }
    }
}