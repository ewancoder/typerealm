﻿using System;
using System.Collections.Generic;
using TypeRealm.ConsoleApp.Data;
using TypeRealm.ConsoleApp.Messaging;
using TypeRealm.ConsoleApp.Output;
using TypeRealm.ConsoleApp.Typing;
using TypeRealm.Messages;

namespace TypeRealm.ConsoleApp
{
    public enum GameState
    {
        Reconnecting = -2,
        Disconnected = -1,
        Unset = 0,
        AtLocation = 1,
        OnTheRoad = 2
    }

    internal sealed class Game
    {
        private readonly ITextStore _texts;
        private readonly IMessageSender _messages;
        private readonly IPrinter _printer;
        private readonly Queue<string> _notifications;

        private GameState _state;
        private IInputHandler _inputHandler;
        private Status _status;

        public Game(ITextStore textStore, IMessageSender messageSender, IPrinter printer)
        {
            _texts = textStore;
            _messages = messageSender;
            _printer = printer;
            _notifications = new Queue<string>();
        }

        public void Input(ConsoleKeyInfo key)
        {
            // Don't accept input until connected and got status.
            if (_state == GameState.Unset || _state == GameState.Disconnected)
                return;

            switch (key.Key)
            {
                case ConsoleKey.Backspace:
                    _inputHandler.Backspace();
                    break;
                case ConsoleKey.Escape:
                    _inputHandler.Escape();
                    break;
                case ConsoleKey.Tab:
                    _inputHandler.Tab();
                    break;
                default:
                    _inputHandler.Type(key.KeyChar);
                    break;
            }

            Print();
        }

        /// <summary>
        /// Detects changes in status and updates client state.
        /// </summary>
        /// <param name="status">New status.</param>
        public void UpdateState(Status status)
        {
            //var previousStatus = Status; // Can be useful for change detection.
            _status = status;

            if (status.MovementStatus != null)
            {
                if (_state != GameState.OnTheRoad)
                {
                    var distance = status.MovementStatus.Progress.Distance;
                    var progress = status.MovementStatus.Progress.Progress;

                    var text = _texts.GetText(distance);
                    var roadTyper = new RoadTyper(_messages, text, progress);

                    SetRoad(roadTyper);
                    return;
                }

                // TODO: No way to tell if some status has changed at server side.
                // If we turned around and the way back has the same distance,
                // everything will stay the same except for progress. BUT:
                // We can't refresh progress here because if some other player walks on the road
                // and we already typed 9/10 characters, the progress will be reset
                // by server side.
            }

            if (_state != GameState.AtLocation)
            {
                var locationTyper = new LocationTyper(_messages, _texts, status.Roads);
                SetLocation(locationTyper);
            }

            Print();
        }

        public void Disconnect()
        {
            _state = GameState.Disconnected;
            Print();
        }

        public void Notify(string message)
        {
            _notifications.Enqueue(message);

            if (_notifications.Count > 5)
                _notifications.Dequeue();

            Print();
        }

        public void Reconnecting()
        {
            _state = GameState.Reconnecting;
            Print();
        }

        private void SetLocation(LocationTyper locationTyper)
        {
            _state = GameState.AtLocation;
            _inputHandler = locationTyper;
        }

        private void SetRoad(RoadTyper roadTyper)
        {
            _state = GameState.OnTheRoad;
            _inputHandler = roadTyper;
        }

        private void Print()
        {
            _printer.Print(_state, _status);
        }
    }
}
