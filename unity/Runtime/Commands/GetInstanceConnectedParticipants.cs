using System;

namespace Dissonity.Commands
{
    [Serializable]
    internal class GetInstanceConnectedParticipants : FrameCommand
    {
        internal override string Command => DiscordCommandType.GetActivityInstanceConnectedParticipants;
    }
}