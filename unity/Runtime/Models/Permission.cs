
namespace Dissonity.Models
{
    public enum Permission : long
    {
        CreateInstantInvite = 1 << 0,
        Administrator = 1 << 3
    }
}