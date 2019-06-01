using TypeRealm.ConsoleApp.Messaging;
using TypeRealm.Messages.Movement;

namespace TypeRealm.ConsoleApp.Typing
{
    internal sealed class RoadTyper : IInputHandler
    {
        private readonly IMessageSender _messages;
        private readonly Typer _typer;
        private int _steps;

        public RoadTyper(IMessageSender messageSender, string text, int progress)
        {
            _messages = messageSender;
            _typer = new Typer(text, progress);
        }

        public void Backspace()
        {
            // Allow backspacing only for errors.
            if (_typer.Error.Length > 0)
                _typer.Backspace();
        }

        public void Escape()
        {
            // Doesn't have escape functionality.
        }

        public void Tab()
        {
            if (_steps > 0)
            {
                _messages.Send(new Move
                {
                    Distance = _steps
                });

                _steps = 0;
            }

            _messages.Send(new TurnAround());
        }

        public void Type(char character)
        {
            _typer.Type(character);

            if (_typer.Error.Length == 0)
                _steps++;

            if (_steps >= 10 || _typer.IsFinishedTyping)
            {
                _messages.Send(new Move
                {
                    Distance = _steps
                });

                _steps = 0;
            }
        }
    }
}
