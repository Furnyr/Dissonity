using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    /// <summary>
    /// This class is the parent for all the user-like structures. <br/> <br/>
    /// It's also the "user" object sent on start.
    /// </summary>
    [Serializable]
    public class BaseUser
    {
        #nullable enable annotations

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("avatar")]
        public string? Avatar { get; set; }
    }
}