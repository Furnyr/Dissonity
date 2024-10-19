using System;
using Newtonsoft.Json;

namespace Dissonity.Models.Builders
{
    /// <summary>
    /// More information about rich presence: https://discord.com/developers/docs/rich-presence/using-with-the-embedded-app-sdk#custom-rich-presence-data
    /// </summary>
    [Serializable]
    public class ActivityBuilder
    {
        #nullable enable annotations

        [JsonProperty("type")]
        public ActivityType Type { get; set; }

        [JsonProperty("timestamps")]
        public Timeframe? Timestamps { get; set; }

        [JsonProperty("details")]
        public string? Details { get; set; }

        [JsonProperty("state")]
        public string? State { get; set; }
        
        [JsonProperty("party")]
        public ActivityParty? Party { get; set; }

        [JsonProperty("assets")]
        public ActivityAssets? Assets { get; set; }
        
        [JsonProperty("secrets")]
        public ActivitySecrets? Secrets { get; set; }
        
        [JsonProperty("instance")]
        public bool? Instance { get; set; }

        internal Activity ToActivity()
        {
            return new Activity()
            {
                Name = "Activity Name",
                Type = Type,
                Timestamps = Timestamps,
                Details = Details,
                State = State,
                Party = Party,
                Assets = Assets,
                Secrets = Secrets,
                Instance = Instance
            };
        }
    }
}