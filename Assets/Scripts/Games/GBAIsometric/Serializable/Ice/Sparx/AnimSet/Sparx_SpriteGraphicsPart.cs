using BinarySerializer;
using BinarySerializer.GBA;

namespace Ray1Map.GBAIsometric
{
    public class Sparx_SpriteGraphicsPart : BinarySerializable
    {
        public GBA_OBJ_ATTR Attribute { get; set; }
        public short XPos { get; set; }
        public short YPos { get; set; }
        public short Width { get; set; } // Width - 1
        public short Height { get; set; } // Height - 1
        public uint TileSetOffset { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Attribute = s.SerializeObject<GBA_OBJ_ATTR>(Attribute, name: nameof(Attribute));
            s.SerializePadding(2, logIfNotNull: true);
            XPos = s.Serialize<short>(XPos, name: nameof(XPos));
            YPos = s.Serialize<short>(YPos, name: nameof(YPos));
            Width = s.Serialize<short>(Width, name: nameof(Width));
            Height = s.Serialize<short>(Height, name: nameof(Height));
            TileSetOffset = s.Serialize<uint>(TileSetOffset, name: nameof(TileSetOffset));
        }
    }
}