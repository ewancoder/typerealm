using System;

namespace TypeRealm.Server
{
    internal interface ILogger
    {
        void Log(string message);
        void Log(string message, Exception exception);
    }
}
