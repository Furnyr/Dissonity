using System;
using Dissonity.Commands.Responses;

namespace Dissonity.Models.Mock
{
    [Serializable]
    [Obsolete]
    public class MockGetChannelData : GetChannelData
    {
        new public string Id = "9123456780";
        
        new public ChannelType Type = ChannelType.GuildText;

        new public string GuildId = "8876543219";
        
        new public string Name = "mock-channel";

        new public string Topic = "Channel topic";
        
        new public double Bitrate = 64;

        new public int UserLimit = 100;
        
        new public int Position = 1;

        // Voice states isn't exposed
        
        public GetChannelData ToChannelData()
        {
            return new GetChannelData()
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