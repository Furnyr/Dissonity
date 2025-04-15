using System;
using Dissonity.Commands.Responses;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockGetChannelPermissionsData : GetChannelPermissionsData
    {
        new public long Permissions = 0;
    }
}