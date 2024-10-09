using System;

namespace Dissonity.Commands
{
    [Serializable]
    internal class UserSettingsGetLocale : FrameCommand
    {
        internal override string Command => DiscordCommandType.UserSettingsGetLocale;
    }
}