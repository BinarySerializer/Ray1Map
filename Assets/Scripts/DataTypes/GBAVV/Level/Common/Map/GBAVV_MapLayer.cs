using System.Linq;
using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_MapLayer : BinarySerializable
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
            // TODO: More data (check for alpha blending and other flags)

            TileMap = s.DoAt(TileMapPointer, () => s.SerializeObject<GBAVV_TileMap>(TileMap, x => x.MapEncoding = MapEncoding, name: nameof(TileMap)));

            s.DoAt(MapTilesPointer, () =>
            {
                var mapTilesLength = TileMap.TileMapSections.SelectMany(x => x.Commands).Select(x => x.Params?.Max() ?? x.Param).Max() + 1;

                // TODO: Is there a flag for this?
                var uncompressed = s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_ThePowerpuffGirlsHimAndSeek ||
                                   s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash2 ||
                                   s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_CrashNitroKart ||
                                   s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_X2WolverinesRevenge ||
                                   s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_UltimateSpiderMan ||
                                   s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_OverTheHedge ||
                                   s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_OverTheHedgeHammyGoesNuts ||
                                   s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_SpiderMan3;

                s.DoEncodedIf(new GBA_LZSSEncoder(), !uncompressed, () =>
                {
                    MapTiles = s.SerializeObjectArray<MapTile>(MapTiles, mapTilesLength * 4, x => x.GBAVV_IsWorldMap = true, name: nameof(MapTiles));
                });
            });
        }
    }
}