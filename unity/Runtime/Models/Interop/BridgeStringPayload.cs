using System;
using Newtonsoft.Json;

namespace Dissonity.Models.Interop
{
    [Serializable]
    internal class BridgeStringPayload
    {
        #nullable enable annotations

        [JsonProperty("str")]
        public string Str { get; set; }
    }
}