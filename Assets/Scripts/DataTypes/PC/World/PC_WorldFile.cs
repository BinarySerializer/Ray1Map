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

        public ushort Unknown6 { get; set; }

        public ushort Unknown2 { get; set; }

        public ushort Unknown4Count { get; set; }

        public byte Unknown3 { get; set; }

        public byte[] Unknown4 { get; set; }

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
                // Serialize unknown header
                Unknown6 = s.Serialize(Unknown6, name: "Unknown6");
                Unknown2 = s.Serialize(Unknown2, name: "Unknown2");
                Unknown4Count = s.Serialize(Unknown4Count, name: "Unknown4Count");
                Unknown3 = s.Serialize(Unknown3, name: "Unknown3");
                Unknown4 = s.SerializeArray<byte>(Unknown4, s.GameSettings.GameMode == GameMode.RayPC || s.GameSettings.GameMode == GameMode.RayPocketPC ? Unknown4Count : Unknown4Count * 8, name: "Unknown4");
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