
using System;
using Dissonity.Models.Interop;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockQueryData : QueryData
    {
        new public string InstanceId = "i-1234567891-ab-987654321-000000000";

        new public string ChannelId = "9123456780";

        new public string GuildId = "8876543219";

        new public string FrameId = "8c98765-ab11-22dd-a7c0-00fd1f12345";

        new public MockPlatform Platform = MockPlatform.Desktop;
    }
}