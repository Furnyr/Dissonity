using System;
using Dissonity.Events;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    // Discord can allow themselves to call a "participant" and a "user" the same way, but C# strong typing makes this a pain.
    // This way, users will know when they can expect the nickname to be there, rather than accessing <User>.Nickname
    // and it always being null.
    [Serializable]
    public class Participant : User
    {
        #nullable enable annotations

        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        [JsonIgnore]
        new public string? DisplayName
        {
            get
            {
                if (Nickname != null) return Nickname;

                if (GlobalName != null) return GlobalName;

                return Username;
            }
        }
    }
}