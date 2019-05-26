using Moq;
using Xunit;

namespace TypeRealm.Server.Tests
{
    public sealed class ConnectedClientTests
    {
        [Fact]
        public void ShouldCreate()
        {
            var playerId = Fixture.PlayerId();
            var connection = new Mock<IConnection>().Object;

            var client = new ConnectedClient(playerId, connection);

            Assert.Equal(playerId, client.PlayerId);
            Assert.Equal(connection, client.Connection);
        }
    }
}
