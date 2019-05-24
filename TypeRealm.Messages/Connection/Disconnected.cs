using ProtoBuf;

namespace TypeRealm.Messages.Connection
{
    public enum DisconnectReason
    {
        None = 0,
        InvalidCredentials = 1,
        CouldNotConnect = 2
    }

    [ProtoContract]
    public sealed class Disconnected
    {
        [ProtoMember(1)]
        public DisconnectReason Reason { get; set; }
    }
}
