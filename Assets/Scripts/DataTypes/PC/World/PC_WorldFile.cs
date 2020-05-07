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

        // Something related to the background PCX files
        public byte Plan0NumPcxCount { get; set; }
        public byte[] Plan0NumPcx { get; set; }

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

        // Includes file name manifest
        public byte[] Unknown5 { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            if (!(Context.Settings.EngineVersion == EngineVersion.RayEduPS1 && FileType != Type.BigRay))
                // Serialize PC Header
                base.SerializeImpl(s);

            if(FileType == Type.World) {
                BG1 = s.Serialize<ushort>(BG1, name: nameof(BG1));
                BG2 = s.Serialize<ushort>(BG2, name: nameof(BG2));
                Plan0NumPcxCount = s.Serialize<byte>(Plan0NumPcxCount, name: nameof(Plan0NumPcxCount));
                VideoBiosCheckSum = s.Serialize<byte>(VideoBiosCheckSum, name: nameof(VideoBiosCheckSum));
                BiosCheckSum = s.Serialize<byte>(BiosCheckSum, name: nameof(BiosCheckSum));
                s.BeginXOR(0x15);
                Plan0NumPcx = s.SerializeArray<byte>(Plan0NumPcx, s.GameSettings.EngineVersion == EngineVersion.RayPC || s.GameSettings.EngineVersion == EngineVersion.RayPocketPC ? Plan0NumPcxCount : Plan0NumPcxCount * 8, name: nameof(Plan0NumPcx));
                s.EndXOR();
            }

            if (FileType == Type.World) {
                SerializeSprites();
                SerializeEta();
            }
            else if (FileType == Type.BigRay)
            {
                DesItemCount = 1;

                DesItems = s.SerializeObjectArray<PC_DES>(DesItems, DesItemCount,
                    onPreSerialize: data => data.FileType = FileType, name: nameof(DesItems));

                SerializeEta();
            }
            else
            {
                SerializeEta();
                SerializeSprites();
            }

            Unknown5 = s.SerializeArray<byte>(Unknown5, s.CurrentLength - s.CurrentPointer.FileOffset, name: nameof(Unknown5));

            // Helper method for reading the eta
            void SerializeEta()
            {
                Eta = s.SerializeArraySize<PC_ETA, byte>(Eta, name: nameof(Eta));
                Eta = s.SerializeObjectArray<PC_ETA>(Eta, Eta.Length, name: nameof(Eta));
            }

            // Helper method for reading the sprites
            void SerializeSprites()
            {
                // Serialize sprites
                DesItemCount = s.Serialize<ushort>(DesItemCount, name: nameof(DesItemCount));

                DesItems = s.SerializeObjectArray<PC_DES>(DesItems, FileType == Type.AllFix && Context.Settings.EngineVersion != EngineVersion.RayEduPS1 ? DesItemCount - 1 : DesItemCount,
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