using System.Collections.Generic;
using TypeRealm.ConsoleApp.Typing;
using TypeRealm.Messages;

namespace TypeRealm.ConsoleApp.Output
{
    internal interface IPrinter
    {
        void Print(GameState state, Status status, LocationTyper locationTyper, RoadTyper roadTyper, IEnumerable<string> notifications);
    }
}
