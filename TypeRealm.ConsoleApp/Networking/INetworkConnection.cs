using System;

namespace TypeRealm.ConsoleApp.Networking
{
    public interface INetworkConnection : IDisposable
    {
        object Read();
        void Write(object message);
    }
}
