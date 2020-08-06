using System;

namespace R1Engine
{
    /// <summary>
    /// Common map tile data
    /// </summary>
    public class MapTile : R1Serializable, ICloneable
    {
        /// <summary>
        /// The tile map x position
        /// </summary>
        public ushort TileMapX { get; set; }

        /// <summary>
        /// The tile map y position
        /// </summary>
        public ushort TileMapY { get; set; }

        /// <summary>
        /// The tile collision type
        /// </summary>
        public byte CollisionType { get; set; }

        public byte PC_Unk1 { get; set; }

        /// <summary>
        /// The transparency mode for this cell
        /// </summary>
        public PC_MapTileTransparencyMode PC_TransparencyMode { get; set; }

        public byte PC_Unk2 { get; set; }

        public bool HorizontalFlip { get; set; }
        public bool VerticalFlip { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            if (s.GameSettings.GameModeSelection == GameModeSelection.MapperPC || s.GameSettings.EngineVersion == EngineVersion.RayGBA|| s.GameSettings.EngineVersion == EngineVersion.RayDSi)
            {
                TileMapY = s.Serialize<ushort>(TileMapY, name: nameof(TileMapY));
                TileMapX = 0;
                CollisionType = (byte)s.Serialize<ushort>((ushort)CollisionType, name: nameof(CollisionType));
            }
            else if (s.GameSettings.MajorEngineVersion == MajorEngineVersion.PC)
            {
                TileMapY = s.Serialize<ushort>(TileMapY, name: nameof(TileMapY));
                TileMapX = 0;
                CollisionType = s.Serialize<byte>(CollisionType, name: nameof(CollisionType));
                PC_Unk1 = s.Serialize<byte>(PC_Unk1, name: nameof(PC_Unk1));
                PC_TransparencyMode = s.Serialize<PC_MapTileTransparencyMode>(PC_TransparencyMode, name: nameof(PC_TransparencyMode));
                PC_Unk2 = s.Serialize<byte>(PC_Unk2, name: nameof(PC_Unk2));
            }
            else if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol3 || s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol6)
            {
                int value = 0;

                value = BitHelpers.SetBits(value, TileMapX, 10, 0);
                value = BitHelpers.SetBits(value, TileMapY, 6, 10);
                value = BitHelpers.SetBits(value, (int)CollisionType, 8, 16);

                value = s.Serialize<int>(value, name: nameof(value));

                TileMapX = (ushort)BitHelpers.ExtractBits(value, 10, 0);
                TileMapY = (ushort)BitHelpers.ExtractBits(value, 6, 10);
                CollisionType = (byte)BitHelpers.ExtractBits(value, 8, 16);
            }
            else if (s.GameSettings.EngineVersion == EngineVersion.RaySaturn)
            {
                ushort value = 0;

                value = (ushort)BitHelpers.SetBits(value, TileMapX, 4, 0);
                value = (ushort)BitHelpers.SetBits(value, TileMapY, 12, 4);

                value = s.Serialize<ushort>(value, name: nameof(value));

                TileMapX = (ushort)BitHelpers.ExtractBits(value, 4, 0);
                TileMapY = (ushort)BitHelpers.ExtractBits(value, 12, 4);

                CollisionType = s.Serialize<byte>((byte)CollisionType, name: nameof(CollisionType));

                // TODO: Serialize this? Is it part of collision type? This appears in other PS1 versions too and is skipped there. It appears to always be 0 though.
                s.Serialize<byte>(0);
            }
            else if (s.GameSettings.MajorEngineVersion == MajorEngineVersion.Jaguar)
            {
                ushort value = 0;

                value = (ushort)BitHelpers.SetBits(value, TileMapY, 12, 0);
                value = (ushort)BitHelpers.SetBits(value, (int)CollisionType, 4, 12);

                value = s.Serialize<ushort>(value, name: nameof(value));

                TileMapY = (ushort)BitHelpers.ExtractBits(value, 12, 0);
                TileMapX = 0;
                CollisionType = (byte)BitHelpers.ExtractBits(value, 4, 12);
            }
            else if (s.GameSettings.MajorEngineVersion == MajorEngineVersion.SNES)
            {
                ushort value = 0;

                value = (ushort)BitHelpers.SetBits(value, TileMapY, 10, 0);
                value = (ushort)BitHelpers.SetBits(value, HorizontalFlip ? 1 : 0, 1, 10);
                value = (ushort)BitHelpers.SetBits(value, VerticalFlip ? 1 : 0, 1, 11);
                value = (ushort)BitHelpers.SetBits(value, (int)CollisionType, 4, 12);

                value = s.Serialize<ushort>(value, name: nameof(value));

                TileMapY = (ushort)BitHelpers.ExtractBits(value, 10, 0);
                TileMapX = 0;
                HorizontalFlip = BitHelpers.ExtractBits(value, 1, 10) == 1;
                VerticalFlip = BitHelpers.ExtractBits(value, 1, 11) == 1;
                CollisionType = (byte)BitHelpers.ExtractBits(value, 4, 12);
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
                    TileMapX = (ushort)BitHelpers.ExtractBits(value, 4, 0);
                    TileMapY = (ushort)BitHelpers.ExtractBits(value, 6, 4);
                    CollisionType = (byte)(BitHelpers.ExtractBits(value, 6, 10));
                }
                else if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JP)
                {
                    TileMapX = (ushort)BitHelpers.ExtractBits(value, 9, 0);
                    TileMapY = 0;
                    CollisionType = (byte)BitHelpers.ExtractBits(value, 7, 9);
                }
            }
        }

        public MapTile CloneObj() => (MapTile)Clone();
        public object Clone() => MemberwiseClone();
    }
}