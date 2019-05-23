using ProtoBuf;

namespace TypeRealm.Messages.Movement
{
    [ProtoContract]
    public sealed class EnterRoad
    {
        [ProtoMember(1)]
        public int RoadId { get; set; }
    }
}
