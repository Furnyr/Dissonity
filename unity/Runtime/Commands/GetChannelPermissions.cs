using System;

namespace Dissonity.Commands
{
    [Serializable]
    internal class GetChannelPermissions : FrameCommand
    {
        internal override string Command => DiscordCommandType.GetChannelPermissions;
    }
}