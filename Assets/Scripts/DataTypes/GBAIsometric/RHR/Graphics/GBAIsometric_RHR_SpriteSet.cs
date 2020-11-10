namespace R1Engine
{
    public class GBAIsometric_RHR_SpriteSet : R1Serializable
    {
        public Pointer<GBAIsometric_RHR_GraphicsData> GraphicsDataPointer { get; set; }
        public Pointer Pointer_04 { get; set; }
        public Pointer Pointer_08 { get; set; }
        public byte Byte_0C { get; set; }
        public byte SpriteCount { get; set; }
        public Pointer NamePointer { get; set; }

        public string Name { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            GraphicsDataPointer = s.SerializePointer<GBAIsometric_RHR_GraphicsData>(GraphicsDataPointer, resolve: true, name: nameof(GraphicsDataPointer));
            Pointer_04 = s.SerializePointer(Pointer_04, name: nameof(Pointer_04));
            Pointer_08 = s.SerializePointer(Pointer_08, name: nameof(Pointer_08));
            Byte_0C = s.Serialize<byte>(Byte_0C, name: nameof(Byte_0C));
            SpriteCount = s.Serialize<byte>(SpriteCount, name: nameof(SpriteCount));
            s.Serialize<ushort>(default, name: "Padding");
            NamePointer = s.SerializePointer(NamePointer, name: nameof(NamePointer));

            Name = s.DoAt(NamePointer, () => s.SerializeString(Name, name: nameof(Name)));
        }
    }
}