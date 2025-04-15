using System;
using UnityEngine;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockActivityParty : ActivityParty
    {
        new public string Id = "mock-id";
        
        [HideInInspector]
        new public int[] Size = new int[0];

        public ActivityParty ToActivityParty()
        {
            return new ActivityParty()
            {
                Id = Id,
                Size = Size
            };
        }
    }
}