using ProtoBuf;

namespace TypeRealm.Messages
{
    [ProtoContract]
    public sealed class Authorize
    {
        [ProtoMember(1)]
        public string Login { get; set; }

        [ProtoMember(2)]
        public string Password { get; set; }

        [ProtoMember(3)]
        public string PlayerName { get; set; }
    }
}
