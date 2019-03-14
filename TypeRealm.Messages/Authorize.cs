using ProtoBuf;

namespace TypeRealm.Messages
{
    [ProtoContract]
    public sealed class Authorize
    {
        [ProtoMember(1)]
        public string PlayerId { get; set; }
    }
}
