using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class Entitlement
    {
        #nullable enable annotations
        
        [JsonProperty("application_id")]
        public long ApplicationId { get; set; }

        [JsonProperty("gift_code_flags")]
        public long GiftCodeFlags { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("sku_id")]
        public long SkuId { get; set; }

        [JsonProperty("type")]
        public EntitlementType Type { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }
 
        [JsonProperty("consumed")]
        public bool? Consumed { get; set; }

        [JsonProperty("deleted")]
        public bool? Deleted { get; set; }

        /// <summary>
        /// ISO string
        /// </summary>
        [JsonProperty("ends_at")]
        public string? EndsAt { get; set; }

        [JsonProperty("gift_code_batch_id")]
        public string? GiftCodeBatchId { get; set; }

        [JsonProperty("gifter_user_id")]
        public long? GifterUserId { get; set; }

        /// <summary>
        /// ISO string
        /// </summary>
        [JsonProperty("starts_at")]
        public string? StartsAt { get; set; }

        // These properties are supported in the official SDK,
        // but I don't have enough information on how to maintain them.

        // [JsonProperty("parent_id")]
        // public string? ParentId { get; set; }

        // [JsonProperty("branches")]
        // public string[] Branches { get; set; } = new string[0];
    }
}