namespace R1Engine
{
    public class GBAIsometric_RHR_AnimSet_Unk : R1Serializable
    {
        public ushort Unknown { get; set; }
        public bool IsLastUnk { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeBitValues<ushort>(bitFunc => {
                Unknown = (ushort)bitFunc(Unknown, 15, name: nameof(Unknown));
                IsLastUnk = bitFunc(IsLastUnk ? 1 : 0, 1, name: nameof(IsLastUnk)) == 1;
            });
        }
    }
}