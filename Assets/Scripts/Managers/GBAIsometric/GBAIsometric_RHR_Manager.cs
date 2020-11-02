using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

namespace R1Engine
{
    public class GBAIsometric_RHR_Manager : IGameManager
    {
        public const int CellSize = 8;

        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 20).ToArray()),
            new GameInfo_World(1, Enumerable.Range(0, 5).ToArray()),
        });

        public GBAIsometric_RHR_Pointer[] GetMenuMaps(int level)
        {
            switch (level)
            {
                case 0:
                    return new GBAIsometric_RHR_Pointer[]
                    {
                        GBAIsometric_RHR_Pointer.Map_WorldMap
                    };
                case 1:
                    return new GBAIsometric_RHR_Pointer[]
                    {
                        GBAIsometric_RHR_Pointer.Map_Menu0,
                        GBAIsometric_RHR_Pointer.Map_Menu1,
                        GBAIsometric_RHR_Pointer.Map_Menu2,
                        GBAIsometric_RHR_Pointer.Map_Menu3,
                    };
                case 2:
                    return new GBAIsometric_RHR_Pointer[]
                    {
                        GBAIsometric_RHR_Pointer.Map_PauseFrame
                    };
                case 3:
                    return new GBAIsometric_RHR_Pointer[]
                    {
                        GBAIsometric_RHR_Pointer.Map_ScoreScreen
                    };
                case 4:
                    return new GBAIsometric_RHR_Pointer[]
                    {
                        GBAIsometric_RHR_Pointer.Map_Blank,
                        GBAIsometric_RHR_Pointer.Map_LicenseScreen1,
                        GBAIsometric_RHR_Pointer.Map_UbisoftScreen,
                        GBAIsometric_RHR_Pointer.Map_DigitalEclipseLogo1,
                        GBAIsometric_RHR_Pointer.Map_DigitalEclipseLogo2,
                        GBAIsometric_RHR_Pointer.Map_LicenseScreen2,
                        GBAIsometric_RHR_Pointer.Map_GameLogo,
                    };

                default:
                    return new GBAIsometric_RHR_Pointer[0];
            }
        }

        public virtual string GetROMFilePath => $"ROM.gba";

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
                new GameAction("Export Music & Sample Data", false, true, (input, output) => ExportMusicAsync(settings, output))
        };

        public async UniTask ExportMusicAsync(GameSettings settings, string outputPath) {
            using (var context = new Context(settings)) {
                var s = context.Deserializer;

                void ExportSampleSigned(string directory, string filename, sbyte[] data, uint sampleRate, ushort channels) {
                    // Create the directory
                    Directory.CreateDirectory(directory);

                    byte[] unsignedData = data.Select(b => (byte)(b + 128)).ToArray();

                    // Create WAV data
                    var formatChunk = new WAVFormatChunk() {
                        ChunkHeader = "fmt ",
                        FormatType = 1,
                        ChannelCount = channels,
                        SampleRate = sampleRate,
                        BitsPerSample = 8,
                    };

                    var wav = new WAV {
                        Magic = "RIFF",
                        FileTypeHeader = "WAVE",
                        Chunks = new WAVChunk[]
                        {
                            formatChunk,
                            new WAVChunk()
                            {
                                ChunkHeader = "data",
                                Data = unsignedData
                            }
                        }
                    };

                    formatChunk.ByteRate = (formatChunk.SampleRate * formatChunk.BitsPerSample * formatChunk.ChannelCount) / 8;
                    formatChunk.BlockAlign = (ushort)((formatChunk.BitsPerSample * formatChunk.ChannelCount) / 8);

                    // Get the output path
                    var outputFilePath = Path.Combine(directory, filename + ".wav");

                    // Create and open the output file
                    using (var outputStream = File.Create(outputFilePath)) {
                        // Create a context
                        using (var wavContext = new Context(settings)) {
                            // Create a key
                            const string wavKey = "wav";

                            // Add the file to the context
                            wavContext.AddFile(new StreamFile(wavKey, outputStream, wavContext));

                            // Write the data
                            FileFactory.Write<WAV>(wavKey, wav, wavContext);
                        }
                    }
                }

                await LoadFilesAsync(context);

                Pointer ptr = context.FilePointer(GetROMFilePath);
                var pointerTable = PointerTables.GBAIsometric_PointerTable(s.GameSettings.GameModeSelection, ptr.file);
                MusyX_File musyxFile = null;
                s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.MusyxFile], () => {
                    musyxFile = s.SerializeObject<MusyX_File>(musyxFile, name: nameof(musyxFile));
                });
                string outPath = outputPath + "/Sounds/";
                for (int i = 0; i < musyxFile.SampleTable.Value.Samples.Length; i++) {
                    var e = musyxFile.SampleTable.Value.Samples[i].Value;
                    //Util.ByteArrayToFile(outPath + $"{i}_{e.Offset.AbsoluteOffset:X8}.bin", e.SampleData);
                    ExportSampleSigned(outPath, $"{i}_{musyxFile.SampleTable.Value.Samples[i].pointer.SerializedOffset:X8}", e.SampleData, e.SampleRate, 1);
                }
                outPath = outputPath + "/SongData/";
                for (int i = 0; i < musyxFile.SongTable.Value.Length; i++) {
                    var songBytes = musyxFile.SongTable.Value.SongBytes[i];
                    Util.ByteArrayToFile(outPath + $"{i}_{musyxFile.SongTable.Value.Songs[i].SerializedOffset:X8}.son", songBytes);
                }
                outPath = outputPath + "/InstrumentData/";
                for (int i = 0; i < musyxFile.InstrumentTable.Value.Instruments.Length; i++) {
                    var instrumentBytes = musyxFile.InstrumentTable.Value.InstrumentBytes[i];
                    Util.ByteArrayToFile(outPath + $"{i}_{musyxFile.InstrumentTable.Value.Instruments[i].SerializedOffset:X8}.bin", instrumentBytes);
                }
            }
        }

        public UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            // Read the rom
            var rom = FileFactory.Read<GBAIsometric_ROM>(GetROMFilePath, context);

            var isMenu = context.Settings.World == 1;

            var levelInfo = !isMenu ? rom.LevelInfos[context.Settings.Level] : null;
            var levelData = levelInfo?.LevelDataPointer.Value;

            var availableMaps = !isMenu ? levelData.MapLayers.Select(x => x.DataPointer.Value).Reverse() : rom.MenuMaps;

            // Not all levels have maps
            if (levelInfo?.MapPointer?.Value != null)
                availableMaps = availableMaps.Append(levelInfo.MapPointer.Value);

            var tileSets = new Dictionary<GBAIsometric_TileSet, Unity_MapTileMap>();

            var maps = availableMaps.Select(x =>
            {
                var width = (ushort)(x.Width * 8);
                var height = (ushort)(x.Height * 8);
                var tileSetData = x.TileSetPointer.Value;

                if (!tileSets.ContainsKey(tileSetData))
                    tileSets.Add(tileSetData, LoadTileMap(context, x));

                return new Unity_Map
                {
                    Width = width,
                    Height = height,
                    TileSetWidth = 1,
                    TileSet = new Unity_MapTileMap[]
                    {
                        tileSets[tileSetData]
                    },
                    MapTiles = GetMapTiles(x, tileSets[tileSetData].GBAIsometric_BaseLength).Select(t => new Unity_Tile(t)
                    {
                        DebugText = $"Combined tiles: {t.CombinedTiles?.Length}"
                    }).ToArray()
                };
            }).ToArray();

            var objManager = new Unity_ObjectManager_GBAIsometric(context, rom.ObjectTypes, levelData?.ObjectsCount ?? 0);

            var allObjects = new List<Unity_Object>();

            if (levelData != null)
            {
                // Add normal objects
                allObjects.AddRange(levelData.Objects.Select(x => (Unity_Object)new Unity_Object_GBAIsometric(x, objManager)));

                // Add waypoints
                allObjects.AddRange(levelData.Waypoints.Select(x => (Unity_Object)new Unity_Object_GBAIsometricWaypoint(x, objManager)));
            }

            // Add localization
            var loc = rom.Localization.Localization.Select((x, i) => new
            {
                key = rom.Localization.Localization[0][i],
                strings = x
            }).ToDictionary(x => x.key, x => x.strings);

            return UniTask.FromResult(new Unity_Level(
                maps: maps, 
                objManager: objManager,
                eventData: allObjects,
                cellSize: CellSize,
                localization: loc));
        }

        public MapTile[] GetMapTiles(GBAIsometric_MapLayer mapLayer, int tileSetBaseLength)
        {
            var width = mapLayer.Width * 8;
            var height = mapLayer.Height * 8;
            var tiles = new MapTile[width * height];
            var tileSet = mapLayer.TileSetPointer.Value;

            for (int blockY = 0; blockY < mapLayer.Height; blockY++)
            {
                for (int blockX = 0; blockX < mapLayer.Width; blockX++)
                {
                    ushort[] tileBlock = tileSet.Get8x8Map(mapLayer.MapData[blockY * mapLayer.Width + blockX]); // 64x64

                    var actualX = blockX * 8;
                    var actualY = blockY * 8;

                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            var tileValue = tileBlock[y * 8 + x];

                            MapTile getMapTile(ushort value)
                            {
                                var index = BitHelpers.ExtractBits(value, 14, 2);

                                if (index >= tileSet.GraphicsDataPointer.Value.CompressionLookupBufferLength) {
                                    var additionalTileIndex = tileSet.GraphicsDataPointer.Value.TotalLength - 1 - index;
                                    index = (int)(tileSetBaseLength + additionalTileIndex);
                                }

                                return new MapTile()
                                {
                                    TileMapY = (ushort)index,
                                    VerticalFlip = BitHelpers.ExtractBits(value, 1, 1) == 1,
                                    HorizontalFlip = BitHelpers.ExtractBits(value, 1, 0) == 1
                                };
                            }

                            if (BitHelpers.ExtractBits(tileValue, 2, 14) == 3) {
                                // Combined tile
                                int index = BitHelpers.ExtractBits(tileValue, 14, 0);
                                ushort offset = tileSet.CombinedTileOffsets[index];
                                int numTilesToCombine = tileSet.CombinedTileOffsets[index+1] - offset;
                                if(numTilesToCombine <= 1) numTilesToCombine = 1;
                                for (int i = 0; i < numTilesToCombine; i++) {
                                    ushort data = tileSet.CombinedTileData[offset+i];
                                    var tile = getMapTile(data);

                                    if (i > 0)
                                    {
                                        tiles[(actualY + y) * width + (actualX + x)].CombinedTiles[i - 1] = tile;
                                    }
                                    else
                                    {
                                        tiles[(actualY + y) * width + (actualX + x)] = tile;
                                        tiles[(actualY + y) * width + (actualX + x)].CombinedTiles = new MapTile[numTilesToCombine - 1];
                                    }
                                }
                            } else {
                                var tile = getMapTile(tileValue);

                                tiles[(actualY + y) * width + (actualX + x)] = tile;
                            }
                        }
                    }
                }
            }

            return tiles;
        }

        public Unity_MapTileMap LoadTileMap(Context context, GBAIsometric_MapLayer mapLayer)
        {
            var s = context.Deserializer;
            Unity_MapTileMap t = null;
            var tileMap = mapLayer.TileSetPointer.Value;
            var palTable = tileMap.PaletteIndexTablePointer?.Value;
            var is8bit = mapLayer.StructType == GBAIsometric_MapLayer.MapLayerType.Map;
            Color[][] palettes = null;
            Color[] defaultPalette;

            if (mapLayer.MapPalette != null)
            {
                defaultPalette = mapLayer.MapPalette.Select((c, i) =>
                {
                    if (i != 0)
                        c.Alpha = 255;
                    return c.GetColor();
                }).ToArray();
            }
            else
            {
                defaultPalette = Util.CreateDummyPalette(256, wrap: 16).Select((x, i) => x.GetColor()).ToArray();
                palettes = tileMap.Palettes?.Select(x => x.Select((c, i) =>
                {
                    if (i != 0)
                        c.Alpha = 255;
                    return c.GetColor();
                }).ToArray()).ToArray();
            }

            s.DoEncoded(new RHR_SpriteEncoder(is8bit, tileMap.GraphicsDataPointer.Value.CompressionLookupBuffer, tileMap.GraphicsDataPointer.Value.CompressedDataPointer), () =>
            {
                byte[] fullSheet = s.SerializeArray<byte>(default, s.CurrentLength, name: nameof(fullSheet));

                var tex = Util.ToTileSetTexture(fullSheet, defaultPalette, is8bit, CellSize, false, wrap: 16, getPalFunc: i => palettes?.ElementAtOrDefault(tileMap.PaletteIndexTablePointer.Value.PaletteIndices[i]));
                //Util.ByteArrayToFile(context.BasePath + "/tileset_tex/" + tileMap.GraphicsDataPointer.Value.Offset.StringAbsoluteOffset + ".png", tex.EncodeToPNG());

                // Create the tile array
                var baseLength = (tex.width / CellSize) * (tex.height / CellSize);
                var additionalLength = palTable?.SecondaryPaletteIndices.Length ?? 0;
                var tiles = new Unity_TileTexture[baseLength + additionalLength];

                // Keep track of the index
                var index = 0;

                // Extract every tile
                for (int y = 0; y < tex.height; y += CellSize)
                {
                    for (int x = 0; x < tex.width; x += CellSize)
                    {
                        // Create a tile
                        tiles[index] = tex.CreateTile(new Rect(x, y, CellSize, CellSize));

                        index++;
                    }
                }

                if (palTable != null)
                {
                    int tileSize = is8bit ? (CellSize * CellSize) : (CellSize * CellSize) / 2;

                    for (int i = 0; i < palTable.SecondaryPaletteIndices.Length; i++)
                    {
                        var tileTex = TextureHelpers.CreateTexture2D(CellSize, CellSize);

                        tileTex.FillInTile(fullSheet, tileSize * palTable.SecondaryTileIndices[i], palettes[palTable.SecondaryPaletteIndices[i]], is8bit, CellSize, false, 0, 0);

                        tileTex.Apply();

                        tiles[baseLength + i] = tileTex.CreateTile();
                    }
                }

                t = new Unity_MapTileMap(tiles)
                {
                    GBAIsometric_BaseLength = baseLength
                };
            });

            return t;
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public virtual async UniTask LoadFilesAsync(Context context) => await context.AddGBAMemoryMappedFile(GetROMFilePath, 0x08000000);
    }
}