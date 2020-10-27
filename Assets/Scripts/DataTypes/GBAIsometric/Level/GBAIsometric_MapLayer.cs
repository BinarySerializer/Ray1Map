using System;
using UnityEngine;

namespace R1Engine
{
    public class GBAIsometric_MapLayer : R1Serializable
    {
        public Pointer<GBAIsometric_LevelDataLayerDataPointers> PointersPointer { get; set; }
        public MapLayerType StructType { get; set; }
        public ushort Width { get; set; } // 40, 24 or 12 - 4 for maps
        public ushort Height { get; set; } // 20 or 12 - 4 for maps
        public ushort Ushort_0A { get; set; } // Always 0
        public Pointer MapDataPointer { get; set; }
        public Pointer Pointer_20 { get; set; } // Only valid for maps

        public ARGB1555Color[] Palette { get; set; }
        public byte[] RemainingData { get; set; }

        // Parsed
        public ushort[] MapData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            PointersPointer = s.SerializePointer<GBAIsometric_LevelDataLayerDataPointers>(PointersPointer, resolve: true, name: nameof(PointersPointer));
            StructType = s.Serialize<MapLayerType>(StructType, name: nameof(StructType));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            Ushort_0A = s.Serialize<ushort>(Ushort_0A, name: nameof(Ushort_0A));
            MapDataPointer = s.SerializePointer(MapDataPointer, name: nameof(MapDataPointer));
            Pointer_20 = s.SerializePointer(Pointer_20, name: nameof(Pointer_20));

            if (StructType == MapLayerType.Map)
            {
                Palette = s.SerializeObjectArray<ARGB1555Color>(Palette, 256, name: nameof(Palette));
                RemainingData = s.SerializeArray<byte>(RemainingData, 44, name: nameof(RemainingData));
            }

            // TODO: Remove try/catch
            try
            {
                s.DoAt(MapDataPointer, () =>
                {
                    s.DoEncoded(new RHREncoder(), () =>
                    {
                        MapData = s.SerializeArray<ushort>(MapData, Width * Height, name: nameof(MapData));
                    });
                });
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to decompress {MapDataPointer}: {ex.Message}\n{ex.InnerException?.StackTrace}");
            }
        }

        public enum MapLayerType : ushort
        {
            Normal = 1,
            Map = 2
        }
    }
}