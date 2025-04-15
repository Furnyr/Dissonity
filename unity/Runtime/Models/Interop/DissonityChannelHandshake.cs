
using System;
using Newtonsoft.Json;

namespace Dissonity.Models.Interop
{
    /// <summary>
    /// First payload received through the <c> dissonity </c>  hiRPC channel.
    /// </summary>
    [Serializable]
    internal class DissonityChannelHandshake
    {
        #nullable enable annotations

        [JsonProperty("hash")]
        public string? Hash { get; set; }
        
        [JsonProperty("raw_multi_event")]
        public RawMultiEvent? RawMultiEvent { get; set; }
    }
}