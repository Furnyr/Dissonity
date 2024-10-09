using System;

namespace Dissonity.Commands
{
    [Serializable]
    internal class GetEntitlements : FrameCommand
    {
        internal override string Command => DiscordCommandType.GetEntitlementsEmbedded;
    }
}