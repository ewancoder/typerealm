using System;
using System.IO;
using ProtoBuf;
using Xunit;

namespace TypeRealm.Messages.Tests
{
    public sealed class MessageSerializerTests
    {
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
