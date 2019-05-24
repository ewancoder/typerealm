using System;
using TypeRealm.ConsoleApp.Data;
using TypeRealm.ConsoleApp.Networking;
using TypeRealm.Messages.Connection;

namespace TypeRealm.ConsoleApp
{
    static class Program
    {
        const int Port = 30100;

        static void Main()
        {
            Console.WriteLine("===== TypeRealm =====");

            Console.Write("Server: ");
            var server = Console.ReadLine();

            Console.Write("Login: ");
            var login = Console.ReadLine();

            Console.Write("Password: ");
            var password = Console.ReadLine();

            Console.Write("Player name: ");
            var playerName = Console.ReadLine();

            var authorize = new Authorize
            {
                Login = login,
                Password = password,
                PlayerName = playerName
            };

            var connectionFactory = new TcpConnectionFactory(server, Port);
            var dataStore = new InMemoryDataStore();

            var output = new ConsoleOutput();
            var printer = new Printer(output, dataStore);

            // Game constructor synchronously connects to the server.
            using (var game = new Game(connectionFactory, printer, authorize))
            {
                // If connection was unsuccessful - exit.
                if (!game.IsRunning)
                {
                    Console.WriteLine("Game over.");
                    Console.ReadLine();
                    return;
                }

                Console.CursorVisible = false;
                while (true)
                {
                    var key = Console.ReadKey(true);

                    if (!game.IsRunning)
                        return;

                    game.Input(key);
                }
            }
        }
    }
}
