namespace R1Engine
{
    public class R1_PC_VersionMemoryInfo : R1Serializable
    {
        public uint TailleMainMemTmp { get; set; }
        public uint TailleMainMemFix { get; set; }
        public uint TailleMainMemSprite { get; set; }
        public uint TailleMainMemWorld { get; set; }
        public uint TailleMainMemLevel { get; set; }
        public uint SamplesTableMemory { get; set; }

        // Some fixed data
        public uint Unk7 { get; set; }
        public uint Unk8 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            TailleMainMemTmp = s.Serialize<uint>(TailleMainMemTmp, name: nameof(TailleMainMemTmp));
            s.Log($"{nameof(TailleMainMemTmp)}: {TailleMainMemTmp << 10} bytes");
            
            TailleMainMemFix = s.Serialize<uint>(TailleMainMemFix, name: nameof(TailleMainMemFix));
            s.Log($"{nameof(TailleMainMemFix)}: {TailleMainMemFix << 10} bytes");
            
            TailleMainMemSprite = s.Serialize<uint>(TailleMainMemSprite, name: nameof(TailleMainMemSprite));
            s.Log($"{nameof(TailleMainMemSprite)}: {TailleMainMemSprite << 10} bytes");
            
            TailleMainMemWorld = s.Serialize<uint>(TailleMainMemWorld, name: nameof(TailleMainMemWorld));
            s.Log($"{nameof(TailleMainMemWorld)}: {TailleMainMemWorld << 10} bytes");
            
            TailleMainMemLevel = s.Serialize<uint>(TailleMainMemLevel, name: nameof(TailleMainMemLevel));
            s.Log($"{nameof(TailleMainMemLevel)}: {TailleMainMemLevel << 10} bytes");
            
            SamplesTableMemory = s.Serialize<uint>(SamplesTableMemory, name: nameof(SamplesTableMemory));
            s.Log($"{nameof(SamplesTableMemory)}: {SamplesTableMemory << 10} bytes");

            if (s.GameSettings.EngineVersion == EngineVersion.R1_PC_Kit)
            {
                Unk7 = s.Serialize<uint>(Unk7, name: nameof(Unk7));
                s.Log($"{nameof(Unk7)}: {Unk7 << 10} bytes");
                
                Unk8 = s.Serialize<uint>(Unk8, name: nameof(Unk8));
                s.Log($"{nameof(Unk8)}: {Unk8 << 10} bytes");
            }
        }
    }
}