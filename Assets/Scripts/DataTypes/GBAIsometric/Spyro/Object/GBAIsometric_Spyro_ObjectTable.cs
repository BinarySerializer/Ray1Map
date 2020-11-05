namespace R1Engine
{
    public class GBAIsometric_Spyro_ObjectTable : R1Serializable
    {
        public uint Count { get; set; }
        public GBAIsometric_Object[] Objects { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Count = s.Serialize<uint>(Count, name: nameof(Count));
            Objects = s.SerializeObjectArray<GBAIsometric_Object>(Objects, Count, name: nameof(Objects));
        }
    }
}