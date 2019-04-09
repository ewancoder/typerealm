using System;
using System.Net.Sockets;
using System.Threading.Tasks;
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

            using (var client = new TcpClient())
            {
                client.Connect(server, Port);

                var stream = client.GetStream();
                var isConnected = true;

                Task.Run(() =>
                {
                    while (true)
                    {
                        // TODO: Add try/catch.
                        var message = MessageSerializer.Read(stream);

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
                    MessageSerializer.Write(stream, new Authorize
                    {
                        Login = login,
                        Password = password,
                        PlayerName = playerName
                    });

                    Console.WriteLine("Sent Authorize message.");

                    var command = Console.ReadLine();

                    if (command == "exit")
                    {
                        MessageSerializer.Write(stream, new Quit());
                        Console.WriteLine("Sent quit message.");
                    }
                }

                Console.WriteLine("Stopped sending messages loop. Press ENTER to exit.");
                Console.ReadLine();
            }
        }
    }
}
