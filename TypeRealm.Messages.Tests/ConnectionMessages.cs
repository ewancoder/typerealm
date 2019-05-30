using TypeRealm.Messages.Connection;
using Xunit;

namespace TypeRealm.Messages.Tests
{
    public sealed class ConnectionMessages
    {
        [Fact]
        public void ShouldSerializeAuthorizeMessage()
        {
            Should.Serialize(new Authorize
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
        public void ShouldSerializeDisconnectedMessage()
        {
            Should.Serialize(new Disconnected
            {
                Reason = DisconnectReason.InvalidCredentials
            }, message =>
            {
                Assert.Equal(DisconnectReason.InvalidCredentials, message.Reason);
            });
        }

        [Fact]
        public void ShouldSerializeHeartbeatMessage()
        {
            Should.Serialize(new Heartbeat(), message => { });
        }

        [Fact]
        public void ShouldSerializeQuitMessage()
        {
            Should.Serialize(new Quit(), message => { });
        }
    }
}
