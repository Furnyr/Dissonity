using System;
using Dissonity.Commands.Responses;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockChannel : ChannelRpc
    {
        new public long Id = 123456789;
        
        new public ChannelType Type = ChannelType.GuildText;

        new public long GuildId = 123456789;

        public MockGetChannelPermissionsData ChannelPermissions = new();
        
        new public string Name = "mock-channel";

        new public string Topic = "Channel topic";
        
        new public double Bitrate = 64;

        new public int UserLimit = 100;
        
        new public int Position = 1;

        // Voice states isn't exposed
        
        public ChannelRpc ToChannelRpc()
        {
            return new ChannelRpc()
            {
                Id = Id,
                Type = Type,
                GuildId = GuildId,
                Name = Name,
                Topic = Topic,
                Bitrate = Bitrate,
                UserLimit = UserLimit,
                Position = Position
            };
        }
    }
}