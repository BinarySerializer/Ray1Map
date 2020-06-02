using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => new KeyValuePair<World, int[]>[0];

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
                new GameAction("Extract Compressed Data", false, true, (input, output) => ExtractCompressedDataAsync(settings, output, false)),
                new GameAction("Extract Compressed Data (888)", false, true, (input, output) => ExtractCompressedDataAsync(settings, output, true)),
            };
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
                    while (s.CurrentPointer.FileOffset < file.Length)
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
                                    var values = s.SerializeArray<ushort>(default, s.CurrentLength / 2);

                                    var output = new byte[(values.Length / 2) * 3];

                                    for (int i = 0; i < values.Length; i += 2)
                                    {
                                        var v = values[i];

                                        // Write RGB values
                                        output[(i / 2) * 3 + 0] = (byte)((BitHelpers.ExtractBits(v, 5, 6) / 31f) * 255);
                                        output[(i / 2) * 3 + 1] = (byte)((BitHelpers.ExtractBits(v, 6, 0) / 63f) * 255);
                                        output[(i / 2) * 3 + 2] = (byte)((BitHelpers.ExtractBits(v, 5, 11) / 31f) * 255);
                                    }

                                    Util.ByteArrayToFile(Path.Combine(outputPath, $"decompressedBlock_{p.FileOffset}"), output);
                                }
                                else
                                {
                                    Util.ByteArrayToFile(Path.Combine(outputPath, $"decompressedBlock_{p.FileOffset}"), s.SerializeArray<byte>(default, s.CurrentLength));
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

            // Dummy tileset
            commonLev.Maps[0].TileSet[0] = new Common_Tileset(Enumerable.Repeat(new ARGB1555Color(0, 0, 0), 16*16).ToArray(), 1, 16);

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
                        TileSetGraphicIndex = 0,
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