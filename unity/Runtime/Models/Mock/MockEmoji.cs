using System;
using UnityEngine;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockEmoji : Emoji
    {
        new public long Id = 123456789;

        new public string Name = "Mock name";

        [HideInInspector]
        new public long[] Roles = new long[0];
        
        [HideInInspector]
        new public MockUser User = new();
        
        new public bool RequireColons = true;
        
        new public bool Managed = true;

        new public bool Animated = false;

        new public bool Available = true;

        public Emoji ToEmoji()
        {
            return new Emoji()
            {
                Id = Id,
                Name = Name,
                Roles = Roles,
                User = User,
                RequireColons = RequireColons,
                Managed = Managed,
                Animated = Animated,
                Available = Available
            };
        }
    }
}