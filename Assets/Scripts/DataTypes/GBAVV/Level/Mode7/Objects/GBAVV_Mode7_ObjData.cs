namespace R1Engine
{
    public class GBAVV_Mode7_ObjData : R1Serializable
    {
        public uint Uint_00 { get; set; }
        public uint ObjectsCount { get; set; }
        public GBAVV_Mode7_Object[] Objects { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Uint_00 = s.Serialize<uint>(Uint_00, name: nameof(Uint_00));
            ObjectsCount = s.Serialize<uint>(ObjectsCount, name: nameof(ObjectsCount));
            Objects = s.SerializeObjectArray<GBAVV_Mode7_Object>(Objects, ObjectsCount, name: nameof(Objects));
        }
    }
}