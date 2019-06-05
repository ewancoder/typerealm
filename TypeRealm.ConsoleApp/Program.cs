using System;
using TypeRealm.ConsoleApp.Data;
using TypeRealm.ConsoleApp.Messaging;
using TypeRealm.ConsoleApp.Networking;
using TypeRealm.ConsoleApp.Output;
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

            var dataStore = new DataStoreFactory().LoadFromFile("../../../Data/data.json");
            var textStore = new TextStoreFactory().LoadFromFile("../../../Data/texts.txt");

            var output = new ConsoleOutput();
            var printer = new Printer(output, dataStore);

            var dispatcher = new GameMessageDispatcher();

            // Game constructor synchronously connects to the server.
            using (var messages = new MessageProcessor(connectionFactory, dispatcher, authorize, Reconnect.Default()))
            {
                var game = new Game(textStore, messages, printer);
                dispatcher.SetGame(game);
                messages.Connect();

                // If connection was unsuccessful - exit.
                if (!messages.IsConnected)
                {
                    Console.WriteLine("Game over.");
                    Console.ReadLine();
                    return;
                }

                Console.CursorVisible = false;
                while (true)
                {
                    var key = Console.ReadKey(true);

                    if (!messages.IsConnected)
                    {
                        Console.WriteLine("Game over.");
                        Console.ReadLine();
                        return;
                    }

                    game.Input(key);
                }
            }
        }
    }
}
