using System;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBARRR
{
    public class GAX2_PatternHeader : BinarySerializable
    {
        public ushort SequenceOffset { get; set; }
        public sbyte Transpose { get; set; }
        public byte Reserved { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            SequenceOffset = s.Serialize<ushort>(SequenceOffset, name: nameof(SequenceOffset));
            Transpose = s.Serialize<sbyte>(Transpose, name: nameof(Transpose));
            Reserved = s.Serialize<byte>(Reserved, name: nameof(Reserved));
        }
    }
}