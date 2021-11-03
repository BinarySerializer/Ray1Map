using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_AnimPattern : BinarySerializable
    {
        public byte X { get; set; }
        public byte Y { get; set; } // In pixels
        public byte RelativeTileIndex { get; set; }
        public byte PalIndex { get; set; }
        public byte SpriteSize { get; set; }
        public Shape SpriteShape { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            X = s.Serialize<byte>(X, name: nameof(X));
            Y = s.Serialize<byte>(Y, name: nameof(Y));
            RelativeTileIndex = s.Serialize<byte>(RelativeTileIndex, name: nameof(RelativeTileIndex));

            s.SerializeBitValues<byte>(bitFunc => {
                SpriteSize = (byte)bitFunc(SpriteSize, 2, name: nameof(SpriteSize));
                SpriteShape = (Shape)bitFunc((int)SpriteShape, 2, name: nameof(SpriteShape));
                PalIndex = (byte)bitFunc(PalIndex, 4, name: nameof(PalIndex));
            });
        }
        public enum Shape {
            Square,
            Wide,
            Tall
        }
    }
}