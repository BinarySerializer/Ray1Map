using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Tile texture data for Rayman 1 (PC)
    /// </summary>
    public class PC_R1_TileTexture : ISerializableFile
    {
        /// <summary>
        /// The size of the texture in pixels
        /// </summary>
        public const int Size = 16;

        /// <summary>
        /// The offset for this texture, as defines in the textures offset table. This value is not a part of the texture and has to be set manually.
        /// </summary>
        public uint Offset { get; set; }

        /// <summary>
        /// The color indexes for this texture
        /// </summary>
        public byte[,] ColorIndexes { get; set; }

        /// <summary>
        /// Unknown array of bytes, always 32 in length
        /// </summary>
        public byte[] Unknown1 { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public virtual void Deserialize(Stream stream)
        {
            // Set the color array
            ColorIndexes = new byte[Size, Size];

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    ColorIndexes[x, y] = stream.Read<byte>();
                }
            }

            Unknown1 = stream.ReadBytes(32);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public virtual void Serialize(Stream stream)
        {
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    stream.Write(ColorIndexes[x, y]);
                }
            }

            stream.Write(Unknown1);
        }
    }
}