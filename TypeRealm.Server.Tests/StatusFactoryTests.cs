using Moq;
using System;
using System.Linq;
using TypeRealm.Domain;
using TypeRealm.Messages.Movement;
using Xunit;

namespace TypeRealm.Server.Tests
{
    public sealed class StatusFactoryTests
    {
        private readonly Mock<IPlayerRepository> _playerRepositoryMock;
        private readonly StatusFactory _sut;

        public StatusFactoryTests()
        {
            _playerRepositoryMock = new Mock<IPlayerRepository>();
            _sut = new StatusFactory(_playerRepositoryMock.Object);
        }

        [Fact]
        public void ShouldThrowIfPlayerDoesNotExist()
        {
            Assert.Throws<InvalidOperationException>(
                () => _sut.MakeStatus(PlayerId.New(), Enumerable.Empty<PlayerId>()));
        }

        [Fact]
        public void ShouldThrowIfAnyOfActivePlayersDoNotExist()
        {
            var playerId = Fixture.PlayerId();
            var anotherPlayerId = PlayerId.New();

            AddToRepository(playerId, Fixture.PlayerName(), Fixture.LocationId());

            Assert.Throws<InvalidOperationException>(() => _sut.MakeStatus(playerId, new[]
            {
                playerId, anotherPlayerId
            }));
        }

        [Fact]
        public void ShouldMakeStatusWithoutNeighbors()
        {
            var playerId = PlayerId.New();
            var locationId = new LocationId(10);

            AddToRepository(playerId, new PlayerName("player 1"), locationId);

            var status = _sut.MakeStatus(playerId, new[] { playerId });

            Assert.Equal("player 1", status.Name);
            Assert.Equal(locationId.Value, status.LocationId);
            Assert.NotNull(status.Neighbors);
            Assert.Empty(status.Neighbors);
        }

        [Fact]
        public void ShouldMakeStatusWithoutNeighborsWhenTheyAreInDifferentArea()
        {
            var player1 = PlayerId.New();
            var player2 = PlayerId.New();

            var location1 = new LocationId(10);
            var location2 = new LocationId(20);

            AddToRepository(player1, new PlayerName("player 1"), location1);
            AddToRepository(player2, new PlayerName("player 2"), location2);

            var status = _sut.MakeStatus(player1, new[]
            {
                player2, player1
            });

            Assert.Equal("player 1", status.Name);
            Assert.Equal(location1.Value, status.LocationId);
            Assert.NotNull(status.Neighbors);
            Assert.Empty(status.Neighbors);
        }

        [Fact]
        public void ShouldMakeStatusWithNeighbors()
        {
            var player1 = PlayerId.New();
            var player2 = PlayerId.New();
            var player3 = PlayerId.New();

            var location1 = new LocationId(10);
            var location2 = new LocationId(20);
            var location3 = new LocationId(10);

            AddToRepository(player1, new PlayerName("player 1"), location1);
            AddToRepository(player2, new PlayerName("player 2"), location2);
            AddToRepository(player3, new PlayerName("player 3"), location3);

            var status = _sut.MakeStatus(player1, new[]
            {
                player2, player3, player1
            });

            Assert.Equal("player 1", status.Name);
            Assert.Equal(location1.Value, status.LocationId);
            Assert.NotNull(status.Neighbors);
            Assert.Single(status.Neighbors);
            Assert.Equal("player 3", status.Neighbors.Single());
        }

        [Fact]
        public void ShouldHaveEmptyMovementStatusWhenNotMoving()
        {
            var playerId = PlayerId.New();
            var locationId = new LocationId(10);

            AddToRepository(playerId, new PlayerName("player 1"), locationId);

            var status = _sut.MakeStatus(playerId, new[] { playerId });

            Assert.Null(status.MovementStatus);
        }

        [Fact]
        public void ShouldHaveProperMovementStatusWhenMoving()
        {
            var playerId = PlayerId.New();

            AddToRepository(playerId, new RoadId(15), 100, 40);

            var status = _sut.MakeStatus(playerId, new[] { playerId });

            Assert.NotNull(status.MovementStatus);
            Assert.Equal(15, status.MovementStatus.RoadId);
            Assert.Equal(MovementDirection.Forward, status.MovementStatus.Direction);
            Assert.Equal(100, status.MovementStatus.Progress.Distance);
            Assert.Equal(40, status.MovementStatus.Progress.Progress);
        }

        private void AddToRepository(PlayerId playerId, PlayerName playerName, LocationId locationId)
        {
            var player = Fixture.Player(playerId, playerName, locationId);

            _playerRepositoryMock
                .Setup(x => x.Find(playerId))
                .Returns(player);
        }

        private void AddToRepository(PlayerId playerId, RoadId roadId, Distance distance, Distance progress)
        {
            var player = Fixture.Player(playerId, roadId, distance, progress);

            _playerRepositoryMock
                .Setup(x => x.Find(playerId))
                .Returns(player);
        }
    }
}
