using System;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBARRR
{
    public class GAX2_SampleTable : BinarySerializable
    {
        public uint Length { get; set; }
        public Pointer Silence { get; set; }
        public uint Unknown { get; set; }
        public GAX2_Sample[] Entries { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Silence = s.SerializePointer(Silence, name: nameof(Silence));
            Unknown = s.Serialize<uint>(Unknown, name: nameof(Unknown));
            Entries = s.SerializeObjectArray<GAX2_Sample>(Entries, Length, name: nameof(Entries));
        }
    }
}