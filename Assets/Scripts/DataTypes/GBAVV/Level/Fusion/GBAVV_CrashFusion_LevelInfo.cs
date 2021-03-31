using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_CrashFusion_LevelInfo : BinarySerializable
    {
        public GBAVV_Fusion_Manager.FusionLevInfo LevInfo { get; set; } // Set before serializing if it's the current level

        public Pointer LevelNamePointer { get; set; }
        public Pointer MapDataPointer { get; set; }
        public Pointer Pointer_08 { get; set; }
        public uint Uint_0C { get; set; }

        // Serialized from pointers
        public GBAVV_LocalizedString LevelName { get; set; }
        public GBAVV_Map MapData { get; set; }
        public GBAVV_Fusion_MapInfo MapInfo { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LevelNamePointer = s.SerializePointer(LevelNamePointer, name: nameof(LevelNamePointer));
            MapDataPointer = s.SerializePointer(MapDataPointer, name: nameof(MapDataPointer));
            Pointer_08 = s.SerializePointer(Pointer_08, name: nameof(Pointer_08));
            Uint_0C = s.Serialize<uint>(Uint_0C, name: nameof(Uint_0C));

            LevelName = s.DoAt(LevelNamePointer, () => s.SerializeObject<GBAVV_LocalizedString>(LevelName, name: nameof(LevelName)));

            if (LevInfo != null)
            {
                if (LevInfo.Fusion_Type == GBAVV_Fusion_Manager.FusionLevInfo.FusionType.Normal)
                {
                    MapData = s.DoAt(MapDataPointer, () => s.SerializeObject<GBAVV_Map>(MapData, name: nameof(MapData)));
                }
                else
                {
                    MapInfo = s.DoAt(MapDataPointer, () => s.SerializeObject<GBAVV_Fusion_MapInfo>(MapInfo, x => x.Type = LevInfo.Fusion_Type, name: nameof(MapInfo)));
                    MapData = MapInfo.MapData;
                }
            }
        }
    }
}