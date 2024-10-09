using System;
using Dissonity.Events;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    [Serializable]
    internal class GetSkusResponse : DiscordEvent
    {
        [JsonProperty("data")]
        new public GetSkusData Data { get; set; }
    }
     
    [Serializable]
    public class GetSkusData
    {
        [JsonProperty("skus")]
        public Sku[] Skus { get; set; } = new Sku[0];
    }
}