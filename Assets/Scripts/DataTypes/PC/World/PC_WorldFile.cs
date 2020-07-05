namespace R1Engine
{
    /// <summary>
    /// World data for PC
    /// </summary>
    public class PC_WorldFile : PC_BaseFile
    {
        #region Public Properties

        public Type FileType { get; set; }

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

        /// <summary>
        /// The amount of DES items
        /// </summary>
        public ushort DesItemCount { get; set; }

        /// <summary>
        /// The DES items
        /// </summary>
        public PC_DES[] DesItems { get; set; }

        /// <summary>
        /// The ETA items
        /// </summary>
        public PC_ETA[] Eta { get; set; }

        public byte WorldDefineChecksum { get; set; }
        
        // What is this?
        public byte[] WorldDefines { get; set; }
        
        public string[] DESFileNames { get; set; }

        public string[] ETAFileNames { get; set; }

        public uint RaymanExeCheckSum3 { get; set; }

        public uint[] DESDataIndices { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            // Serialize PC Header
            if (!(Context.Settings.EngineVersion == EngineVersion.RayEduPS1 && FileType != Type.BigRay))
                base.SerializeImpl(s);

            if (FileType == Type.World) 
            {
                // Serialize world data
                BG1 = s.Serialize<ushort>(BG1, name: nameof(BG1));
                BG2 = s.Serialize<ushort>(BG2, name: nameof(BG2));
                Plan0NumPcxCount = s.Serialize<byte>(Plan0NumPcxCount, name: nameof(Plan0NumPcxCount));
                VideoBiosCheckSum = s.Serialize<byte>(VideoBiosCheckSum, name: nameof(VideoBiosCheckSum));
                BiosCheckSum = s.Serialize<byte>(BiosCheckSum, name: nameof(BiosCheckSum));

                if (s.GameSettings.EngineVersion == EngineVersion.RayPC || s.GameSettings.EngineVersion == EngineVersion.RayPocketPC)
                    s.DoXOR(0x15, () => Plan0NumPcx = s.SerializeArray<byte>(Plan0NumPcx, Plan0NumPcxCount, name: nameof(Plan0NumPcx)));
                else
                    s.DoXOR(0x19, () => Plan0NumPcxFiles = s.SerializeStringArray(Plan0NumPcxFiles, Plan0NumPcxCount, 8, name: nameof(Plan0NumPcxFiles)));

                // Serialize DES & ETA
                SerializeDES();
                SerializeETA();
                
                // Kit and EDU have more data...
                if (s.GameSettings.EngineVersion == EngineVersion.RayKitPC || s.GameSettings.EngineVersion == EngineVersion.RayEduPC)
                {
                    // Serialize world defines
                    WorldDefineChecksum = s.DoChecksum(new Checksum8Calculator(false), () =>
                    {
                        s.DoXOR(0x71, () => WorldDefines = s.SerializeArray<byte>(WorldDefines, 26, name: nameof(WorldDefines)));
                    }, ChecksumPlacement.Before, name: nameof(WorldDefineChecksum));

                    // Serialize file tables
                    if (s.GameSettings.EngineVersion == EngineVersion.RayKitPC)
                    {
                        DESFileNames = s.SerializeStringArray(DESFileNames, 100, 13, name: nameof(DESFileNames));
                        ETAFileNames = s.SerializeStringArray(ETAFileNames, 60, 13, name: nameof(ETAFileNames));
                    }
                }
            }
            else if (FileType == Type.BigRay)
            {
                DesItemCount = 1;

                DesItems = s.SerializeObjectArray<PC_DES>(DesItems, DesItemCount,
                    onPreSerialize: data => data.FileType = FileType, name: nameof(DesItems));

                SerializeETA();
            }
            else
            {
                SerializeETA();
                SerializeDES();

                RaymanExeCheckSum3 = s.Serialize(RaymanExeCheckSum3, name: nameof(RaymanExeCheckSum3));

                // NOTE: The length is always hard-coded in the games, so it's not dependent on the DES count value
                DESDataIndices = s.SerializeArray<uint>(DESDataIndices, DesItemCount - 1, name: nameof(DESDataIndices));
            }

            // Helper method for reading the ETA
            void SerializeETA()
            {
                Eta = s.SerializeArraySize<PC_ETA, byte>(Eta, name: nameof(Eta));
                Eta = s.SerializeObjectArray<PC_ETA>(Eta, Eta.Length, name: nameof(Eta));
            }

            // Helper method for reading the DES
            void SerializeDES()
            {
                // Serialize sprites
                DesItemCount = s.Serialize<ushort>(DesItemCount, name: nameof(DesItemCount));

                DesItems = s.SerializeObjectArray<PC_DES>(DesItems, FileType == Type.AllFix ? DesItemCount - 1 : DesItemCount,
                    onPreSerialize: data => data.FileType = FileType, name: nameof(DesItems));
            }
        }

        #endregion

        public enum Type {
            World,
            AllFix,
            BigRay
        }
    }
}