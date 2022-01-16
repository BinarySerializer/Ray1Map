using System;
using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_IceDragon_DataPointer : GBAIsometric_IceDragon_DataRef
    {
        public GBAIsometric_IceDragon_CompressionType Pre_CompressionType { get; set; }

        public Pointer DataPointer { get; set; }

        public override void DoAt(Action<long> action)
        {
            BinaryDeserializer s = Context.Deserializer;
            s.DoAt(DataPointer, () => GBAIsometric_IceDragon_Resources.DoAtResource(s, Pre_CompressionType, action));
        }

        public override void SerializeImpl(SerializerObject s)
        {
            DataPointer = s.SerializePointer(DataPointer, name: nameof(DataPointer));
        }
    }
}