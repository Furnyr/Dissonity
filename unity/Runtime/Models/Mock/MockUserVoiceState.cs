using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockUserVoiceState : UserVoiceState
    {
        new public bool Mute = false;

        new public float Volume = 1f;

        new public string Nickname = "Mock nickname";
        
        new public MockUser User = new();

        new public MockVoiceState VoiceState = new();
    }
}