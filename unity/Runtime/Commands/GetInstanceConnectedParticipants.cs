using System;
using Dissonity.Commands.Responses;

namespace Dissonity.Commands
{
    [Serializable]
    internal class GetInstanceConnectedParticipants : FrameCommand<GetInstanceConnectedParticipantsResponse>
    {
        internal override string Command => DiscordCommandType.GetActivityInstanceConnectedParticipants;
    }
}