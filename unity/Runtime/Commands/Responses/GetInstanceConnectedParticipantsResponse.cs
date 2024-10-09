using System;
using Dissonity.Events;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    [Serializable]
    internal class GetInstanceConnectedParticipantsResponse : DiscordEvent
    {
        [JsonProperty("data")]
        new public GetInstanceConnectedParticipantsData Data { get; set; }
    }
     
    [Serializable]
    public class GetInstanceConnectedParticipantsData
    {
        [JsonProperty("participants")]
        public Participant[] Participants { get; set; } = new Participant[0];
    }
}