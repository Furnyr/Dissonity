using System;
using Dissonity.Commands.Responses;

namespace Dissonity.Commands
{
    [Serializable]
    internal class OpenInviteDialog : FrameCommand<NoResponse>
    {
        internal override string Command => DiscordCommandType.OpenInviteDialog;
    }
}