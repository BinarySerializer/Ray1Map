namespace BinarySerializer.KlonoaDTP
{
    // TODO: Not totally sure about this, but some backgrounds should animate so this might be it?
    public class PS1Klonoa_File_TileAnimations : PS1Klonoa_BaseFile
    {
        public int Count { get; set; }
        public Entry[] Entries { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Count = s.Serialize<int>(Count, name: nameof(Count));
            Entries = s.SerializeObjectArray<Entry>(Entries, Count, name: nameof(Entries));
        }

        public class Entry : BinarySerializable
        {
            public byte[] Data { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Data = s.SerializeArray<byte>(Data, 64, name: nameof(Data));
            }
        }
    }
}