namespace R1Engine
{
    public class GBACrash_Animation : R1Serializable
    {
        public Pointer FrameTablePointer { get; set; }
        public byte[] Data { get; set; }

        // Serialized from pointers

        public ushort[] FrameTable { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FrameTablePointer = s.SerializePointer(FrameTablePointer, name: nameof(FrameTablePointer));
            Data = s.SerializeArray<byte>(Data, 24, name: nameof(Data));

            FrameTable = s.DoAt(FrameTablePointer, () => s.SerializeArray<ushort>(FrameTable, Data[18], name: nameof(FrameTable)));
        }
    }
}