using System;

namespace TypeRealm.ConsoleApp.Networking
{
    public interface IConnection : IDisposable
    {
        object Read();
        void Write(object message);
    }
}
