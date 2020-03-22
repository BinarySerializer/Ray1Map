using System;
using System.ComponentModel;

namespace R1Engine
{
    /// <summary>
    /// World data for PC
    /// </summary>
    [Description("Rayman PC World File")]
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
        public PC_DesItem[] DesItems { get; set; }

        /// <summary>
        /// The ETA data
        /// </summary>
        public PC_Eta[][][] Eta { get; set; }

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

                DesItems = s.SerializeObjectArray<PC_DesItem>(DesItems, DesItemCount,
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
                Eta = s.SerializeArraySize<PC_Eta[][], byte>(Eta, name: "Eta");
                for (int i = 0; i < Eta.Length; i++) {
                    Eta[i] = s.SerializeArraySize<PC_Eta[], byte>(Eta[i], name: "Eta[" + i + "]");

                    for (int j = 0; j < Eta[i].Length; j++) {
                        Eta[i][j] = s.SerializeArraySize<PC_Eta, byte>(Eta[i][j], name: "Eta[" + i + "][" + j + "]");
                        Eta[i][j] = s.SerializeObjectArray<PC_Eta>(Eta[i][j], Eta[i][j].Length, name: "Eta[" + i + "][" + j + "]");
                    }
                }
            }

            // Helper method for reading the sprites
            void SerializeSprites()
            {
                // Serialize sprites
                DesItemCount = s.Serialize(DesItemCount, name: "DesItemCount");

                if (FileType == Type.AllFix && s is BinaryDeserializer)
                    DesItemCount--;

                DesItems = s.SerializeObjectArray<PC_DesItem>(DesItems, DesItemCount,
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