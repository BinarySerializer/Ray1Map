using System.Linq;

namespace R1Engine
{
    public class GBC_Map : GBC_BaseBlock 
    {
        public uint Width { get; set; }
        public uint Height { get; set; }
        public MapTile[] MapTiles { get; set; }

        // GBC
        public byte PaletteCount { get; set; }
        public byte VRAMMapCount1 { get; set; }
        public byte VRAMMapCount2 { get; set; }
        public GBC_TileVRAMMap[] VRAMMap1 { get; set; }
        public GBC_TileVRAMMap[] VRAMMap2 { get; set; }
        public ARGB1555Color[] Palette { get; set; }

        // Parsed from offset table
        public GBC_TileKit TileKit { get; set; }
        public GBC_MapBlock Block1 { get; set; }
        public GBC_MapBlock Block2 { get; set; }
        public GBC_MapBlock Collision { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            base.SerializeImpl(s);
            SerializeOffsetTable(s);
            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1) {
                Width = s.Serialize<byte>((byte)Width, name: nameof(Width));
                Height = s.Serialize<byte>((byte)Height, name: nameof(Height));
                PaletteCount = s.Serialize<byte>(PaletteCount, name: nameof(PaletteCount));
                VRAMMapCount1 = s.Serialize<byte>(VRAMMapCount1, name: nameof(VRAMMapCount1));
                VRAMMapCount2 = s.Serialize<byte>(VRAMMapCount2, name: nameof(VRAMMapCount2));
                VRAMMap1 = s.SerializeObjectArray<GBC_TileVRAMMap>(VRAMMap1, VRAMMapCount1, name: nameof(VRAMMap1));
                VRAMMap2 = s.SerializeObjectArray<GBC_TileVRAMMap>(VRAMMap2, VRAMMapCount2, name: nameof(VRAMMap2));
                UnityEngine.Debug.Log(VRAMMap1.Sum(v => v.TileCount));
                UnityEngine.Debug.Log(VRAMMap2.Sum(v => v.TileCount));
                Palette = s.SerializeObjectArray<ARGB1555Color>(Palette, PaletteCount * 4, name: nameof(Palette));


                Block1 = s.DoAt(OffsetTable.GetPointer(1), () => s.SerializeObject<GBC_MapBlock>(Block1, onPreSerialize: b => {
                    b.Width = (byte)Width;
                    b.Height = (byte)Height;
                    b.Type = MapTile.GBC_TileType.Block1;
                }, name: nameof(Block1)));
                Block2 = s.DoAt(OffsetTable.GetPointer(2), () => s.SerializeObject<GBC_MapBlock>(Block2, onPreSerialize: b => {
                    b.Width = (byte)Width;
                    b.Height = (byte)Height;
                    b.Type = MapTile.GBC_TileType.Block2;
                }, name: nameof(Block1)));
                Collision = s.DoAt(OffsetTable.GetPointer(3), () => s.SerializeObject<GBC_MapBlock>(Collision, onPreSerialize: b => {
                    b.Width = (byte)Width;
                    b.Height = (byte)Height;
                    b.Type = MapTile.GBC_TileType.Collision;
                }, name: nameof(Block1)));
            } else {
                Width = s.Serialize<uint>(Width, name: nameof(Width));
                Height = s.Serialize<uint>(Height, name: nameof(Height));
                MapTiles = s.SerializeObjectArray<MapTile>(MapTiles, Width * Height, name: nameof(MapTiles));
            }

            TileKit = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBC_TileKit>(TileKit, name: nameof(TileKit)));

        }
    }
}