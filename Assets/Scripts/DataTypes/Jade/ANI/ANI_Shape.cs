using BinarySerializer;

namespace R1Engine.Jade
{
    public class ANI_Shape : Jade_File
    {
        public byte StructsCount { get; set; } // Max 64
        public ANI_ShapeStruct[] Structs { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            StructsCount = s.Serialize<byte>(StructsCount, name: nameof(StructsCount));
            Structs = s.SerializeObjectArray(Structs, StructsCount, name: nameof(Structs));
        }

        public class ANI_ShapeStruct : BinarySerializable
        {
            public sbyte Byte_00 { get; set; }
            public byte Byte_01 { get; set; }
            public byte Byte_02 { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Byte_00 = s.Serialize<sbyte>(Byte_00, name: nameof(Byte_00));

                if (Byte_00 >= 0 && Byte_00 < 64)
                {
                    Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
                    Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
                }
            }
        }
    }
}