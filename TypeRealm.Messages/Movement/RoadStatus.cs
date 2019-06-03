using ProtoBuf;

namespace TypeRealm.Messages.Movement
{
    [ProtoContract]
    public sealed class RoadStatus
    {
        [ProtoMember(1)]
        public int RoadId { get; set; }

        [ProtoMember(2)]
        public MovementDirection Direction { get; set; }
    }
}
