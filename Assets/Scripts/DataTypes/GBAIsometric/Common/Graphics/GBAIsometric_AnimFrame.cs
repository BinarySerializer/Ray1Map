namespace R1Engine
{
    public class GBAIsometric_AnimFrame : R1Serializable
    {
        public ushort PatternIndex { get; set; }
        public ushort TileIndicesIndex { get; set; }

        public ushort Spyro_UnkIndex { get; set; }
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
                Spyro_UnkIndex = s.Serialize<ushort>(Spyro_UnkIndex, name: nameof(Spyro_UnkIndex));
                Spyro_Byte02 = s.Serialize<byte>(Spyro_Byte02, name: nameof(Spyro_Byte02));
                Spyro_Byte03 = s.Serialize<byte>(Spyro_Byte03, name: nameof(Spyro_Byte03));
            }
        }
    }
}