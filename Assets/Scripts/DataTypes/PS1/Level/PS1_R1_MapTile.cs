using System;

namespace R1Engine
{
    /// <summary>
    /// Map tile data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_MapTile : R1Serializable
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
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            ushort value = 0;
            if (s.GameSettings.EngineVersion == EngineVersion.RayPS1) {
                value = (ushort)BitHelpers.SetBits(value, TileMapX, 4, 0);
                value = (ushort)BitHelpers.SetBits(value, TileMapY, 6, 4);
                value = (ushort)BitHelpers.SetBits(value, (int)CollisionType, 6, 10);
            } else if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JP) {
                value = (ushort)BitHelpers.SetBits(value, TileMapX, 9, 0);
                value = (ushort)BitHelpers.SetBits(value, (int)CollisionType, 7, 9);
            }
            value = s.Serialize<ushort>(value, name: "value");
            if (s.GameSettings.EngineVersion == EngineVersion.RayPS1) {
                TileMapX = BitHelpers.ExtractBits(value, 4, 0);
                TileMapY = BitHelpers.ExtractBits(value, 6, 4);
                CollisionType = (TileCollisionType)(BitHelpers.ExtractBits(value, 6, 10));
            } else if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JP) {
                TileMapY = 0;
                TileMapX = BitHelpers.ExtractBits(value, 9, 0);
                CollisionType = (TileCollisionType)BitHelpers.ExtractBits(value, 7, 9);
            }
        }
    }
}