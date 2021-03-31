using BinarySerializer;

namespace R1Engine
{
    public class R1_PC_VersionMemoryInfo : BinarySerializable
    {
        public uint TailleMainMemTmp { get; set; }
        public uint TailleMainMemFix { get; set; }
        public uint TailleMainMemWorld { get; set; }
        public uint TailleMainMemLevel { get; set; }
        public uint TailleMainMemSprite { get; set; }
        public uint TailleMainMemSamplesTable { get; set; }
        public uint TailleMainMemEdit { get; set; }
        public uint TailleMainMemSaveEvent { get; set; }

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
            
            TailleMainMemWorld = s.Serialize<uint>(TailleMainMemWorld, name: nameof(TailleMainMemWorld));
            s.Log($"{nameof(TailleMainMemWorld)}: {TailleMainMemWorld << 10} bytes");
            
            TailleMainMemLevel = s.Serialize<uint>(TailleMainMemLevel, name: nameof(TailleMainMemLevel));
            s.Log($"{nameof(TailleMainMemLevel)}: {TailleMainMemLevel << 10} bytes");
            
            TailleMainMemSprite = s.Serialize<uint>(TailleMainMemSprite, name: nameof(TailleMainMemSprite));
            s.Log($"{nameof(TailleMainMemSprite)}: {TailleMainMemSprite << 10} bytes");
            
            TailleMainMemSamplesTable = s.Serialize<uint>(TailleMainMemSamplesTable, name: nameof(TailleMainMemSamplesTable));
            s.Log($"{nameof(TailleMainMemSamplesTable)}: {TailleMainMemSamplesTable << 10} bytes");

            if (s.GetR1Settings().EngineVersion == EngineVersion.R1_PC_Kit)
            {
                TailleMainMemEdit = s.Serialize<uint>(TailleMainMemEdit, name: nameof(TailleMainMemEdit));
                s.Log($"{nameof(TailleMainMemEdit)}: {TailleMainMemEdit << 10} bytes");
                
                TailleMainMemSaveEvent = s.Serialize<uint>(TailleMainMemSaveEvent, name: nameof(TailleMainMemSaveEvent));
                s.Log($"{nameof(TailleMainMemSaveEvent)}: {TailleMainMemSaveEvent << 10} bytes");
            }
        }
    }
}