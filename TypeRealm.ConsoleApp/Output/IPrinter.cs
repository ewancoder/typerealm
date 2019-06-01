using TypeRealm.Messages;

namespace TypeRealm.ConsoleApp.Output
{
    public interface IPrinter
    {
        /*void Print(Status status, IEnumerable<string> notifications);
        void DisconnectedWithReason(string reason);
        void Reconnecting();*/
        //void PrintLoadingScreen();
        //void PrintDisconnectedScreen();
        void Print(GameState state, Status status);
    }
}
