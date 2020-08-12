using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace R1Engine
{
    public class GBA_R3_Manager : IGameManager
    {
        public KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => new KeyValuePair<World, int[]>[]
        {
            new KeyValuePair<World, int[]>(World.Jungle, Enumerable.Range(0, 65).ToArray()), 
        };

        public string[] GetEduVolumes(GameSettings settings) => new string[0];

        public virtual string GetROMFilePath => $"ROM.gba";

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Compressed Blocks", false, true, (input, output) => ExportAllCompressedBlocksAsync(settings, output)),
            new GameAction("Export Vignette", false, true, (input, output) => ExtractVignetteAsync(settings, output)),
        };

        // TODO: Find the way the game gets the vignette offsets and find remaining vignettes
        public async UniTask ExtractVignetteAsync(GameSettings settings, string outputDir)
        {
            // Create a context
            using (var context = new Context(settings))
            {
                // Load the ROM
                await LoadFilesAsync(context);

                // Get the file
                var file = context.GetFile(GetROMFilePath);

                // Get the deserialize
                var s = context.Deserializer;

                // TODO: Find scrolling vignette for proto
                var isPrototype = settings.GameModeSelection == GameModeSelection.Rayman3GBAUSPrototype;

                // TODO: Move pointers to pointer table
                int vigCount = isPrototype ? 18 : 20;
                uint palOffset = (uint)(isPrototype ? 0x0DC46A : 0x0B37C0);
                uint vigOffset = (uint)(isPrototype ? 0x45FE3C : 0x20ED94);

                var palettes = new ARGB1555Color[vigCount][];

                for (int i = 0; i < vigCount; i++)
                    palettes[i] = s.DoAt((file.StartPointer + palOffset) + (512 * i), () => s.SerializeObjectArray<ARGB1555Color>(default, 256));

                // Go to vignette offset
                s.Goto(file.StartPointer + vigOffset);

                // Export every vignette
                for (int i = 0; i < vigCount; i++)
                {
                    // Get the offset
                    var offset = s.CurrentPointer;

                    // Decode data
                    byte[] data = null;
                    s.DoEncoded(new LZSSEncoder(), () => data = s.SerializeArray<byte>(default, s.CurrentLength));

                    int width;
                    int height;

                    switch (data.Length)
                    {
                        case 0x9600:
                            width = 240;
                            height = 160;
                            break;

                        case 0x5A00:
                            width = 192;
                            height = 120;
                            break;

                        // TODO: Support scrolling vignette

                        default:
                            throw new Exception("Vignette length is not supported");
                    }

                    // Create a texture
                    var tex = new Texture2D(width, height); ;

                    var palIndex = i;

                    if (isPrototype)
                    {
                        // Palettes for 5 and 6 are swapped
                        if (palIndex == 5)
                            palIndex = 6;
                        else if (palIndex == 6)
                            palIndex = 5;
                    }
                    else
                    {
                        // Palettes for 6 and 7 are swapped
                        if (palIndex == 6)
                            palIndex = 7;
                        else if (palIndex == 7)
                            palIndex = 6;
                    }

                    // Set pixels
                    for (int y = 0; y < tex.height; y++)
                    {
                        for (int x = 0; x < tex.width; x++)
                        {
                            var c = palettes[palIndex][data[y * tex.width + x]].GetColor();

                            // Remove transparency
                            c.a = 1;

                            // Set pixel and reverse height
                            tex.SetPixel(x, tex.height - y - 1, c);
                        }
                    }

                    tex.Apply();

                    // Export
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"Vig_{i}_0x{offset.AbsoluteOffset:X8}.png"), tex.EncodeToPNG());

                    // Align
                    s.Align();
                }
            }
        }

        public async UniTask ExportAllCompressedBlocksAsync(GameSettings settings, string outputDir)
        {
            // Create a context
            using (var context = new Context(settings))
            {
                // Load the ROM
                await LoadFilesAsync(context);

                // Get the file
                var file = (GBAMemoryMappedFile)context.GetFile(GetROMFilePath);

                // Get the deserialize
                var s = context.Deserializer;

                // Keep track of blocks
                var blocks = new List<Tuple<long, long, int>>();

                // Enumerate every fourth byte (compressed blocks are always aligned to 4)
                for (int i = 0; i < file.Length; i += 4)
                {
                    // Go to the offset
                    s.Goto(file.StartPointer + i);

                    // Check for compression header
                    if (s.Serialize<byte>(default) == 0x10)
                    {
                        // Get the decompressed size
                        var decompressedSizeValue = s.SerializeArray<byte>(default, 3);
                        Array.Resize(ref decompressedSizeValue, 4);
                        var decompressedSize = BitConverter.ToUInt32(decompressedSizeValue, 0);

                        // Skip if the decompressed size is too low
                        if (decompressedSize <= 32) 
                            continue;
                        
                        // Go back to the offset
                        s.Goto(file.StartPointer + i);

                        // Attempt to decompress
                        try
                        {
                            byte[] data = null;

                            s.DoEncoded(new LZSSEncoder(), () => data = s.SerializeArray<byte>(default, s.CurrentLength));

                            // Make sure we got some data
                            if (data != null && data.Length > 32)
                            {
                                Util.ByteArrayToFile(Path.Combine(outputDir, $"Block_0x{(file.StartPointer + i).AbsoluteOffset:X8}.dat"), data);

                                blocks.Add(new Tuple<long, long, int>((file.StartPointer + i).AbsoluteOffset, s.CurrentPointer - (file.StartPointer + i), data.Length));
                            }
                        }
                        catch
                        {
                            // Ignore exceptions...
                        }
                    }
                }

                var log = new List<string>();

                for (int i = 0; i < blocks.Count; i++)
                {
                    var (offset, compressedSize, size) = blocks[i];

                    var end = offset + compressedSize;

                    log.Add($"0x{offset:X8} - 0x{end:X8} (0x{compressedSize:X8} - 0x{size:X8}) - ");

                    if (i != blocks.Count - 1)
                    {
                        var dif = blocks[i + 1].Item1 - end;

                        if (dif >= 4)
                            log.Add($"0x{end:X8} - 0x{end + dif:X8} (0x{dif:X8})              - ");
                    }
                }

                File.WriteAllLines(Path.Combine(outputDir, "blocks_log.txt"), log);
            }
        }

        public virtual async UniTask<Common_Lev> CreateCommonLev(Context context, GBA_LevelBlock levelBlock) {
            if (levelBlock.PlayField.IsMode7)
            {
                // For now we return a dummy map to not break screenshot enumeration
                return new Common_Lev()
                {
                    // Create the map
                    Maps = new Common_LevelMap[]
                    {
                        new Common_LevelMap()
                        {
                            // Set the dimensions
                            Width = 1,
                            Height = 1,

                            // Create the tile arrays
                            TileSet = new Common_Tileset[]
                            {
                                new Common_Tileset(new Tile[]
                                {
                                    new Tile()
                                }), 
                            },
                            MapTiles = new Editor_MapTile[]
                            {
                                new Editor_MapTile(new MapTile()), 
                            },
                            TileSetWidth = 1
                        }
                    },

                    // Create the events list
                    EventData = new List<Editor_EventData>(),
                };
            }

            // Get the play field
            var playField = levelBlock.PlayField.PlayField2D;

            // Get the primary map (BG_2)
            var map = playField.Layers.FirstOrDefault(x => x.LayerID == 1) ?? playField.Layers.First(x => !x.Is8bpp);

            // Get the collision data
            var cMap = playField.Layers.First(x => x.IsCollisionBlock);

            // Get the tilemap to use
            byte[] tileMap;
            bool is8bpp;
            GBA_Palette tilePalette;
            int numBits = 11;
            if (context.Settings.EngineVersion == EngineVersion.BatmanVengeanceGBA) {
                is8bpp = map.Tilemap.Is8bpp;
                tileMap = is8bpp ? map.Tilemap.TileMap8bpp : map.Tilemap.TileMap4bpp;
                tilePalette = playField.TilePalette;
                numBits = 10;
            } else {
                is8bpp = map.Is8bpp;
                tileMap = is8bpp ? playField.Tilemap.TileMap8bpp : playField.Tilemap.TileMap4bpp;
                tilePalette = playField.Tilemap.TilePalette;
                if (context.Settings.EngineVersion == EngineVersion.PrinceOfPersiaGBA || context.Settings.EngineVersion == EngineVersion.StarWarsGBA) {
                    numBits = 14;
                }
            }

            int tilemapLength = (tileMap.Length / (is8bpp ? 64 : 32)) + 1;

            // Convert levelData to common level format
            Common_Lev commonLev = new Common_Lev {
                // Create the map
                Maps = new Common_LevelMap[]
                {
                    new Common_LevelMap()
                    {
                        // Set the dimensions
                        Width = map.Width,
                        Height = map.Height,

                        // Create the tile arrays
                        TileSet = new Common_Tileset[3],
                        MapTiles = map.MapData.Select((x, i) => new Editor_MapTile(new MapTile()
                        {
                            CollisionType = (byte)cMap.CollisionData.ElementAtOrDefault(i),
                            TileMapY = (ushort)(BitHelpers.ExtractBits(x, numBits, 0)),
                            HorizontalFlip = BitHelpers.ExtractBits(x, 1, numBits) == 1,
                        })
                        {
                            DebugText = Convert.ToString(x, 2).PadLeft(16, '0')
                        }).ToArray(),
                        TileSetWidth = 1
                    }
                },

                // Create the events list
                EventData = new List<Editor_EventData>(),
            };

            const int paletteSize = 16;
            const int tileWidth = 8;
            int tileSize = is8bpp ? (tileWidth * tileWidth) : (tileWidth * tileWidth) / 2;

            var tiles = new Tile[tilemapLength];

            // Create empty tile
            var emptyTileTex = new Texture2D(Settings.CellSize, Settings.CellSize) {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            emptyTileTex.SetPixels(Enumerable.Repeat(Color.clear, Settings.CellSize * Settings.CellSize).ToArray());
            emptyTileTex.Apply();
            Tile emptyTile = ScriptableObject.CreateInstance<Tile>();
            emptyTile.sprite = Sprite.Create(emptyTileTex, new Rect(0, 0, Settings.CellSize, Settings.CellSize), new Vector2(0.5f, 0.5f), Settings.CellSize, 20);

            tiles[0] = emptyTile;

            // Hack: Create a tilemap for each palette
            for (int i = 1; i < tilemapLength; i++) {
                // Get the palette to use
                var pals = map.MapData.Where(x => BitHelpers.ExtractBits(x, 11, 0) == i).Select(x => BitHelpers.ExtractBits(x, 4, 12)).Distinct().ToArray();

                if (pals.Length > 1)
                    Debug.LogWarning($"Tile {i} has several possible palettes!");

                var p = pals.FirstOrDefault();

                var tex = new Texture2D(Settings.CellSize, Settings.CellSize) {
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp
                };

                for (int y = 0; y < tileWidth; y++) {
                    for (int x = 0; x < tileWidth; x++) {
                        Color c;

                        int index = ((i - 1) * tileSize) + ((y * tileWidth + x) / (is8bpp ? 1 : 2));
                        
                        if (is8bpp) 
                        {
                            var b = tileMap[index];

                            c = tilePalette.Palette[b].GetColor();

                            if (b != 0)
                                c = new Color(c.r, c.g, c.b, 1f);
                        } 
                        else 
                        {
                            var b = tileMap[index];
                            var v = BitHelpers.ExtractBits(b, 4, x % 2 == 0 ? 0 : 4);

                            c = tilePalette.Palette[p * paletteSize + v].GetColor();

                            if (v != 0)
                                c = new Color(c.r, c.g, c.b, 1f);
                        }

                        // Upscale to 16x16 for now...
                        tex.SetPixel(x * 2, y * 2, c);
                        tex.SetPixel(x * 2 + 1, y * 2, c);
                        tex.SetPixel(x * 2 + 1, y * 2 + 1, c);
                        tex.SetPixel(x * 2, y * 2 + 1, c);
                    }
                }

                tex.Apply();

                // Create a tile
                Tile t = ScriptableObject.CreateInstance<Tile>();
                t.sprite = Sprite.Create(tex, new Rect(0, 0, Settings.CellSize, Settings.CellSize), new Vector2(0.5f, 0.5f), Settings.CellSize, 20);

                tiles[i] = t;
            }

            commonLev.Maps[0].TileSet[0] = new Common_Tileset(tiles);

            commonLev.EventData = levelBlock.Actors.Select(x => new Editor_EventData(new EventData() {
                XPosition = x.XPos * 2,
                YPosition = x.YPos * 2
            }) {
                Type = x.ActorID,
                DESKey = String.Empty,
                ETAKey = String.Empty,
                DebugText = $"{nameof(GBA_Actor.Int_08)}: {x.Int_08}{Environment.NewLine}" +
                            $"{nameof(GBA_Actor.Byte_04)}: {x.Byte_04}{Environment.NewLine}" +
                            $"{nameof(GBA_Actor.ActorID)}: {x.ActorID}{Environment.NewLine}" +
                            $"{nameof(GBA_Actor.GraphicsDataIndex)}: {x.GraphicsDataIndex}{Environment.NewLine}" +
                            $"{nameof(GBA_Actor.Byte_07)}: {x.Byte_07}{Environment.NewLine}"
            }).ToList();

            await UniTask.CompletedTask;
            return commonLev;
        }

        public virtual async UniTask<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            Controller.status = $"Loading data";
            await Controller.WaitIfNecessary();

            // Read the rom
            var rom = FileFactory.Read<GBA_R3_ROM>(GetROMFilePath, context);

            var commonLev = await CreateCommonLev(context, rom.Data.LevelBlock);

            return new GBA_EditorManager(commonLev, context);
        }

        public void SaveLevel(Context context, BaseEditorManager editorManager) => throw new NotImplementedException();

        public virtual async UniTask LoadFilesAsync(Context context)
        {
            await FileSystem.PrepareFile(context.BasePath + GetROMFilePath);

            var file = new GBAMemoryMappedFile(context, 0x08000000)
            {
                filePath = GetROMFilePath,
            };
            context.AddFile(file);
        }
    }
}