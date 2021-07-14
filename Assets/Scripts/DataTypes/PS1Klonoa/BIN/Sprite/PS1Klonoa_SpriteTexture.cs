namespace BinarySerializer.KlonoaDTP
{
    /// <summary>
    /// A texture for a sprite
    /// </summary>
    public class PS1Klonoa_SpriteTexture : BinarySerializable
    {
        public short XPos { get; set; }
        public short YPos { get; set; }

        public byte TexturePageOffsetY { get; set; }
        public byte TexturePageOffsetX { get; set; }

        public byte Height { get; set; }
        public byte Width { get; set; }

        public int PalOffsetY { get; set; }
        public int PalOffsetX { get; set; }

        public int TexturePage { get; set; } // 0-31
        public bool FlipX { get; set; }
        public bool FlipY { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            XPos = s.Serialize<short>(XPos, name: nameof(XPos));
            YPos = s.Serialize<short>(YPos, name: nameof(YPos));
            TexturePageOffsetY = s.Serialize<byte>(TexturePageOffsetY, name: nameof(TexturePageOffsetY));
            TexturePageOffsetX = s.Serialize<byte>(TexturePageOffsetX, name: nameof(TexturePageOffsetX));
            Height = s.Serialize<byte>(Height, name: nameof(Height));
            Width = s.Serialize<byte>(Width, name: nameof(Width));
            s.SerializeBitValues<ushort>(bitFunc =>
            {
                PalOffsetY = bitFunc(PalOffsetY, 4, name: nameof(PalOffsetY));
                PalOffsetX = bitFunc(PalOffsetX, 12, name: nameof(PalOffsetX));
            });
            s.SerializeBitValues<ushort>(bitFunc =>
            {
                TexturePage = bitFunc(TexturePage, 5, name: nameof(TexturePage));
                bitFunc(default, 9, name: "Padding");
                FlipX = bitFunc(FlipX ? 1 : 0, 1, name: nameof(FlipX)) == 1;
                FlipY = bitFunc(FlipY ? 1 : 0, 1, name: nameof(FlipY)) == 1;
            });
        }
    }
}