using System;
using Dissonity.Events;
using Dissonity.Models.Interop;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal abstract class FrameCommand<TResponse> : DiscordCommand<TResponse> where TResponse : DiscordEvent
    {
        internal override Opcode Opcode => Opcode.Frame;

        [JsonIgnore]
        internal abstract string Command { get; }
    }
}