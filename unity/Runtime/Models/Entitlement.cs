using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class Entitlement
    {
        #nullable enable annotations
        
        [JsonProperty("application_id")]
        public string ApplicationId { get; set; }

        [JsonProperty("gift_code_flags")]
        public int GiftCodeFlags { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("sku_id")]
        public string SkuId { get; set; }

        [JsonProperty("type")]
        public EntitlementType Type { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("branches")]
        public string[] Branches { get; set; } = new string[0];
 
        [JsonProperty("consumed")]
        public bool? Consumed { get; set; }

        [JsonProperty("deleted")]
        public bool? Deleted { get; set; }

        /// <summary>
        /// ISO string
        /// </summary>
        [JsonProperty("ends_at")]
        public string EndsAt { get; set; }

        [JsonProperty("gift_code_batch_id")]
        public string? GiftCodeBatchId { get; set; }

        [JsonProperty("gifter_user_id")]
        public string? GifterUserId { get; set; }

        [JsonProperty("parent_id")]
        public string? ParentId { get; set; }

        /// <summary>
        /// ISO string
        /// </summary>
        [JsonProperty("starts_at")]
        public string StartsAt { get; set; }
    }
}