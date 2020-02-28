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
        public byte[,] Alpha { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public override void Deserialize(Stream stream)
        {
            ColorIndexes = new byte[Size, Size];

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    ColorIndexes[x, y] = stream.Read<byte>();
                }
            }

            Alpha = new byte[Size, Size];

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    Alpha[x, y] = stream.Read<byte>();
                }
            }

            Unknown1 = stream.ReadBytes(32);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public override void Serialize(Stream stream)
        {
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    stream.Write(ColorIndexes[x, y]);
                }
            }

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    stream.Write(Alpha[x, y]);
                }
            }

            stream.Write(Unknown1);
        }
    }
}