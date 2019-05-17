using System;
using TypeRealm.Domain;

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
            var locationStore = new InMemoryLocationStore(new LocationId(1));
            var messageDispatcher = new EchoMessageDispatcher();
            var authorizationService = new AuthorizationService(logger, accountRepository, playerRepository, locationStore);
            var clientListenerFactory = new TcpClientListenerFactory(logger);

            using (var server = new Server(Port, logger, authorizationService, messageDispatcher, clientListenerFactory))
            {
                Console.ReadLine();
            }
        }
    }
}
