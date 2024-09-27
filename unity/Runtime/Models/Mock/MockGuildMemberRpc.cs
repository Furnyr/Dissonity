using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockGuildMemberRpc : GuildMemberRpc
    {
        #nullable enable annotations

        new public string UserId = "9123456780";

        new public string GuildId = "8876543219";

        new public string Avatar = "4v4t43h4sh";

        new public MockAvatarDecoration AvatarDecoration = new();

        new public string ColorString = "#5865F2";
        
        new public string Nickname = "Mock nickname";
    }
}