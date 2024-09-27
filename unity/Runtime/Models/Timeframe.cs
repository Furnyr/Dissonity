using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class Timeframe
    {
        [JsonProperty("start")]
        public int? Start { get; set; }

        [JsonProperty("end")]
        public int? End { get; set; }
    }
}