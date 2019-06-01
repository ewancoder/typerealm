namespace TypeRealm.ConsoleApp.Output
{
    public interface IOutput
    {
        void Clear();
        void WriteLine(string value);
        void WriteLine();
    }
}
