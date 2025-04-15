
namespace Dissonity.Models
{
    public enum UserFlags : long
    {
        Staff = 1 << 0,
        Partner = 1 << 1,
        Hypesquad = 1 << 2,
        BugHunterLevel1 = 1 << 3,
        HypesquadOnlineHouse1 = 1 << 6,
        HypesquadOnlineHouse2 = 1 << 7,
        HypesquadOnlineHouse3 = 1 << 8,
        PremiumEarlySupporter = 1 << 9,
        TeamPseudoUser = 1 << 10,
        BugHunterLevel2 = 1 << 14,
        VerifiedBot = 1 << 16,
        VerifiedDeveloper = 1 << 17,
        CertifiedModerator = 1 << 18,
        BotHttpInteractions = 1 << 19,
        ActiveDeveloper = 1 << 22
    }
}