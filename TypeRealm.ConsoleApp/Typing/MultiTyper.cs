using System;
using System.Collections.Generic;
using System.Linq;
using TypeRealm.ConsoleApp.Data;

namespace TypeRealm.ConsoleApp.Typing
{
    internal abstract class MultiTyper : IInputHandler
    {
        private readonly ITextStore _texts;
        private readonly Dictionary<char, Typer> _typers
            = new Dictionary<char, Typer>();

        private Typer _focusedTyper;

        protected MultiTyper(ITextStore textStore)
        {
            _texts = textStore;
        }

        public void Type(char character)
        {
            if (_focusedTyper == null)
            {
                if (!_typers.ContainsKey(character))
                    return;

                _focusedTyper = _typers[character];
            }

            _focusedTyper.Type(character);

            if (_focusedTyper.IsFinishedTyping)
            {
                ResetUniqueFocusedTyper();

                OnTyped(_focusedTyper);
                _focusedTyper = null;
            }
        }

        public void Backspace()
        {
            if (_focusedTyper == null)
                return;

            _focusedTyper.Backspace();

            if (!_focusedTyper.IsTyping)
                _focusedTyper = null;
        }

        public void Escape()
        {
            if (_focusedTyper != null)
            {
                _focusedTyper.Reset();
                _focusedTyper = null;
            }
        }

        public virtual void Tab() { }

        protected abstract void OnTyped(Typer typer);

        protected Typer MakeUniqueTyper()
        {
            var word = GetUniqueWord();

            var typer = new Typer(word);
            _typers.Add(word[0], typer);

            return typer;
        }

        private void ResetUniqueFocusedTyper()
        {
            var word = GetUniqueWord();

            _focusedTyper.Reset(word);
            _typers.Remove(_typers.Single(x => x.Value == _focusedTyper).Key);
            _typers.Add(word[0], _focusedTyper);
        }

        private string GetUniqueWord()
        {
            // TODO: Get enumerator and use it to get words one by one.
            // But then it will need to be disposable.
            // Maybe make a shared service for this.
            foreach (var word in _texts.GetPhrases())
            {
                if (_typers.ContainsKey(word[0]))
                    continue;

                return word;
            }

            throw new InvalidOperationException("Unique word for MultiTyper is not found.");
        }
    }
}
