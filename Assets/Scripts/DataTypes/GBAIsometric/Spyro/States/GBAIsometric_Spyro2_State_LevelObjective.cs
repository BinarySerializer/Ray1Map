namespace R1Engine
{
    public class GBAIsometric_Spyro2_State_LevelObjective : R1Serializable
    {
        public ushort ObjectType { get; set; }
        public ushort Ushort_02 { get; set; }
        public ushort Ushort_04 { get; set; }
        public ushort Ushort_06 { get; set; }
        public ushort AnimationGroupIndex_0 { get; set; }
        public ushort AnimSetIndex_0 { get; set; }
        public ushort AnimationGroupIndex_1 { get; set; }
        public ushort AnimSetIndex_1 { get; set; }
        public ushort AnimationGroupIndex_2 { get; set; }
        public ushort AnimSetIndex_2 { get; set; }
        public Pointer UnkPointer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectType = s.Serialize<ushort>(ObjectType, name: nameof(ObjectType));
            Ushort_02 = s.Serialize<ushort>(Ushort_02, name: nameof(Ushort_02));
            Ushort_04 = s.Serialize<ushort>(Ushort_04, name: nameof(Ushort_04));
            Ushort_06 = s.Serialize<ushort>(Ushort_06, name: nameof(Ushort_06));
            AnimationGroupIndex_0 = s.Serialize<ushort>(AnimationGroupIndex_0, name: nameof(AnimationGroupIndex_0));
            AnimSetIndex_0 = s.Serialize<ushort>(AnimSetIndex_0, name: nameof(AnimSetIndex_0));
            AnimationGroupIndex_1 = s.Serialize<ushort>(AnimationGroupIndex_1, name: nameof(AnimationGroupIndex_1));
            AnimSetIndex_1 = s.Serialize<ushort>(AnimSetIndex_1, name: nameof(AnimSetIndex_1));
            AnimationGroupIndex_2 = s.Serialize<ushort>(AnimationGroupIndex_2, name: nameof(AnimationGroupIndex_2));
            AnimSetIndex_2 = s.Serialize<ushort>(AnimSetIndex_2, name: nameof(AnimSetIndex_2));
            UnkPointer = s.SerializePointer(UnkPointer, name: nameof(UnkPointer));
        }
    }
}