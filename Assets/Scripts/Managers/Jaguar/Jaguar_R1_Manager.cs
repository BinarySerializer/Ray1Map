using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Asyncoroutine;
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
                new GameAction("Export Sprites", false, true, (input, output) => ExportAllSpritesAsync(settings, output)),
                new GameAction("Extract Vignette", false, true, (input, output) => ExtractVignetteAsync(settings, output)),
                new GameAction("Extract Compressed Data", false, true, (input, output) => ExtractCompressedDataAsync(settings, output, false)),
                new GameAction("Extract Compressed Data (888)", false, true, (input, output) => ExtractCompressedDataAsync(settings, output, true)),
                new GameAction("Convert Music to MIDI", false, true, (input, output) => ConvertMusicAsync(settings, output)),
                new GameAction("Fix memory dump byte swapping", false, false, (input, output) => FixMemoryDumpByteSwapping(settings)),
            };
        }

        /// <summary>
        /// Exports every sprite from the game
        /// </summary>
        /// <param name="baseGameSettings">The game settings</param>
        /// <param name="outputDir">The output directory</param>
        /// <returns>The task</returns>
        public virtual async Task ExportAllSpritesAsync(GameSettings baseGameSettings, string outputDir)
        {
            // Create the context
            using (var context = new Context(baseGameSettings))
            {
                // Load the game data
                await LoadFilesAsync(context);

                // Serialize the rom
                var rom = FileFactory.Read<Jaguar_R1_ROM>(GetROMFilePath, context);

                // Get the level counts
                var levels = GetNumLevels;

                // Get the deserializer
                var s = context.Deserializer;

                // TODO: Export big ray and font - from seventh world?

                // Get allfix sprite commands
                var allfixCmds = rom.FixSpritesLoadCommands.Commands.Where(x => x.Type == Jaguar_R1_LevelLoadCommand.LevelLoadCommandType.Sprites).ToArray();

                // Export allfix
                await ExportGroupAsync(allfixCmds, Enumerable.Repeat(rom.SpritePalette, allfixCmds.Length).ToArray(), "Allfix");

                // Enumerate every world
                foreach (var world in GetLevels(baseGameSettings))
                {
                    // Get the world index
                    var worldIndex = levels.FindItemIndex(x => x.Key == world.Key);

                    // Get the level load commands
                    var lvlCmds = rom.MapDataLoadCommands[worldIndex];

                    // TODO: Why does Cave 3 not have palettes or cmds?
                    // Get palettes for the levels
                    var palettes = lvlCmds.
                        Select((x, i) => x?.Commands?.FirstOrDefault(c => c.Type == Jaguar_R1_LevelLoadCommand.LevelLoadCommandType.Palette)?.PalettePointer).
                        Select((x, i) => x == null ? rom.SpritePalette : s.DoAt<RGB556Color[]>(x, () => s.SerializeObjectArray<RGB556Color>(default, 256, name: $"SpritePalette[{i}]"))).
                        ToArray();

                    // Get the world and level sprite commands and palettes
                    var worldCmds = new List<Jaguar_R1_LevelLoadCommand>();
                    var worldPal = new List<RGB556Color[]>();

                    // Add world data
                    worldCmds.AddRange(rom.WorldSpritesLoadCommands[worldIndex].Commands.Where(x => x.Type == Jaguar_R1_LevelLoadCommand.LevelLoadCommandType.Sprites));
                    worldPal.AddRange(Enumerable.Repeat(palettes.First(), worldCmds.Count));

                    // TODO: Some sprites get the wrong palette, like the Bzzit ones - why?
                    // Enumerate every level
                    for (int lvl = 0; lvl < lvlCmds.Length; lvl++)
                    {
                        foreach (var c in lvlCmds[lvl]?.Commands?.Where(x => x.Type == Jaguar_R1_LevelLoadCommand.LevelLoadCommandType.Sprites).Where(x => worldCmds.All(y => y.ImageBufferPointer != x.ImageBufferPointer)) ?? new Jaguar_R1_LevelLoadCommand[0])
                        {
                            worldCmds.Add(c);
                            worldPal.Add(palettes[lvl]);
                        }
                    }

                    // Export world
                    await ExportGroupAsync(worldCmds, worldPal, world.Key.ToString());
                }

                // Helper method for exporting a collection of DES
                async Task ExportGroupAsync(IReadOnlyList<Jaguar_R1_LevelLoadCommand> cmds, IReadOnlyList<RGB556Color[]> palettes, string name)
                {
                    // Enumerate every graphics
                    for (var desIndex = 0; desIndex < cmds.Count; desIndex++)
                    {
                        // Get values for current DES
                        var cmd = cmds[desIndex];
                        var pal = palettes[desIndex];

                        // Get the image buffer
                        byte[] imgBuffer = null;
                        s.DoAt(cmd.ImageBufferPointer, () => s.DoEncoded(new RNCEncoder(), () => imgBuffer = s.SerializeArray<byte>(default, s.CurrentLength, "ImageBuffer")));

                        // Get the DES
                        var des = rom.EventDefinitions.FirstOrDefault(x => x.ImageBufferMemoryPointerPointer == cmd.ImageBufferMemoryPointerPointer);
                        // TODO: fix this

                        // TODO: This doesn't always work - why?
                        if (des == null)
                        {
                            Debug.LogWarning($"No DES found!");
                            continue;
                        }

                        var imgIndex = 0;

                        // Export every sprite
                        foreach (var d in des.ImageDescriptors)
                        {
                            // TODO: Remove the try/catch once we fix the width!
                            try
                            {
                                // Get the texture
                                var tex = GetSpriteTexture(d, pal, imgBuffer);

                                // Export if not null
                                if (tex != null)
                                    Util.ByteArrayToFile(Path.Combine(outputDir, name, $"{desIndex} - {imgIndex} - 0x{d.Offset.FileOffset:X8}.png"), tex.EncodeToPNG());
                            }
                            catch (Exception ex)
                            {
                                Debug.LogWarning(ex.Message);
                            }

                            imgIndex++;
                        }
                    }

                    // Unload textures
                    await Resources.UnloadUnusedAssets();
                }
            }
        }

        /// <summary>
        /// Gets the texture for a sprite
        /// </summary>
        /// <param name="d">The image descriptor</param>
        /// <param name="pal">The palette</param>
        /// <param name="imgBuffer">The image buffer</param>
        /// <returns>The sprite texture</returns>
        public Texture2D GetSpriteTexture(Common_ImageDescriptor d, ARGBColor[] pal, byte[] imgBuffer)
        {
            // Make sure the sprite is valid
            if (d.Index == 0x00 || d.Index == 0xFF)
                return null;

            // Create a texture
            var tex = new Texture2D(d.OuterWidth, d.OuterHeight)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            bool is8Bit = BitHelpers.ExtractBits(d.Jag_Byte0E, 1, 4) != 0;

            // Set every pixel
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    var index = y * tex.width + x;

                    int palIndex;
                    if (is8Bit)
                    {
                        palIndex = imgBuffer[d.ImageBufferOffset + index];

                        tex.SetPixel(x, tex.height - y - 1, palIndex == 0 ? new Color() : pal[palIndex].GetColor());
                    }
                    else
                    {
                        int indexInPal = BitHelpers.ExtractBits(d.Jag_Byte0A, 4, 1);
                        palIndex = imgBuffer[d.ImageBufferOffset + index / 2];
                        palIndex = BitHelpers.ExtractBits(palIndex, 4, index % 2 == 0 ? 4 : 0);

                        tex.SetPixel(x, tex.height - y - 1, palIndex == 0 ? new Color() : pal[indexInPal * 16 + palIndex].GetColor());
                    }
                }
            }

            tex.Apply();

            return tex;
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

        public async Task ConvertMusicAsync(GameSettings settings, string outputPath) {
            // Create a context
            using (var context = new Context(settings)) {
                // Get a deserializer
                var s = context.Deserializer;

                // Add the file
                var file = await LoadExtraFile(context, GetROMFilePath, GetROMBaseAddress);
                var pointerTable = PointerTables.GetJaguarPointerTable(s.GameSettings.GameModeSelection, file);
                s.DoAt(pointerTable[Jaguar_R1_Pointer.Music], () => {
                    // Read the music table
                    Jaguar_R1_MusicDescriptor[] MusicTable = s.SerializeObjectArray<Jaguar_R1_MusicDescriptor>(null, 0x20, name: nameof(MusicTable));
                    // Immediately after this: pointer to sample buffer?

                    // For each entry
                    MidiWriter w = new MidiWriter();
                    for (int i = 0; i < MusicTable.Length; i++) {
                        w.Write(MusicTable[i],
                            Path.Combine(outputPath,
                            $"Track{i}_{MusicTable[i].MusicDataPointer.AbsoluteOffset:X8}.mid"));
                    }
                });
            }
        }


        public async Task FixMemoryDumpByteSwapping(GameSettings settings) {
            await Task.CompletedTask;
            // Create a context
            using (var context = new Context(settings)) {
                // Get a deserializer
                var s = context.Deserializer;
                string[] files = Directory.EnumerateFiles(context.BasePath, "*.jag", SearchOption.TopDirectoryOnly).ToArray();
                foreach (string filepath in files) {
                    // Add the file
                    string path = filepath.Substring(context.BasePath.Length);
                    var file = new LinearSerializedFile(context) {
                        filePath = path,
                        Endianness = BinaryFile.Endian.Little
                    };
                    context.AddFile(file);
                    ushort[] data = s.DoAt(file.StartPointer, () => {
                        return s.SerializeArray<ushort>(null, s.CurrentLength / 2, name: nameof(data));
                    });

                    using (MemoryStream ms = new MemoryStream()) {
                        Writer w = new Writer(ms, isLittleEndian: false);
                        foreach (ushort u in data) {
                            w.Write(u);
                        }
                        ms.Position = 0;
                        Util.ByteArrayToFile(context.BasePath + path + ".fixed", ms.ToArray());
                    }
                }
            }
        }

        /*public void InitRAM(Context context) {
            // Load at 1 to prevent 0 being read as a valid pointer
            var file = new MemoryMappedByteArrayFile("RAM", 0x00200000 - 1, context, 1) {
                Endianness = BinaryFile.Endian.Big
            };
            context.AddFile(file);
        }*/

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <returns>The editor manager</returns>
        public virtual Task<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            //InitRAM(context);
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

            // Load tile set and treat black as transparent
            commonLev.Maps[0].TileSet[0] = new Common_Tileset(rom.TileData.Select(x => x.Blue == 0 && x.Red == 0 && x.Green == 0 ? new RGB556Color(0, 0, 0, 0) : x).ToArray(), 1, 16);

            var eventDesigns = new Dictionary<Pointer, Common_Design>();
            var eventETA = new Dictionary<Pointer, Common_EventState[][]>();

            // TODO: Fix with correct link index
            var linkIndex = 0;

            // Load events
            for (var i = 0; i < rom.EventData.EventData.Length; i++)
            {
                // Get the map base position, based on the event map
                var mapPos = rom.EventData.EventIndexMap.FindItemIndex(z => z == i + 1);

                // Get the x and y positions
                var mapY = (uint)Math.Floor(mapPos / (double)(rom.EventData.Width));
                var mapX = (uint)(mapPos - (mapY * rom.EventData.Width));
                
                // Calculate the actual position on the map
                mapX *= 4 * Settings.CellSize;
                mapY *= 4 * Settings.CellSize;

                // Add every event on this tile
                for (int j = 0; j < rom.EventData.EventData[i].Length; j++)
                {
                    var e = rom.EventData.EventData[i][j];

                    // Add if not found
                    if (e.EventDefinition.ImageDescriptorsPointer != null && !eventDesigns.ContainsKey(e.EventDefinition.ImageDescriptorsPointer))
                    {
                        Common_Design finalDesign = new Common_Design
                        {
                            Sprites = new List<Sprite>(),
                            Animations = new List<Common_Animation>()
                        };

                        // Get every sprite
                        foreach (Common_ImageDescriptor img in e.EventDefinition.ImageDescriptors)
                        {
                            // TODO: Remove try catch
                            try
                            {
                                // Get the texture for the sprite, or null if not loading textures
                                Texture2D tex = loadTextures ? GetSpriteTexture(img, rom.SpritePalette, rom.ImageBuffers[e.EventDefinition.ImageBufferMemoryPointerPointer]) : null;

                                // Add it to the array
                                finalDesign.Sprites.Add(tex == null ? null : Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), 16, 20));
                            }
                            catch (Exception ex)
                            {
                                finalDesign.Sprites.Add(null);
                            }
                        }

                        if (e.EventDefinition.States != null)
                            // Add animations
                            finalDesign.Animations.AddRange(e.EventDefinition.States.Where(x => x.Animation != null).Select(x => x.Animation.ToCommonAnimation()));

                        // Add to the designs
                        eventDesigns.Add(e.EventDefinition.ImageDescriptorsPointer, finalDesign);
                    }

                    // Add if not found
                    if (e.EventDefinition.EventStatesPointer != null && e.EventDefinition.States != null && !eventETA.ContainsKey(e.EventDefinition.EventStatesPointer))
                    {
                        // Create a common state array
                        var states = new Common_EventState[e.EventDefinition.States.Length][];

                        // Add dummy states
                        for (byte s = 0; s < states.Length; s++)
                        {
                            states[s] = new Common_EventState[]
                            {
                                new Common_EventState
                                {
                                    AnimationIndex = s,
                                    LinkedEtat = (byte)e.EventDefinition.States.FindItemIndex(x => x == e.EventDefinition.States[s].LinkedState),
                                    
                                    // TODO: Set these correctly
                                    AnimationSpeed = 2,
                                }
                            };
                        }

                        // Add to the states
                        eventETA.Add(e.EventDefinition.EventStatesPointer, states);
                    }

                    // Add the event
                    commonLev.EventData.Add(new Common_EventData
                    {
                        // TODO: Set this to the state index
                        Etat = 0,
                        
                        LinkIndex = linkIndex,

                        XPosition = mapX + e.OffsetX,
                        YPosition = mapY + e.OffsetY,

                        DESKey = e.EventDefinition.ImageDescriptorsPointer?.ToString() ?? String.Empty,
                        ETAKey = e.EventDefinition.EventStatesPointer?.ToString() ?? String.Empty,
                        
                        // These are not available on Jaguar
                        SubEtat = 0,
                        Type = EventType.TYPE_BADGUY1,
                        OffsetBX = 0,
                        OffsetBY = 0,
                        OffsetHY = 0,
                        FollowSprite = 0,
                        HitPoints = 0,
                        Layer = 0,
                        MapLayer = null,
                        HitSprite = 0,
                        FollowEnabled = false,
                        FlipHorizontally = false,
                        LabelOffsets = new ushort[0],
                        CommandCollection = null,

                        DebugText = $"Unk_0A: {e.Unk_0A}{Environment.NewLine}" +
                                    $"Unk_0C: {e.Unk_0C}{Environment.NewLine}" +
                                    $"MapPos: {mapPos}{Environment.NewLine}" +
                                    $"DES: {e.EventDefinitionPointer}{Environment.NewLine}" +
                                    $"OffsetX: {e.OffsetX}{Environment.NewLine}" +
                                    $"OffsetY: {e.OffsetY}{Environment.NewLine}"
                    });

                    linkIndex++;
                }
            }

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
                        // We need to ignore some bits since it's sometimes set when the tile index should be 0
                        TileSetGraphicIndex = cell.TileMapX & 0x7FF,

                        CollisionType = cell.CollisionType,
                        PaletteIndex = 1,
                        XPosition = cellX,
                        YPosition = cellY
                    };
                }
            }

            return Task.FromResult<BaseEditorManager>(new PS1EditorManager(commonLev, context, eventDesigns, eventETA, null));
        }

        /// <summary>
        /// Saves the specified level
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="editorManager">The editor manager</param>
        public void SaveLevel(Context context, BaseEditorManager editorManager) => throw new NotImplementedException();

        /// <summary>
        /// Preloads all the necessary files into the context
        /// </summary>
        /// <param name="context">The serialization context</param>
        public async Task LoadFilesAsync(Context context) => await LoadExtraFile(context, GetROMFilePath, GetROMBaseAddress);

        public virtual async Task<MemoryMappedFile> LoadExtraFile(Context context, string path, uint baseAddress)
        {
            await FileSystem.PrepareFile(context.BasePath + path);

            // TODO: Maybe change this - using this for now to allow invalid pointers
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