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

            using (var client = new TcpClient())
            {
                client.Connect(server, Port);

                var stream = client.GetStream();

                Task.Run(() =>
                {
                    while (true)
                    {
                        // TODO: Add try/catch.
                        var message = MessageSerializer.Read(stream);

                        if (message is Disconnected disconnected)
                        {
                            Console.WriteLine($"You have been disconnected. Reason: {disconnected.Reason.ToString()}");
                            return;
                        }
                    }
                });

                while (true)
                {
                    MessageSerializer.Write(stream, new Authorize
                    {
                        Login = login,
                        Password = password
                    });

                    Console.WriteLine("Sent Authorize message.");

                    var command = Console.ReadLine();

                    if (command == "exit")
                    {
                        MessageSerializer.Write(stream, new Quit());
                        Console.ReadLine();
                        return; // TODO: Return only if disconnect has been acknowledged. Return to main menu.
                    }
                }
            }
        }
    }
}
