
namespace Dissonity.Models.Interop
{
    public enum RpcCloseCode
    {
        CloseNormal = 1000,
        CloseUnsupported = 1003,
        CloseAbnormal = 1006,
        InvalidClientid = 4000,
        InvalidOrigin = 4001,
        Ratelimited = 4002,
        TokenRevoked = 4003,
        InvalidVersion = 4004,
        InvalidEncoding = 4005
    }
}