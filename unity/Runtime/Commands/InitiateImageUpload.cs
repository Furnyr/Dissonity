using System;
using Dissonity.Commands.Responses;

namespace Dissonity.Commands
{
    [Serializable]
    internal class InitiateImageUpload : FrameCommand<InitiateImageUploadResponse>
    {
        internal override string Command => DiscordCommandType.InitiateImageUpload;
    }
}