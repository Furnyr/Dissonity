using System;

namespace Dissonity.Commands
{
    [Serializable]
    internal class InitiateImageUpload : FrameCommand
    {
        internal override string Command => DiscordCommandType.InitiateImageUpload;
    }
}