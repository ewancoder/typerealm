using System;

namespace TypeRealm.Server
{
    static class Program
    {
        const int Port = 30100;

        static void Main()
        {
            Console.WriteLine("===== TypeRealm server =====");

            var logger = new ConsoleLogger();
            var accountRepository = new InMemoryAccountRepository();
            var playerRepository = new InMemoryPlayerRepository();
            var messageDispatcher = new EchoMessageDispatcher();

            using (var server = new Server(Port, logger, accountRepository, playerRepository, messageDispatcher))
            {
                Console.ReadLine();
            }
        }
    }
}
