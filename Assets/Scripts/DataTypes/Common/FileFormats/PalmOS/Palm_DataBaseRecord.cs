namespace R1Engine
{
    public class Palm_DatabaseRecord : R1Serializable
    {
        public Palm_Database.DatabaseType Type { get; set; } // Set this before serializing
        public uint Length { get; set; } // Set this after serializing

        public Pointer DataPointer { get; set; }

        public string Name { get; set; }
        public ushort ID { get; set; }

        public byte Attributes { get; set; }
        public byte[] UniqueID { get; set; } // 24-bit integer

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
                Attributes = s.Serialize<byte>(Attributes, name: nameof(Attributes));
                UniqueID = s.SerializeArray<byte>(UniqueID, 3, name: nameof(UniqueID));
            }
        }
    }
}