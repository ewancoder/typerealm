using System;

namespace TypeRealm.ConsoleApp
{
    public sealed class ConsoleOutput : IOutput
    {
        public void Clear()
        {
            Console.Clear();
        }

        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }
    }
}
