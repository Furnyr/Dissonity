
using System;
using Dissonity.Models.Interop;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockQueryData : QueryData
    {
        new public string InstanceId = "i-1234567891-ab-987654321-000000000";

        new public string LocationId = "gc-912952092627435520-912954213460484116";

        new public long ChannelId = 123456789;

        new public long GuildId = 123456789;

        new public string FrameId = "8c98765-ab11-22dd-a7c0-00fd1f12345";

        new public MockPlatform Platform = MockPlatform.Desktop;

        new public string MobileAppVersion = "250.0";

        new public string CustomId = "custom-id";

        new public string ReferrerId = "referrer-id";
    }
}