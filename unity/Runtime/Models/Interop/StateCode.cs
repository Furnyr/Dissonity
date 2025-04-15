
namespace Dissonity.Models.Interop
{
    /// <summary>
    /// State of the hiRPC connection.
    /// </summary>
    internal enum StateCode
    {
        Unfunctional = 0,
        OutsideDiscord = 1,
        Errored = 2,
        Loading = 3,
        Stable = 4
    }
}