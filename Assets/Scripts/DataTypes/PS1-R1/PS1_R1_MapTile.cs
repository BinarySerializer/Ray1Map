using System.IO;

namespace R1Engine 
{
    /// <summary>
    /// Map tile data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_MapTile : ISerializableFile
    {
        /// <summary>
        /// The x position
        /// </summary>
        public int XPosition { get; set; }

        /// <summary>
        /// The y position
        /// </summary>
        public int YPosition;

        /// <summary>
        /// The collision type
        /// </summary>
        public TileCollisionType CollisionType { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public void Deserialize(Stream stream)
        {
            var byte1 = stream.Read<byte>();
            var byte2 = stream.Read<byte>();

            int g = byte1 + ((byte2 & 3) << 8);

            XPosition = g & 15;
            YPosition = g >> 4;
            CollisionType = (TileCollisionType)(byte2 >> 2);
        }

        public void Serialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}