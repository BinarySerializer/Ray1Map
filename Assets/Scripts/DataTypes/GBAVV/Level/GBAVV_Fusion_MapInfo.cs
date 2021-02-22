namespace R1Engine
{
    public class GBAVV_Fusion_MapInfo : R1Serializable
    {
        public GBAVV_BaseManager.LevInfo.FusionType Type { get; set; } // Set before serializing

        public Pointer MapDataPointer { get; set; }
        public int Param_0 { get; set; }
        public int Param_1 { get; set; }

        // Serialized from pointers
        public GBAVV_WorldMap_Data MapData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (Type == GBAVV_BaseManager.LevInfo.FusionType.TimedLevel || Type == GBAVV_BaseManager.LevInfo.FusionType.LevIntInt)
            {
                MapDataPointer = s.SerializePointer(MapDataPointer, name: nameof(MapDataPointer));
                Param_0 = s.Serialize<int>(Param_0, name: nameof(Param_0));

                if (Type == GBAVV_BaseManager.LevInfo.FusionType.LevIntInt)
                    Param_1 = s.Serialize<int>(Param_1, name: nameof(Param_1));
            }


            MapData = s.DoAt(MapDataPointer, () => s.SerializeObject<GBAVV_WorldMap_Data>(MapData, name: nameof(MapData)));
        }
    }
}