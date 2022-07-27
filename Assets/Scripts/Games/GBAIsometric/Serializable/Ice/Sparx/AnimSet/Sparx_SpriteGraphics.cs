using System.Linq;
using BinarySerializer;
using BinarySerializer.Nintendo.GBA;

namespace Ray1Map.GBAIsometric
{
    public class Sparx_SpriteGraphics : BinarySerializable
    {
        public Pointer<Palette> Palette { get; set; }
        public Pointer TileSetPointer { get; set; }
        public ushort PartsCount { get; set; }
        public ushort Ushort_0A { get; set; } // Always 0
        public Pointer PartsPointer { get; set; }
        public short XPos { get; set; }
        public short YPos { get; set; }
        public short Width { get; set; }
        public short Height { get; set; }

        public byte[] TileSet { get; set; }
        public Sparx_SpriteGraphicsPart[] Parts { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Palette = s.SerializePointer<Palette>(Palette, name: nameof(Palette))?.ResolveObject(s);
            TileSetPointer = s.SerializePointer(TileSetPointer, name: nameof(TileSetPointer));
            PartsCount = s.Serialize<ushort>(PartsCount, name: nameof(PartsCount));
            Ushort_0A = s.Serialize<ushort>(Ushort_0A, name: nameof(Ushort_0A));
            PartsPointer = s.SerializePointer(PartsPointer, name: nameof(PartsPointer));
            XPos = s.Serialize<short>(XPos, name: nameof(XPos));
            YPos = s.Serialize<short>(YPos, name: nameof(YPos));
            Width = s.Serialize<short>(Width, name: nameof(Width));
            Height = s.Serialize<short>(Height, name: nameof(Height));

            s.DoAt(PartsPointer, () => Parts = s.SerializeObjectArray<Sparx_SpriteGraphicsPart>(Parts, PartsCount, name: nameof(Parts)));

            long tileSetLength = Parts.Max(x => x.TileSetOffset + x.Attribute.GetSpriteShape().TilesCount * 0x20);
            s.DoAt(TileSetPointer, () => TileSet = s.SerializeArray<byte>(TileSet, tileSetLength, name: nameof(TileSet)));
        }
    }
}