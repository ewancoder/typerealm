using System;
using System.IO;

namespace TypeRealm.Server
{
    internal interface IClientListenerFactory
    {
        IDisposable StartListening(int port, Action<Stream> streamHandler);
    }
}
