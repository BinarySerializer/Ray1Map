using System;
using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Fusion_MapInfo : BinarySerializable
    {
        public GBAVV_Fusion_Manager.FusionLevInfo.FusionType Type { get; set; } // Set before serializing

        public Pointer MapDataPointer { get; set; }
        public GBAVV_Time Time { get; set; }
        public int Param_0 { get; set; }
        public int Param_1 { get; set; }

        // Serialized from pointers
        public GBAVV_Map MapData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (Type == GBAVV_Fusion_Manager.FusionLevInfo.FusionType.LevTime)
            {
                MapDataPointer = s.SerializePointer(MapDataPointer, name: nameof(MapDataPointer));
                Time = s.SerializeObject<GBAVV_Time>(Time, name: nameof(Time));
            }
            else if (Type == GBAVV_Fusion_Manager.FusionLevInfo.FusionType.LevInt)
            {
                MapDataPointer = s.SerializePointer(MapDataPointer, name: nameof(MapDataPointer));
                Param_0 = s.Serialize<int>(Param_0, name: nameof(Param_0));
            }
            else if (Type == GBAVV_Fusion_Manager.FusionLevInfo.FusionType.LevIntInt)
            {
                MapDataPointer = s.SerializePointer(MapDataPointer, name: nameof(MapDataPointer));
                Param_0 = s.Serialize<int>(Param_0, name: nameof(Param_0));
                Param_1 = s.Serialize<int>(Param_1, name: nameof(Param_1));
            }
            else if (Type == GBAVV_Fusion_Manager.FusionLevInfo.FusionType.IntLevel)
            {
                Param_0 = s.Serialize<int>(Param_0, name: nameof(Param_0));
                MapDataPointer = s.SerializePointer(MapDataPointer, name: nameof(MapDataPointer));
            }
            else if (Type == GBAVV_Fusion_Manager.FusionLevInfo.FusionType.Unknown)
            {
                throw new Exception("Can't serialize unknown map type");
            }

            MapData = s.DoAt(MapDataPointer, () => s.SerializeObject<GBAVV_Map>(MapData, name: nameof(MapData)));
        }
    }
}