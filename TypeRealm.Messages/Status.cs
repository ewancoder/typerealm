using ProtoBuf;
using TypeRealm.Messages.Movement;

namespace TypeRealm.Messages
{
    [ProtoContract]
    public sealed class Status
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public int LocationId { get; set; }

        [ProtoMember(3)]
        public MovementStatus MovementStatus { get; set; }
    }
}
