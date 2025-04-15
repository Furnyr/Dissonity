using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockPresence : Presence
    {
        new public string Status = "Mock status";

        new public MockActivity Activity = new();

        public Presence ToPresence()
        {
            return new Presence()
            {
                Status = Status,
                Activity = Activity.ToActivity()
            };
        }
    }
}