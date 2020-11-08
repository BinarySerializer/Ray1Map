namespace R1Engine
{
    public class GBAIsometric_Spyro_AnimDescriptor : R1Serializable
    {
        public ushort AnimSpeed { get; set; }
        public ushort AnimIndex { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            AnimSpeed = s.Serialize<ushort>(AnimSpeed, name: nameof(AnimSpeed));
            AnimIndex = s.Serialize<ushort>(AnimIndex, name: nameof(AnimIndex));
        }
    }
}