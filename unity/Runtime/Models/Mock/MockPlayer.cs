using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockPlayer
    {
        public MockParticipant Participant = new();
        public MockGuildMemberRpc GuildMemberRpc = new();
        public MockUserVoiceState UserVoiceState = new();
    }
}