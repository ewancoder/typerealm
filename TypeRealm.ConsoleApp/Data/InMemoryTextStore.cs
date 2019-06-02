using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TypeRealm.ConsoleApp.Data
{
    public sealed class InMemoryTextStore : ITextStore
    {
        private static readonly string[] _texts = new[]
        {
            @"
You might be one of those people that perform very well when it comes to sports.
If you are and you are in high school, you might have the idea that your ability
at a given sport could end up putting you in line for a sports scholarship that
will either entirely pay for college, or at least partially pay for your higher
education. However, there is more to getting a scholarship than just being good
at sports. One thing you have to understand is that with sports scholarships
there are a few ways to get them.",
            @"
Before we begin, I would like to briefly explain why open source is important.
When we think of the software we use to write, most people think of programs
written by big corporations like Microsoft Word Scrivener. These programs cost
money and are built by large teams of programmers. At anytime these companies
and these products could go away and not be available anymore.
Open source programs are a little different. The vast majority are free. The
code used to create them is freely available, meaning that if the original
developer stops work on his project, someone else can take it up. It also means
that if you have some coding knowledge, you too can contribute to the project.
Open source developers typically respond much quicker to their users than huge
multinational organizations."
        };

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

        private static IEnumerable<string> GetPhrases(
            int fromWordLength, int toWordLength, int minimalPhraseLength)
        {
            var phrase = new StringBuilder();

            foreach (var word in GetWords(fromWordLength, toWordLength))
            {
                phrase.Append(word);

                if (phrase.Length > minimalPhraseLength)
                {
                    yield return phrase.ToString();
                    phrase.Clear();
                }
            }
        }

        private static IEnumerable<string> GetWords(int fromLength, int toLength)
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
        private static IEnumerable<string> GetTexts()
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

    // TODO: Move to common assembly.
    internal static class Randomizer
    {
        private static readonly Random _global = new Random();
        [ThreadStatic] private static Random _local;

        private static Random Local
        {
            get
            {
                if (_local == null)
                {
                    lock (_global)
                    {
                        if (_local == null)
                        {
                            var seed = _global.Next();
                            _local = new Random(seed);
                        }
                    }
                }

                return _local;
            }
        }

        public static int Next()
        {
            return Local.Next();
        }

        public static int Next(int min, int max)
        {
            return Local.Next(min, max);
        }
    }
}
