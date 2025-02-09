
using System;
using Newtonsoft.Json;

namespace Dissonity.Models.Interop
{
    /// <summary>
    /// Data sent outside of Unity, through hiRPC. <br/> <br/>
    /// The opposite of <c> InteropMessage </c>.
    /// </summary>
    [Serializable]
    internal class BridgeMessage
    {
        #nullable enable annotations
        
        [JsonProperty("nonce")]
        public string? Nonce { get; set; }

        [JsonProperty("app_hash")]
        public string? AppHash { get; set; }

        [JsonProperty("data")]
        public object? Data { get; set; }

        [JsonProperty("channel")]
        public string? Channel { get; set; }
    }
}