using System;
using System.IO;
using ProtoBuf;
using Xunit;

namespace TypeRealm.Messages.Tests
{
    public class MessageSerializerTests
    {
        [Fact]
        public void ShouldSerializeAuthorizeMessage()
        {
            ShouldSerialize(new Authorize
            {
                Login = "login",
                Password = "password"
            }, message =>
            {
                Assert.Equal("login", message.Login);
                Assert.Equal("password", message.Password);
            });
        }

        [Fact]
        public void ShouldSerializeQuitMessage()
        {
            ShouldSerialize(new Quit(), message => { });
        }

        [Fact]
        public void ShouldSerializeDisconnectedMessage()
        {
            ShouldSerialize(new Disconnected(), message => { });
        }

        [Fact]
        public void ShouldNotSerializeUnknownMessage()
        {
            using (var stream = new MemoryStream())
            {
                Assert.Throws<InvalidOperationException>(
                    () => MessageSerializer.Write(stream, new object()));
            }

            using (var stream = new MemoryStream())
            {
                Assert.Throws<InvalidOperationException>(
                    () => MessageSerializer.Read(stream));
            }
        }

        [Fact]
        public void ShouldNotDeserializeUnknownFieldNumber()
        {
            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                Serializer.NonGeneric.SerializeWithLengthPrefix(
                    stream, "test", PrefixStyle.Base128, 50000);

                bytes = stream.ToArray();
            }

            using (var stream = new MemoryStream(bytes))
            {
                Assert.Throws<InvalidOperationException>(
                    () => MessageSerializer.Read(stream));
            }
        }

        private static void ShouldSerialize<T>(T message, Action<T> assert)
            where T : class
        {
            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                MessageSerializer.Write(stream, message);

                bytes = stream.ToArray();
            }

            Assert.NotEmpty(bytes);

            using (var stream = new MemoryStream(bytes))
            {
                var deserialized = MessageSerializer.Read(stream) as T;

                Assert.NotNull(deserialized);
                assert(deserialized);
            }
        }
    }
}
