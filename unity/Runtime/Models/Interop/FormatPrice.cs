using System;
using Newtonsoft.Json;

namespace Dissonity.Models.Interop
{
    [Serializable]
    internal class FormatPrice
    {
        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }
    }
}