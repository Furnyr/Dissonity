
namespace Dissonity.Models
{
    public enum ChannelType
    {
        Unhandled = -1,
        GuildText = 0,
        Dm = 1,
        GuildVoice = 2,
        GroupDm = 3,
        GuildCategory = 4,
        GuildAnnouncement = 5,
        GuildStore = 6,
        AnnouncementThread = 10,
        PublicThread = 11,
        PrivateThread = 12,
        GuildStageVoice = 13,
        GuildDirectory = 14,
        GuildForum = 15,
    }
}