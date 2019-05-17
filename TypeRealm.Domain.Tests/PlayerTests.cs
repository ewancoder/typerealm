using System;
using Xunit;

namespace TypeRealm.Domain.Tests
{
    public class PlayerTests
    {
        [Fact]
        public void ShouldCreatePlayer()
        {
            var playerId = Fixture.PlayerId();
            var accountId = Fixture.AccountId();
            var playerName = Fixture.PlayerName();
            var locationId = Fixture.LocationId();

            var player = new Player(playerId, accountId, playerName, locationId);

            Assert.Equal(playerId, player.PlayerId);
            Assert.Equal(accountId, player.AccountId);
            Assert.Equal(playerName, player.Name);
            Assert.Equal(locationId, player.LocationId);
        }

        [Fact]
        public void ShouldNotEnterVillageForestRoadFromCastle()
        {
            var player = Fixture.PlayerAt(Locations.Castle());

            var villageToForest = Roads.FromTo(
                Locations.Village(), Locations.Forest());

            Assert.Throws<InvalidOperationException>(() => player.EnterRoad(villageToForest));
        }

        [Fact]
        public void ShouldEnterVillageForestRoadFromVillageForward()
        {
            var player = Fixture.PlayerAt(Locations.Village());

            var villageToForest = Roads.FromTo(
                Locations.Village(), Locations.Forest());

            player.EnterRoad(villageToForest);

            player.AssertMovingAt(villageToForest, Distance.Zero, RoadDirection.Forward);
        }

        [Fact]
        public void ShouldEnterVillageForestRoadFromForestBackward()
        {
            var player = Fixture.PlayerAt(Locations.Forest());

            var villageToForest = Roads.FromTo(
                Locations.Village(), Locations.Forest());

            player.EnterRoad(villageToForest);

            player.AssertMovingAt(villageToForest, Distance.Zero, RoadDirection.Backward);
        }

        [Fact]
        public void ShouldStandOnSamePlace()
        {
            var villageToForest = Roads.FromTo(
                Locations.Village(), Locations.Forest());

            var player = Fixture.PlayerAt(villageToForest, Distance.Zero, RoadDirection.Forward);

            player.Move(Distance.Zero);

            player.AssertMovingAt(villageToForest, Distance.Zero, RoadDirection.Forward);
        }

        [Fact]
        public void ShouldMove()
        {
            var villageToForest = Roads.FromTo(
                Locations.Village(), Locations.Forest(), 100);

            var player = Fixture.PlayerAt(villageToForest, Distance.Zero, RoadDirection.Forward);

            player.Move(10);
            player.AssertMovingAt(villageToForest, 10, RoadDirection.Forward);

            player.Move(15);
            player.AssertMovingAt(villageToForest, 25, RoadDirection.Forward);
        }

        [Fact]
        public void ShouldArriveToDestination()
        {
            var villageToForest = Roads.FromTo(
                Locations.Village(), Locations.Forest(), 100);

            var player = Fixture.PlayerAt(villageToForest, new Distance(20), RoadDirection.Forward);

            player.Move(80);
            player.AssertStayingAt(Locations.Forest());
        }

        [Fact]
        public void ShouldNotAllowGoingBeyondDestination()
        {
            var villageToForest = Roads.FromTo(
                Locations.Village(), Locations.Forest(), 100);

            var player = Fixture.PlayerAt(villageToForest, new Distance(20), RoadDirection.Forward);

            Assert.Throws<InvalidOperationException>(() => player.Move(81));
        }

        [Fact]
        public void ShouldStayAtVillageIfTurnedBackImmediately()
        {
            var villageToForest = Roads.FromTo(
                Locations.Village(), Locations.Forest(), 100);

            var player = Fixture.PlayerAt(villageToForest, Distance.Zero, RoadDirection.Forward);
            player.TurnAround();

            player.AssertStayingAt(Locations.Village());
        }

        [Fact]
        public void ShouldStayAtForestIfTurnedBackImmediately()
        {
            var villageToForest = Roads.FromTo(
                Locations.Village(), Locations.Forest(), 100);

            var player = Fixture.PlayerAt(villageToForest, Distance.Zero, RoadDirection.Backward);
            player.TurnAround();

            player.AssertStayingAt(Locations.Forest());
        }

        [Fact]
        public void ShouldTurnBack()
        {
            var villageToForest = Roads.FromTo(
                Locations.Village(), Locations.Forest(), 100);

            var player = Fixture.PlayerAt(villageToForest, 20, RoadDirection.Forward);

            player.TurnAround();
            player.AssertMovingAt(villageToForest, 80, RoadDirection.Backward);

            player.TurnAround();
            player.AssertMovingAt(villageToForest, 20, RoadDirection.Forward);
        }

        [Fact]
        public void ShouldTurnBackOnUnevenRoad()
        {
            var villageToForest = Roads.FromTo(
                Locations.Village(), 100, Locations.Forest(), 1000);

            var player = Fixture.PlayerAt(villageToForest, 20, RoadDirection.Forward);

            player.TurnAround();
            player.AssertMovingAt(villageToForest, 800, RoadDirection.Backward);

            player.TurnAround();
            player.AssertMovingAt(villageToForest, 20, RoadDirection.Forward);
        }

        [Fact]
        public void CannotEnterRoadFromAnotherRoad()
        {
            var forestRoad = Roads.FromTo(Locations.Village(), Locations.Forest());
            var castleRoad = Roads.FromTo(Locations.Village(), Locations.Castle());

            var player = Fixture.PlayerAt(forestRoad, Distance.Zero, RoadDirection.Forward);

            Assert.Throws<InvalidOperationException>(() => player.EnterRoad(castleRoad));
        }

        [Fact]
        public void CannotMoveWhileNotOnRoad()
        {
            var player = Fixture.PlayerAt(Locations.Forest());

            Assert.Throws<InvalidOperationException>(() => player.Move(10));
        }

        [Fact]
        public void CannotTurnAroundWhileNotOnRoad()
        {
            var player = Fixture.PlayerAt(Locations.Forest());

            Assert.Throws<InvalidOperationException>(() => player.TurnAround());
        }
    }
}
