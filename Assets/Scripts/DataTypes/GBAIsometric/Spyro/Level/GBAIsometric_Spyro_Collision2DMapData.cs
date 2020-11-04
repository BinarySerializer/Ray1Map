﻿namespace R1Engine
{
    public class GBAIsometric_Spyro_Collision2DMapData : R1Serializable
    {
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public byte TileWidth { get; set; }
        public byte TileHeight { get; set; }
        public ushort Ushort_06 { get; set; } // Always 8?

        public byte[] Collision { get; set; }
        
        public override void SerializeImpl(SerializerObject s)
        {            
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            TileWidth = s.Serialize<byte>(TileWidth, name: nameof(TileWidth));
            TileHeight = s.Serialize<byte>(TileHeight, name: nameof(TileHeight));
            Ushort_06 = s.Serialize<ushort>(Ushort_06, name: nameof(Ushort_06));

            Collision = s.SerializeArray<byte>(Collision, (Width / TileWidth) * (Height / TileHeight), name: nameof(Collision));
        }
    }
}