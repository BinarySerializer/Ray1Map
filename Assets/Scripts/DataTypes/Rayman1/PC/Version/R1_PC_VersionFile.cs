namespace R1Engine
{
    public class R1_PC_VersionFile : R1Serializable
    {
        public byte VersionsCount { get; set; }
        public byte RuntimeCurrentVersion { get; set; }

        public string[] VersionCodes { get; set; }

        public string[] VersionModes { get; set; }

        public R1_PC_VersionMemoryInfo[] VersionMemoryInfos { get; set; }

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
            RuntimeCurrentVersion = s.Serialize<byte>(RuntimeCurrentVersion, name: nameof(RuntimeCurrentVersion));

            s.DoAt(Offset + 0x02, () => VersionCodes = s.SerializeStringArray(VersionCodes, VersionsCount, 5, name: nameof(VersionCodes)));
            s.DoAt(Offset + 0x52, () => VersionModes = s.SerializeStringArray(VersionModes, VersionsCount, 20, name: nameof(VersionModes)));
            s.DoAt(Offset + 0x192, () => VersionMemoryInfos = s.SerializeObjectArray<R1_PC_VersionMemoryInfo>(VersionMemoryInfos, VersionsCount, name: nameof(VersionMemoryInfos)));

            s.DoAt(Offset + (s.GameSettings.EngineVersion == EngineVersion.R1_PC_Kit ? 0x392 : 0x312), () =>
            {
                DefaultPrimaryHeader = s.SerializeString(DefaultPrimaryHeader, 5, name: nameof(DefaultPrimaryHeader));
                DefaultSecondaryHeader = s.SerializeString(DefaultSecondaryHeader, 5, name: nameof(DefaultSecondaryHeader));
                UnkHeaderValue = s.Serialize<ushort>(UnkHeaderValue, name: nameof(UnkHeaderValue));
            });

            // Make sure we actually end up at the end of the file.
            s.Goto(Offset + (s.GameSettings.EngineVersion == EngineVersion.R1_PC_Kit ? 0x39E : 0x31E));
        }
    }
}
