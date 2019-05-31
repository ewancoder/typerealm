using System;
using System.Collections.Generic;
using System.Linq;
using TypeRealm.Domain;
using TypeRealm.Messages;
using TypeRealm.Messages.Movement;

namespace TypeRealm.Server
{
    public sealed class StatusFactory : IStatusFactory
    {
        private readonly IPlayerRepository _playerRepository;

        public StatusFactory(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public Status MakeStatus(PlayerId playerId, IEnumerable<PlayerId> activePlayers)
        {
            var player = _playerRepository.Find(playerId);
            if (player == null)
                throw new InvalidOperationException($"Player {playerId} does not exist.");

            var players = activePlayers
                .Select(id => _playerRepository.Find(id))
                .ToList();

            // TODO: Maybe just log this and don't send update for non-existing players.
            if (players.Any(p => p == null))
                throw new InvalidOperationException($"Not all active players exist in repository.");

            var neighbors = players
                .Where(p => p.IsAtSamePlaceAs(player) && p.PlayerId != player.PlayerId)
                .Select(c => c.Name.Value)
                .ToList();

            var status = new Status
            {
                Name = player.Name,
                LocationId = player.LocationId.Value,
                Neighbors = neighbors
            };

            if (player.MovementInformation != null)
            {
                status.MovementStatus = new MovementStatus
                {
                    RoadId = player.MovementInformation.Road.RoadId.Value,
                    Direction = (MovementDirection)player.MovementInformation.Direction,
                    Progress = new MovementProgress
                    {
                        Distance = player.MovementInformation.Distance,
                        Progress = player.MovementInformation.Progress
                    }
                };
            }

            return status;
        }
    }
}
