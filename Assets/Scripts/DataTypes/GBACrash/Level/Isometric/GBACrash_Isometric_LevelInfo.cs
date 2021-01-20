using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class GBACrash_Isometric_LevelInfo : R1Serializable
    {
        public bool SerializeData { get; set; } // Set before serializing

        public Pointer NamePointer { get; set; }

        public ushort MapWidth { get; set; }
        public ushort MapHeight { get; set; }
        public ushort TileSetCount_Total { get; set; }
        public ushort TileSetCount_4bpp { get; set; }
        public Pointer TileSetPointer { get; set; }
        public Pointer MapTilesPointer { get; set; }
        public Pointer[] MapLayerPointers { get; set; }
        public Pointer TilePalettePointer { get; set; }

        public ushort CollisionWidth { get; set; }
        public ushort CollisionHeight { get; set; }
        public Pointer CollisionMapPointer { get; set; }
        public Pointer CollisionTilesPointer { get; set; }
        public Pointer CollisionTypePointers { get; set; }

        public Pointer Pointer_38 { get; set; }
        public Pointer Pointer_3C { get; set; }
        public Pointer Pointer_40 { get; set; }
        public Pointer Pointer_44 { get; set; }
        public Pointer Pointer_48 { get; set; }
        public Pointer Pointer_4C { get; set; }
        public FixedPointInt XPosition { get; set; }
        public FixedPointInt YPosition { get; set; }

        // Serialized from pointers

        public string Name { get; set; }
        
        public GBACrash_Isometric_TileSet TileSet { get; set; }
        public MapTile[] MapTiles { get; set; }
        public GBACrash_Isometric_MapLayer[] MapLayers { get; set; }
        public RGBA5551Color[] TilePalette { get; set; }
        
        public ushort[] CollisionMap { get; set; }
        public GBACrash_Isometric_CollisionTile[] CollisionTiles { get; set; }
        public GBACrash_Isometric_CollisionType[] CollisionTypes { get; set; }

        public GBACrash_Isometric_UnkStruct_0[] Pointer_4C_Structs { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            NamePointer = s.SerializePointer(NamePointer, name: nameof(NamePointer));
            MapWidth = s.Serialize<ushort>(MapWidth, name: nameof(MapWidth));
            MapHeight = s.Serialize<ushort>(MapHeight, name: nameof(MapHeight));
            TileSetCount_Total = s.Serialize<ushort>(TileSetCount_Total, name: nameof(TileSetCount_Total));
            TileSetCount_4bpp = s.Serialize<ushort>(TileSetCount_4bpp, name: nameof(TileSetCount_4bpp));
            TileSetPointer = s.SerializePointer(TileSetPointer, name: nameof(TileSetPointer));
            MapTilesPointer = s.SerializePointer(MapTilesPointer, name: nameof(MapTilesPointer));
            MapLayerPointers = s.SerializePointerArray(MapLayerPointers, 4, name: nameof(MapLayerPointers));
            TilePalettePointer = s.SerializePointer(TilePalettePointer, name: nameof(TilePalettePointer));
            CollisionWidth = s.Serialize<ushort>(CollisionWidth, name: nameof(CollisionWidth));
            CollisionHeight = s.Serialize<ushort>(CollisionHeight, name: nameof(CollisionHeight));
            CollisionMapPointer = s.SerializePointer(CollisionMapPointer, name: nameof(CollisionMapPointer));
            CollisionTilesPointer = s.SerializePointer(CollisionTilesPointer, name: nameof(CollisionTilesPointer));
            CollisionTypePointers = s.SerializePointer(CollisionTypePointers, name: nameof(CollisionTypePointers));
            Pointer_38 = s.SerializePointer(Pointer_38, name: nameof(Pointer_38));
            Pointer_3C = s.SerializePointer(Pointer_3C, name: nameof(Pointer_3C));
            Pointer_40 = s.SerializePointer(Pointer_40, name: nameof(Pointer_40));
            Pointer_44 = s.SerializePointer(Pointer_44, name: nameof(Pointer_44));
            Pointer_48 = s.SerializePointer(Pointer_48, name: nameof(Pointer_48));
            Pointer_4C = s.SerializePointer(Pointer_4C, name: nameof(Pointer_4C));
            XPosition = s.SerializeObject<FixedPointInt>(XPosition, name: nameof(XPosition));
            YPosition = s.SerializeObject<FixedPointInt>(YPosition, name: nameof(YPosition));

            Name = s.DoAt(NamePointer, () => s.SerializeString(Name, name: nameof(Name)));

            if (!SerializeData)
                return;

            TileSet = s.DoAt(TileSetPointer, () => s.SerializeObject<GBACrash_Isometric_TileSet>(TileSet, x =>
            {
                x.TileSetCount_Total = TileSetCount_Total;
                x.TileSetCount_4bpp = TileSetCount_4bpp;
            }, name: nameof(TileSet)));

            if (MapLayers == null)
                MapLayers = new GBACrash_Isometric_MapLayer[MapLayerPointers.Length];

            for (int i = 0; i < MapLayers.Length; i++)
                MapLayers[i] = s.DoAt(MapLayerPointers[i], () => s.SerializeObject<GBACrash_Isometric_MapLayer>(MapLayers[i], name: $"{nameof(MapLayers)}[{i}]"));

            var mapTilesLength = MapLayers.SelectMany(x => x.TileMapRows).SelectMany(x => x.Commands).Select(x => x.Params?.Max() ?? x.Param).Max() + 1;
            MapTiles = s.DoAt(MapTilesPointer, () => s.SerializeObjectArray<MapTile>(MapTiles, mapTilesLength * 4, x => x.Is8Bpp = true, name: nameof(MapTiles)));
            TilePalette = s.DoAt(TilePalettePointer, () => s.SerializeObjectArray<RGBA5551Color>(TilePalette, 256, name: nameof(TilePalette)));

            CollisionMap = s.DoAt(CollisionMapPointer, () => s.SerializeArray<ushort>(CollisionMap, CollisionWidth * CollisionHeight, name: nameof(CollisionMap)));
            CollisionTiles = s.DoAt(CollisionTilesPointer, () => s.SerializeObjectArray<GBACrash_Isometric_CollisionTile>(CollisionTiles, CollisionMap.Max() + 1, name: nameof(CollisionTiles)));
            CollisionTypes = s.DoAt(CollisionTypePointers, () => s.SerializeObjectArray<GBACrash_Isometric_CollisionType>(CollisionTypes, CollisionTiles.Max(x => x.TypeIndex) + 1, name: nameof(CollisionTypes)));

            s.DoAt(Pointer_4C, () =>
            {
                if (Pointer_4C_Structs == null)
                {
                    var objects = new List<GBACrash_Isometric_UnkStruct_0>();
                    var index = 0;

                    while (true)
                    {
                        var obj = s.SerializeObject<GBACrash_Isometric_UnkStruct_0>(default, name: $"{nameof(Pointer_4C_Structs)}[{index++}]");

                        if (obj.Int_00 == -1)
                            break;

                        objects.Add(obj);
                    }

                    Pointer_4C_Structs = objects.ToArray();
                }
                else
                {
                    s.SerializeObjectArray<GBACrash_Isometric_UnkStruct_0>(Pointer_4C_Structs, Pointer_4C_Structs.Length, name: nameof(Pointer_4C_Structs));
                }
            });
        }
    }

    public class GBACrash_Isometric_UnkStruct_0 : R1Serializable
    {
        public int Int_00 { get; set; }
        public short XPos_0 { get; set; }
        public short XPos_1 { get; set; }
        public short YPos_0 { get; set; }
        public short YPos_1 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
            XPos_0 = s.Serialize<short>(XPos_0, name: nameof(XPos_0));
            XPos_1 = s.Serialize<short>(XPos_1, name: nameof(XPos_1));
            YPos_0 = s.Serialize<short>(YPos_0, name: nameof(YPos_0));
            YPos_1 = s.Serialize<short>(YPos_1, name: nameof(YPos_1));
        }
    }
}