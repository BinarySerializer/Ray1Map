using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Map2D_MapLayer : BinarySerializable
    {
        public Pointer TileMapPointer { get; set; } // The 16x8 tilemap, containing offsets to the map block section of the level data block. Each tile block consists of commands which fill a total of 128 tiles (16*8)
        public uint DataBlockOffset { get; set; } // The base offset for the layer data in the level block
        public Pointer TileSetPointer { get; set; }
        public uint ScrollX { get; set; }
        public uint ScrollY { get; set; }
        public ushort LayerPrio { get; set; }
        public ushort TileMapWidth { get; set; }
        public ushort TileMapHeight { get; set; }
        public ushort MapWidth { get; set; }
        public ushort MapHeight { get; set; }
        public ushort Ushort_1E { get; set; }

        // Serialized from pointers

        public ushort[] TileMap { get; set; }
        public GBAVV_Map2D_TileSetBlock TileSet { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileMapPointer = s.SerializePointer(TileMapPointer, name: nameof(TileMapPointer));
            DataBlockOffset = s.Serialize<uint>(DataBlockOffset, name: nameof(DataBlockOffset));
            TileSetPointer = s.SerializePointer(TileSetPointer, name: nameof(TileSetPointer));
            ScrollX = s.Serialize<uint>(ScrollX, name: nameof(ScrollX));
            ScrollY = s.Serialize<uint>(ScrollY, name: nameof(ScrollY));
            LayerPrio = s.Serialize<ushort>(LayerPrio, name: nameof(LayerPrio));
            TileMapWidth = s.Serialize<ushort>(TileMapWidth, name: nameof(TileMapWidth));
            TileMapHeight = s.Serialize<ushort>(TileMapHeight, name: nameof(TileMapHeight));
            MapWidth = s.Serialize<ushort>(MapWidth, name: nameof(MapWidth));
            MapHeight = s.Serialize<ushort>(MapHeight, name: nameof(MapHeight));
            Ushort_1E = s.Serialize<ushort>(Ushort_1E, name: nameof(Ushort_1E));

            TileMap = s.DoAt(TileMapPointer, () => s.SerializeArray<ushort>(TileMap, TileMapWidth * TileMapHeight, name: nameof(TileMap)));
            TileSet = s.DoAt(TileSetPointer, () => s.SerializeObject<GBAVV_Map2D_TileSetBlock>(TileSet, name: nameof(TileSet)));
        }
    }
}