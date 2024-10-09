using System;

namespace Dissonity.Commands
{
    [Serializable]
    internal class GetSkus : FrameCommand
    {
        internal override string Command => DiscordCommandType.GetSkusEmbedded;
    }
}