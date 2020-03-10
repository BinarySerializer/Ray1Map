using System;

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
            // Read the value
            var value = deserializer.Read<ushort>();

            if (deserializer.GameSettings.GameMode == GameMode.RayPS1)
            {
                TileMapX = BitHelpers.ExtractBits(value, 4, 0);
                TileMapY = BitHelpers.ExtractBits(value, 6, 4);
                CollisionType = (TileCollisionType)(BitHelpers.ExtractBits(value, 6, 10));
            }
            else if (deserializer.GameSettings.GameMode == GameMode.RayPS1JP)
            {
                TileMapY = 0;
                TileMapX = BitHelpers.ExtractBits(value, 9, 0);
                CollisionType = (TileCollisionType)BitHelpers.ExtractBits(value, 7, 9);
            }
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            if (serializer.GameSettings.GameMode == GameMode.RayPS1)
            {
                // TODO: Clean up

                var graphic = (TileMapX + (TileMapY << 4));
                var byte1 = (byte)graphic;
                var byte2 = (byte)(((int)CollisionType << 2) + (graphic >> 8));

                serializer.Write(byte1);
                serializer.Write(byte2);
            }
            else if (serializer.GameSettings.GameMode == GameMode.RayPS1JP)
            {
                throw new NotImplementedException();
            }
        }
    }
}