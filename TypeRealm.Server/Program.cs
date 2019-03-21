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
            var playerRepository = new InMemoryPlayerRepository();

            using (var server = new Server(Port, logger, playerRepository))
            {
                Console.ReadLine();
            }
        }
    }
}
