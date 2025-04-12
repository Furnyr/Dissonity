using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockActivity : Activity
    {
        #nullable enable annotations

        new public string Name = "Mock name";

        new public ActivityType Type = ActivityType.Playing;
        
        new public string Url = "mock-url";
        
        new public long CreatedAt = 1420070400000;
        
        new public MockActivityTimestamps Timestamps = new();
        
        new public long ApplicationId = 123456789;
        
        new public string Details = "Mock details";

        new public string State = "Mock state";
        
        new public MockEmoji Emoji = new();
        
        new public MockActivityParty Party = new();
        
        new public MockActivityAssets Assets = new();
        
        new public MockActivitySecrets Secrets = new();
        
        new public bool Instance = true;
        
        new public long Flags = 0;

        public Activity ToActivity()
        {
            return new Activity()
            {
                Name = Name,
                Type = Type,
                Url = Url,
                CreatedAt = CreatedAt,
                Timestamps = Timestamps.ToActivityTimestamps(),
                ApplicationId = ApplicationId,
                Details = Details,
                State = State,
                Emoji = Emoji.ToEmoji(),
                Party = Party.ToActivityParty(),
                Assets = Assets.ToActivityAssets(),
                Secrets = Secrets.ToActivitySecrets(),
                Instance = Instance,
                Flags = Flags
            };
        }
    }
}