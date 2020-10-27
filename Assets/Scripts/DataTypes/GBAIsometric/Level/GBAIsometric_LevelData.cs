using System;
using UnityEngine;

namespace R1Engine
{
    public class GBAIsometric_LevelData : R1Serializable
    {
        public GBAIsometric_LevelDataLayer[] MapLayers { get; set; }

        public int ObjectsCount { get; set; }
        public int WaypointsCount { get; set; }

        public int UnkStructsWidth { get; set; }
        public int UnkStructsHeight { get; set; }

        public Pointer ObjectsPointer { get; set; }
        public Pointer WaypointsPointer { get; set; }
        public Pointer UnkStructsPointer { get; set; }

        public byte Byte_6C { get; set; }
        public byte Byte_6D { get; set; }
        public byte Byte_6E { get; set; }
        public byte Byte_6F { get; set; }

        public GBAIsometric_LocIndex LevelNameLocIndex { get; set; }

        // Parsed from pointers

        public GBAIsometric_Object[] Objects { get; set; }
        public GBAIsometric_Waypoint[] Waypoints { get; set; }
        public GBAIsometric_UnkStruct[] UnkStructs { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapLayers = s.SerializeObjectArray<GBAIsometric_LevelDataLayer>(MapLayers, 4, name: nameof(MapLayers));

            ObjectsCount = s.Serialize<int>(ObjectsCount, name: nameof(ObjectsCount));
            WaypointsCount = s.Serialize<int>(WaypointsCount, name: nameof(WaypointsCount));
            UnkStructsWidth = s.Serialize<int>(UnkStructsWidth, name: nameof(UnkStructsWidth));
            UnkStructsHeight = s.Serialize<int>(UnkStructsHeight, name: nameof(UnkStructsHeight));

            ObjectsPointer = s.SerializePointer(ObjectsPointer, name: nameof(ObjectsPointer));
            WaypointsPointer = s.SerializePointer(WaypointsPointer, name: nameof(WaypointsPointer));
            UnkStructsPointer = s.SerializePointer(UnkStructsPointer, name: nameof(UnkStructsPointer));

            Byte_6C = s.Serialize<byte>(Byte_6C, name: nameof(Byte_6C));
            Byte_6D = s.Serialize<byte>(Byte_6D, name: nameof(Byte_6D));
            Byte_6E = s.Serialize<byte>(Byte_6E, name: nameof(Byte_6E));
            Byte_6F = s.Serialize<byte>(Byte_6F, name: nameof(Byte_6F));

            LevelNameLocIndex = s.SerializeObject<GBAIsometric_LocIndex>(LevelNameLocIndex, name: nameof(LevelNameLocIndex));

            // Parse from pointers
            Objects = s.DoAt(ObjectsPointer, () => s.SerializeObjectArray<GBAIsometric_Object>(Objects, ObjectsCount, name: nameof(Objects)));
            Waypoints = s.DoAt(WaypointsPointer, () => s.SerializeObjectArray<GBAIsometric_Waypoint>(Waypoints, WaypointsCount, name: nameof(Waypoints)));

            // TODO: Remove try/catch
            try
            {
                s.DoAt(UnkStructsPointer, () =>
                {
                    s.DoEncoded(new RHREncoder(), () =>
                    {
                        UnkStructs = s.SerializeObjectArray<GBAIsometric_UnkStruct>(UnkStructs, UnkStructsWidth * UnkStructsHeight, name: nameof(UnkStructs));
                    });
                });
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to decompress {UnkStructsPointer}: {ex.Message}\n{ex.InnerException?.StackTrace}");
            }
        }
    }
}