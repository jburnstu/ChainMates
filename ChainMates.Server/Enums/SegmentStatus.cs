using NuGet.Protocol.Plugins;

namespace ChainMates.Server.enums
{
    public enum SegmentStatus
    {
        InProgress = 1,
        AvailableForModeration = 2,
        LockedForModeration = 3,
        AvailableForAddition = 4,
        LockedForAddition = 5, // Might move this logic at some point to sit outside of status
        Abandoned = 6
    }
}
