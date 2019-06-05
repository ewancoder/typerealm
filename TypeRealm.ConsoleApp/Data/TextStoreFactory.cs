using System.IO;
using System.Linq;

namespace TypeRealm.ConsoleApp.Data
{
    public sealed class TextStoreFactory
    {
        public ITextStore LoadFromFile(string fileName)
        {
            // TODO: Consider not loading all texts into memory.
            var texts = File.ReadAllLines(fileName)
                .Where(line => line != string.Empty)
                .ToArray();

            return new InMemoryTextStore(texts);
        }
    }
}
