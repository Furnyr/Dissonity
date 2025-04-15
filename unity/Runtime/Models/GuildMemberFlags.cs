
namespace Dissonity.Models
{
    public enum GuildMemberFlags : long
    {
        DidRejoin = 1 << 0,
        CompletedOnboarding = 1 << 1,
        BypassesVerification = 1 << 2,
        StartedOnboarding = 1 << 3,
        IsGuest = 1 << 4,
        StartedHomeActions = 1 << 5,
        CompletedHomeActions = 1 << 6,
        AutomodQuarantinedUsername = 1 << 7,
        DmSettingsUpsellAcknowledged = 1 << 9
    }
}