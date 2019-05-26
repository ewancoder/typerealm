using TypeRealm.Domain;
using TypeRealm.Messages.Movement;
using TypeRealm.Server.Messaging;

namespace TypeRealm.Server.Handlers
{
    internal sealed class MovementHandler : IMessageHandler
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IRoadStore _roadStore;

        public MovementHandler(
            IPlayerRepository playerRepository,
            IRoadStore roadStore)
        {
            _playerRepository = playerRepository;
            _roadStore = roadStore;
        }

        public void Handle(ConnectedClient sender, object message)
        {
            if (message is EnterRoad enterRoad)
            {
                Handle(sender, enterRoad);
                return;
            }

            if (message is Move move)
            {
                Handle(sender, move);
                return;
            }

            if (message is TurnAround turnAround)
            {
                Handle(sender, turnAround);
                return;
            }
        }

        private void Handle(ConnectedClient sender, EnterRoad message)
        {
            var player = _playerRepository.Find(sender.PlayerId);
            var road = _roadStore.Find(message.RoadId);

            player.EnterRoad(road);
            _playerRepository.Save(player);
        }

        private void Handle(ConnectedClient sender, Move message)
        {
            var player = _playerRepository.Find(sender.PlayerId);

            player.Move(message.Distance);
            _playerRepository.Save(player);
        }

        private void Handle(ConnectedClient sender, TurnAround message)
        {
            var player = _playerRepository.Find(sender.PlayerId);

            player.TurnAround();
            _playerRepository.Save(player);
        }
    }
}
