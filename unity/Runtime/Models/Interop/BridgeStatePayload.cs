using System;
using Newtonsoft.Json;

namespace Dissonity.Models.Interop
{
    [Serializable]
    internal class BridgeStatePayload
    {
        #nullable enable annotations

        [JsonProperty("code")]
        public BridgeStateCode Code { get; set; }
    }
}