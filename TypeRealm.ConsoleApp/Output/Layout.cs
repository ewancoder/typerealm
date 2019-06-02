using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TypeRealm.ConsoleApp.Output
{
    internal static class Layout
    {
        private const int TyperSeparatorLength = 3;
        private const int PaddingWidth = 3;
        private const int SeparatorWidth = 5;
        private const int HalfWidth = 90;
        private const int FullWidth = HalfWidth * 2 + SeparatorWidth + PaddingWidth * 2;

        private static string Separator => new string(' ', SeparatorWidth);
        private static string HalfBar => new string('-', HalfWidth);

        public static string Padding => new string(' ', PaddingWidth);
        public static string TyperSeparator => new string(' ', TyperSeparatorLength);
        public static string FullBar => new string('-', FullWidth);
        public static string LeftBar => $"{new string(' ', PaddingWidth) + HalfBar}";
        public static string RightBar => $"{new string(' ', PaddingWidth + HalfWidth + SeparatorWidth)}{HalfBar}";

        public static IEnumerable<string> LayoutToLeft(string text)
        {
            foreach (var line in WrapTextToHalfWidth(text))
            {
                yield return $"{Padding}{line.PadRight(HalfWidth)}{Separator}";
            }
        }

        private static IEnumerable<string> WrapTextToHalfWidth(string text)
        {
            // Cuts last word of wrapped line. WordWrap implementation.
            while (text.Length > 0)
            {
                var part = text.Substring(0, Math.Min(HalfWidth, text.Length));

                if (part.Contains(' ') && part != text)
                {
                    while (part[part.Length - 1] != ' ')
                    {
                        part = part.Substring(0, part.Length - 1);
                    }

                    part = part.Substring(0, part.Length - 1);
                    text = text.Substring(1);
                }

                text = text.Substring(part.Length);

                yield return part;
            }
        }

        public static string WrapFull(string text)
        {
            var builder = new StringBuilder();

            foreach (var line in WrapTextToHalfWidth(text))
            {
                builder.Append($"{line}\n");
            }

            return builder.ToString().Trim('\n');
        }
    }
}
