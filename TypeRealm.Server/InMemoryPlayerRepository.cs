﻿using System;
using System.Collections.Generic;
using System.Linq;
using TypeRealm.Domain;

namespace TypeRealm.Server
{
    internal sealed class InMemoryPlayerRepository : IPlayerRepository
    {
        private readonly List<Player> _players = new List<Player>();

        public Player FindByName(Guid accountId, string name)
        {
            var player = _players.SingleOrDefault(p => p.Name == name);

            if (player?.AccountId != accountId)
                return null;

            return player;
        }

        public void Save(Player player)
        {
            if (_players.Contains(player))
                return;

            if (_players.Any(p => p.PlayerId == player.PlayerId)
                || _players.Any(p => p.Name == player.Name))
                throw new InvalidOperationException("The player already exists.");

            _players.Add(player);
        }
    }
}
