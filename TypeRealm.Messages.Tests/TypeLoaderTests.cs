using System.Linq;
using TypeRealm.Messages.Connection;
using TypeRealm.Messages.Movement;
using Xunit;

namespace TypeRealm.Messages.Tests
{
    public sealed class TypeLoaderTests
    {
        [Fact]
        public void ShouldLoadAllTypes()
        {
            var types = TypeLoader.Messages.ToList();
            Assert.Equal(11, types.Count);

            foreach (var type in new[]
            {
                typeof(Authorize),
                typeof(Disconnected),
                typeof(Heartbeat),
                typeof(Quit),

                typeof(EnterRoad),
                typeof(Move),
                typeof(MovementStatus),
                typeof(MovementProgress),
                typeof(TurnAround),

                typeof(Say),
                typeof(Status)
            })
            {
                Assert.Contains(type, types);
            }
        }
    }
}
