using System;

namespace TypeRealm.ConsoleApp.Networking
{
    public sealed class Reconnect
    {
        public Reconnect(int tries, TimeSpan interval, TimeSpan heartbeatReconnectTimeout)
        {
            Tries = tries;
            RetryWaitTime = interval;
            HeartbeatReconnectTimeout = heartbeatReconnectTimeout;
        }

        public int Tries { get; }
        public TimeSpan RetryWaitTime { get; }
        public TimeSpan HeartbeatReconnectTimeout { get; }

        public static Reconnect Default() => new Reconnect(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));
    }
}
