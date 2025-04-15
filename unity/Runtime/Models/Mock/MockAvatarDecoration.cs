using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockAvatarDecoration : AvatarDecoration
    {
        new public string Asset = "4v4t43h4sh";

        new public long SkuId = 123456789;

        public AvatarDecoration ToAvatarDecoration()
        {
            return new AvatarDecoration()
            {
                Asset = Asset,
                SkuId = SkuId
            };
        }
    }
}