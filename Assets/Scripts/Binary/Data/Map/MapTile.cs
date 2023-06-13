using BinarySerializer;
using System;
using BinarySerializer.Ray1;
using Ray1Map.GBARRR;

namespace Ray1Map
{
    // TODO: Clean up this class. Use separate MapTile classes for different games and then a common Unity one for the 
    //       editor in Ray1Map. For GBA games the tile class in the GBA library should be used.

    /// <summary>
    /// Common map tile data
    /// </summary>
    public class MapTile : BinarySerializable, ICloneable
    {
        #region Tile Properties (used in ray1map)

        // The tile x and y position on the tileset. If the tileset is 1-dimension, use TileMapY.
        public ushort TileMapX { get; set; }
        public ushort TileMapY { get; set; }

        // The tile collision type
        public ushort CollisionType { get; set; }

        // Flip flags
        public bool HorizontalFlip { get; set; }
        public bool VerticalFlip { get; set; }

        // Tiles to combine this tile with
        public MapTile[] CombinedTiles { get; set; }

        #endregion

        #region Game Properties

        public byte PC_Unk1 { get; set; }
        public BinarySerializer.Ray1.Block.BlockRenderMode PC_TransparencyMode { get; set; }
        public byte PC_Unk2 { get; set; }

        public byte PaletteIndex { get; set; }
        public bool Priority { get; set; }

        public byte GBARRR_MenuUnk { get; set; }
        public byte GBC_BankNumber { get; set; }
        public byte GBC_Unused { get; set; }

        public bool GBAVV_UnknownCollisionFlag { get; set; }
        public byte GBAVV_UnknownData { get; set; }
        public GBAVV_CollisionTileShape? GBAVV_CollisionShape { get; set; }
        public bool UsesCollisionShape => GBAVV_CollisionShape != null;

        #endregion

        #region PreSerialize Properties

        public GBA_TileType GBATileType { get; set; } = GBA_TileType.Normal;
        public GBC_TileType GBCTileType { get; set; } = GBC_TileType.Full;
        public bool Is8Bpp { get; set; }
        public bool GBAVV_IsWorldMap { get; set; }
        public bool IsFirstBlock { get; set; }

        public bool Pre_GBAKlonoa_Is8Bit { get; set; }

        public GBARRR_MapBlock.MapType GBARRRType { get; set; }

        public enum GBA_TileType
        {
            BGTile,
            Normal,
            FGTile,
            Mode7Tile
        }

        public enum GBC_TileType {
            Full,
            BGMapTileNumbers,
            BGMapAttributes,
            Collision
        }

        public enum GBAVV_CollisionTileShape
        {
            None = 0,
            
            Hill_Left = 1,
            Hill_Right = 2,

            Hill_Half_Left_1 = 3,

            Hill_Half_Left_2 = 5,

            Hill_Half_Right_1 = 6,
            Hill_Half_Right_2 = 7,

            Hill_Third_Left_1 = 8,

            Hill_Third_Left_2 = 10,

            Hill_Third_Left_3 = 12,

            Hill_Third_Right_1 = 13,
            Hill_Third_Right_2 = 14,
            Hill_Third_Right_3 = 15,

            Solid = 43,

            Unknown_46 = 46, // Crash 1 only - used for section where you crawl under some solid tiles, such as Temple of Doom
        }

        #endregion

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            // TODO: Remove all R1 code here since it's been moved to BinarySerializer.Ray1. We can't do it yet though as the memory loading still uses this.

            if (s.GetR1Settings().GameModeSelection == GameModeSelection.MapperPC || s.GetR1Settings().EngineVersion == EngineVersion.R1_GBA|| s.GetR1Settings().EngineVersion == EngineVersion.R1_DSi)
            {
                TileMapY = s.Serialize<ushort>(TileMapY, name: nameof(TileMapY));
                TileMapX = 0;
                CollisionType = s.Serialize<ushort>(CollisionType, name: nameof(CollisionType));
            }
            else if (s.GetR1Settings().EngineVersion == EngineVersion.R1_PC || s.GetR1Settings().EngineVersion == EngineVersion.R1_PC_Kit || s.GetR1Settings().EngineVersion == EngineVersion.R1_PC_Edu || s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_Edu || s.GetR1Settings().EngineVersion == EngineVersion.R1_PocketPC)
            {
                TileMapY = s.Serialize<ushort>(TileMapY, name: nameof(TileMapY));
                TileMapX = 0;
                CollisionType = s.Serialize<byte>((byte)CollisionType, name: nameof(CollisionType));
                PC_Unk1 = s.Serialize<byte>(PC_Unk1, name: nameof(PC_Unk1));
                PC_TransparencyMode = s.Serialize<BinarySerializer.Ray1.Block.BlockRenderMode>(PC_TransparencyMode, name: nameof(PC_TransparencyMode));
                PC_Unk2 = s.Serialize<byte>(PC_Unk2, name: nameof(PC_Unk2));
            }
            else if (s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 || s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_JPDemoVol6)
            {
                s.DoBits<int>(b =>
                {
                    TileMapX = b.SerializeBits<ushort>(TileMapX, 10, name: nameof(TileMapX));
                    TileMapY = b.SerializeBits<ushort>(TileMapY, 6, name: nameof(TileMapY));
                    CollisionType = b.SerializeBits<ushort>(CollisionType, 8, name: nameof(CollisionType));
                });
            }
            else if (s.GetR1Settings().EngineVersion == EngineVersion.R1_Saturn)
            {
                s.DoBits<ushort>(b =>
                {
                    TileMapX = b.SerializeBits<ushort>(TileMapX, 4, name: nameof(TileMapX));
                    TileMapY = b.SerializeBits<ushort>(TileMapY, 12, name: nameof(TileMapY));
                });

                CollisionType = s.Serialize<byte>((byte)CollisionType, name: nameof(CollisionType));
                s.Serialize<byte>(0, name: "Padding");
            }
            else if (s.GetR1Settings().MajorEngineVersion == MajorEngineVersion.Rayman1_Jaguar)
            {
                s.DoBits<ushort>(b =>
                {
                    TileMapY = b.SerializeBits<ushort>(TileMapY, 12, name: nameof(TileMapY));
                    CollisionType = b.SerializeBits<ushort>(CollisionType, 4, name: nameof(CollisionType));
                });

                TileMapX = 0;
            }
            else if (s.GetR1Settings().MajorEngineVersion == MajorEngineVersion.GBA)
            {
                // TODO: Use DoBits

                if (s.GetR1Settings().GBA_IsShanghai || s.GetR1Settings().GBA_IsMilan)
                {
                    s.DoBits<ushort>(b =>
                    {
                        TileMapY = b.SerializeBits<ushort>(TileMapY, 10, name: nameof(TileMapY));
                        HorizontalFlip = b.SerializeBits<bool>(HorizontalFlip, 1, name: nameof(HorizontalFlip));
                        VerticalFlip = b.SerializeBits<bool>(VerticalFlip, 1, name: nameof(VerticalFlip));
                        PaletteIndex = b.SerializeBits<byte>(PaletteIndex, 4, name: nameof(PaletteIndex));
                    });
                }
                else if ((GBATileType == GBA_TileType.BGTile || GBATileType == GBA_TileType.FGTile)
                    && s.GetR1Settings().EngineVersion != EngineVersion.GBA_SplinterCell_NGage
                    && s.GetR1Settings().EngineVersion != EngineVersion.GBA_BatmanVengeance) {
                    int numBits = Is8Bpp ? 9 : 10;

                    if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_BatmanVengeance) {
                        numBits = 8;
                    }/*else if(s.GetR1Settings().EngineVersion >= EngineVersion.GBA_StarWarsTrilogy) {
                        numBits = Is8Bpp ? 9 : 10;
                    } else if (s.GetR1Settings().EngineVersion >= EngineVersion.GBA_SplinterCell) {
                        numBits = 9;
                        if (GBATileType == GBA_TileType.FGTile) numBits = 10;
                    }*/

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

                    s.Log("{0}: {1}", nameof(TileMapY), TileMapY);
                    s.Log("{0}: {1}", nameof(HorizontalFlip), HorizontalFlip);
                    s.Log("{0}: {1}", nameof(IsFirstBlock), IsFirstBlock);
                    s.Log("{0}: {1}", nameof(PaletteIndex), PaletteIndex);
                } else if(GBATileType == GBA_TileType.Mode7Tile) {

                    ushort value = 0;

                    value = (ushort)BitHelpers.SetBits(value, TileMapY, 9, 0);
                    //value = (ushort)BitHelpers.SetBits(value, VerticalFlip ? 1 : 0, 1, numBits);
                    value = (ushort)BitHelpers.SetBits(value, HorizontalFlip ? 1 : 0, 1, 9);

                    value = s.Serialize<ushort>(value, name: nameof(value));

                    TileMapY = (ushort)BitHelpers.ExtractBits(value, 9, 0);
                    TileMapX = 0;
                    IsFirstBlock = BitHelpers.ExtractBits(value, 1, 9) == 1;
                    HorizontalFlip = BitHelpers.ExtractBits(value, 1, 10) == 1;
                    VerticalFlip = BitHelpers.ExtractBits(value, 1, 11) == 1;

                    s.Log("{0}: {1}", nameof(TileMapY), TileMapY);
                    s.Log("{0}: {1}", nameof(HorizontalFlip), HorizontalFlip);
                    s.Log("{0}: {1}", nameof(IsFirstBlock), IsFirstBlock);
                } else {
                    int numBits = Is8Bpp ? 14 : 11;

                    if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_BatmanVengeance) {
                        numBits = 10;
                    } else if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_SplinterCell_NGage) {
                        numBits = Is8Bpp ? 14 : 12;
                    } else if (s.GetR1Settings().EngineVersion >= EngineVersion.GBA_SplinterCell) {
                        numBits = Is8Bpp ? 14 : 12;
                    }

                    ushort value = 0;

                    value = (ushort)BitHelpers.SetBits(value, TileMapY, numBits, 0);
                    value = (ushort)BitHelpers.SetBits(value, HorizontalFlip ? 1 : 0, 1, numBits);
                    value = (ushort)BitHelpers.SetBits(value, PaletteIndex, 4, 12);

                    value = s.Serialize<ushort>(value, name: nameof(value));

                    TileMapY = (ushort)BitHelpers.ExtractBits(value, numBits, 0);
                    if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_SplinterCell_NGage && Is8Bpp) {
                        TileMapY = (ushort)BitHelpers.ExtractBits(value, numBits-1, 0);
                    }
                    TileMapX = 0;
                    HorizontalFlip = BitHelpers.ExtractBits(value, 1, numBits) == 1;
                    if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_BatmanVengeance) {
                        VerticalFlip = BitHelpers.ExtractBits(value, 1, numBits + 1) == 1;
                    }
                    if (!Is8Bpp) {
                        if (s.GetR1Settings().EngineVersion >= EngineVersion.GBA_SplinterCell) {
                            PaletteIndex = (byte)BitHelpers.ExtractBits(value, 3, 13);
                        } else {
                            PaletteIndex = (byte)BitHelpers.ExtractBits(value, 4, 12);
                        }

                        if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_SplinterCell) {
                            PaletteIndex += 8;
                        }
                    } else if(s.GetR1Settings().EngineVersion != EngineVersion.GBA_BatmanVengeance) {
                        VerticalFlip = BitHelpers.ExtractBits(value, 1, numBits + 1) == 1;
                    }

                    s.Log("{0}: {1}", nameof(TileMapY), TileMapY);
                    s.Log("{0}: {1}", nameof(HorizontalFlip), HorizontalFlip);
                    s.Log("{0}: {1}", nameof(PaletteIndex), PaletteIndex);
                }
            }
            else if (s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1 || s.GetR1Settings().EngineVersion == EngineVersion.R2_PS1)
            {
                s.DoBits<ushort>(b =>
                {
                    TileMapX = b.SerializeBits<ushort>(TileMapX, 4, name: nameof(TileMapX));
                    TileMapY = b.SerializeBits<ushort>(TileMapY, 6, name: nameof(TileMapY));
                    CollisionType = b.SerializeBits<ushort>(CollisionType, 6, name: nameof(CollisionType));
                });
            }
            else if (s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_JP)
            {
                s.DoBits<ushort>(b =>
                {
                    TileMapX = b.SerializeBits<ushort>(TileMapX, 9, name: nameof(TileMapX));
                    CollisionType = b.SerializeBits<ushort>(CollisionType, 7, name: nameof(CollisionType));
                });
            }
            else if (s.GetR1Settings().MajorEngineVersion == MajorEngineVersion.GBARRR)
            {
                if (GBARRRType == GBARRR_MapBlock.MapType.Collision)
                {
                    CollisionType = s.Serialize<byte>((byte)CollisionType, name: nameof(CollisionType));
                }
                else if (GBARRRType == GBARRR_MapBlock.MapType.Tiles)
                {
                    TileMapY = s.Serialize<ushort>(TileMapY, name: nameof(TileMapY));
                }
                else if (GBARRRType == GBARRR_MapBlock.MapType.Foreground) {
                    s.DoBits<ushort>(b =>
                    {
                        TileMapY = b.SerializeBits<ushort>(TileMapY, 10, name: nameof(TileMapY));
                        HorizontalFlip = b.SerializeBits<bool>(HorizontalFlip, 1, name: nameof(HorizontalFlip));
                        VerticalFlip = b.SerializeBits<bool>(VerticalFlip, 1, name: nameof(VerticalFlip));
                        PaletteIndex = b.SerializeBits<byte>(PaletteIndex, 4, name: nameof(PaletteIndex));
                        //Unk = (byte)b.SerializeBit<int>(Unk, 4, name: nameof(Unk));
                    });
                }
                else if (GBARRRType == GBARRR_MapBlock.MapType.Menu) {
                    s.DoBits<ushort>(b =>
                    {
                        TileMapY = b.SerializeBits<ushort>(TileMapY, 8, name: nameof(TileMapY));
                        GBARRR_MenuUnk = b.SerializeBits<byte>(GBARRR_MenuUnk, 2, name: nameof(GBARRR_MenuUnk));
                        HorizontalFlip = b.SerializeBits<bool>(HorizontalFlip, 1, name: nameof(HorizontalFlip));
                        VerticalFlip = b.SerializeBits<bool>(VerticalFlip, 1, name: nameof(VerticalFlip));
                        PaletteIndex = b.SerializeBits<byte>(PaletteIndex, 4, name: nameof(PaletteIndex));
                        //Unk = (byte)b.SerializeBit<int>(Unk, 4, name: nameof(Unk));
                    });
                }
                else if (GBARRRType == GBARRR_MapBlock.MapType.Mode7Tiles) {
                    TileMapY = s.Serialize<byte>((byte)TileMapY, name: nameof(TileMapY));
                }
            }
            else if (s.GetR1Settings().MajorEngineVersion == MajorEngineVersion.GBAIsometric)
            {
                s.DoBits<ushort>(b =>
                {
                    TileMapY = b.SerializeBits<ushort>(TileMapY, 10, name: nameof(TileMapY));
                    HorizontalFlip = b.SerializeBits<bool>(HorizontalFlip, 1, name: nameof(HorizontalFlip));
                    VerticalFlip = b.SerializeBits<bool>(VerticalFlip, 1, name: nameof(VerticalFlip));
                    PaletteIndex = b.SerializeBits<byte>(PaletteIndex, 4, name: nameof(PaletteIndex));
                });
            }
            else if (s.GetR1Settings().MajorEngineVersion == MajorEngineVersion.GBC)
            {
                switch (GBCTileType) {
                    case GBC_TileType.Full:
                        s.DoBits<ushort>(b => 
                        {
                            TileMapY = b.SerializeBits<ushort>(TileMapY, 9, name: nameof(TileMapY));
                            HorizontalFlip = b.SerializeBits<bool>(HorizontalFlip, 1, name: nameof(HorizontalFlip));
                            VerticalFlip = b.SerializeBits<bool>(VerticalFlip, 1, name: nameof(VerticalFlip));
                            CollisionType = b.SerializeBits<ushort>(CollisionType, 5, name: nameof(CollisionType));
                        });
                        break;
                    case GBC_TileType.BGMapTileNumbers:
                        TileMapY = s.Serialize<byte>((byte)TileMapY, name: nameof(TileMapY));
                        break;
                    case GBC_TileType.BGMapAttributes:
                        s.DoBits<byte>(b => 
                        {
                            PaletteIndex = b.SerializeBits<byte>(PaletteIndex, 3, name: nameof(PaletteIndex));
                            GBC_BankNumber = b.SerializeBits<byte>(GBC_BankNumber, 1, name: nameof(GBC_BankNumber));
                            GBC_Unused = b.SerializeBits<byte>(GBC_Unused, 1, name: nameof(GBC_Unused));
                            HorizontalFlip = b.SerializeBits<bool>(HorizontalFlip, 1, name: nameof(HorizontalFlip));
                            VerticalFlip = b.SerializeBits<bool>(VerticalFlip, 1, name: nameof(VerticalFlip));
                            Priority = b.SerializeBits<bool>(Priority, 1, name: nameof(Priority));
                        });
                        break;
                    case GBC_TileType.Collision:
                        CollisionType = s.Serialize<byte>((byte)CollisionType, name: nameof(CollisionType));
                        break;
                }
            }
            else if (s.GetR1Settings().MajorEngineVersion == MajorEngineVersion.GBAVV)
            {
                s.DoBits<ushort>(b =>
                {
                    TileMapY = b.SerializeBits<ushort>(TileMapY, GBAVV_IsWorldMap ? 12 : Is8Bpp ? 14 : 10, name: nameof(TileMapY));
                    HorizontalFlip = b.SerializeBits<bool>(HorizontalFlip, 1, name: nameof(HorizontalFlip));
                    VerticalFlip = b.SerializeBits<bool>(VerticalFlip, 1, name: nameof(VerticalFlip));

                    if (!Is8Bpp || GBAVV_IsWorldMap)
                        PaletteIndex = b.SerializeBits<byte>(PaletteIndex, GBAVV_IsWorldMap ? 2 : 4, name: nameof(PaletteIndex));
                });
            }
            else if (s.GetR1Settings().MajorEngineVersion == MajorEngineVersion.Gameloft)
            {
                s.DoBits<byte>(b =>
                {
                    TileMapY = b.SerializeBits<ushort>(TileMapY, 7, name: nameof(TileMapY));
                    HorizontalFlip = b.SerializeBits<bool>(HorizontalFlip, 1, name: nameof(HorizontalFlip));
                });
            }
            else if (s.GetR1Settings().MajorEngineVersion == MajorEngineVersion.GBAKlonoa)
            {
                if (Pre_GBAKlonoa_Is8Bit)
                {
                    TileMapY = s.Serialize<byte>((byte)TileMapY, name: nameof(TileMapY));
                }
                else
                {
                    s.DoBits<ushort>(b =>
                    {
                        TileMapY = b.SerializeBits<ushort>(TileMapY, 10, name: nameof(TileMapY));
                        HorizontalFlip = b.SerializeBits<bool>(HorizontalFlip, 1, name: nameof(HorizontalFlip));
                        VerticalFlip = b.SerializeBits<bool>(VerticalFlip, 1, name: nameof(VerticalFlip));
                        PaletteIndex = b.SerializeBits<byte>(PaletteIndex, 4, name: nameof(PaletteIndex));
                    });
                }
            }
        }

        public MapTile CloneObj() => (MapTile)Clone();
        public object Clone() => MemberwiseClone();

        public BinarySerializer.Ray1.Block ToR1MapTile()
        {
            return new BinarySerializer.Ray1.Block
            {
                TileX = TileMapX,
                TileY = TileMapY,
                BlockType = (BlockType)CollisionType,
                FlipX = HorizontalFlip,
                FlipY = VerticalFlip,
                RenderMode = PC_TransparencyMode,
            };
        }

        public static MapTile FromR1MapTile(BinarySerializer.Ray1.Block block, bool isR2 = false)
        {
            return new MapTile
            {
                TileMapX = block.TileX,
                TileMapY = block.TileY,
                CollisionType = isR2 ? (ushort)block.R2_BlockType : (ushort)block.BlockType,
                HorizontalFlip = block.FlipX,
                VerticalFlip = block.FlipY,
                PC_TransparencyMode = block.RenderMode,
            };
        }

        public static MapTile FromSnesMapTile(BinarySerializer.Nintendo.SNES.MapTile tile)
        {
            return new MapTile
            {
                TileMapX = (ushort)tile.TileIndex,
                PaletteIndex = tile.PaletteIndex,
                Priority = tile.Priority,
                HorizontalFlip = tile.FlipX,
                VerticalFlip = tile.FlipY,
            };
        }
    }
}