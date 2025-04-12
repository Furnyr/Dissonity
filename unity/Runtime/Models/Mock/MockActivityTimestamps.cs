using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockActivityTimestamps : ActivityTimestamps
    {
        new public long Start = 1420070400000;

        new public long End = 1420070400001;

        public ActivityTimestamps ToActivityTimestamps()
        {
            return new ActivityTimestamps()
            {
                Start = Start,
                End = End
            };
        }
    }
}