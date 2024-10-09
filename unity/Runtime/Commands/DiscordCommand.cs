using System;
using Dissonity.Models.Interop;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal abstract class DiscordCommand
    {
        [JsonIgnore]
        internal abstract Opcode Opcode { get; }

        [JsonIgnore]
        internal virtual Guid? Guid { get; } = System.Guid.NewGuid();
    }
}