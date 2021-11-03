using System;
using BinarySerializer;

namespace Ray1Map.Jade {
    public class EVE_Event_AIFunction : BinarySerializable {
        public uint NodesCount { get; set; }
        public AI_Node[] Nodes { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            NodesCount = s.Serialize<uint>(NodesCount, name: nameof(NodesCount));
            Nodes = s.SerializeObjectArray<AI_Node>(Nodes, NodesCount, name: nameof(Nodes));
        }
    }
}