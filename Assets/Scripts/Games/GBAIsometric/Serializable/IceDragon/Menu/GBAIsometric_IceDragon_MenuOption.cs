using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_IceDragon_MenuOption : BinarySerializable
    {
        public int Index { get; set; }
        public GBAIsometric_LocIndex LocIndex { get; set; }
        public int Int_04 { get; set; }
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GetRequiredSettings<GBAIsometricSettings>().EngineVersion == GBAIsometricEngineVersion.Spyro1)
            {
                Index = s.Serialize<int>(Index, name: nameof(Index));
                LocIndex = s.SerializeObject<GBAIsometric_LocIndex>(LocIndex, x => x.Pre_Is32Bit = true, name: nameof(LocIndex));
            }
            else
            {
                Index = s.Serialize<ushort>((ushort)Index, name: nameof(Index));
                LocIndex = s.SerializeObject<GBAIsometric_LocIndex>(LocIndex, name: nameof(LocIndex));
                Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));
                Data = s.SerializeArray<byte>(Data, 12, name: nameof(Data));
            }
        }
    }
}