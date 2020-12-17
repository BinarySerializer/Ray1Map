namespace R1Engine
{
    public class GBA_Cluster : R1Serializable
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

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBA_BatmanVengeance)
            {
                Batman_Data = s.SerializeArray<byte>(Batman_Data, 0x10, name: nameof(Batman_Data));
            }
            else if (s.GameSettings.EngineVersion == EngineVersion.GBA_R3_MadTrax)
            {
                Width = s.Serialize<ushort>(Width, name: nameof(Width));
                Height = s.Serialize<ushort>(Height, name: nameof(Height));

                Batman_Data = s.SerializeArray<byte>(Batman_Data, 8, name: nameof(Batman_Data));

                ScrollX = s.Serialize<int>(ScrollX, name: nameof(ScrollX));
                s.Log($"ScrollXFloat: {ScrollXFloat}");
                ScrollY = s.Serialize<int>(ScrollY, name: nameof(ScrollY));
                s.Log($"ScrollYFloat: {ScrollYFloat}");
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
    }
}