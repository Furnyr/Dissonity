using System;
using Dissonity.Events;
using Dissonity.Models.Interop;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal abstract class DiscordCommand<TResponse> where TResponse : DiscordEvent
    {
        [JsonIgnore]
        internal abstract Opcode Opcode { get; }

        [JsonIgnore]
        internal virtual Guid? Guid { get; } = System.Guid.NewGuid();
    }
}