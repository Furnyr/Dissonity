
using System;
using Newtonsoft.Json;

namespace Dissonity.Models.Interop
{
    /// <summary>
    /// Data received through the <c> dissonity </c>  hiRPC channel.
    /// </summary>
    [Serializable]
    internal class DissonityChannelPayload
    {
        #nullable enable annotations

        [JsonProperty("nonce")]
        public string? Nonce { get; set; }

        [JsonProperty("query")]
        public string? Query { get; set; }

        [JsonProperty("formatted_price")]
        public string? FormattedPrice { get; set; }
    }
}