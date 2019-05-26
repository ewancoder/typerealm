using System;
using TypeRealm.Messages;
using TypeRealm.Server.Messaging;

namespace TypeRealm.Server.Infrastructure
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
