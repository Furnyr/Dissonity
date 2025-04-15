
using System;
using Newtonsoft.Json;

namespace Dissonity.Models.Interop
{
    /// <summary>
    /// First error payload received through the <c> dissonity </c>  hiRPC channel.
    /// </summary>
    [Serializable]
    internal class DissonityChannelError
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}