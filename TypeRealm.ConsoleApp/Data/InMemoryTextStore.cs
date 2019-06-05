using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TypeRealm.ConsoleApp.Data
{
    public sealed class InMemoryTextStore : ITextStore
    {
        private readonly string[] _texts;

        public InMemoryTextStore(string[] texts)
        {
            _texts = texts;
        }

        public IEnumerable<string> GetPhrases()
        {
            // Default values for quick testing.
            return GetPhrases(3, 5, 3);
        }

        public string GetText(int length)
        {
            var builder = new StringBuilder();

            foreach (var text in GetTexts())
            {
                builder.Append(text);

                if (builder.Length >= length)
                    break;

                builder.Append(" ");
            }

            return builder.ToString().Substring(0, length);
        }

        private IEnumerable<string> GetPhrases(
            int fromWordLength, int toWordLength, int minimalPhraseLength)
        {
            var phrase = new StringBuilder();

            foreach (var word in GetWords(fromWordLength, toWordLength))
            {
                phrase.Append($"{word} ");

                if (phrase.Length > minimalPhraseLength + 1)
                {
                    yield return phrase.ToString().Trim();
                    phrase.Clear();
                }
            }
        }

        private IEnumerable<string> GetWords(int fromLength, int toLength)
        {
            foreach (var text in GetTexts())
            {
                // TODO: BAD random sorting.
                var words = text.Split(' ')
                    .Distinct()
                    .Where(w => w.Length >= fromLength && w.Length <= toLength)
                    .Where(w => Regex.IsMatch(w, @"^[a-z]+$"))
                    .OrderBy(w => Randomizer.Next());

                foreach (var word in words)
                {
                    yield return word;
                }
            }
        }

        // TODO: This will eat memory. Don't load all the texts.
        private IEnumerable<string> GetTexts()
        {
            var i = 0;

            // TODO: BAD random sorting.
            var texts = _texts.OrderBy(t => Randomizer.Next()).ToList();

            while (true)
            {
                yield return texts[i]
                    .Trim('\r').Trim('\n')
                    .Replace("\r\n", " ")
                    .Replace("\n", " ");

                i++;

                if (i == texts.Count)
                {
                    i = 0;
                    texts = _texts.OrderBy(t => Randomizer.Next()).ToList();
                }
            }
        }
    }
}
