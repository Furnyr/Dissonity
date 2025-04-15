using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockUserVoiceState : UserVoiceState
    {
        new public bool Mute = false;

        new public float Volume = 1f;

        // Nickname isn't exposed
        
        // User isn't exposed

        new public MockVoiceState VoiceState = new();

        public UserVoiceState ToUserVoiceState()
        {
            return new UserVoiceState()
            {
                Mute = Mute,
                Volume = Volume,
                VoiceState = VoiceState.ToVoiceState()
            };
        }
    }
}