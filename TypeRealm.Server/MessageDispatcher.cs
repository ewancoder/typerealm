using TypeRealm.Domain;
using TypeRealm.Messages.Movement;

namespace TypeRealm.Server
{
    internal sealed class MessageDispatcher : IMessageDispatcher
    {
        private readonly IMessageDispatcher _echo;
        private readonly IPlayerRepository _playerRepository;
        private readonly IRoadStore _roadStore;

        public MessageDispatcher(
            IMessageDispatcher echo,
            IPlayerRepository playerRepository,
            IRoadStore roadStore)
        {
            _echo = echo;
            _playerRepository = playerRepository;
            _roadStore = roadStore;
        }

        public void Dispatch(ConnectedClient client, object message)
        {
            _echo.Dispatch(client, message);

            if (message is EnterRoad enterRoad)
            {
                var player = _playerRepository.Find(client.PlayerId);
                var road = _roadStore.Find(enterRoad.RoadId);

                player.EnterRoad(road);
                return;
            }

            if (message is Move move)
            {
                var player = _playerRepository.Find(client.PlayerId);

                player.Move(move.Distance);
                return;
            }

            if (message is TurnAround turnAround)
            {
                var player = _playerRepository.Find(client.PlayerId);

                player.TurnAround();
                return;
            }
        }
    }
}
