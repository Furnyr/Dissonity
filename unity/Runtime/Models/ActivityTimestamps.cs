using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class ActivityTimestamps
    {
        [JsonProperty("start")]
        public long? Start { get; set; }

        [JsonProperty("end")]
        public long? End { get; set; }
    }
}