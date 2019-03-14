using System;

namespace TypeRealm.Server
{
    internal interface ILogger
    {
        void Log(string message);
        void Log(string message, Exception exception);
    }

    internal sealed class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void Log(string message, Exception exception)
        {
            Console.WriteLine($"{message} {exception}");
        }
    }
}
