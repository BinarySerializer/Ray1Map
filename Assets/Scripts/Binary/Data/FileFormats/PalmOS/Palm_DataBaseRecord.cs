using BinarySerializer;

namespace Ray1Map
{
    public class Palm_DatabaseRecord : BinarySerializable
    {
        public Palm_Database.DatabaseType Type { get; set; } // Set this before serializing
        public long Length { get; set; } // Set this after serializing

        public Pointer DataPointer { get; set; }

        public string Name { get; set; }
        public ushort ID { get; set; }

        public byte Attributes { get; set; }
        public uint UniqueID { get; set; } // 24-bit integer

        public override void SerializeImpl(SerializerObject s)
        {
            if (Type == Palm_Database.DatabaseType.PRC)
            {
                Name = s.SerializeString(Name, 4, name: nameof(Name));
                ID = s.Serialize<ushort>(ID, name: nameof(ID));
                DataPointer = s.SerializePointer(DataPointer, name: nameof(DataPointer));
            }
            else
            {
                DataPointer = s.SerializePointer(DataPointer, name: nameof(DataPointer));
                s.DoBits<uint>(b => 
                {
                    UniqueID = b.SerializeBits<uint>(UniqueID, 24, name: nameof(UniqueID));
                    Attributes = b.SerializeBits<byte>(Attributes, 8, name: nameof(Attributes));
                });
            }
        }
    }
}