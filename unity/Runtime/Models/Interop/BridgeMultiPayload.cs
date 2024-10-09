using System;
using Newtonsoft.Json;

namespace Dissonity.Models.Interop
{
    [Serializable]
    internal class BridgeMultiPayload
    {
        #nullable enable annotations

        [JsonProperty("raw_multi_event")]
        public RawMultiEvent RawMultiEvent { get; set; }
    }
}