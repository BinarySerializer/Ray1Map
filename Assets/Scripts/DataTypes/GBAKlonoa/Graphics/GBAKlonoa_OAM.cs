using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_OAM : BinarySerializable
    {
        public ushort TileIndex { get; set; }
        public byte PaletteIndex { get; set; }
        public sbyte XPos { get; set; }
        public sbyte YPos { get; set; }
        public byte Shape { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileIndex = s.Serialize<ushort>(TileIndex, name: nameof(TileIndex));
            PaletteIndex = s.Serialize<byte>(PaletteIndex, name: nameof(PaletteIndex));
            XPos = s.Serialize<sbyte>(XPos, name: nameof(XPos));
            YPos = s.Serialize<sbyte>(YPos, name: nameof(YPos));
            Shape = s.Serialize<byte>(Shape, name: nameof(Shape));
            s.SerializePadding(2, logIfNotNull: true);
        }
    }
}