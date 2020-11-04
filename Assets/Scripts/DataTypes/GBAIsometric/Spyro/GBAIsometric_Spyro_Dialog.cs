namespace R1Engine
{
    public class GBAIsometric_Spyro_Dialog : R1Serializable
    {
        public uint ID { get; set; }
        public Pointer DialogDataPointer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ID = s.Serialize<uint>(ID, name: nameof(ID));
            DialogDataPointer = s.SerializePointer(DialogDataPointer, name: nameof(DialogDataPointer));
        }
    }
}