using System;
using Dissonity.Models.Interop;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal abstract class FrameCommand : DiscordCommand
    {
        internal override Opcode Opcode => Opcode.Frame;

        [JsonIgnore]
        internal abstract string Command { get; }
    }
}