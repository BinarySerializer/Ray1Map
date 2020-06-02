using System;
using System.Linq;

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
            if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol3 || s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol6)
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
            else if (s.GameSettings.EngineVersion == EngineVersion.RaySaturn)
            {
                ushort value = 0;

                value = (ushort)BitHelpers.SetBits(value, TileMapX, 4, 0);
                value = (ushort)BitHelpers.SetBits(value, TileMapY, 12, 4);

                value = s.Serialize<ushort>(value, name: nameof(value));

                TileMapX = BitHelpers.ExtractBits(value, 4, 0);
                TileMapY = BitHelpers.ExtractBits(value, 12, 4);

                CollisionType = (TileCollisionType)s.Serialize<byte>((byte)CollisionType, name: nameof(CollisionType));

                // TODO: Serialize this? Is it part of collision type? This appears in other PS1 versions too and is skipped there. It appears to always be 0 though.
                s.Serialize<byte>(0);
            }
            else if (s.GameSettings.EngineVersion == EngineVersion.RayJaguar)
            {
                ushort value = 0;

                value = (ushort)BitHelpers.SetBits(value, TileMapX, 12, 0);
                value = (ushort)BitHelpers.SetBits(value, (int)CollisionType, 4, 12);

                value = BitConverter.ToUInt16(s.SerializeArray<byte>(BitConverter.GetBytes(value).Reverse().ToArray(), 2, name: nameof(value)).Reverse().ToArray(), 0);

                TileMapX = BitHelpers.ExtractBits(value, 12, 0);
                CollisionType = (TileCollisionType)BitHelpers.ExtractBits(value, 4, 12);
            }
            else
            {
                ushort value = 0;
                if (s.GameSettings.EngineVersion == EngineVersion.RayPS1 || s.GameSettings.EngineVersion == EngineVersion.Ray2PS1)
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
                if (s.GameSettings.EngineVersion == EngineVersion.RayPS1 || s.GameSettings.EngineVersion == EngineVersion.Ray2PS1)
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