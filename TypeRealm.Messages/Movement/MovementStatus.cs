using ProtoBuf;

namespace TypeRealm.Messages.Movement
{
    [ProtoContract]
    public sealed class MovementStatus
    {
        [ProtoMember(1)]
        public int RoadId { get; set; }

        [ProtoMember(2)]
        public MovementDirection Direction { get; set; }

        [ProtoMember(3)]
        public int ProgressPercentage { get; set; }
    }
}
