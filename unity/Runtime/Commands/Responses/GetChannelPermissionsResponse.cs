using System;
using Dissonity.Events;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    [Serializable]
    internal class GetChannelPermissionsResponse : DiscordEvent
    {
        [JsonProperty("data")]
        new public GetChannelPermissionsData Data { get; set; }
    }
    
    [Serializable]
    public class GetChannelPermissionsData
    {
        #nullable enable annotations

        [JsonProperty("permissions")]
        public long? Permissions { get; set; }
    }
}