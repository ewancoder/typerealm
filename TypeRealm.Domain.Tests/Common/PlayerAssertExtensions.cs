using Xunit;

namespace TypeRealm.Domain.Tests.Common
{
    public static class PlayerAssertExtensions
    {
        public static void AssertStayingAt(this Player player, LocationId locationId)
        {
            Assert.Null(player.MovementInformation);
            Assert.Equal(locationId, player.LocationId);
        }

        public static void AssertMovingAt(this Player player, Road road, Distance progress, RoadDirection direction)
        {
            Assert.NotNull(player.MovementInformation);
            Assert.Equal(road, player.MovementInformation.Road);
            Assert.Equal(progress, player.MovementInformation.Progress);
            Assert.Equal(direction, player.MovementInformation.Direction);
        }
    }
}
