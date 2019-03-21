using ProtoBuf;

namespace TypeRealm.Messages
{
    public enum DisconnectReason
    {
        None = 0,
        InvalidCredentials = 1
    }

    [ProtoContract]
    public sealed class Disconnected
    {
        [ProtoMember(1)]
        public DisconnectReason Reason { get; set; }
    }
}
