namespace R1Engine
{
    public class GBAIsometric_LevelDataLayer : R1Serializable
    {
        public Pointer DataPointer { get; set; }
        public byte[] UnkData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            DataPointer = s.SerializePointer(DataPointer, name: nameof(DataPointer));
            UnkData = s.SerializeArray<byte>(UnkData, 16, name: nameof(UnkData));
        }
    }
}