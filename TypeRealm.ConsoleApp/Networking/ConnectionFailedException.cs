using System;

namespace TypeRealm.ConsoleApp.Networking
{
    public sealed class ConnectionFailedException : Exception
    {
        public ConnectionFailedException() : base("Failed to connect to server.")
        {
        }
    }
}
