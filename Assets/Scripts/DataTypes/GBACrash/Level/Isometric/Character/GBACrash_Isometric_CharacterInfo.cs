namespace R1Engine
{
    public class GBACrash_Isometric_CharacterInfo : R1Serializable
    {
        public Pointer NamePointer { get; set; }
        public uint Uint_04 { get; set; }
        public Pointer Pointer_08 { get; set; }
        public byte[] Bytes_0C { get; set; }

        // Serialized from pointers

        public string Name { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            NamePointer = s.SerializePointer(NamePointer, name: nameof(NamePointer));
            Uint_04 = s.Serialize<uint>(Uint_04, name: nameof(Uint_04));
            Pointer_08 = s.SerializePointer(Pointer_08, name: nameof(Pointer_08));
            Bytes_0C = s.SerializeArray<byte>(Bytes_0C, 68 - 4 * 3, name: nameof(Bytes_0C));

            Name = s.DoAt(NamePointer, () => s.SerializeString(Name, name: nameof(Name)));
        }
    }
}