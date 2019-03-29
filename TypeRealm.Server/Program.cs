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
            var authorizationService = new AuthorizationService(accountRepository, playerRepository, logger);

            using (var server = new Server(Port, logger, authorizationService, messageDispatcher))
            {
                Console.ReadLine();
            }
        }
    }
}
