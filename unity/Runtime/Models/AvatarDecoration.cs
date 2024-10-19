using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class AvatarDecoration
    {
        #nullable enable annotations

        [JsonProperty("asset")]
        public string Asset { get; set; }

        [JsonProperty("skuId")]
        public long? SkuId { get; set; }
    }
}