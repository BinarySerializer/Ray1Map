namespace R1Engine
{
    public class R1_PC_WorldFile : R1_PC_BaseWorldFile
    {
        // Unknown values related to backgrounds (most likely parallax scrolling)
        public ushort BG1 { get; set; }
        public ushort BG2 { get; set; }

        // Some DOS related values?
        public byte BiosCheckSum { get; set; }
        public byte VideoBiosCheckSum { get; set; }

        /// <summary>
        /// The amount of referenced vignette PCX files
        /// </summary>
        public byte Plan0NumPcxCount { get; set; }

        /// <summary>
        /// The referenced PCX files, indexed (Rayman 1 PC)
        /// </summary>
        public byte[] Plan0NumPcx { get; set; }

        /// <summary>
        /// The referenced PCX files, names (RayKit, EDU etc.)
        /// </summary>
        public string[] Plan0NumPcxFiles { get; set; }

        public byte WorldDefineChecksum { get; set; }

        public R1_PC_WorldDefine WorldDefine { get; set; }

        public string[] DESFileNames { get; set; }

        public string[] ETAFileNames { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize PC Header
            base.SerializeImpl(s);

            // Serialize world data
            BG1 = s.Serialize<ushort>(BG1, name: nameof(BG1));
            BG2 = s.Serialize<ushort>(BG2, name: nameof(BG2));
            Plan0NumPcxCount = s.Serialize<byte>(Plan0NumPcxCount, name: nameof(Plan0NumPcxCount));
            VideoBiosCheckSum = s.Serialize<byte>(VideoBiosCheckSum, name: nameof(VideoBiosCheckSum));
            BiosCheckSum = s.Serialize<byte>(BiosCheckSum, name: nameof(BiosCheckSum));

            if (s.GameSettings.EngineVersion == EngineVersion.R1_PC || s.GameSettings.EngineVersion == EngineVersion.R1_PocketPC)
                s.DoXOR(0x15, () => Plan0NumPcx = s.SerializeArray<byte>(Plan0NumPcx, Plan0NumPcxCount, name: nameof(Plan0NumPcx)));
            else
                s.DoXOR(0x19, () => Plan0NumPcxFiles = s.SerializeStringArray(Plan0NumPcxFiles, Plan0NumPcxCount, 8, name: nameof(Plan0NumPcxFiles)));

            // Serialize the DES
            DesItemCount = s.Serialize<ushort>(DesItemCount, name: nameof(DesItemCount));

            DesItems = s.SerializeObjectArray<R1_PC_DES>(DesItems, DesItemCount, onPreSerialize: data => data.FileType = R1_PC_DES.Type.World, name: nameof(DesItems));

            // Serialize the ETA
            Eta = s.SerializeArraySize<R1_PC_ETA, byte>(Eta, name: nameof(Eta));
            Eta = s.SerializeObjectArray<R1_PC_ETA>(Eta, Eta.Length, name: nameof(Eta));

            // Kit and EDU have more data...
            if (s.GameSettings.EngineVersion == EngineVersion.R1_PC_Kit || s.GameSettings.EngineVersion == EngineVersion.R1_PC_Edu)
            {
                // Serialize world defines
                WorldDefineChecksum = s.DoChecksum(new Checksum8Calculator(false), () =>
                {
                    s.DoXOR(0x71, () => WorldDefine = s.SerializeObject<R1_PC_WorldDefine>(WorldDefine, name: nameof(WorldDefine)));
                }, ChecksumPlacement.Before, name: nameof(WorldDefineChecksum));

                // Serialize file tables
                if (s.GameSettings.EngineVersion == EngineVersion.R1_PC_Kit)
                {
                    DESFileNames = s.SerializeStringArray(DESFileNames, 100, 13, name: nameof(DESFileNames));
                    ETAFileNames = s.SerializeStringArray(ETAFileNames, 60, 13, name: nameof(ETAFileNames));
                }
            }
        }
    }
}