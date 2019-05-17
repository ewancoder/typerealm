using System;
using System.Threading.Tasks;
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

            using (var connection = new Connection(connectionFactory, authorize))
            {
                connection.ReconnectAndAuthorize();
                var isConnected = true;

                Task.Run(() =>
                {
                    while (true)
                    {
                        // TODO: Add try/catch.
                        var message = connection.ReceiveMessage();

                        if (message is Disconnected disconnected)
                        {
                            Console.WriteLine($"SERVER: You have been disconnected. Reason: {disconnected.Reason.ToString()}");
                            Console.WriteLine("Stopped waiting for messages from server.");
                            isConnected = false;
                            return;
                        }

                        if (message is Say say)
                        {
                            Console.WriteLine($"SERVER: {say.Message}");
                        }
                    }
                });

                while (isConnected)
                {
                    connection.Send(new Authorize());
                    Console.WriteLine("Sent Authorize message.");

                    var command = Console.ReadLine();

                    if (command == "exit")
                    {
                        connection.Send(new Quit());
                        Console.WriteLine("Sent quit message.");
                    }
                }

                Console.WriteLine("Stopped sending messages loop. Press ENTER to exit.");
                Console.ReadLine();
            }
        }
    }
}
