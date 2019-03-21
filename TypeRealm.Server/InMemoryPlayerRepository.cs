using System.Collections.Generic;

namespace TypeRealm.Server
{
    internal sealed class InMemoryPlayerRepository : IPlayerRepository
    {
        private readonly Dictionary<string, Player> _players
            = new Dictionary<string, Player>();

        private readonly Dictionary<string, string> _passwords
            = new Dictionary<string, string>();

        public Player AuthenticateOrCreate(string login, string password)
        {
            if (!_players.ContainsKey(login))
            {
                return CreatePlayer(login, password);
            }

            if (password != _passwords[login])
                return null;

            return _players[login];
        }

        private Player CreatePlayer(string login, string password)
        {
            var player = new Player();

            _players.Add(login, player);
            _passwords.Add(login, password);

            return player;
        }
    }
}
