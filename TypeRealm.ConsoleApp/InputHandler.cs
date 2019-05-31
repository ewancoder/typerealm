using System;
using TypeRealm.ConsoleApp.Networking;
using TypeRealm.Messages.Connection;
using TypeRealm.Messages.Movement;

namespace TypeRealm.ConsoleApp
{
    public sealed class InputHandler
    {
        private readonly IMessageProcessor _messages;

        public InputHandler(IMessageProcessor messageProcessor)
        {
            _messages = messageProcessor;
        }

        public void Input(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.E)
            {
                _messages.Send(new Quit());
                return;
            }

            if (key.Key == ConsoleKey.R)
            {
                _messages.Send(new EnterRoad
                {
                    RoadId = 1
                });

                return;
            }

            if (key.Key == ConsoleKey.M)
            {
                _messages.Send(new Move
                {
                    Distance = 1
                });

                return;
            }

            if (key.Key == ConsoleKey.T)
            {
                _messages.Send(new TurnAround());
                return;
            }
        }
    }
}
