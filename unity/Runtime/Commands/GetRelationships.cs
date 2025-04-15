using System;

namespace Dissonity.Commands
{
    [Serializable]
    internal class GetRelationships : FrameCommand
    {
        internal override string Command => DiscordCommandType.GetRelationships;
    }
}