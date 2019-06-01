using System;
using TypeRealm.ConsoleApp.Messaging;

namespace TypeRealm.ConsoleApp.Networking
{
    internal interface IMessageProcessor : IMessageSender, IDisposable
    {
        bool IsConnected { get; }
    }
}
