using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockActivityAssets : ActivityAssets
    {
        new public string LargeImage = "mock-image";

        new public string LargeText = "mock-text";

        new public string LargeUrl = "mock-url";
        
        new public string SmallImage = "mock-image";

        new public string SmallUrl = "mock-url";

        new public string SmallText = "mock-text";
        
        public ActivityAssets ToActivityAssets()
        {
            return new ActivityAssets()
            {
                LargeImage = LargeImage,
                LargeText = LargeText,
                LargeUrl = LargeUrl,
                SmallImage = SmallImage,
                SmallText = SmallText,
                SmallUrl = SmallUrl
            };
        }
    }
}