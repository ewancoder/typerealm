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
            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                MessageSerializer.Write(stream, new Authorize
                {
                    PlayerId = "player id"
                });

                bytes = stream.ToArray();
            }

            Assert.NotEmpty(bytes);

            using (var stream = new MemoryStream(bytes))
            {
                var message = MessageSerializer.Read(stream) as Authorize;

                Assert.Equal("player id", message.PlayerId);
            }
        }

        [Fact]
        public void ShouldSerializeQuitMessage()
        {
            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                MessageSerializer.Write(stream, new Quit());

                bytes = stream.ToArray();
            }

            Assert.NotEmpty(bytes);

            using (var stream = new MemoryStream(bytes))
            {
                var message = MessageSerializer.Read(stream) as Quit;

                Assert.NotNull(message);
            }
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
    }
}
