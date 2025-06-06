using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockVoiceState : VoiceState
    {
        new public bool Mute = false;

        new public bool Deaf = false;

        new public bool SelfMute = false;

        new public bool SelfDeaf = false;

        new public bool Suppress = false;

        public VoiceState ToVoiceState()
        {
            return new VoiceState()
            {
                Mute = Mute,
                Deaf = Deaf,
                SelfMute = SelfMute,
                SelfDeaf = SelfDeaf,
                Suppress = Suppress
            };
        }
    }
}