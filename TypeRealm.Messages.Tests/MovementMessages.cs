using TypeRealm.Messages.Movement;
using Xunit;

namespace TypeRealm.Messages.Tests
{
    public sealed class MovementMessages
    {
        [Fact]
        public void ShouldSerializeEnterRoad()
        {
            Should.Serialize(new EnterRoad
            {
                RoadId = 10
            }, message => Assert.Equal(10, message.RoadId));
        }

        [Fact]
        public void ShouldSerializeMove()
        {
            Should.Serialize(new Move
            {
                Distance = 10
            }, message => Assert.Equal(10, message.Distance));
        }

        [Fact]
        public void ShouldSerializeTurnAround()
        {
            Should.Serialize(new TurnAround(), message => { });
        }
    }
}
