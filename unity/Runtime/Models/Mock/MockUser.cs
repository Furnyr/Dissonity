using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockUser : User
    {
        new public string Id = "9123456780";

        new public string Username = "mock";

        new public string GlobalName = "Mock player";

        new public string Avatar = "4v4t43h4sh";

        new public int AccentColor = 0;

        new public MockAvatarDecoration AvatarDecoration = new();

        new public bool Bot = false;

        new public int Flags = 0;

        new public int PremiumType = 0;

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