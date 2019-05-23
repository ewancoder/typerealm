using System;

namespace TypeRealm.Server
{
    internal sealed class ConsoleLogger : ILogger
    {
        private readonly bool _logStackTrace;

        public ConsoleLogger() : this(false)
        {
        }

        public ConsoleLogger(bool logStackTrace)
        {
            _logStackTrace = logStackTrace;
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void Log(string message, Exception exception)
        {
            Console.WriteLine($"{message} {(_logStackTrace ? exception.ToString() : exception.Message)}");
        }
    }
}
