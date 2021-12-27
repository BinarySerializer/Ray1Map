using System;
using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_DataPointer : GBAIsometric_Spyro_DataRef
    {
        public GBAIsometric_Spyro_CompressionType Pre_CompressionType { get; set; }

        public Pointer DataPointer { get; set; }

        public override void DoAt(Action<long> action)
        {
            BinaryDeserializer s = Context.Deserializer;
            s.DoAt(DataPointer, () => GBAIsometric_Spyro_DataTable.DoAtData(s, Pre_CompressionType, action));
        }

        public override void SerializeImpl(SerializerObject s)
        {
            DataPointer = s.SerializePointer(DataPointer, name: nameof(DataPointer));
        }
    }
}