namespace R1Engine
{
    public class GBAIsometric_LevelDataLayer : R1Serializable
    {
        public Pointer<GBAIsometric_MapLayer> DataPointer { get; set; }
        public byte[] UnkData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            DataPointer = s.SerializePointer<GBAIsometric_MapLayer>(DataPointer, resolve: true, name: nameof(DataPointer));
            UnkData = s.SerializeArray<byte>(UnkData, 16, name: nameof(UnkData));
        }
    }
}