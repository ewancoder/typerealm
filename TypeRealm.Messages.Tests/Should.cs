using System;
using System.IO;
using Xunit;

namespace TypeRealm.Messages.Tests
{
    public static class Should
    {
        public static void Serialize<T>(T message, Action<T> assert)
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
