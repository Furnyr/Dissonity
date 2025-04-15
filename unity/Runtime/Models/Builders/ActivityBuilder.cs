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

        [JsonProperty("timestamps", NullValueHandling = NullValueHandling.Ignore)]
        public ActivityTimestamps? Timestamps { get; set; }

        [JsonProperty("details", NullValueHandling = NullValueHandling.Ignore)]
        public string? Details { get; set; }

        [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
        public string? State { get; set; }
        
        [JsonProperty("party", NullValueHandling = NullValueHandling.Ignore)]
        public ActivityParty? Party { get; set; }

        [JsonProperty("assets", NullValueHandling = NullValueHandling.Ignore)]
        public ActivityAssets? Assets { get; set; }
        
        [JsonProperty("secrets", NullValueHandling = NullValueHandling.Ignore)]
        public ActivitySecrets? Secrets { get; set; }
        
        [JsonProperty("instance", NullValueHandling = NullValueHandling.Ignore)]
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