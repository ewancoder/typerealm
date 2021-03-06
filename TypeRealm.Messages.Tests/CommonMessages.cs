﻿using System.Collections.Generic;
using TypeRealm.Messages.Movement;
using Xunit;

namespace TypeRealm.Messages.Tests
{
    public sealed class CommonMessages
    {
        [Fact]
        public void ShouldSerializeSayMessage()
        {
            Should.Serialize(new Say
            {
                Message = "message"
            }, message =>
            {
                Assert.Equal("message", message.Message);
            });
        }

        [Fact]
        public void ShouldSerializeStatusMessage()
        {
            Should.Serialize(new Status
            {
                LocationId = 10,
                Name = "name",
                MovementStatus = new MovementStatus
                {
                    Direction = MovementDirection.Backward,
                    Progress = new MovementProgress
                    {
                        Distance = 100,
                        Progress = 50
                    },
                    RoadId = 30
                },
                Neighbors = new List<string>
                {
                    "neighbor 1",
                    "neighbor 2"
                },
                Roads = new List<RoadStatus>
                {
                    new RoadStatus
                    {
                        RoadId = 15,
                        Direction = MovementDirection.Forward
                    },
                    new RoadStatus
                    {
                        RoadId = 25,
                        Direction = MovementDirection.Backward
                    }
                }
            }, message =>
            {
                Assert.Equal(10, message.LocationId);
                Assert.Equal("name", message.Name);
                Assert.Equal(MovementDirection.Backward, message.MovementStatus.Direction);
                Assert.Equal(100, message.MovementStatus.Progress.Distance);
                Assert.Equal(50, message.MovementStatus.Progress.Progress);
                Assert.Equal(30, message.MovementStatus.RoadId);
                Assert.Equal(2, message.Neighbors.Count);
                Assert.Equal("neighbor 1", message.Neighbors[0]);
                Assert.Equal("neighbor 2", message.Neighbors[1]);
                Assert.Equal(2, message.Roads.Count);
                Assert.Equal(15, message.Roads[0].RoadId);
                Assert.Equal(MovementDirection.Forward, message.Roads[0].Direction);
                Assert.Equal(25, message.Roads[1].RoadId);
                Assert.Equal(MovementDirection.Backward, message.Roads[1].Direction);
            });
        }

        [Fact]
        public void ShouldNotForgetToCreateDefaultListsForStatusMessage()
        {
            Should.Serialize(new Status(), message =>
            {
                Assert.Empty(message.Neighbors);
                Assert.Empty(message.Roads);
            });
        }
    }
}
