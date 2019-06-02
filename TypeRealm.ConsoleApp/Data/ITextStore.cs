using System.Collections.Generic;

namespace TypeRealm.ConsoleApp.Data
{
    public interface ITextStore
    {
        /// <summary>
        /// Gets random words or phrases that you need to type to select / click something.
        /// </summary>
        IEnumerable<string> GetPhrases();

        /// <summary>
        /// Gets text paragraph that you need to type in order to get somewhere.
        /// </summary>
        /// <param name="length">Length of paragraph. It's usually the distance of the road.</param>
        string GetText(int length);
    }
}
