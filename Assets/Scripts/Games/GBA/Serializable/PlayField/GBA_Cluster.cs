using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_Cluster : BinarySerializable
    {
        public int ScrollX { get; set; }
        public int ScrollY { get; set; }

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public byte Byte_0C { get; set; }

        // Seems to be 1 when a special effect should be used (like scrolling sky, floating skulls etc. - the type of effect is determined from the game info level table)
        public byte Byte_0D { get; set; }

        public byte Byte_0E { get; set; }
        public byte Byte_0F { get; set; }

        public float ScrollXFloat => ScrollX / (float)0x10000;
        public float ScrollYFloat => ScrollY / (float)0x10000;

        public byte[] Batman_Data { get; set; }

        public byte Shanghai_Byte_04 { get; set; } // 0 or 3
        public byte Shanghai_Byte_05 { get; set; }
        public byte Shanghai_Byte_06 { get; set; }
        public byte Shanghai_Byte_07 { get; set; }
        public byte Shanghai_Byte_08 { get; set; }
        public byte Shanghai_MapTileSize { get; set; } // The size of each tile in bytes
        public byte Shanghai_Byte_09 { get; set; }
        public byte Shanghai_Byte_0A { get; set; }
        public byte Shanghai_Byte_0B { get; set; }

        public Milan_CompressionType Milan_MapCompressionType { get; set; }
        public bool Milan_UnkFlag { get; set; } // True if collision?
        public byte Milan_Byte_05 { get; set; }
        public byte Milan_Byte_06 { get; set; } // Width related
        public byte Milan_Byte_07 { get; set; } // Height related
        public ushort Milan_Ushort_08 { get; set; }
        public ushort Milan_Ushort_0A { get; set; } // Padding?

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_BatmanVengeance)
            {
                Batman_Data = s.SerializeArray<byte>(Batman_Data, 0x10, name: nameof(Batman_Data));
            }
            else if (s.GetR1Settings().GBA_IsShanghai || s.GetR1Settings().GBA_IsMilan)
            {
                Width = s.Serialize<ushort>(Width, name: nameof(Width));
                Height = s.Serialize<ushort>(Height, name: nameof(Height));

                if (s.GetR1Settings().GBA_IsMilan)
                {
                    s.DoBits<byte>(b =>
                    {
                        Milan_MapCompressionType = (Milan_CompressionType)b.SerializeBits<int>((byte)Milan_MapCompressionType, 7, name: nameof(Milan_MapCompressionType));
                        Milan_UnkFlag = b.SerializeBits<int>(Milan_UnkFlag ? 1 : 0, 1, name: nameof(Milan_UnkFlag)) == 1;
                    });

                    Milan_Byte_05 = s.Serialize<byte>(Milan_Byte_05, name: nameof(Milan_Byte_05));
                    Milan_Byte_06 = s.Serialize<byte>(Milan_Byte_06, name: nameof(Milan_Byte_06));
                    Milan_Byte_07 = s.Serialize<byte>(Milan_Byte_07, name: nameof(Milan_Byte_07));
                    Milan_Ushort_08 = s.Serialize<ushort>(Milan_Ushort_08, name: nameof(Milan_Ushort_08));
                    Milan_Ushort_0A = s.Serialize<ushort>(Milan_Ushort_0A, name: nameof(Milan_Ushort_0A));
                }
                else
                {
                    Shanghai_Byte_04 = s.Serialize<byte>(Shanghai_Byte_04, name: nameof(Shanghai_Byte_04));
                    Shanghai_Byte_05 = s.Serialize<byte>(Shanghai_Byte_05, name: nameof(Shanghai_Byte_05));
                    Shanghai_Byte_06 = s.Serialize<byte>(Shanghai_Byte_06, name: nameof(Shanghai_Byte_06));
                    Shanghai_Byte_07 = s.Serialize<byte>(Shanghai_Byte_07, name: nameof(Shanghai_Byte_07));

                    if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_DonaldDuck && Shanghai_Byte_04 == 3)
                    {
                        Shanghai_MapTileSize = s.Serialize<byte>(Shanghai_MapTileSize, name: nameof(Shanghai_MapTileSize));
                        Shanghai_Byte_09 = s.Serialize<byte>(Shanghai_Byte_09, name: nameof(Shanghai_Byte_09));
                    }
                    else
                    {
                        Shanghai_Byte_08 = s.Serialize<byte>(Shanghai_Byte_08, name: nameof(Shanghai_Byte_08));
                        Shanghai_MapTileSize = s.Serialize<byte>(Shanghai_MapTileSize, name: nameof(Shanghai_MapTileSize));
                    }

                    Shanghai_Byte_0A = s.Serialize<byte>(Shanghai_Byte_0A, name: nameof(Shanghai_Byte_0A));
                    Shanghai_Byte_0B = s.Serialize<byte>(Shanghai_Byte_0B, name: nameof(Shanghai_Byte_0B));

                    ScrollX = s.Serialize<int>(ScrollX, name: nameof(ScrollX));
                    s.Log($"ScrollXFloat: {ScrollXFloat}");
                    ScrollY = s.Serialize<int>(ScrollY, name: nameof(ScrollY));
                    s.Log($"ScrollYFloat: {ScrollYFloat}");
                }
            }
            else
            {
                ScrollX = s.Serialize<int>(ScrollX, name: nameof(ScrollX));
                s.Log($"ScrollXFloat: {ScrollXFloat}");
                ScrollY = s.Serialize<int>(ScrollY, name: nameof(ScrollY));
                s.Log($"ScrollYFloat: {ScrollYFloat}");
                Width = s.Serialize<ushort>(Width, name: nameof(Width));
                Height = s.Serialize<ushort>(Height, name: nameof(Height));
                Byte_0C = s.Serialize<byte>(Byte_0C, name: nameof(Byte_0C));
                Byte_0D = s.Serialize<byte>(Byte_0D, name: nameof(Byte_0D));
                Byte_0E = s.Serialize<byte>(Byte_0E, name: nameof(Byte_0E));
                Byte_0F = s.Serialize<byte>(Byte_0F, name: nameof(Byte_0F));
            }
        }

        public enum Milan_CompressionType : byte
        {
            Collision_None = 0,
            Collision_RL = 1,
            Collsiion_LZSS = 2,

            Map_None = 3,
            Map_RL = 4,
            Map_LZSS = 5
        }
    }
}