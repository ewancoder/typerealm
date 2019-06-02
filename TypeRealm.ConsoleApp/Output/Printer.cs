using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypeRealm.ConsoleApp.Data;
using TypeRealm.ConsoleApp.Typing;
using TypeRealm.Messages;

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

    internal sealed class Printer : IPrinter
    {
        private readonly IOutput _output;
        private readonly IDataStore _dataStore;
        private readonly object _lock = new object();

        public Printer(IOutput output, IDataStore dataStore)
        {
            _output = output;
            _dataStore = dataStore;
        }

        public void Print(GameState state, Status status, LocationTyper locationTyper, RoadTyper roadTyper, IEnumerable<string> notifications)
        {
            lock (_lock)
            {
                _output.Clear();

                switch (state)
                {
                    case GameState.Unset:
                        PrintLoadingScreen();
                        break;
                    case GameState.Disconnected:
                        PrintDisconnectedScreen();
                        break;
                    case GameState.Reconnecting:
                        PrintReconnectingScreen();
                        break;
                    case GameState.AtLocation:
                        PrintLocationScreen(status, locationTyper, notifications);
                        break;
                    case GameState.OnTheRoad:
                        PrintRoadScreen(status, roadTyper, notifications);
                        break;
                    default:
                        throw new InvalidOperationException("Unknown GameState. Cannot print.");
                }
            }
        }

        private void PrintLoadingScreen()
        {
            PrintLeft("LOADING...");
        }

        private void PrintDisconnectedScreen()
        {
            // TODO: Add reason.
            PrintLeft("DISCONNECTED.");
        }

        public void PrintReconnectingScreen()
        {
            PrintLeft($"Lost connection, trying to reconnect...");
        }

        private void PrintLocationScreen(Status status, LocationTyper locationTyper, IEnumerable<string> notifications)
        {
            PrintHeader(status.Name);

            var location = _dataStore.GetLocation(status.LocationId);

            _output.WriteLine(Layout.FullBar);
            _output.WriteLine();
            PrintLeft(location.Name);
            _output.WriteLine();
            PrintToLeft(location.Description);
            _output.WriteLine(Layout.RightBar);

            var roads = status.Roads
                .Select(roadId => new
                {
                    RoadId = roadId,
                    Data = _dataStore.GetRoadFrom(roadId, status.LocationId)
                })
                .Select(road => new
                {
                    RoadId = road.RoadId,
                    Name = road.Data.Name,
                    Description = road.Data.Description
                })
                .OrderBy(x => x.RoadId);

            foreach (var road in roads)
            {
                var lines = Layout.LayoutToLeft(road.Description);

                var isFirstLine = true;
                foreach (var line in lines)
                {
                    if (isFirstLine)
                    {
                        _output.Write($"{line}{road.Name}{Layout.TyperSeparator}");
                        _output.Write(locationTyper.GetTyperFor(road.RoadId));
                        _output.WriteLine();
                    }
                    else
                    {
                        _output.WriteLine(line);
                    }

                    isFirstLine = false;
                }

                _output.WriteLine();
            }

            _output.WriteLine(Layout.FullBar);
            _output.WriteLine();

            foreach (var neighbor in status.Neighbors)
            {
                PrintLeft(neighbor);
            }

            _output.WriteLine(Layout.FullBar);
            _output.WriteLine();

            PrintLeft("Notifications:");
            PrintNotifications(notifications);
        }

        private void PrintToLeft(string text)
        {
            foreach (var line in Layout.LayoutToLeft(text))
            {
                _output.WriteLine(line);
            }
        }

        private void PrintRoadScreen(Status status, RoadTyper roadTyper, IEnumerable<string> notifications)
        {
            PrintHeader(status.Name);

            var road = _dataStore.GetRoadFrom(status.MovementStatus.RoadId, status.LocationId);

            _output.WriteLine(Layout.FullBar);
            _output.WriteLine();
            PrintLeft(road.Name);
            _output.WriteLine();
            PrintToLeft(road.Description);
            _output.WriteLine();
            _output.WriteLine(Layout.FullBar);

            _output.WriteLine();
            _output.Write(Layout.Padding);
            _output.Write(roadTyper.Typer);
            _output.WriteLine();
            _output.WriteLine(Layout.FullBar);
            _output.WriteLine();

            PrintLeft("Notifications:");
            PrintNotifications(notifications);
        }

        private void PrintHeader(string header)
        {
            _output.WriteLine(Layout.FullBar);
            _output.WriteLine();
            PrintLeft(header);
            _output.WriteLine();
            _output.WriteLine(Layout.FullBar);
        }

        private void PrintLeft(string line)
        {
            _output.WriteLine($"{Layout.Padding}{line}");
        }

        private void PrintNotifications(IEnumerable<string> notifications)
        {
            foreach (var notification in notifications)
            {
                PrintLeft(notification);
            }
        }
    }
}
