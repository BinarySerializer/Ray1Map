using BinarySerializer;

namespace R1Engine
{
    public class PS1Klonoa_ArchiveFile_Unk0 : PS1Klonoa_ArchiveFile<PS1Klonoa_ArchiveFile_Unk0.Block>
    {
        public class Block : BinarySerializable
        {
            public Struct[] Values { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Values = s.SerializeObjectArray<Struct>(Values, 200, name: nameof(Values));
            }

            public class Struct : BinarySerializable
            {
                public sbyte SByte_00 { get; set; }
                public sbyte SByte_01 { get; set; }
                public sbyte SByte_02 { get; set; }
                public byte Byte_03 { get; set; }

                public override void SerializeImpl(SerializerObject s)
                {
                    SByte_00 = s.Serialize<sbyte>(SByte_00, name: nameof(SByte_00));
                    SByte_01 = s.Serialize<sbyte>(SByte_01, name: nameof(SByte_01));
                    SByte_02 = s.Serialize<sbyte>(SByte_02, name: nameof(SByte_02));
                    s.SerializePadding(1, logIfNotNull: true);
                }
            }
        }
    }
}