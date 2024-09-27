using System;
using Dissonity.Commands.Responses;

namespace Dissonity.Commands
{
    [Serializable]
    internal class EncourageHardwareAcceleration : FrameCommand<EncourageHardwareAccelerationResponse>
    {
        internal override string Command => DiscordCommandType.EncourageHwAcceleration;
    }
}