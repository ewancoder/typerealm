using TypeRealm.ConsoleApp.Typing;

namespace TypeRealm.ConsoleApp.Output
{
    internal interface IOutput
    {
        /// <summary>
        /// Clears output.
        /// </summary>
        void Clear();

        /// <summary>
        /// Writes empty line (or appends a newline after a written line).
        /// </summary>
        void WriteLine();

        /// <summary>
        /// Writes a value without new line at the end.
        /// </summary>
        /// <param name="value">Value to write.</param>
        void Write(string value);

        /// <summary>
        /// Writes a typer without new line at the end.
        /// </summary>
        /// <param name="typer">Typer to write.</param>
        void Write(Typer typer);
    }
}
