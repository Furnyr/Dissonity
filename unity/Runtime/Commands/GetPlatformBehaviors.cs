using System;

namespace Dissonity.Commands
{
    [Serializable]
    internal class GetPlatformBehaviors : FrameCommand
    {
        internal override string Command => DiscordCommandType.GetPlatformBehaviors;
    }
}