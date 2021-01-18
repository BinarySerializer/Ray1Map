namespace R1Engine
{
    public class GBACrash_Isometric_CharacterInfo : R1Serializable
    {
        public Pointer NamePointer { get; set; }
        public uint Uint_04 { get; set; }
        public Pointer ModelPointer { get; set; }
        public byte[] Bytes_0C { get; set; }

        // Serialized from pointers

        public string Name { get; set; }
        public GBACrash_Isometric_CharacterModel Model { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            NamePointer = s.SerializePointer(NamePointer, name: nameof(NamePointer));
            Uint_04 = s.Serialize<uint>(Uint_04, name: nameof(Uint_04));
            ModelPointer = s.SerializePointer(ModelPointer, name: nameof(ModelPointer));
            Bytes_0C = s.SerializeArray<byte>(Bytes_0C, 68 - 4 * 3, name: nameof(Bytes_0C));

            Name = s.DoAt(NamePointer, () => s.SerializeString(Name, name: nameof(Name)));
            Model = s.DoAt(ModelPointer, () => s.SerializeObject<GBACrash_Isometric_CharacterModel>(Model, name: nameof(Model)));
        }
    }
}