namespace R1Engine
{
    public class GBAIsometric_AnimFrame : R1Serializable
    {
        public ushort PatternIndex { get; set; }
        public ushort TileIndicesIndex { get; set; }

        public int Spyro_UnkBitFieldValue { get; set; }
        public byte Spyro_Byte02 { get; set; }
        public byte Spyro_Byte03 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_RHR)
            {
                PatternIndex = s.Serialize<ushort>(PatternIndex, name: nameof(PatternIndex));
                TileIndicesIndex = s.Serialize<ushort>(TileIndicesIndex, name: nameof(TileIndicesIndex));
            }
            else
            {
                s.SerializeBitValues<ushort>(bitFunc =>
                {
                    PatternIndex = (ushort)bitFunc(PatternIndex, 11, name: nameof(PatternIndex));
                    Spyro_UnkBitFieldValue = bitFunc(Spyro_UnkBitFieldValue, 5, name: nameof(Spyro_UnkBitFieldValue));
                });
                Spyro_Byte02 = s.Serialize<byte>(Spyro_Byte02, name: nameof(Spyro_Byte02));
                Spyro_Byte03 = s.Serialize<byte>(Spyro_Byte03, name: nameof(Spyro_Byte03));
            }
        }
    }
}