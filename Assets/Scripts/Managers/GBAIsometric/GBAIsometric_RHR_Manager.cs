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
                        GBAIsometric_RHR_Pointer.Map_PauseFrame1,
                        GBAIsometric_RHR_Pointer.Map_PauseFrame2,
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
                var pointerTable = PointerTables.GBAIsometric_RHR_PointerTable(s.GameSettings.GameModeSelection, ptr.file);
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

        public void CreateCollisionModel(Context context, GBAIsometric_RHR_LevelData levelData) {
            GameObject parent = new GameObject("Collision parent");
            for (int y = 0; y < levelData.CollisionHeight; y++) {
                for (int x = 0; x < levelData.CollisionWidth; x++) {
                    int ind = y * levelData.CollisionWidth + x;
                    int startPos = -1;
                    int endPos = levelData.CollisionData[ind].Data[0];
                    int type = levelData.CollisionData[ind].Data[1];
                    float height = endPos - startPos / 2f;
                    GameObject gao = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    gao.name = $"{levelData.CollisionData[ind].Data[0]:X2} {levelData.CollisionData[ind].Data[1]:X2} {levelData.CollisionData[ind].Data[2]:X2} {levelData.CollisionData[ind].Data[3]:X2}";
                    gao.transform.SetParent(parent.transform);
                    gao.transform.localScale = new Vector3(1f,height,1f);
                    gao.transform.localPosition = new Vector3(x,startPos + height/2f,-y);
                    UnityEngine.Random.InitState(type);
                    Color color = UnityEngine.Random.ColorHSV(0, 1, 0.2f, 1f, 0.4f, 1.0f);
                    gao.GetComponent<Renderer>().material.color = color;
                }
            }
        }

        public UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            // Read the rom
            var rom = FileFactory.Read<GBAIsometric_RHR_ROM>(GetROMFilePath, context);

            var isMenu = context.Settings.World == 1;

            var levelInfo = !isMenu ? rom.LevelInfos[context.Settings.Level] : null;
            var levelData = levelInfo?.LevelDataPointer.Value;
            var animPal = rom.PaletteAnimations.ElementAtOrDefault(levelInfo?.PaletteShiftIndex ?? -1);

            var availableMaps = !isMenu ? levelData.MapLayers.Select(x => x.DataPointer.Value).Reverse() : rom.MenuMaps;

            // Not all levels have maps
            if (levelInfo?.MapPointer?.Value != null)
                availableMaps = availableMaps.Append(levelInfo.MapPointer.Value);

            var tileSets = new Dictionary<GBAIsometric_RHR_TileSet, Unity_MapTileMap>();
            CreateCollisionModel(context, levelData);

            var maps = availableMaps.Select(x =>
            {
                var width = (ushort)(x.Width * 8);
                var height = (ushort)(x.Height * 8);
                var tileSetData = x.TileSetPointer.Value;

                if (!tileSets.ContainsKey(tileSetData))
                    tileSets.Add(tileSetData, LoadTileMap(context, x, animPal));

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
                        DebugText = $"Combined tiles: {t.CombinedTiles?.Length}",
                        CombinedTiles = t.CombinedTiles?.Select(ct => new Unity_Tile(ct)).ToArray()
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

        public MapTile[] GetMapTiles(GBAIsometric_RHR_MapLayer mapLayer, int tileSetBaseLength)
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

        public Unity_MapTileMap LoadTileMap(Context context, GBAIsometric_RHR_MapLayer mapLayer, GBAIsometric_RHR_PaletteAnimationTable palAnimTable)
        {
            var s = context.Deserializer;
            Unity_MapTileMap t = null;
            var tileMap = mapLayer.TileSetPointer.Value;
            var palTable = tileMap.PaletteIndexTablePointer?.Value;
            var is8bit = mapLayer.StructType == GBAIsometric_RHR_MapLayer.MapLayerType.Map;
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

            s.DoEncoded(new RHR_SpriteEncoder(is8bit, tileMap.GraphicsDataPointer.Value.CompressionLookupBuffer, tileMap.GraphicsDataPointer.Value.CompressedDataPointer), () => {
                byte[] fullSheet = s.SerializeArray<byte>(default, s.CurrentLength, name: nameof(fullSheet));

                var tex = Util.ToTileSetTexture(fullSheet, defaultPalette, is8bit, CellSize, false, wrap: 16, getPalFunc: i => palettes?.ElementAtOrDefault(palTable.PaletteIndices[i]));
                //Util.ByteArrayToFile(context.BasePath + "/tileset_tex/" + tileMap.GraphicsDataPointer.Value.Offset.StringAbsoluteOffset + ".png", tex.EncodeToPNG());

                // Group animated tile info by the palette
                var palAnimGroups = palettes == null ? null : palAnimTable?.Entries.GroupBy(x => x.PaletteIndex).Select(x => {
                    // Get the data
                    var entries = x.ToArray();
                    var palIndex = x.Key;

                    // Get the lowest common multiple
                    var lengths = entries.Select(y => Math.Abs(y.EndIndex - y.StartIndex) * Math.Abs(y.Speed)).ToArray();
                    int length = 0;
                    if (lengths.Length < 2) {
                        length = lengths[0];
                    } else {
                        length = Util.LCM(lengths);
                    }
                    // Create an array of new palettes, skipping the first one (the default one)
                    var framePals = new List<Color[]>();
                    var frameIndices = new int[length - 1];

                    // Get the original palette to use as a template
                    var prevPal = palettes[palIndex];

                    // Shift colors and create new palettes for every frame
                    for (int i = 0; i < length - 1; i++) {
                        int frame = i + 1;
                        Color[] newPal = new Color[16];
                        // Set to original palette
                        for (int j = 0; j < newPal.Length; j++) {
                            newPal[j] = palettes[palIndex][j];
                        }
                        for (int j = 0; j < entries.Length; j++) {
                            int len = Math.Abs(entries[j].EndIndex - entries[j].StartIndex);
                            int speed = Math.Abs(entries[j].Speed);
                            if(speed == 0 || len < 2) continue;
                            int sign = Math.Sign(entries[j].Speed);
                            int shift = (frame / speed) % len;
                            if (shift > 0) {
                                for (int k = 0; k < len; k++) {
                                    int targetK = (sign > 0 ? (k + shift) : (k + len - shift)) % len;
                                    newPal[targetK + entries[j].StartIndex] = palettes[palIndex][k + entries[j].StartIndex];
                                }
                            }
                        }
                        if (Enumerable.SequenceEqual(newPal, palettes[palIndex])) {
                            frameIndices[i] = 0;
                        } else {
                            bool frameIndexFound = false;
                            for (int j = 0; j < framePals.Count; j++) {
                                if (Enumerable.SequenceEqual(newPal, framePals[j])) {
                                    frameIndexFound = true;
                                    frameIndices[i] = j+1;
                                    break;
                                }
                            }
                            if (!frameIndexFound) {
                                framePals.Add(newPal);
                                frameIndices[i] = framePals.Count;
                            }
                        }
                    }

                    return new
                    {
                        PalIndex = palIndex,
                        Entries = entries,
                        Palettes = framePals,
                        FramePaletteIndices = frameIndices
                    };
                }).ToArray();

                // Create the tile array
                var baseLength = (tex.width / CellSize) * (tex.height / CellSize);
                var additionalLength = palTable?.SecondaryPaletteIndices.Length ?? 0;
                var tiles = new Unity_TileTexture[baseLength + additionalLength];
                var animPalTiles = new List<Unity_TileTexture>();
                var unityAnimTiles = new List<Unity_AnimatedTile>();
                int tileSize = is8bit ? (CellSize * CellSize) : (CellSize * CellSize) / 2;

                // Helper for adding palette animations to a tile
                void addTileAnimation(int tileIndex, int palIndex, int tileOffset)
                {
                    // Get the animation group for the current palette
                    var animGroup = palAnimGroups?.FirstOrDefault(x => x.PalIndex == palIndex);

                    // Make sure the palette is animated, otherwise return
                    if (animGroup == null)
                        return;

                    // Get the base index in the tileset to add the animated variants to
                    var baseIndex = tiles.Length + animPalTiles.Count;

                    // Add animation data
                    unityAnimTiles.Add(new Unity_AnimatedTile()
                    {
                        AnimationSpeed = 0.5f, // TODO: Correct,
                        TileIndices = new int[]
                        {
                            tileIndex // First index must always be the original tile
                        }.Concat(animGroup.FramePaletteIndices.Select(fpi => fpi > 0 ? (fpi - 1 + baseIndex) : tileIndex)).ToArray()
                    });

                    // Add new tiles for every frame using new palettes
                    foreach (var pal in animGroup.Palettes)
                    {
                        var tileTex = TextureHelpers.CreateTexture2D(CellSize, CellSize);

                        tileTex.FillInTile(fullSheet, tileOffset, pal, is8bit, CellSize, false, 0, 0);

                        tileTex.Apply();

                        animPalTiles.Add(tileTex.CreateTile());
                    }
                }

                // Keep track of the index
                var index = 0;

                // Extract every tile from the primary tileset texture
                for (int y = 0; y < tex.height; y += CellSize)
                {
                    for (int x = 0; x < tex.width; x += CellSize)
                    {
                        // Create a tile
                        tiles[index] = tex.CreateTile(new Rect(x, y, CellSize, CellSize));

                        if (palTable?.PaletteIndices.Length > index)
                            addTileAnimation(index, palTable.PaletteIndices[index], tileSize * index);

                        index++;
                    }
                }

                if (palTable != null)
                {
                    for (int i = 0; i < palTable.SecondaryPaletteIndices.Length; i++)
                    {
                        var tileTex = TextureHelpers.CreateTexture2D(CellSize, CellSize);
                        var offset = tileSize * palTable.SecondaryTileIndices[i];

                        tileTex.FillInTile(fullSheet, offset, palettes[palTable.SecondaryPaletteIndices[i]], is8bit, CellSize, false, 0, 0);

                        tileTex.Apply();

                        addTileAnimation(baseLength + i, palTable.SecondaryPaletteIndices[i], offset);

                        tiles[baseLength + i] = tileTex.CreateTile();
                    }
                }

                t = new Unity_MapTileMap(tiles.Concat(animPalTiles).ToArray())
                {
                    AnimatedTiles = unityAnimTiles.ToArray(),
                    GBAIsometric_BaseLength = baseLength,
                };
            });

            return t;
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public virtual async UniTask LoadFilesAsync(Context context) => await context.AddGBAMemoryMappedFile(GetROMFilePath, 0x08000000);
    }
}