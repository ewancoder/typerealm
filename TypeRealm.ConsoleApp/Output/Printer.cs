using System;
using TypeRealm.ConsoleApp.Data;
using TypeRealm.Messages;
using TypeRealm.Messages.Movement;

namespace TypeRealm.ConsoleApp.Output
{
    public sealed class Printer : IPrinter
    {
        private readonly IOutput _output;
        private readonly IDataStore _dataStore;

        public Printer(IOutput output, IDataStore dataStore)
        {
            _output = output;
            _dataStore = dataStore;
        }

        public void Print(GameState state, Status status)
        {
            _output.Clear();

            if (state == GameState.Unset || status == null)
            {
                PrintLoadingScreen();
                return;
            }

            if (state == GameState.Disconnected)
            {
                PrintDisconnectedScreen();
                return;
            }

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
                    PrintLocationScreen(status);
                    break;
                case GameState.OnTheRoad:
                    PrintRoadScreen(status);
                    break;
                default:
                    throw new InvalidOperationException("Unknown GameState. Cannot print.");
            }
        }

        private void PrintLoadingScreen()
        {
            _output.WriteLine("LOADING...");
        }

        private void PrintDisconnectedScreen()
        {
            // TODO: Add reason.
            _output.WriteLine("DISCONNECTED.");
        }

        public void PrintReconnectingScreen()
        {
            _output.WriteLine($"Lost connection, trying to reconnect...");
        }

        private void PrintLocationScreen(Status status)
        {
            PrintStatus(status);
        }

        private void PrintRoadScreen(Status status)
        {
            PrintStatus(status);
        }

        // HACK method to show all the data from Status object.
        private void PrintStatus(Status status)
        {
            var location = _dataStore.GetLocation(status.LocationId);

            _output.WriteLine("Neighbors:");
            foreach (var neighbor in status.Neighbors)
            {
                _output.WriteLine(neighbor);
            }
            _output.WriteLine();

            _output.WriteLine($"Name: {status.Name}");
            _output.WriteLine($"Location: {location.Name}: {location.Description}");
            _output.WriteLine("Roads:");

            foreach (var road in status.Roads)
            {
                _output.WriteLine(_dataStore.GetRoad(road).ForwardSide.Name);
                _output.WriteLine(_dataStore.GetRoad(road).BackwardSide.Name);
            }

            if (status.MovementStatus == null)
            {
                _output.WriteLine("Not moving.");
            }
            else
            {
                var road = status.MovementStatus.Direction == MovementDirection.Forward
                    ? _dataStore.GetRoad(status.MovementStatus.RoadId).ForwardSide
                    : _dataStore.GetRoad(status.MovementStatus.RoadId).BackwardSide;

                _output.WriteLine($"Road: {road.Name}: {road.Description}");
                _output.WriteLine($"Direction: {status.MovementStatus.Direction.ToString()}");
                _output.WriteLine($"Distance: {status.MovementStatus.Progress.Distance}");
                _output.WriteLine($"Progress: {status.MovementStatus.Progress.Progress}");
            }
        }

        /*private void PrintNotifications(IEnumerable<string> notifications)
        {
            _output.WriteLine("Notifications:");

            foreach (var notification in notifications)
            {
                _output.WriteLine(notification);
            }

            _output.WriteLine();
        }*/
    }
}
