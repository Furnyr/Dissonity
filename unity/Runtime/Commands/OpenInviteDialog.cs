using System;

namespace Dissonity.Commands
{
    [Serializable]
    internal class OpenInviteDialog : FrameCommand
    {
        internal override string Command => DiscordCommandType.OpenInviteDialog;
    }
}