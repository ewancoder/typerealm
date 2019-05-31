using System;
using TypeRealm.ConsoleApp.Messages;
using TypeRealm.Messages;
using TypeRealm.Messages.Connection;

namespace TypeRealm.ConsoleApp.Networking
{
    public sealed class GameMessageDispatcher : IMessageDispatcher
    {
        private readonly OutputHandler _outputHandler;

        public GameMessageDispatcher(OutputHandler outputHandler)
        {
            _outputHandler = outputHandler;
        }

        public void Dispatch(object message)
        {
            if (message is Status status)
            {
                _outputHandler.Update(status);
                return;
            }

            if (message is Disconnected disconnected)
            {
                // Stop listening.
                _outputHandler.Disconnect(disconnected.Reason.ToString());
                return;
            }

            if (message is Say say)
            {
                _outputHandler.Notify(say.Message);
                return;
            }

            if (message is Reconnecting reconnecting)
            {
                _outputHandler.Reconnecting();
                return;
            }

            throw new InvalidOperationException("Unknown message cannot be dispatched.");
        }
    }
}
