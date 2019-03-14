using System;

namespace TypeRealm.Server
{
    static class Program
    {
        static void Main()
        {
            Console.WriteLine("===== TypeRealm server =====");

            var logger = new ConsoleLogger();
            using (var server = new Server(logger))
            {
                Console.ReadLine();
            }
        }
    }
}
