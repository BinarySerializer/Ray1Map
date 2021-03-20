using System.Linq;

namespace R1Engine
{
    public class GBAVV_MapLayer : R1Serializable
    {
        public GBAVV_TileMap.Encoding MapEncoding { get; set; } = GBAVV_TileMap.Encoding.Columns; // Set before serializing

        public Pointer MapTilesPointer { get; set; }
        public Pointer TileMapPointer { get; set; }
        public uint LayerPrio { get; set; }
        public uint ScrollX { get; set; }
        public uint ScrollY { get; set; }
        public ushort Ushort_14 { get; set; }
        public ushort Ushort_16 { get; set; }

        // Serialized from pointers
        public MapTile[] MapTiles { get; set; }
        public GBAVV_TileMap TileMap { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapTilesPointer = s.SerializePointer(MapTilesPointer, name: nameof(MapTilesPointer));
            TileMapPointer = s.SerializePointer(TileMapPointer, name: nameof(TileMapPointer));
            LayerPrio = s.Serialize<uint>(LayerPrio, name: nameof(LayerPrio));
            ScrollX = s.Serialize<uint>(ScrollX, name: nameof(ScrollX));
            ScrollY = s.Serialize<uint>(ScrollY, name: nameof(ScrollY));
            Ushort_14 = s.Serialize<ushort>(Ushort_14, name: nameof(Ushort_14));
            Ushort_16 = s.Serialize<ushort>(Ushort_16, name: nameof(Ushort_16));

            TileMap = s.DoAt(TileMapPointer, () => s.SerializeObject<GBAVV_TileMap>(TileMap, x => x.MapEncoding = MapEncoding, name: nameof(TileMap)));

            s.DoAt(MapTilesPointer, () =>
            {
                var mapTilesLength = TileMap.TileMapSections.SelectMany(x => x.Commands).Select(x => x.Params?.Max() ?? x.Param).Max() + 1;

                s.DoEncodedIf(new GBA_LZSSEncoder(), s.GameSettings.GBAVV_IsFusion || s.GameSettings.EngineVersion == EngineVersion.GBAVV_BruceLeeReturnOfTheLegend || s.GameSettings.EngineVersion == EngineVersion.GBAVV_FindingNemo, () =>
                {
                    MapTiles = s.SerializeObjectArray<MapTile>(MapTiles, mapTilesLength * 4, x => x.GBAVV_IsWorldMap = true, name: nameof(MapTiles));
                });
            });
        }
    }
}