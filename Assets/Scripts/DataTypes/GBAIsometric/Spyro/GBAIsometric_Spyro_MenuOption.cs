using BinarySerializer;

namespace R1Engine
{
    public class GBAIsometric_Spyro_MenuOption : BinarySerializable
    {
        public ushort Index { get; set; }
        public GBAIsometric_LocIndex LocIndex { get; set; }
        public int Int_00 { get; set; }
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Index = s.Serialize<ushort>(Index, name: nameof(Index));
            LocIndex = s.SerializeObject<GBAIsometric_LocIndex>(LocIndex, name: nameof(LocIndex));
            Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
            Data = s.SerializeArray<byte>(Data, 12, name: nameof(Data));
        }
    }
}