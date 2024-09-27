using Dissonity.Events;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    internal class GetInstanceConnectedParticipantsResponse : DiscordEvent
    {
        [JsonProperty("data")]
        new public GetInstanceConnectedParticipantsData Data { get; set; }
    }
     
    public class GetInstanceConnectedParticipantsData
    {
        [JsonProperty("participants")]
        public Participant[] Participants { get; set; }
    }
}