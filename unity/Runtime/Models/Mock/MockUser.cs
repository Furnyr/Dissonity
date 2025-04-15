using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockUser : User
    {
        new public long Id = 9123456780;

        new public string Username = "mock_username";

        new public string GlobalName = "Mock user";

        new public string Avatar = "4v4t43h4sh";

        new public int AccentColor = 0;

        new public MockAvatarDecoration AvatarDecoration = new();

        new public bool Bot = false;

        new public long Flags = 0;

        new public PremiumType PremiumType = PremiumType.None;

        public User ToUser()
        {
            return new User()
            {
                GlobalName = GlobalName,
                AccentColor = AccentColor,
                AvatarDecoration = AvatarDecoration,
                Bot = Bot,
                Flags = Flags,
                PremiumType = PremiumType,
                Id = Id,
                Username = Username,
                Avatar = Avatar
            };
        }
    }
}