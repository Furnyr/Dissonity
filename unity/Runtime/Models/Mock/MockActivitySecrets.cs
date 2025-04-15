using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockActivitySecrets : ActivitySecrets
    {
        new public string Join = "mock-join";
        new public string Match = "mock-match";
        
        public ActivitySecrets ToActivitySecrets()
        {
            return new ActivitySecrets()
            {
                Join = Join,
                Match = Match
            };
        }
    }
}