using System;
using TypeRealm.Messages;
using TypeRealm.Messages.Connection;

namespace TypeRealm.ConsoleApp.Networking
{
    public sealed class GameMessageDispatcher : IMessageDispatcher
    {
        private readonly Game _game; // Hack for dispatching for now.

        public GameMessageDispatcher(Game game)
        {
            _game = game;
        }

        public void Dispatch(object message)
        {
            if (message is Status status)
            {
                _game.Update(status);
                return;
            }

            if (message is Disconnected disconnected)
            {
                // Stop listening.
                _game.Disconnect(disconnected.Reason.ToString());
                return;
            }

            if (message is Say say)
            {
                _game.Notify(say.Message);
                return;
            }

            throw new InvalidOperationException("Unknown message cannot be dispatched.");
        }
    }
}
