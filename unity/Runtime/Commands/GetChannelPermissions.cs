using System;
using Dissonity.Commands.Responses;

namespace Dissonity.Commands
{
    [Serializable]
    internal class GetChannelPermissions : FrameCommand<GetChannelPermissionsResponse>
    {
        internal override string Command => DiscordCommandType.GetChannelPermissions;
    }
}