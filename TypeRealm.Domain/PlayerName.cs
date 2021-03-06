﻿using System;

namespace TypeRealm.Domain
{
    public sealed class PlayerName : Primitive<string>
    {
        public PlayerName(string value) : base(value)
        {
            if (value.Length < 3)
                throw new ArgumentException("Player name should be at least 3 characters long.", nameof(value));

            if (value.Length > 20)
                throw new ArgumentException("Maximum length of the player name is 20 characters.", nameof(value));
        }

        public static implicit operator string(PlayerName playerName)
            => playerName.Value;

        public static implicit operator PlayerName(string value)
            => new PlayerName(value);
    }
}
