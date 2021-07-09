using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_LevelStartInfos : BinarySerializable
    {
        public GBAKlonoa_LevelStartInfo StartInfo_Entry { get; set; }
        public GBAKlonoa_LevelStartInfo[] StartInfos_Yellow { get; set; }
        public GBAKlonoa_LevelStartInfo[] StartInfos_Green { get; set; }
        public GBAKlonoa_LevelStartInfo DCT_StartInfo_Unknown { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            StartInfo_Entry = s.SerializeObject<GBAKlonoa_LevelStartInfo>(StartInfo_Entry, name: nameof(StartInfo_Entry));
            StartInfos_Yellow = s.SerializeObjectArray<GBAKlonoa_LevelStartInfo>(StartInfos_Yellow, 10, name: nameof(StartInfos_Yellow));
            StartInfos_Green = s.SerializeObjectArray<GBAKlonoa_LevelStartInfo>(StartInfos_Green, 10, name: nameof(StartInfos_Green));

            if (s.GetR1Settings().EngineVersion == EngineVersion.KlonoaGBA_DCT)
                DCT_StartInfo_Unknown = s.SerializeObject<GBAKlonoa_LevelStartInfo>(DCT_StartInfo_Unknown, name: nameof(DCT_StartInfo_Unknown));
        }
    }
}