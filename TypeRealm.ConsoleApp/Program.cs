using System;
using TypeRealm.ConsoleApp.Data;
using TypeRealm.ConsoleApp.Networking;
using TypeRealm.Messages;

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

            using (var game = new Game(connectionFactory, printer, authorize))
            {
                Console.CursorVisible = false;
                game.Update();

                while (game.IsRunning)
                {
                    var key = Console.ReadKey(true);
                    game.Input(key);
                }
            }
        }
    }
}
