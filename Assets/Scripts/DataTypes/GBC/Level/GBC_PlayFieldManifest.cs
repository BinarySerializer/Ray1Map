namespace R1Engine
{
    public class GBC_PlayFieldManifest : GBC_Block
    {
        public uint PlayFieldsCount { get; set; }
        public GBC_Pointer[] PlayFieldsPointers { get; set; }

        // Parsed
        public GBC_PlayField PlayField { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            base.SerializeImpl(s);

            // Serialize data
            PlayFieldsCount = s.Serialize<uint>(PlayFieldsCount, name: nameof(PlayFieldsCount));
            PlayFieldsPointers = s.SerializeObjectArray<GBC_Pointer>(PlayFieldsPointers, PlayFieldsCount, name: nameof(PlayFieldsPointers));

            // Parse data from pointers
            PlayField = PlayFieldsPointers[s.GameSettings.Level].DoAtBlock(() => s.SerializeObject<GBC_PlayField>(PlayField, name: nameof(PlayField)));
        }
    }
}