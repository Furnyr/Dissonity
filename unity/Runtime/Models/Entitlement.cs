using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    //todo Not released yet. Some models related to this functionality may not be implemented.
    [Serializable]
    internal class Entitlement
    {
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

        [JsonProperty("branches", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Branches { get; set; }
 
        [JsonProperty("consumed")]
        public bool? Consumed { get; set; }

        [JsonProperty("deleted")]
        public bool? Deleted { get; set; }

        [JsonProperty("ends_at")]
        public string EndsAtString { get; set; }

        [JsonProperty("gift_code_batch_id")]
        public string GiftCodeBatchId { get; set; }

        [JsonProperty("gifter_user_id")]
        public string GifterUserId { get; set; }

        [JsonProperty("parent_id")]
        public string ParentId { get; set; }

        [JsonProperty("starts_at")]
        public string StartsAtString { get; set; }
    }
}