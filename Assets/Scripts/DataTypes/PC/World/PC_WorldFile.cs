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
        public override void Serialize(BinarySerializer serializer)
        {
            // Serialize PC Header
            base.Serialize(serializer);

            if (serializer.FileName.Contains(".wld"))
            {
                // Serialize unknown header
                serializer.Serialize(nameof(Unknown6));
                serializer.Serialize(nameof(Unknown2));
                serializer.Serialize(nameof(Unknown4Count));
                serializer.Serialize(nameof(Unknown3));
                serializer.SerializeArray<byte>(nameof(Unknown4), serializer.GameSettings.GameMode == GameMode.RayPC || serializer.GameSettings.GameMode == GameMode.RayPocketPC ? Unknown4Count : Unknown4Count * 8);
            }

            if (serializer.FileName.Contains(".wld"))
            {
                SerializeSprites();
                SerializeEta();
            }
            else if (serializer.FileName.Contains("bray.dat") || serializer.FileName.Contains("bigray.dat"))
            {
                DesItemCount = 1;

                serializer.SerializeArray<PC_DesItem>(nameof(DesItems), DesItemCount);

                SerializeEta();
            }
            else
            {
                SerializeEta();
                SerializeSprites();
            }

            serializer.SerializeArray<byte>(nameof(Unknown5), serializer.BaseStream.Length - serializer.BaseStream.Position);

            // Helper method for reading the eta
            void SerializeEta()
            {
                if (serializer.Mode == SerializerMode.Read)
                {
                    // Read the ETA data into a 3-fold array
                    Eta = new PC_Eta[serializer.Read<byte>()][][];

                    for (int i = 0; i < Eta.Length; i++)
                    {
                        Eta[i] = new PC_Eta[serializer.Read<byte>()][];

                        for (int j = 0; j < Eta[i].Length; j++)
                        {
                            Eta[i][j] = new PC_Eta[serializer.Read<byte>()];

                            for (int k = 0; k < Eta[i][j].Length; k++)
                            {
                                Eta[i][j][k] = serializer.Read<PC_Eta>();
                            }
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            // Helper method for reading the sprites
            void SerializeSprites()
            {
                // Serialize sprites
                serializer.Serialize(nameof(DesItemCount));

                if (serializer.FileName.Contains("allfix.dat") && serializer.Mode == SerializerMode.Read)
                    DesItemCount--;

                serializer.SerializeArray<PC_DesItem>(nameof(DesItems), DesItemCount);
            }
        }

        #endregion
    }
}