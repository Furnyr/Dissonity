using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockRelationship : Relationship
    {
        new public RelationshipType Type = RelationshipType.Friend;

        new public MockUser User = new();

        new public MockPresence Presence = new();

        public Relationship ToRelationship()
        {
            return new Relationship()
            {
                Type = Type,
                User = User.ToUser(),
                Presence = Presence.ToPresence()
            };
        }
    }
}