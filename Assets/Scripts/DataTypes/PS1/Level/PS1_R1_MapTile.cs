using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Map tile data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_MapTile : IBinarySerializable
    {
        /// <summary>
        /// The tile map x position
        /// </summary>
        public int TileMapX { get; set; }

        /// <summary>
        /// The tile map y position
        /// </summary>
        public int TileMapY { get; set; }

        /// <summary>
        /// The collision type
        /// </summary>
        public TileCollisionType CollisionType { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            var byte1 = deserializer.Read<byte>();
            var byte2 = deserializer.Read<byte>();

            int g = byte1 + ((byte2 & 3) << 8);

            TileMapX = g & 15;
            TileMapY = g >> 4;
            CollisionType = (TileCollisionType)(byte2 >> 2);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            var graphic = (TileMapX + (TileMapY << 4));
            var byte1 = (byte)graphic;
            var byte2 = (byte)(((int)CollisionType << 2) + (graphic >> 8));

            serializer.Write(byte1);
            serializer.Write(byte2);
        }
    }
}