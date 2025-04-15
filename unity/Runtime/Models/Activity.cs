using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class Activity
    {
        #nullable enable annotations

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public ActivityType Type { get; set; }
        
        [JsonProperty("url")]
        public string? Url { get; set; }
        
        [JsonProperty("created_at")]
        public long? CreatedAt { get; set; }
        
        [JsonProperty("timestamps")]
        public ActivityTimestamps? Timestamps { get; set; }
        
        [JsonProperty("application_id")]
        public long? ApplicationId { get; set; }
        
        [JsonProperty("details")]
        public string? Details { get; set; }

        [JsonProperty("state")]
        public string? State { get; set; }
        
        [JsonProperty("emoji")]
        public Emoji? Emoji { get; set; }
        
        [JsonProperty("party")]
        public ActivityParty? Party { get; set; }
        
        [JsonProperty("assets")]
        public ActivityAssets? Assets { get; set; }
        
        [JsonProperty("secrets")]
        public ActivitySecrets? Secrets { get; set; }
        
        [JsonProperty("instance")]
        public bool? Instance { get; set; }
        
        [JsonProperty("flags")]
        public long? Flags { get; set; }

        // In <Relationship>.presence.activity, there are more fields,
        // but there's not much documentation around them and seem to be related
        // to the Social SDK. I'm choosing not to create another model like PresenceActivity.
    }
}