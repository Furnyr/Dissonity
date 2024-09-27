using System;
using Dissonity.Commands.Responses;

namespace Dissonity.Commands
{
    [Serializable]
    internal class GetPlatformBehaviors : FrameCommand<GetPlatformBehaviorsResponse>
    {
        internal override string Command => DiscordCommandType.GetPlatformBehaviors;
    }
}