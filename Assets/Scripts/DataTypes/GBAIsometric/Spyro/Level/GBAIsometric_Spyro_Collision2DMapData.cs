namespace R1Engine
{
    public class GBAIsometric_Spyro_Collision2DMapData : R1Serializable
    {
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public byte TileWidth { get; set; }
        public byte TileHeight { get; set; }
        public ushort Ushort_06 { get; set; } // Always 8?
        public uint UInt_04 { get; set; } // Spyro 2, padding?

        public byte[] Collision { get; set; }
        
        public override void SerializeImpl(SerializerObject s)
        {            
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_Spyro3)
            {
                TileWidth = s.Serialize<byte>(TileWidth, name: nameof(TileWidth));
                TileHeight = s.Serialize<byte>(TileHeight, name: nameof(TileHeight));
                Ushort_06 = s.Serialize<ushort>(Ushort_06, name: nameof(Ushort_06));
            }
            else {
                UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
                TileWidth = 1;
                TileHeight = 1;
            }

            Collision = s.SerializeArray<byte>(Collision, (Width / TileWidth) * (Height / TileHeight), name: nameof(Collision));
        }
    }
}