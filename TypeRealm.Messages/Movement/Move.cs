using ProtoBuf;

namespace TypeRealm.Messages.Movement
{
    [ProtoContract]
    public sealed class Move
    {
        [ProtoMember(1)]
        public int Distance { get; set; }
    }
}
