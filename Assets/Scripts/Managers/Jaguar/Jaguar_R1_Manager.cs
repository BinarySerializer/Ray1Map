using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Game manager for Jaguar
    /// </summary>
    public class Jaguar_R1_Manager : IGameManager {
        #region Values and paths

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => GetNumLevels.OrderBy(x => x.Key).Select(x => new KeyValuePair<World, int[]>(x.Key, Enumerable.Range(1, x.Value).ToArray())).ToArray();

        /// <summary>
        /// Gets the available educational volumes
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The available educational volumes</returns>
        public string[] GetEduVolumes(GameSettings settings) => new string[0];

        /// <summary>
        /// Gets the file path to the ROM file
        /// </summary>
        public virtual string GetROMFilePath => $"ROM.j64";

        /// <summary>
        /// Gets the base address for the ROM file
        /// </summary>
        protected virtual uint GetROMBaseAddress => 0x00800000;

        /// <summary>
        /// Gets the available levels ordered based on the global level array
        /// </summary>
        public virtual KeyValuePair<World, int>[] GetNumLevels => new KeyValuePair<World, int>[]
        {
            new KeyValuePair<World, int>(World.Jungle, 21),
            new KeyValuePair<World, int>(World.Mountain, 14),
            new KeyValuePair<World, int>(World.Cave, 13),
            new KeyValuePair<World, int>(World.Music, 19),
            new KeyValuePair<World, int>(World.Image, 14),
            new KeyValuePair<World, int>(World.Cake, 4)
        };

        /// <summary>
        /// Gets the vignette addresses and widths
        /// </summary>
        protected virtual KeyValuePair<uint, int>[] GetVignette => new KeyValuePair<uint, int>[]
        {
            // Vignette
            new KeyValuePair<uint, int>(GetROMBaseAddress + 43680, 384),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 127930, 160),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 140541, 136),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 150788, 160),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 162259, 80),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 169031, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 246393, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 300827, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 329569, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 351048, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 372555, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 391386, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 409555, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 423273, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 429878, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 450942, 320),

            // Background/foreground
            new KeyValuePair<uint, int>(GetROMBaseAddress + 1353130, 192),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 1395878, 384),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 1462294, 384),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 1553686, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 1743668, 144),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 1750880, 48),

            new KeyValuePair<uint, int>(GetROMBaseAddress + 1809526, 192),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 1845684, 384),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 1928746, 192),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 1971368, 192),

            new KeyValuePair<uint, int>(GetROMBaseAddress + 2205640, 384),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 2269442, 384),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 2355852, 160),

            new KeyValuePair<uint, int>(GetROMBaseAddress + 2702140, 384),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 2803818, 192),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 2824590, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 2916108, 192),

            new KeyValuePair<uint, int>(GetROMBaseAddress + 3078442, 192),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 3118496, 384),

            new KeyValuePair<uint, int>(GetROMBaseAddress + 3276778, 384),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 3323878, 320),
        };

        #endregion

        #region Manager Methods

        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public GameAction[] GetGameActions(GameSettings settings)
        {
            return new GameAction[]
            {
                new GameAction("Extract Vignette", false, true, (input, output) => ExtractVignetteAsync(settings, output)),
                new GameAction("Extract Compressed Data", false, true, (input, output) => ExtractCompressedDataAsync(settings, output, false)),
                new GameAction("Extract Compressed Data (888)", false, true, (input, output) => ExtractCompressedDataAsync(settings, output, true)),
            };
        }

        /// <summary>
        /// Extracts all vignette
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="outputPath">The path to extract to</param>
        /// <returns>The task</returns>
        public async Task ExtractVignetteAsync(GameSettings settings, string outputPath)
        {
            // Create a context
            using (var context = new Context(settings))
            {
                // Get a deserializer
                var s = context.Deserializer;

                // Add the file
                var file = await LoadExtraFile(context, GetROMFilePath, GetROMBaseAddress);

                // Export every vignette
                foreach (var vig in GetVignette)
                {
                    s.DoAt(new Pointer(vig.Key, file), () =>
                    {
                        s.DoEncoded(new RNCEncoder(), () =>
                        {
                            var values = s.SerializeObjectArray<RGB556Color>(default, s.CurrentLength / 2);

                            var tex = new Texture2D(vig.Value, values.Length / vig.Value)
                            {
                                filterMode = FilterMode.Point,
                                wrapMode = TextureWrapMode.Clamp
                            };

                            for (int y = 0; y < tex.height; y++)
                            {
                                for (int x = 0; x < tex.width; x++)
                                {
                                    tex.SetPixel(x, tex.height - y - 1, values[y * tex.width + x].GetColor());
                                }
                            }

                            tex.Apply();

                            Util.ByteArrayToFile(Path.Combine(outputPath, $"Vig_{vig.Key:X8}.png"), tex.EncodeToPNG());
                        });
                    });
                }
            }
        }

        /// <summary>
        /// Extracts all the compressed data from the rom
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="outputPath">The path to extract to</param>
        /// <param name="as888">Indicates if the blocks should be converted to RGB-888</param>
        public async Task ExtractCompressedDataAsync(GameSettings settings, string outputPath, bool as888)
        {
            // Create a context
            using (var context = new Context(settings))
            {
                // Get a deserializer
                var s = context.Deserializer;

                // Add the file
                var file = await LoadExtraFile(context, GetROMFilePath, GetROMBaseAddress);

                s.DoAt(file.StartPointer, () =>
                {
                    // Enumerate every byte
                    while (s.CurrentPointer.FileOffset < file.Length - 4)
                    {
                        // Read the next 4 bytes and check if the header matches
                        var header = s.Serialize<uint>(default);

                        if (header == 0x524E4302)
                        {
                            // Go back four steps
                            s.Goto(s.CurrentPointer - 4);

                            // Get the current pointer
                            var p = s.CurrentPointer;

                            s.DoEncoded(new RNCEncoder(), () =>
                            {
                                if (as888)
                                {
                                    var values = s.SerializeObjectArray<RGB556Color>(default, s.CurrentLength / 2);

                                    var output = new byte[values.Length * 3];
                                        
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        var v = values[i];

                                        // Write RGB values
                                        output[i * 3 + 0] = v.Red;
                                        output[i * 3 + 1] = v.Green;
                                        output[i * 3 + 2] = v.Blue;
                                    }

                                    Util.ByteArrayToFile(Path.Combine(outputPath, $"decompressedBlock_{p.FileOffset}_{p.FileOffset + 0x00800000:X8}"), output);
                                }
                                else
                                {
                                    Util.ByteArrayToFile(Path.Combine(outputPath, $"decompressedBlock_{p.FileOffset}_{p.FileOffset + 0x00800000:X8}"), s.SerializeArray<byte>(default, s.CurrentLength));
                                }
                            });
                        }
                        else
                        {
                            // Go back three steps
                            s.Goto(s.CurrentPointer - 3);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <returns>The editor manager</returns>
        public virtual async Task<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            // Read the rom
            var rom = FileFactory.Read<Jaguar_R1_ROM>(GetROMFilePath, context);

            // Get the map
            var map = rom.MapData;

            // Convert levelData to common level format
            Common_Lev commonLev = new Common_Lev
            {
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
                        Tiles = new Common_Tile[map.Width * map.Height]
                    }
                },

                // Create the events list
                EventData = new List<Common_EventData>(),
            };

            // Load tile set
            commonLev.Maps[0].TileSet[0] = new Common_Tileset(rom.TileData.Select(x => x.Blue == 0 && x.Red == 0 && x.Green == 0 ? new RGB556Color(0, 0, 0, 0) : x).ToArray(), 1, 16);

            // Enumerate each cell
            for (int cellY = 0; cellY < map.Height; cellY++)
            {
                for (int cellX = 0; cellX < map.Width; cellX++)
                {
                    // Get the cell
                    var cell = map.Tiles[cellY * map.Width + cellX];

                    // Set the common tile
                    commonLev.Maps[0].Tiles[cellY * map.Width + cellX] = new Common_Tile()
                    {
                        TileSetGraphicIndex = cell.TileMapX & 0x7FF,
                        CollisionType = cell.CollisionType,
                        PaletteIndex = 1,
                        XPosition = cellX,
                        YPosition = cellY
                    };
                }
            }

            return new PS1EditorManager(commonLev, context, 
                // TODO: Load graphics and ETA
                new Dictionary<Pointer, Common_Design>(), new Dictionary<Pointer, Common_EventState[][]>());
        }

        /// <summary>
        /// Saves the specified level
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="commonLevelData">The common level data</param>
        public void SaveLevel(Context context, Common_Lev commonLevelData) => throw new NotImplementedException();

        /// <summary>
        /// Preloads all the necessary files into the context
        /// </summary>
        /// <param name="context">The serialization context</param>
        public async Task LoadFilesAsync(Context context) => await LoadExtraFile(context, GetROMFilePath, GetROMBaseAddress);

        public virtual async Task<MemoryMappedFile> LoadExtraFile(Context context, string path, uint baseAddress)
        {
            await FileSystem.PrepareFile(context.BasePath + path);

            var file = new MemoryMappedFile(context, baseAddress)
            {
                filePath = path,
                Endianness = BinaryFile.Endian.Big
            };
            context.AddFile(file);

            return file;
        }

        #endregion
    }
}