using System;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Events
{
    [Serializable]
    internal class ActivityInstanceParticipantsUpdate : DiscordEvent
    {
        [JsonProperty("data")]
        new public ActivityInstanceParticipantsUpdateData Data { get; set; }
    }

    [Serializable]
    internal class ActivityInstanceParticipantsUpdateData 
    {
        [JsonProperty("participants")]
        public Participant[] Participants { get; set; } = new Participant[0];
    }
}