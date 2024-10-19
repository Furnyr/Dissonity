using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockGuildMemberRpc : GuildMemberRpc
    {
        #nullable enable annotations

        // User id isn't exposed

        new public long GuildId = 123456789;

        new public string Avatar = "4v4t43h4sh";

        new public MockAvatarDecoration AvatarDecoration = new();

        new public string ColorString = "#5865F2";
        
        // Nickname isn't exposed

        public GuildMemberRpc ToGuildMemberRpc()
        {
            return new GuildMemberRpc()
            {
                GuildId = GuildId,
                Avatar = Avatar,
                AvatarDecoration = AvatarDecoration.ToAvatarDecoration(),
                ColorString = ColorString
            };
        }
    }
}