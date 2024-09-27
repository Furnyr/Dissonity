using System;
using Dissonity.Commands.Responses;

namespace Dissonity.Commands
{
    [Serializable]
    internal class UserSettingsGetLocale : FrameCommand<UserSettingsGetLocaleResponse>
    {
        internal override string Command => DiscordCommandType.UserSettingsGetLocale;
    }
}