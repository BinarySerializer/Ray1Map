namespace R1Engine
{
    public class GBAIsometric_RHR_SpriteSet : R1Serializable
    {
        public Pointer Pointer_00 { get; set; }
        public Pointer Pointer_04 { get; set; }
        public Pointer Pointer_08 { get; set; }
        public uint Uint_0C { get; set; }
        public Pointer NamePointer { get; set; }

        public string Name { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Pointer_00 = s.SerializePointer(Pointer_00, name: nameof(Pointer_00));
            Pointer_04 = s.SerializePointer(Pointer_04, name: nameof(Pointer_04));
            Pointer_08 = s.SerializePointer(Pointer_08, name: nameof(Pointer_08));
            Uint_0C = s.Serialize<uint>(Uint_0C, name: nameof(Uint_0C));
            NamePointer = s.SerializePointer(NamePointer, name: nameof(NamePointer));

            Name = s.DoAt(NamePointer, () => s.SerializeString(Name, name: nameof(Name)));
        }
    }
}