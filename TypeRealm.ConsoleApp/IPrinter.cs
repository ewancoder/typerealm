using System.Collections.Generic;
using TypeRealm.Messages;

namespace TypeRealm.ConsoleApp
{
    public interface IPrinter
    {
        void Print(Status status, IEnumerable<string> notifications);
        void DisconnectedWithReason(string reason);
    }
}
