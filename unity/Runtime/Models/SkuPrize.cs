using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    //todo Not released yet. Some models related to this functionality may not be implemented.
    [Serializable]
    internal class SkuPrize
    {
        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }
}