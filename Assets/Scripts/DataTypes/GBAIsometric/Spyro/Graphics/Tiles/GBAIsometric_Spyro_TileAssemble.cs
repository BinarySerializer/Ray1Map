namespace R1Engine
{
    public class GBAIsometric_Spyro_TileAssemble : R1Serializable
    {
        public long BlockSize { get; set; }

        public int GroupLength => GroupWidth * GroupHeight;

        public ushort GroupWidth { get; set; }
        public ushort GroupHeight { get; set; }
        public uint Uint_04 { get; set; }

        public MapTile[][] TileGroups { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            GroupWidth = s.Serialize<ushort>(GroupWidth, name: nameof(GroupWidth));
            GroupHeight = s.Serialize<ushort>(GroupHeight, name: nameof(GroupHeight));
            Uint_04 = s.Serialize<uint>(Uint_04, name: nameof(Uint_04));

            if (TileGroups == null)
                TileGroups = new MapTile[((BlockSize - 8) / GroupLength) / 2][];

            for (int i = 0; i < TileGroups.Length; i++)
                TileGroups[i] = s.SerializeObjectArray<MapTile>(TileGroups[i], GroupLength, name: $"{nameof(TileGroups)}[{i}]");
        }
    }
}