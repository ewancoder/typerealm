using System;
using TypeRealm.ConsoleApp.Typing;

namespace TypeRealm.ConsoleApp.Output
{
    internal sealed class ConsoleOutput : IOutput
    {
        public void Clear()
        {
            Console.Clear();
        }

        public void WriteLine()
        {
            Console.WriteLine();
        }

        public void Write(string value)
        {
            Console.Write(value);
        }

        public void Write(Typer typer)
        {
            // Highlight already typed part.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.Write(typer.Typed);

            // Highlight mistakes.
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(typer.Error);

            // Highlight the rest of text if typer is focused.
            if (typer.IsTyping)
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            else
                Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(typer.NotTyped);

            // Revert console color to default.
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
