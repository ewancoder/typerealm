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
        public MovementStatus MovementStatus { get; set; }

        [ProtoMember(4)]
        public List<string> Neighbors { get; set; } = new List<string>();
    }
}
