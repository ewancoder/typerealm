using System;
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

            var connectionFactory = new ConnectionFactory(
                new TcpConnectionFactory(server, Port), authorize);

            var output = new ConsoleOutput();

            using (var game = new Game(connectionFactory, output))
            {
                Console.CursorVisible = false;
                game.Update();

                while (game.IsConnected)
                {
                    var key = Console.ReadKey(true);
                    game.Input(key);
                }
            }
        }
    }
}
