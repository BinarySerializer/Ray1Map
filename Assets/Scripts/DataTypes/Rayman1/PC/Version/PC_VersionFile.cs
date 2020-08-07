namespace R1Engine
{
    public class PC_VersionFile : R1Serializable
    {
        public byte VersionsCount { get; set; }
        public byte Unk1 { get; set; }

        public string[] VersionCodes { get; set; }

        public string[] VersionModes { get; set; }

        public PC_VersionEntry[] VersionEntries { get; set; }

        public string DefaultPrimaryHeader { get; set; }
        public string DefaultSecondaryHeader { get; set; }
        public ushort UnkHeaderValue { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            VersionsCount = s.Serialize<byte>(VersionsCount, name: nameof(VersionsCount));
            Unk1 = s.Serialize<byte>(Unk1, name: nameof(Unk1));

            s.DoAt(Offset + 0x02, () => VersionCodes = s.SerializeStringArray(VersionCodes, VersionsCount, 5, name: nameof(VersionCodes)));
            s.DoAt(Offset + 0x52, () => VersionModes = s.SerializeStringArray(VersionModes, VersionsCount, 20, name: nameof(VersionModes)));
            s.DoAt(Offset + 0x192, () => VersionEntries = s.SerializeObjectArray<PC_VersionEntry>(VersionEntries, VersionsCount, name: nameof(VersionEntries)));

            s.DoAt(Offset + (s.GameSettings.EngineVersion == EngineVersion.RayKitPC ? 0x392 : 0x312), () =>
            {
                DefaultPrimaryHeader = s.SerializeString(DefaultPrimaryHeader, 5, name: nameof(DefaultPrimaryHeader));
                DefaultSecondaryHeader = s.SerializeString(DefaultSecondaryHeader, 5, name: nameof(DefaultSecondaryHeader));
                UnkHeaderValue = s.Serialize<ushort>(UnkHeaderValue, name: nameof(UnkHeaderValue));
            });
        }
    }
}