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
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemo)
            {
                int value = 0;

                value = BitHelpers.SetBits(value, TileMapX, 10, 0);
                value = BitHelpers.SetBits(value, TileMapY, 6, 10);
                value = BitHelpers.SetBits(value, (int)CollisionType, 8, 16);

                value = s.Serialize<int>(value, name: nameof(value));

                TileMapX = BitHelpers.ExtractBits(value, 10, 0);
                TileMapY = BitHelpers.ExtractBits(value, 6, 10);
                CollisionType = (TileCollisionType)BitHelpers.ExtractBits(value, 8, 16);
            }
            else
            {
                ushort value = 0;
                if (s.GameSettings.EngineVersion == EngineVersion.RayPS1)
                {
                    value = (ushort)BitHelpers.SetBits(value, TileMapX, 4, 0);
                    value = (ushort)BitHelpers.SetBits(value, TileMapY, 6, 4);
                    value = (ushort)BitHelpers.SetBits(value, (int)CollisionType, 6, 10);
                }
                else if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JP)
                {
                    value = (ushort)BitHelpers.SetBits(value, TileMapX, 9, 0);
                    value = (ushort)BitHelpers.SetBits(value, (int)CollisionType, 7, 9);
                }
                value = s.Serialize<ushort>(value, name: nameof(value));
                if (s.GameSettings.EngineVersion == EngineVersion.RayPS1)
                {
                    TileMapX = BitHelpers.ExtractBits(value, 4, 0);
                    TileMapY = BitHelpers.ExtractBits(value, 6, 4);
                    CollisionType = (TileCollisionType)(BitHelpers.ExtractBits(value, 6, 10));
                }
                else if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JP)
                {
                    TileMapY = 0;
                    TileMapX = BitHelpers.ExtractBits(value, 9, 0);
                    CollisionType = (TileCollisionType)BitHelpers.ExtractBits(value, 7, 9);
                }
            }
        }
    }
}