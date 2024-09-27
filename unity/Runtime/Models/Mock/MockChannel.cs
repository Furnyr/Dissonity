using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockChannel
    {
        public MockGetChannelData ChannelData = new();

        public MockGetChannelPermissionsData ChannelPermissions = new();
    }
}