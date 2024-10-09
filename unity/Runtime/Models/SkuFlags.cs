
namespace Dissonity.Models
{
    public enum SkuFlags : long
    {
        Available = 1 << 2,
        GuildSubscription = 1 << 7,
        UserSubscription = 1 << 8
    }
}