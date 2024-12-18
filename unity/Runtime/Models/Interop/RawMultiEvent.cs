using System;
using Newtonsoft.Json;

namespace Dissonity.Models.Interop
{
    /// <summary>
    /// This is not a normal RPC event, this event is sent through the hiRPC
    /// once the authentication process has finished successfully.
    /// </summary>
    [Serializable]
    internal class RawMultiEvent
    {
        [JsonProperty("ready")]
        public string ReadyMessage { get; set; }

        [JsonProperty("authorize")]
        public string AuthorizeMessage { get; set; }

        [JsonProperty("authenticate")]
        public string AuthenticateMessage { get; set; }

        [JsonProperty("response")]
        public string ServerResponse { get; set; }
    }
}