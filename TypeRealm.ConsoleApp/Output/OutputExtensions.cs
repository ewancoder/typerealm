using TypeRealm.ConsoleApp.Typing;

namespace TypeRealm.ConsoleApp.Output
{
    internal static class OutputExtensions
    {
        public static void WriteLine(this IOutput output, string value)
        {
            output.Write(value);
            output.WriteLine();
        }

        public static void WriteLine(this IOutput output, Typer typer)
        {
            output.Write(typer);
            output.WriteLine();
        }
    }
}
