using System;

namespace Dissonity.Commands
{
    [Serializable]
    internal class EncourageHardwareAcceleration : FrameCommand
    {
        internal override string Command => DiscordCommandType.EncourageHwAcceleration;
    }
}