using System.Collections.Generic;
using TypeRealm.ConsoleApp.Data;
using TypeRealm.Messages;
using TypeRealm.Messages.Movement;

namespace TypeRealm.ConsoleApp
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

        public void Print(Status status, IEnumerable<string> notifications)
        {
            _output.Clear();

            if (status == null)
            {
                _output.WriteLine("Loading...");
                return;
            }

            PrintNotifications(notifications);
            PrintStatus(status);
        }

        public void DisconnectedWithReason(string reason)
        {
            _output.Clear();
            _output.WriteLine($"Disconnected with reason: {reason}.");
        }

        private void PrintNotifications(IEnumerable<string> notifications)
        {
            _output.WriteLine("Notifications:");
            foreach (var notification in notifications)
            {
                _output.WriteLine(notification);
            }
            _output.WriteLine();
        }

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
    }
}
