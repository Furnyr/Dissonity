
using System;
using Newtonsoft.Json;

namespace Dissonity.Models.Interop
{
    [Serializable]
    internal class BridgeStateMessage
    {
        #nullable enable annotations

        [JsonProperty("code")]
        public BridgeStateCode Code { get; set; }
    }
}