using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_Isometric_CharacterModel : BinarySerializable
    {
        public uint Magic { get; set; } // TH2
        public uint Block1_Length { get; set; } // Length / 2
        public uint Uint_08 { get; set; }
        public uint Block1_Structs_Count { get; set; }
        public uint Block0_Structs_Count { get; set; }
        public uint Block0_Offset { get; set; }
        public uint Block1_Offset { get; set; }
        public uint Uint_1C { get; set; }

        public Block0_Struct[] Block0_Structs { get; set; }
        public byte Block1_Byte_00 { get; set; }
        public byte Block1_Byte_01 { get; set; }
        public byte Block1_Byte_02 { get; set; }
        public byte Block1_Byte_03 { get; set; }
        public Block1_Struct[] Block1_Structs { get; set; }
        public byte[] Block1_Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Magic = s.Serialize<uint>(Magic, name: nameof(Magic));
            Block1_Length = s.Serialize<uint>(Block1_Length, name: nameof(Block1_Length));
            Uint_08 = s.Serialize<uint>(Uint_08, name: nameof(Uint_08));
            Block1_Structs_Count = s.Serialize<uint>(Block1_Structs_Count, name: nameof(Block1_Structs_Count));
            Block0_Structs_Count = s.Serialize<uint>(Block0_Structs_Count, name: nameof(Block0_Structs_Count));
            Block0_Offset = s.Serialize<uint>(Block0_Offset, name: nameof(Block0_Offset));
            Block1_Offset = s.Serialize<uint>(Block1_Offset, name: nameof(Block1_Offset));
            Uint_1C = s.Serialize<uint>(Uint_1C, name: nameof(Uint_1C));

            Block0_Structs = s.DoAt(Offset + Block0_Offset, () => s.SerializeObjectArray<Block0_Struct>(Block0_Structs, Block0_Structs_Count, name: nameof(Block0_Structs)));
            s.DoAt(Offset + Block1_Offset, () =>
            {
                Block1_Byte_00 = s.Serialize<byte>(Block1_Byte_00, name: nameof(Block1_Byte_00));
                Block1_Byte_01 = s.Serialize<byte>(Block1_Byte_01, name: nameof(Block1_Byte_01));
                Block1_Byte_02 = s.Serialize<byte>(Block1_Byte_02, name: nameof(Block1_Byte_02));
                Block1_Byte_03 = s.Serialize<byte>(Block1_Byte_03, name: nameof(Block1_Byte_03));

                Block1_Structs = s.SerializeObjectArray<Block1_Struct>(Block1_Structs, Block1_Structs_Count, name: nameof(Block1_Structs));
                s.Align();
                Block1_Data = s.SerializeArray<byte>(Block1_Data, (Offset + Block1_Offset + Block1_Length * 2) - s.CurrentPointer, name: nameof(Block1_Data));
            });
        }

        public class Block0_Struct : BinarySerializable
        {
            public byte[] Data { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Data = s.SerializeArray<byte>(Data, 8, name: nameof(Data));
            }
        }
        public class Block1_Struct : BinarySerializable
        {
            public byte[] Data { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Data = s.SerializeArray<byte>(Data, 3, name: nameof(Data));
            }
        }
    }
}