using Dissonity.Json;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [JsonConverter(typeof(SafeEnumConverter))]
    public enum EntitlementType
    {
        Unknown = -1,
        Purchase = 1,
        PremiumSubscription = 2,
        DeveloperGift = 3,
        TestModePurchase = 4,
        FreePurchase = 5,
        UserGift = 6,
        PremiumPurchase = 7
    }
}