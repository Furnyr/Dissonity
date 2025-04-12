using Dissonity.Json;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [JsonConverter(typeof(SafeEnumConverter))]
    public enum RelationshipType
    {
        Unknown = -1,
        None = 0,
        Friend = 1,
        Blocked = 2,
        PendingIncoming = 3,
        PendingOutgoing = 4,
        Implicit = 5,
        Suggestion = 6
    }
}