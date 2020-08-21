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
        public R1_PC_MapTileTransparencyMode PC_TransparencyMode { get; set; }

        public byte PC_Unk2 { get; set; }

        public bool HorizontalFlip { get; set; }
        public bool VerticalFlip { get; set; }

        public byte PaletteIndex { get; set; }

        public bool IsBGTile { get; set; }
        public bool Is8Bpp { get; set; }
        public bool IsFirstBlock { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            if (s.GameSettings.GameModeSelection == GameModeSelection.MapperPC || s.GameSettings.EngineVersion == EngineVersion.R1_GBA|| s.GameSettings.EngineVersion == EngineVersion.R1_DSi)
            {
                TileMapY = s.Serialize<ushort>(TileMapY, name: nameof(TileMapY));
                TileMapX = 0;
                CollisionType = (byte)s.Serialize<ushort>((ushort)CollisionType, name: nameof(CollisionType));
            }
            else if (s.GameSettings.EngineVersion == EngineVersion.R1_PC || s.GameSettings.EngineVersion == EngineVersion.R1_PC_Kit || s.GameSettings.EngineVersion == EngineVersion.R1_PC_Edu || s.GameSettings.EngineVersion == EngineVersion.R1_PS1_Edu || s.GameSettings.EngineVersion == EngineVersion.R1_PocketPC)
            {
                TileMapY = s.Serialize<ushort>(TileMapY, name: nameof(TileMapY));
                TileMapX = 0;
                CollisionType = s.Serialize<byte>(CollisionType, name: nameof(CollisionType));
                PC_Unk1 = s.Serialize<byte>(PC_Unk1, name: nameof(PC_Unk1));
                PC_TransparencyMode = s.Serialize<R1_PC_MapTileTransparencyMode>(PC_TransparencyMode, name: nameof(PC_TransparencyMode));
                PC_Unk2 = s.Serialize<byte>(PC_Unk2, name: nameof(PC_Unk2));
            }
            else if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 || s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol6)
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
            else if (s.GameSettings.EngineVersion == EngineVersion.R1_Saturn)
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
            else if (s.GameSettings.MajorEngineVersion == MajorEngineVersion.Rayman1_Jaguar)
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
            else if (s.GameSettings.MajorEngineVersion == MajorEngineVersion.GBA)
            {
                if (IsBGTile) {
                    int numBits = Is8Bpp ? 9 : 10;

                    if (s.GameSettings.EngineVersion <= EngineVersion.GBA_BatmanVengeance) {
                        numBits = 8;
                    } else if (s.GameSettings.EngineVersion >= EngineVersion.GBA_SplinterCell) {
                        numBits = 9;
                    }

                    ushort value = 0;

                    value = (ushort)BitHelpers.SetBits(value, TileMapY, numBits, 0);
                    //value = (ushort)BitHelpers.SetBits(value, VerticalFlip ? 1 : 0, 1, numBits);
                    value = (ushort)BitHelpers.SetBits(value, HorizontalFlip ? 1 : 0, 1, numBits);
                    value = (ushort)BitHelpers.SetBits(value, PaletteIndex, 4, 12);

                    value = s.Serialize<ushort>(value, name: nameof(value));

                    TileMapY = (ushort)BitHelpers.ExtractBits(value, numBits, 0);
                    TileMapX = 0;
                    if (Is8Bpp) {
                        IsFirstBlock = BitHelpers.ExtractBits(value, 1, 9) == 1;
                    }
                    HorizontalFlip = BitHelpers.ExtractBits(value, 1, 10) == 1;
                    VerticalFlip = BitHelpers.ExtractBits(value, 1, 11) == 1;
                    PaletteIndex = (byte)BitHelpers.ExtractBits(value, 4, 12);

                    s.Log($"{nameof(TileMapY)}: {TileMapY}");
                    s.Log($"{nameof(HorizontalFlip)}: {HorizontalFlip}");
                    s.Log($"{nameof(IsFirstBlock)}: {IsFirstBlock}");
                    s.Log($"{nameof(PaletteIndex)}: {PaletteIndex}");
                } else {
                    int numBits = Is8Bpp ? 14 : 11;

                    if (s.GameSettings.EngineVersion <= EngineVersion.GBA_BatmanVengeance)
                        numBits = 10;
                    else if (s.GameSettings.EngineVersion >= EngineVersion.GBA_SplinterCell) {
                        numBits = Is8Bpp ? 14 : 12;
                    }

                    ushort value = 0;

                    value = (ushort)BitHelpers.SetBits(value, TileMapY, numBits, 0);
                    value = (ushort)BitHelpers.SetBits(value, HorizontalFlip ? 1 : 0, 1, numBits);
                    value = (ushort)BitHelpers.SetBits(value, PaletteIndex, 4, 12);

                    value = s.Serialize<ushort>(value, name: nameof(value));

                    TileMapY = (ushort)BitHelpers.ExtractBits(value, numBits, 0);
                    TileMapX = 0;
                    HorizontalFlip = BitHelpers.ExtractBits(value, 1, numBits) == 1;
                    if (!Is8Bpp) {
                        if (s.GameSettings.EngineVersion >= EngineVersion.GBA_SplinterCell) {
                            PaletteIndex = (byte)BitHelpers.ExtractBits(value, 3, 13);
                        } else {
                            PaletteIndex = (byte)BitHelpers.ExtractBits(value, 4, 12);
                        }
                    } else {
                        VerticalFlip = BitHelpers.ExtractBits(value, 1, numBits + 1) == 1;
                    }

                    s.Log($"{nameof(TileMapY)}: {TileMapY}");
                    s.Log($"{nameof(HorizontalFlip)}: {HorizontalFlip}");
                    s.Log($"{nameof(PaletteIndex)}: {PaletteIndex}");
                }
            }

            else
            {
                ushort value = 0;
                if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1 || s.GameSettings.EngineVersion == EngineVersion.R2_PS1)
                {
                    value = (ushort)BitHelpers.SetBits(value, TileMapX, 4, 0);
                    value = (ushort)BitHelpers.SetBits(value, TileMapY, 6, 4);
                    value = (ushort)BitHelpers.SetBits(value, (int)CollisionType, 6, 10);
                }
                else if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JP)
                {
                    value = (ushort)BitHelpers.SetBits(value, TileMapX, 9, 0);
                    value = (ushort)BitHelpers.SetBits(value, (int)CollisionType, 7, 9);
                }
                value = s.Serialize<ushort>(value, name: nameof(value));
                if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1 || s.GameSettings.EngineVersion == EngineVersion.R2_PS1)
                {
                    TileMapX = (ushort)BitHelpers.ExtractBits(value, 4, 0);
                    TileMapY = (ushort)BitHelpers.ExtractBits(value, 6, 4);
                    CollisionType = (byte)(BitHelpers.ExtractBits(value, 6, 10));
                }
                else if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JP)
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