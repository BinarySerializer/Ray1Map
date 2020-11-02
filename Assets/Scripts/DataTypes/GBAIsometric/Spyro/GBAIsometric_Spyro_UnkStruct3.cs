namespace R1Engine
{
    public class GBAIsometric_Spyro_UnkStruct3 : R1Serializable
    {
        public uint ID { get; set; }
        public Pointer Pointer_04 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ID = s.Serialize<uint>(ID, name: nameof(ID));
            Pointer_04 = s.SerializePointer(Pointer_04, name: nameof(Pointer_04));
        }
    }
}