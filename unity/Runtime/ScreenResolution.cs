
namespace Dissonity
{
    public enum ScreenResolution
    {
        // Use the resolution in the Unity WebGL settings
        Default = 1,

        // Use the resolution from the Unity viewport
        Viewport = 2,

        // Let Unity handle the resolution
        Dynamic = 3,

        // Try to use as much space as possible
        Max = 4
    }
}