using System;

namespace TypeRealm.ConsoleApp.Networking
{
    public interface IMessageProcessor : IDisposable
    {
        bool IsConnected { get; }
        void Send(object message);
    }
}
