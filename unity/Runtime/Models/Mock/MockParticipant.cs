using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockParticipant : MockUser
    {
        public string Nickname = "Mock nickname";

        public Participant ToParticipant()
        {
            return new Participant()
            {
                Nickname = Nickname,
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