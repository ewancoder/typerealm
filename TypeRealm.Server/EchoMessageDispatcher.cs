﻿using System;
using TypeRealm.Messages;

namespace TypeRealm.Server
{
    internal sealed class EchoMessageDispatcher : IMessageDispatcher
    {
        public void Dispatch(ConnectedClient client, object message)
        {
            Console.WriteLine($"Received {message} from {client.PlayerId}.");

            client.Connection.Write(new Say
            {
                Message = $"Server received a {message} message."
            });
        }
    }
}
