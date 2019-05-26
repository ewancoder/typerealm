using System;
using TypeRealm.Domain;
using TypeRealm.Server.Infrastructure;
using TypeRealm.Server.Networking;

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
            var roadStore = new InMemoryRoadStore();
            var messageDispatcher = new MessageDispatcher(
                new EchoMessageDispatcher(), playerRepository, roadStore);
            var authorizationService = new AuthorizationService(logger, accountRepository, playerRepository, locationStore);
            var statusFactory = new StatusFactory(playerRepository);
            var clientListenerFactory = new TcpClientListenerFactory(logger);

            using (var server = new Server(Port, TimeSpan.FromSeconds(1), logger, authorizationService, messageDispatcher, statusFactory, clientListenerFactory))
            {
                Console.ReadLine();
            }
        }
    }
}
