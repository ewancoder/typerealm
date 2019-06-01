using System.Collections.Generic;
using System.Linq;
using TypeRealm.ConsoleApp.Data;
using TypeRealm.ConsoleApp.Messaging;
using TypeRealm.Messages.Movement;

namespace TypeRealm.ConsoleApp.Typing
{
    internal sealed class LocationTyper : MultiTyper
    {
        private readonly IMessageSender _messages;
        private readonly Dictionary<int, Typer> _roadTypers
            = new Dictionary<int, Typer>();

        public LocationTyper(IMessageSender messageSender, ITextStore textStore, IEnumerable<int> roads)
            : base(textStore)
        {
            _messages = messageSender;

            foreach (var road in roads)
            {
                _roadTypers.Add(road, MakeUniqueTyper());
            }
        }

        protected override void OnTyped(Typer typer)
        {
            var roadId = _roadTypers.Single(t => t.Value == typer).Key;

            _messages.Send(new EnterRoad
            {
                RoadId = roadId
            });
        }
    }
}
