
using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockDictionary<T>
    {
        public string Id;
        public T Value;
    }
}