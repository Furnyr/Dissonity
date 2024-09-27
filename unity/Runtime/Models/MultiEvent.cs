using Dissonity.Commands.Responses;
using Dissonity.Events;

namespace Dissonity.Models
{
    /// <summary>
    /// This is not a normal RPC event, this "event" is sent by the RpcBridge
    /// once the authentication process has finished successfully and Unity has loaded.
    /// </summary>
    public class MultiEvent
    {
        public ReadyEventData ReadyData { get; set; }

        public AuthorizeData AuthorizeData { get; set; }

        public AuthenticateData AuthenticateData { get; set; }

        public object ServerResponse { get; set; }
    }
}