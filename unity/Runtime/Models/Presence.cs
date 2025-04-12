using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class Presence
    {
        #nullable enable annotations

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("activity")]
        public Activity? Activity { get; set; }
    }
}