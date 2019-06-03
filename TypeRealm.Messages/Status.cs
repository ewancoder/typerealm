using System;
using System.Collections.Generic;
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
        public List<RoadStatus> Roads { get; set; } = new List<RoadStatus>();

        [ProtoMember(4)]
        public MovementStatus MovementStatus { get; set; }

        [ProtoMember(5)]
        public List<string> Neighbors { get; set; } = new List<string>();
    }
}
