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
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            // Serialize PC Header
            base.SerializeImpl(s);

            if(FileType == Type.World) {
                BG1 = s.Serialize(BG1, name: "BG1");
                BG2 = s.Serialize(BG2, name: "BG2");
                Plan0NumPcxCount = s.Serialize(Plan0NumPcxCount, name: "Plan0NumPcxCount");
                VideoBiosCheckSum = s.Serialize(VideoBiosCheckSum, name: "VideoBiosCheckSum");
                BiosCheckSum = s.Serialize(BiosCheckSum, name: "BiosCheckSum");
                s.BeginXOR(0x15);
                Plan0NumPcx = s.SerializeArray<byte>(Plan0NumPcx, s.GameSettings.GameMode == GameMode.RayPC || s.GameSettings.GameMode == GameMode.RayPocketPC ? Plan0NumPcxCount : Plan0NumPcxCount * 8, name: "Plan0NumPcx");
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
                    onPreSerialize: data => data.FileType = FileType, name: "DesItems");

                SerializeEta();
            }
            else
            {
                SerializeEta();
                SerializeSprites();
            }

            Unknown5 = s.SerializeArray<byte>(Unknown5, s.CurrentLength - s.CurrentPointer.FileOffset, name: "Unknown5");

            // Helper method for reading the eta
            void SerializeEta()
            {
                Eta = s.SerializeArraySize<PC_ETA, byte>(Eta, name: "Eta");
                Eta = s.SerializeObjectArray<PC_ETA>(Eta, Eta.Length, name: "Eta");
            }

            // Helper method for reading the sprites
            void SerializeSprites()
            {
                // Serialize sprites
                DesItemCount = s.Serialize(DesItemCount, name: "DesItemCount");

                if (FileType == Type.AllFix && s is BinaryDeserializer)
                    DesItemCount--;

                DesItems = s.SerializeObjectArray<PC_DES>(DesItems, DesItemCount,
                    onPreSerialize: data => data.FileType = FileType, name: "DesItems");
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