namespace R1Engine
{
    public class GBAIsometric_Spyro3_State_NPC : R1Serializable
    {
        public ushort ObjectType { get; set; }
        public short Short_02 { get; set; }
        public ushort Ushort_04 { get; set; }
        public ushort Ushort_06 { get; set; }
        public ushort AnimationGroupIndex { get; set; }
        public ushort AnimSetIndex { get; set; }
        public byte[] UnkBytes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectType = s.Serialize<ushort>(ObjectType, name: nameof(ObjectType));
            Short_02 = s.Serialize<short>(Short_02, name: nameof(Short_02));
            Ushort_04 = s.Serialize<ushort>(Ushort_04, name: nameof(Ushort_04));
            Ushort_06 = s.Serialize<ushort>(Ushort_06, name: nameof(Ushort_06));
            AnimationGroupIndex = s.Serialize<ushort>(AnimationGroupIndex, name: nameof(AnimationGroupIndex));
            AnimSetIndex = s.Serialize<ushort>(AnimSetIndex, name: nameof(AnimSetIndex));
            UnkBytes = s.SerializeArray<byte>(UnkBytes, 12, name: nameof(UnkBytes));
        }
    }
}