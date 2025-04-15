using Dissonity.Commands.Responses;
using Dissonity.Events;

namespace Dissonity.Models
{
    /// <summary>
    /// This is not a normal RPC event, this "event" is sent through hiRPC
    /// once the authentication process has finished successfully and Unity has loaded.
    /// </summary>
    public class MultiEvent
    {
        #nullable enable annotations

        public ReadyEventData ReadyData { get; set; }

        public AuthorizeData AuthorizeData { get; set; }

        public AuthenticateData AuthenticateData { get; set; }

        public object? ServerResponse { get; set; }
    }
}