using System;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class GAX2_Sample : BinarySerializable {
        public Pointer SampleOffset { get; set; }
        public uint Length { get; set; }

        public byte[] Sample { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            SampleOffset = s.SerializePointer(SampleOffset, name: nameof(SampleOffset));
            Length = s.Serialize<uint>(Length, name: nameof(Length));

            s.DoAt(SampleOffset, () => {
                Sample = s.SerializeArray<byte>(Sample, Length, name: nameof(Sample));
            });
        }
    }
}