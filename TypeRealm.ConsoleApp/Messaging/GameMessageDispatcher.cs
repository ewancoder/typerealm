using System;
using TypeRealm.ConsoleApp.Messages;
using TypeRealm.Messages;
using TypeRealm.Messages.Connection;

namespace TypeRealm.ConsoleApp.Messaging
{
    internal sealed class GameMessageDispatcher : IMessageDispatcher
    {
        private Game _game;

        // HACK: We have a circular dependency.
        public void SetGame(Game game)
        {
            _game = game;
        }

        public void Dispatch(object message)
        {
            if (message is Status status)
            {
                _game.UpdateState(status);
                return;
            }

            if (message is Disconnected disconnected)
            {
                _game.Disconnect();
                return;
            }

            if (message is Say say)
            {
                _game.Notify(say.Message);
                return;
            }

            if (message is Reconnecting reconnecting)
            {
                _game.Reconnecting();
                return;
            }

            throw new InvalidOperationException("Unknown message cannot be dispatched.");
        }
    }
}
