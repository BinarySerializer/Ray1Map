using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class Sparx_Frame : BinarySerializable
    {
        public short Short_00 { get; set; } // -1 when invalid, otherwise 1
        public ushort SpritesCount { get; set; }
        public Pointer SpritesPointer { get; set; }
        public byte[] Bytes_08 { get; set; } // Always 0

        public Sparx_Sprite[] Sprites { get; set; } // 0 or 1

        public override void SerializeImpl(SerializerObject s)
        {
            Short_00 = s.Serialize<short>(Short_00, name: nameof(Short_00));
            SpritesCount = s.Serialize<ushort>(SpritesCount, name: nameof(SpritesCount));
            SpritesPointer = s.SerializePointer(SpritesPointer, name: nameof(SpritesPointer));
            Bytes_08 = s.SerializeArray<byte>(Bytes_08, 8, name: nameof(Bytes_08));

            s.DoAt(SpritesPointer, () => Sprites = s.SerializeObjectArray<Sparx_Sprite>(Sprites, SpritesCount, name: nameof(Sprites)));
        }
    }
}