using System.Linq;

namespace R1Engine
{
    public class GBAVV_WorldMap_MapLayer : R1Serializable
    {
        public Pointer MapTilesPointer { get; set; }
        public Pointer TileMapPointer { get; set; }
        public uint LayerPrio { get; set; }
        public uint ScrollX { get; set; }
        public uint ScrollY { get; set; }
        public uint Uint_14 { get; set; }
        public uint Uint_18 { get; set; }

        // Serialized from pointers

        public MapTile[] MapTiles { get; set; }
        public GBAVV_Isometric_MapLayer TileMap { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapTilesPointer = s.SerializePointer(MapTilesPointer, name: nameof(MapTilesPointer));
            TileMapPointer = s.SerializePointer(TileMapPointer, name: nameof(TileMapPointer));
            LayerPrio = s.Serialize<uint>(LayerPrio, name: nameof(LayerPrio));
            ScrollX = s.Serialize<uint>(ScrollX, name: nameof(ScrollX));
            ScrollY = s.Serialize<uint>(ScrollY, name: nameof(ScrollY));
            Uint_14 = s.Serialize<uint>(Uint_14, name: nameof(Uint_14));
            Uint_18 = s.Serialize<uint>(Uint_18, name: nameof(Uint_18));

            TileMap = s.DoAt(TileMapPointer, () => s.SerializeObject<GBAVV_Isometric_MapLayer>(TileMap, x => x.IsWorldMap = true, name: nameof(TileMap)));

            s.DoAt(MapTilesPointer, () =>
            {
                var mapTilesLength = TileMap.TileMapRows.SelectMany(x => x.Commands).Select(x => x.Params?.Max() ?? x.Param).Max() + 1;

                s.DoEncodedIf(new GBA_LZSSEncoder(), s.GameSettings.EngineVersion == EngineVersion.GBAVV_Fusion, () =>
                {
                    MapTiles = s.SerializeObjectArray<MapTile>(MapTiles, mapTilesLength * 4, x => x.GBAVV_IsWorldMap = true, name: nameof(MapTiles));
                });
            });
        }
    }
}