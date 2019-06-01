using System;

namespace TypeRealm.ConsoleApp.Data
{
    public sealed class InMemoryTextStore : ITextStore
    {
        public string GetActionPhrase()
        {
            return new Random().Next(1, 100) + "action";
        }

        public string GetText(int length)
        {
            return "some very long text";
        }
    }
}
