﻿using System;
using System.Net.Sockets;
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

            Console.Write("Your name: ");
            var playerName = Console.ReadLine();

            using (var client = new TcpClient())
            {
                client.Connect(server, Port);

                var stream = client.GetStream();

                while (true)
                {
                    MessageSerializer.Write(stream, new Authorize
                    {
                        PlayerId = playerName
                    });

                    Console.WriteLine("Sent Authorize message.");
                    Console.ReadLine();
                }
            }
        }
    }
}