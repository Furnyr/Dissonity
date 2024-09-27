using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class Participant : User
    {
        #nullable enable annotations

        [JsonProperty("nickname")]
        public string? Nickname { get; set; }
    }
}