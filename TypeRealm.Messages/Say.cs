using ProtoBuf;

namespace TypeRealm.Messages
{
    [ProtoContract]
    public sealed class Say
    {
        [ProtoMember(1)]
        public string Message { get; set; }
    }
}
