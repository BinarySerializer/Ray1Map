using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_RHR_FlagSprite : BinarySerializable
    {
        public uint LocID { get; set; }
        public Pointer SpritePointer { get; set; }
        public Pointer PalettePointer0 { get; set; }
        public Pointer PalettePointer1 { get; set; }

        public GBAIsometric_RHR_Sprite Sprite { get; set; }
        public RGBA5551Color[] Palette0 { get; set; }
        public RGBA5551Color[] Palette1 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LocID = s.Serialize<uint>(LocID, name: nameof(LocID));
            SpritePointer = s.SerializePointer(SpritePointer, name: nameof(SpritePointer));
            PalettePointer0 = s.SerializePointer(PalettePointer0, name: nameof(PalettePointer0));
            PalettePointer1 = s.SerializePointer(PalettePointer1, name: nameof(PalettePointer1));

            Sprite = s.DoAt(SpritePointer, () => s.SerializeObject<GBAIsometric_RHR_Sprite>(Sprite, name: nameof(Sprite)));
            Palette0 = s.DoAt(PalettePointer0, () => s.SerializeObjectArray<RGBA5551Color>(Palette0, 16, name: nameof(Palette0)));
            Palette1 = s.DoAt(PalettePointer1, () => s.SerializeObjectArray<RGBA5551Color>(Palette1, 16, name: nameof(Palette1)));
        }
    }
}