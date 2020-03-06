using System;
using System.ComponentModel;

namespace R1Engine
{
    // TODO: Add support for bray.dat and read the data which appears at the end of the allfix.dat file

    /// <summary>
    /// World data for PC
    /// </summary>
    [Description("Rayman PC World File")]
    public class PC_WorldFile : IBinarySerializable
    {
        #region Public Properties

        public ushort Unknown1 { get; set; }

        public ushort Unknown2 { get; set; }

        public ushort Unknown4Count { get; set; }

        public byte Unknown3 { get; set; }

        public byte[] Unknown4 { get; set; }

        /// <summary>
        /// The amount of sprite groups
        /// </summary>
        public ushort SpriteGroupCount { get; set; }

        /// <summary>
        /// The sprite groups
        /// </summary>
        public PC_DesItem[] SpriteGroups { get; set; }

        /// <summary>
        /// The ETA data
        /// </summary>
        public PC_Eta[][][] Eta { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            if (deserializer.FileExtension == ".wld")
            {
                // Read unknown header
                Unknown1 = deserializer.Read<ushort>();
                Unknown2 = deserializer.Read<ushort>();
                Unknown4Count = deserializer.Read<ushort>();
                Unknown3 = deserializer.Read<byte>();
                Unknown4 = deserializer.Read<byte>(Unknown4Count);
            }

            if (deserializer.FileExtension == ".wld")
            {
                ReadSprites();
                ReadEta();
            }
            else
            {
                ReadEta();
                ReadSprites();
            }

            // Helper method for reading the eta
            void ReadEta()
            {
                // Read the ETA data into a 3-fold array
                Eta = new PC_Eta[deserializer.Read<byte>()][][];

                for (int i = 0; i < Eta.Length; i++)
                {
                    Eta[i] = new PC_Eta[deserializer.Read<byte>()][];

                    for (int j = 0; j < Eta[i].Length; j++)
                    {
                        Eta[i][j] = new PC_Eta[deserializer.Read<byte>()];

                        for (int k = 0; k < Eta[i][j].Length; k++)
                        {
                            Eta[i][j][k] = deserializer.Read<PC_Eta>();
                        }
                    }
                }
            }

            // Helper method for reading the sprites
            void ReadSprites()
            {
                // Read sprites
                SpriteGroupCount = deserializer.Read<ushort>();

                if (deserializer.FileName == "allfix.dat")
                    SpriteGroupCount--;

                SpriteGroups = deserializer.Read<PC_DesItem>(SpriteGroupCount);
            }
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}