using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Transparent tile texture data for Rayman 1 (PC)
    /// </summary>
    public class PC_R1_TransparentTileTexture : PC_R1_TileTexture
    {
        /// <summary>
        /// The alpha channel values for each texture pixel
        /// </summary>
        public byte[] Alpha { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public override void Deserialize(Stream stream)
        {
            ColorIndexes = stream.ReadBytes(PC_R1_Manager.CellSize * PC_R1_Manager.CellSize);
            Alpha = stream.ReadBytes(PC_R1_Manager.CellSize * PC_R1_Manager.CellSize);
            Unknown1 = stream.ReadBytes(32);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public override void Serialize(Stream stream)
        {
            stream.Write(ColorIndexes);
            stream.Write(Alpha);
            stream.Write(Unknown1);
        }
    }
}