using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class SkuPrice
    {
        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }
}