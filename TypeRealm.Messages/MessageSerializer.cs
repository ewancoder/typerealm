using System;
using System.IO;
using ProtoBuf;
using TypeRealm.Messages.Movement;

namespace TypeRealm.Messages
{
    public static class MessageSerializer
    {
        private static readonly IndexedCollection<Type> _messages = new IndexedCollection<Type>(new[]
        {
            typeof(Authorize),
            typeof(Disconnected),
            typeof(EnterRoad),
            typeof(Say),
            typeof(Status),
            typeof(Move),
            typeof(TurnAround),
            typeof(Quit)
        });

        public static object Read(Stream stream)
        {
            object message;

            if (Serializer.NonGeneric.TryDeserializeWithLengthPrefix(
                stream,
                PrefixStyle.Base128,
                fieldNumber => _messages.GetValue(fieldNumber - 1),
                out message))
            {
                return message;
            }

            throw new InvalidOperationException("Could not deserialize message from stream.");
        }

        public static void Write(Stream stream, object message)
        {
            var fieldNumber = _messages.GetIndex(message.GetType()) + 1;

            Serializer.NonGeneric.SerializeWithLengthPrefix(
                stream, message, PrefixStyle.Base128, fieldNumber);
        }
    }
}
