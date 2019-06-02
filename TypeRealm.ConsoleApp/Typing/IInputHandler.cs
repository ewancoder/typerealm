using System.Collections.Generic;

namespace TypeRealm.ConsoleApp.Typing
{
    internal interface IInputHandler
    {
        void Type(char character);
        void Backspace();
        void Escape();
        void Tab();
    }
}
