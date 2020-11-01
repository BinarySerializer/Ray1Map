using System;

namespace R1Engine
{
    public class GBAIsometric_MapLayer : R1Serializable
    {
        public Pointer<GBAIsometric_TileSet> TileSetPointer { get; set; }
        public MapLayerType StructType { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public ushort Ushort_0A { get; set; } // Always 0
        public Pointer MapDataPointer { get; set; }

        // Maps only
        public Pointer MapPalettePointer { get; set; }
        public ARGB1555Color[] MapPalette { get; set; }

        // Parsed
        public ushort[] MapData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileSetPointer = s.SerializePointer<GBAIsometric_TileSet>(TileSetPointer, resolve: true, name: nameof(TileSetPointer));
            StructType = s.Serialize<MapLayerType>(StructType, name: nameof(StructType));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            Ushort_0A = s.Serialize<ushort>(Ushort_0A, name: nameof(Ushort_0A));
            MapDataPointer = s.SerializePointer(MapDataPointer, name: nameof(MapDataPointer));
            MapPalettePointer = s.SerializePointer(MapPalettePointer, name: nameof(MapPalettePointer));
            MapPalette = s.DoAt(MapPalettePointer, () => s.SerializeObjectArray<ARGB1555Color>(MapPalette, 256, name: nameof(MapPalette)));

            s.DoAt(MapDataPointer, () =>
            {
                s.DoEncoded(new RHREncoder(), () => MapData = s.SerializeArray<ushort>(MapData, Width * Height, name: nameof(MapData)));
            });

            // Debug tilemaps
            //ushort[] fullMap = CreateFullMap(MapData);
            //string logString = $"{Offset}: Max Tilemap Value - {fullMap.Max()}";
            //UnityEngine.Debug.Log(logString);
            //s.Log(logString);
            //byte[] result = new byte[fullMap.Length * sizeof(ushort)];
            //Buffer.BlockCopy(fullMap, 0, result, 0, result.Length);
            //Util.ByteArrayToFile(s.Context.BasePath + "full_tilemap/" + Offset.StringAbsoluteOffset + ".bin", result);
        }

        public enum MapLayerType : ushort
        {
            Menu = 0,
            Normal = 1,
            Map = 2
        }

        public ushort[] CreateFullMap(ushort[] mapData) {
            ushort[] fullMap = new ushort[mapData.Length * 64];
            for (int i = 0; i < mapData.Length; i++) {
                ushort[] tempMap = TileSetPointer.Value.Get8x8Map(mapData[i]);
                Array.Copy(tempMap, 0, fullMap, i * 64, tempMap.Length);
            }
            return fullMap;
        }
    }
}