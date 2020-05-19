using Asyncoroutine;
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
    /// Base game manager for GBA
    /// </summary>
    public class GBA_R1_Manager : IGameManager {
        #region Values and paths

        /// <summary>
        /// The amount of levels in the game
        /// </summary>
        public const int LevelCount = 22 + 18 + 13 + 13 + 12 + 4 + 6;

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => GetGlobalLevels.GroupBy(x => x).Select(x => new KeyValuePair<World, int[]>(x.Key, Enumerable.Range(1, x.Count()).ToArray())).ToArray();

        /// <summary>
        /// Gets the available educational volumes
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The available educational volumes</returns>
        public string[] GetEduVolumes(GameSettings settings) => new string[0];

        /// <summary>
        /// Gets the available levels ordered based on the global level array
        /// </summary>
        public virtual World[] GetGlobalLevels => new World[]
        {
            World.Jungle,
            World.Jungle,
            World.Jungle,
            World.Jungle,
            World.Jungle,
            World.Jungle,
            World.Jungle,
            World.Jungle,
            World.Jungle,
            World.Jungle,
            World.Jungle,
            World.Jungle,
            World.Jungle,
            World.Jungle,
            World.Jungle,
            World.Jungle,
            World.Jungle,
            World.Jungle,
            World.Jungle,
            World.Jungle,
            World.Jungle,
            World.Jungle,
            World.Music,
            World.Music,
            World.Music,
            World.Music,
            World.Music,
            World.Music,
            World.Music,
            World.Music,
            World.Music,
            World.Music,
            World.Music,
            World.Music,
            World.Music,
            World.Music,
            World.Music,
            World.Music,
            World.Music,
            World.Music,
            World.Mountain,
            World.Mountain,
            World.Mountain,
            World.Mountain,
            World.Mountain,
            World.Mountain,
            World.Mountain,
            World.Mountain,
            World.Mountain,
            World.Mountain,
            World.Mountain,
            World.Mountain,
            World.Mountain,
            World.Image,
            World.Image,
            World.Image,
            World.Image,
            World.Image,
            World.Image,
            World.Image,
            World.Image,
            World.Image,
            World.Image,
            World.Image,
            World.Image,
            World.Image,
            World.Cave,
            World.Cave,
            World.Cave,
            World.Cave,
            World.Cave,
            World.Cave,
            World.Cave,
            World.Cave,
            World.Cave,
            World.Cave,
            World.Cave,
            World.Cave,
            World.Cake,
            World.Cake,
            World.Cake,
            World.Cake,
            World.Jungle,
            World.Music,
            World.Mountain,
            World.Image,
            World.Cave,
            World.Cake,
        };

        /// <summary>
        /// Gets the file path to the ROM file
        /// </summary>
        public virtual string GetROMFilePath => $"ROM.gba";

        /// <summary>
        /// Gets the global level index from the world and level
        /// </summary>
        /// <param name="world">The world</param>
        /// <param name="lvl">The level</param>
        /// <returns>The global index</returns>
        public int GetGlobalLevelIndex(World world, int lvl)
        {
            var lvls = GetGlobalLevels;
            var worldIndex = 0;
            for (int i = 0; i < lvls.Length; i++)
            {
                if (lvls[i] == world)
                {
                    if (worldIndex == lvl - 1)
                        return i;

                    worldIndex++;
                }
            }

            return -1;
        }

        /// <summary>
        /// True if colors are 4-bit, false if they're 8-bit
        /// </summary>
        public virtual bool Is4Bit => true;

        /// <summary>
        /// True if palette indexes are used, false if not
        /// </summary>
        public virtual bool UsesPaletteIndex => true;

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
                new GameAction("Export Vignette", false, true, (input, output) => ExtractVignetteAsync(settings, output)),
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
                // Load the rom
                await LoadFilesAsync(context);

                // Serialize the rom
                var rom = FileFactory.Read<GBA_R1_ROM>(GetROMFilePath, context);

                // Get a pointer table
                var pointerTable = PointerTables.GetGBAPointerTable(baseGameSettings.GameModeSelection, rom.Offset.file);

                var graphics = new Dictionary<Pointer, List<World>>();

                // Enumerate every world
                foreach (var world in GetLevels(baseGameSettings))
                {
                    baseGameSettings.World = world.Key;

                    // Enumerate every level
                    foreach (var lvl in world.Value)
                    {
                        baseGameSettings.Level = lvl;

                        // Serialize the event data
                        var eventData = new GBA_R1_LevelEventData();
                        eventData.SerializeData(context.Deserializer, pointerTable[GBA_R1_ROMPointer.EventGraphicsPointers], pointerTable[GBA_R1_ROMPointer.EventDataPointers], pointerTable[GBA_R1_ROMPointer.EventGraphicsGroupCountTablePointers], pointerTable[GBA_R1_ROMPointer.LevelEventGraphicsGroupCounts]);

                        // Get the event graphics
                        for (var i = 0; i < eventData.GraphicData.Length; i++)
                        {
                            var key = eventData.GraphicDataPointers[i];

                            if (!graphics.ContainsKey(key))
                                graphics.Add(key, new List<World>());

                            if (!graphics[key].Contains(world.Key))
                                graphics[key].Add(world.Key);
                        }
                    }
                }

                // Add unused graphics
                graphics.Add(pointerTable[GBA_R1_ROMPointer.DrumWalkerGraphics], new List<World>());
                graphics.Add(pointerTable[GBA_R1_ROMPointer.ClockGraphics], new List<World>());
                graphics.Add(pointerTable[GBA_R1_ROMPointer.InkGraphics], new List<World>());
                graphics.Add(pointerTable[GBA_R1_ROMPointer.FontSmallGraphics], new List<World>());
                graphics.Add(pointerTable[GBA_R1_ROMPointer.FontLargeGraphics], new List<World>());
                graphics.Add(pointerTable[GBA_R1_ROMPointer.PinsGraphics], new List<World>());

                var desIndex = 0;

                // Enumerate every graphics
                foreach (var gp in graphics)
                {
                    var imgIndex = 0;

                    // Get the graphic data
                    var g = FileFactory.Read<GBA_R1_EventGraphicsData>(gp.Key, context);

                    // Get the world name
                    var worldName = gp.Value.Count > 1 ? "Allfix" : (!gp.Value.Any() ? "Other" : gp.Value.First().ToString());

                    // Enumerate every image descriptor
                    foreach (var img in g.ImageDescriptors)
                    {
                        // Get the texture
                        var tex = GetSpriteTexture(context, g, img, rom.SpritePalettes);

                        // Make sure it's not null
                        if (tex == null)
                        {
                            imgIndex++;
                            continue;
                        }

                        Util.ByteArrayToFile(Path.Combine(outputDir, worldName, $"{desIndex} - {imgIndex}.png"), tex.EncodeToPNG());

                        imgIndex++;
                    }

                    desIndex++;
                }

                // Unload textures
                await Resources.UnloadUnusedAssets();
            }
        }

        // Hacky method for finding unused sprites - make sure you uncomment the code in GBA_R1_EventGraphicsData!
        public virtual async Task ExportUnusedSpritesAsync(GameSettings baseGameSettings, string outputDir)
        {
            // Create the context
            using (var context = new Context(baseGameSettings))
            {
                // Load the rom
                await LoadFilesAsync(context);

                // Serialize the rom
                var rom = FileFactory.Read<GBA_R1_ROM>(GetROMFilePath, context);

                // Get used graphics
                var graphics = new List<Pointer>();

                var pointerTable = PointerTables.GetGBAPointerTable(baseGameSettings.GameModeSelection, rom.Offset.file);

                // Enumerate every world
                foreach (var world in GetLevels(baseGameSettings))
                {
                    baseGameSettings.World = world.Key;

                    // Enumerate every level
                    foreach (var lvl in world.Value)
                    {
                        baseGameSettings.Level = lvl;

                        // Serialize the event data
                        var eventData = new GBA_R1_LevelEventData();
                        eventData.SerializeData(context.Deserializer, pointerTable[GBA_R1_ROMPointer.EventGraphicsPointers], pointerTable[GBA_R1_ROMPointer.EventDataPointers], pointerTable[GBA_R1_ROMPointer.EventGraphicsGroupCountTablePointers], pointerTable[GBA_R1_ROMPointer.LevelEventGraphicsGroupCounts]);

                        // Get the event graphics
                        for (var i = 0; i < eventData.GraphicData.Length; i++)
                        {
                            if (!graphics.Contains(eventData.GraphicDataPointers[i]))
                                graphics.Add(eventData.GraphicDataPointers[i]);
                        }
                    }
                }

                var s = context.Deserializer;

                // Enumerate every fourth byte (we assume it's aligned this way)
                for (int i = 0; i < s.CurrentLength; i += 4)
                {
                    s.Goto(rom.Offset + i);

                    File.WriteAllText(Path.Combine(outputDir, "log.txt"), $"{s.CurrentPointer.FileOffset} / {s.CurrentLength}");

                    try
                    {
                        // Make sure the graphic isn't referenced already
                        if (graphics.Any(x => x == s.CurrentPointer))
                            continue;

                        // Serialize it
                        var g = s.SerializeObject<GBA_R1_EventGraphicsData>(default);

                        var imgIndex = 0;

                        // Enumerate every image descriptor
                        foreach (var img in g.ImageDescriptors)
                        {
                            // Get the texture
                            var tex = GetSpriteTexture(context, g, img, rom.SpritePalettes);

                            // Make sure it's not null
                            if (tex == null)
                            {
                                imgIndex++;
                                continue;
                            }

                            Util.ByteArrayToFile(Path.Combine(outputDir, $"{(rom.Offset + i).FileOffset} - {imgIndex}.png"), tex.EncodeToPNG());

                            imgIndex++;
                        }

                        // Unload textures
                        await Resources.UnloadUnusedAssets();
                    }
                    catch
                    {
                        // Do nothing...
                    }
                }
            }
        }

        public virtual async Task ExtractVignetteAsync(GameSettings settings, string outputDir)
        {
            // Create a context
            using (var context = new Context(settings))
            {
                // Load the ROM
                await LoadFilesAsync(context);

                // Read data from the ROM
                var rom = FileFactory.Read<GBA_R1_ROM>(GetROMFilePath, context);

                // Extract every background vignette
                for (int i = 0; i < rom.BackgroundVignettes.Length; i++)
                {
                    // Get the vignette
                    var vig = rom.BackgroundVignettes[i];

                    // Make sure we have image data
                    if (vig.ImageData == null)
                        continue;

                    // Get the texture
                    var tex = GetVignetteTexture(vig);

                    // Save the texture
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"BG_{i}.png"), tex.EncodeToPNG());
                }

                // Extract every intro vignette
                for (int i = 0; i < rom.IntroVignettes.Length; i++)
                {
                    // Get the vignette
                    var vig = rom.IntroVignettes[i];

                    // Get the texture
                    var tex = GetVignetteTexture(vig);

                    // Save the texture
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"Intro_{i}.png"), tex.EncodeToPNG());
                }

                // Extract world map vignette

                // Get the world map texture
                var worldMapTex = GetVignetteTexture(rom.WorldMapVignette);

                // Save the texture
                Util.ByteArrayToFile(Path.Combine(outputDir, $"WorldMap.png"), worldMapTex.EncodeToPNG());
            }
        }


        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="levelMapData">The level to get the tile set for</param>
        /// <returns>The tile set to use</returns>
        public virtual Common_Tileset GetTileSet(Context context, GBA_R1_LevelMapData levelMapData) {
            // Read the tiles
            int block_size = Is4Bit ? 0x20 : 0x40;

            // If there are no tile blocks, return a dummy tile set
            if (levelMapData.TileBlockIndices == null)
            {
                var dummy = new Texture2D(256, Settings.CellSize)
                {
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp
                };
                dummy.SetPixels(new Color[dummy.width * dummy.height]);
                dummy.Apply();

                return new Common_Tileset(dummy, Settings.CellSize);
            }

            uint length = (uint)levelMapData.TileBlockIndices.Length * 8 * 8;

            // Get the tile-set texture
            var tex = new Texture2D(256, Mathf.CeilToInt(length / 256f / Settings.CellSize) * Settings.CellSize) {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            for (int i = 0; i < levelMapData.TileBlockIndices.Length; i++) {
                ushort blockIndex = levelMapData.TileBlockIndices[i];

                var x = ((i / 4) * 2) % (256/8) + ((i % 2) == 0 ? 0 : 1);
                var y = (((i / 4) * 2) / (256/8)) * 2 + ((i % 4) < 2 ? 0 : 1);

                var curOff = block_size * blockIndex;

                if (UsesPaletteIndex && levelMapData.TilePaletteIndices[i] >= 10)
                    Debug.LogWarning("Tile palette index exceeded 9: " + i + " - " + levelMapData.TilePaletteIndices[i]);

                FillSpriteTextureBlock(tex, 0, 0, x, y, levelMapData.TileData, curOff, levelMapData.TilePalettes, UsesPaletteIndex ? levelMapData.TilePaletteIndices[i] : 0, false, reverseHeight: false);
            }

            tex.Apply();

            return new Common_Tileset(tex, Settings.CellSize);
        }

        /// <summary>
        /// Gets the sprite texture for an event
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="e">The event</param>
        /// <param name="s">The image descriptor to use</param>
        /// <param name="pal">The sprite palette</param>
        /// <returns>The texture</returns>
        public virtual Texture2D GetSpriteTexture(Context context, GBA_R1_EventGraphicsData e, Common_ImageDescriptor s, ARGB1555Color[] pal)
        {
            if (s.Index == 0 || s.InnerWidth == 0 || s.InnerHeight == 0)
                return null;

            // Create the texture
            Texture2D tex = new Texture2D(s.OuterWidth, s.OuterHeight, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            tex.SetPixels(new Color[tex.width * tex.height]);

            var offset = s.ImageBufferOffset;
            var curOff = (int)offset;
            if (UsesPaletteIndex) {
                if (curOff % 4 != 0)
                    curOff += 4 - (curOff % 4);
            }

            int block_size = Is4Bit ? 0x20 : 0x40;

            //Controller.print((e.ImageBufferPointer + offset) + " - " + offset);

            while (e.ImageBuffer[curOff] != 0xFF)
            {
                if (e.ImageBuffer[curOff] == 0xFE) {
                    curOff++;
                    continue;
                }
                var structure = e.ImageBuffer[curOff];
                var blockX = e.ImageBuffer[curOff + 1];
                var blockY = e.ImageBuffer[curOff + 2];
                var paletteInd = UsesPaletteIndex ? e.ImageBuffer[curOff + 3] : 0;
                bool doubleScale = (structure & 0x10) != 0;
                curOff += UsesPaletteIndex ? 4 : 3;
                switch (structure & 0xF)
                {
                    case 11:
                        for (int y = 0; y < 8; y++)
                        {
                            for (int x = 0; x < 4; x++)
                            {
                                FillSpriteTextureBlock(tex, blockX, blockY, x, y, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                                curOff += block_size;
                            }
                        }
                        break;
                    case 10:
                        for (int y = 0; y < 4; y++)
                        {
                            for (int x = 0; x < 2; x++)
                            {
                                FillSpriteTextureBlock(tex, blockX, blockY, x, y, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                                curOff += block_size;
                            }
                        }
                        break;
                    case 9:
                        for (int y = 0; y < 4; y++)
                        {
                            FillSpriteTextureBlock(tex, blockX, blockY, 0, y, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                            curOff += block_size;
                        }
                        break;
                    case 8:
                        // 2 blocks vertically
                        for (int y = 0; y < 2; y++)
                        {
                            FillSpriteTextureBlock(tex, blockX, blockY, 0, y, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                            curOff += block_size;
                        }
                        break;
                    case 7:
                        for (int y = 0; y < 4; y++)
                        {
                            for (int x = 0; x < 8; x++)
                            {
                                FillSpriteTextureBlock(tex, blockX, blockY, x, y, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                                curOff += block_size;
                            }
                        }
                        break;
                    case 6:
                        for (int y = 0; y < 2; y++)
                        {
                            for (int x = 0; x < 4; x++)
                            {
                                FillSpriteTextureBlock(tex, blockX, blockY, x, y, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                                curOff += block_size;
                            }
                        }
                        break;
                    case 5:
                        for (int x = 0; x < 4; x++)
                        {
                            FillSpriteTextureBlock(tex, blockX, blockY, x, 0, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                            curOff += block_size;
                        }
                        //curOff += block_size * 4;
                        break;
                    case 4:
                        // 2 blocks horizontally
                        for (int x = 0; x < 2; x++)
                        {
                            FillSpriteTextureBlock(tex, blockX, blockY, x, 0, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                            curOff += block_size;
                        }
                        break;
                    case 3:
                        for (int y = 0; y < 8; y++) {
                            for (int x = 0; x < 8; x++) {
                                FillSpriteTextureBlock(tex, blockX, blockY, x, y, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                                curOff += block_size;
                            }
                        }
                        break;
                    case 2:
                        for (int y = 0; y < 4; y++)
                        {
                            for (int x = 0; x < 4; x++)
                            {
                                FillSpriteTextureBlock(tex, blockX, blockY, x, y, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                                curOff += block_size;
                            }
                        }
                        break;
                    case 1:
                        // 4 blocks
                        for (int y = 0; y < 2; y++)
                        {
                            for (int x = 0; x < 2; x++)
                            {
                                FillSpriteTextureBlock(tex, blockX, blockY, x, y, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                                curOff += block_size;
                            }
                        }
                        break;
                    case 0:
                        // 1 block
                        FillSpriteTextureBlock(tex, blockX, blockY, 0, 0, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                        curOff += block_size;
                        break;
                    default:
                        Controller.print("Didn't recognize command " + structure + " - " + e.ImageBufferPointer + " - " + curOff + (e.ImageBufferPointer + offset));
                        break;
                }
            }

            tex.Apply();

            return tex;
        }

        public Texture2D GetVignetteTexture(GBA_R1_BaseVignette vig)
        {
            // Create the texture
            var tex = new Texture2D(vig.Width * 8, vig.Height * 8)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            for (int y = 0; y < vig.Height; y++)
            {
                for (int x = 0; x < vig.Width; x++)
                {
                    var index = y * vig.Width + x;
                    var blockIndex = vig.BlockIndices[index];

                    var curOff = (Is4Bit ? 0x20 : 0x40) * blockIndex;

                    FillSpriteTextureBlock(tex, x * 8, y * 8, 0, 0, vig.ImageData, curOff, vig.Palettes, vig.PaletteIndices?[index] ?? 0, false);
                }
            }

            tex.Apply();

            return tex;
        }
        public Texture2D GetVignetteTexture(GBA_R1_IntroVignette vig)
        {
            // Create the texture
            var tex = new Texture2D(vig.Width * 8, vig.Height * 8)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            for (int y = 0; y < vig.Height; y++)
            {
                for (int x = 0; x < vig.Width; x++)
                {
                    var index = y * vig.Width + x;
                    var imgValue = vig.ImageValues[index];

                    var blockIndex = BitHelpers.ExtractBits(imgValue, 12, 0);
                    var palIndex = BitHelpers.ExtractBits(imgValue, 4, 12);

                    var curOff = (Is4Bit ? 0x20 : 0x40) * blockIndex;

                    FillSpriteTextureBlock(tex, x * 8, y * 8, 0, 0, vig.ImageData, curOff, vig.Palettes, palIndex, false);
                }
            }

            tex.Apply();

            return tex;
        }

        public void FillSpriteTextureBlock(Texture2D tex,
            int blockX, int blockY,
            int relX, int relY,
            byte[] imageBuffer, int imageBufferOffset,
            IList<ARGB1555Color> pal, int paletteInd, bool doubleScale, bool reverseHeight = true) {
            for (int y = 0; y < 8; y++) {
                for (int x = 0; x < 8; x++) {
                    int actualX = blockX + (doubleScale ? relX * 2 : relX) * 8 + (doubleScale ? x * 2 : x);
                    int actualY = blockY + (doubleScale ? relY * 2 : relY) * 8 + (doubleScale ? y * 2 : y);
                    
                    if (actualX >= tex.width || actualY >= tex.height)
                        continue;
                    
                    int off = y * 8 + x;

                    int b = 0;
                    Color c;
                    if (Is4Bit) {
                        if (imageBufferOffset + off / 2 > imageBuffer.Length)
                            continue;
                        b = imageBuffer[imageBufferOffset + off / 2];

                        b = BitHelpers.ExtractBits(b, 4, off % 2 == 0 ? 0 : 4);
                        c = pal[(paletteInd * 0x10 + b) % pal.Count].GetColor();

                        if (b != 0)
                            c = new Color(c.r, c.g, c.b, 1f);
                    } else {
                        if (imageBufferOffset + off > imageBuffer.Length)
                            continue;
                        b = imageBuffer[imageBufferOffset + off];
                        c = pal[(paletteInd * 0x100 + b) % pal.Count].GetColor();
                        if (b != 0)
                            c = new Color(c.r, c.g, c.b, 1f);
                        else
                            c = new Color(0, 0, 0, 0f);
                    }
                        
                    if (reverseHeight)
                    {
                        tex.SetPixel(actualX, tex.height - 1 - actualY, c);
                        if (doubleScale)
                        {
                            tex.SetPixel(actualX, tex.height - 1 - actualY - 1, c);
                            tex.SetPixel(actualX + 1, tex.height - 1 - actualY, c);
                            tex.SetPixel(actualX + 1, tex.height - 1 - actualY - 1, c);
                        }
                    }
                    else
                    {
                        tex.SetPixel(actualX, actualY, c);
                        if (doubleScale)
                        {
                            tex.SetPixel(actualX, actualY + 1, c);
                            tex.SetPixel(actualX + 1, actualY, c);
                            tex.SetPixel(actualX + 1, actualY + 1, c);
                        }
                    }
                }
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
            Controller.status = $"Loading ROM";
            await Controller.WaitIfNecessary();

            // Read data from the ROM
            var rom = FileFactory.Read<GBA_R1_ROM>(GetROMFilePath, context);

            return await LoadAsync(context, loadTextures, rom.LevelMapData, rom.LevelEventData, rom.SpritePalettes);
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <param name="map">The level map data</param>
        /// <param name="eventData">The level event data</param>
        /// <param name="spritePalette">The sprite palette</param>
        /// <returns>The editor manager</returns>
        public virtual async Task<BaseEditorManager> LoadAsync(Context context, bool loadTextures, GBA_R1_LevelMapData map, GBA_R1_LevelEventData eventData, ARGB1555Color[] spritePalette)
        {
            Controller.status = $"Loading level data";
            await Controller.WaitIfNecessary();

            map.SerializeLevelData(context.Deserializer);

            Controller.status = $"Loading tile set";
            await Controller.WaitIfNecessary();

            Common_Tileset tileset = GetTileSet(context, map);

            // Convert levelData to common level format
            Common_Lev commonLev = new Common_Lev 
            {
                // Create the map
                Maps = new Common_LevelMap[]
                {
                    new Common_LevelMap()
                    {
                        // Set the dimensions
                        Width = map.MapData.Width,
                        Height = map.MapData.Height,

                        // Create the tile arrays
                        TileSet = new Common_Tileset[3],
                        Tiles = new Common_Tile[map.MapData.Width * map.MapData.Height]
                    }
                },

                // Create the events list
                EventData = new List<Common_EventData>(),
            };

            commonLev.Maps[0].TileSet[0] = tileset;

            Controller.status = $"Loading events";
            await Controller.WaitIfNecessary();

            var eventDesigns = new Dictionary<Pointer, Common_Design>();
            var eventETA = new Dictionary<Pointer, Common_EventState[][]>();

            // Create a linking table
            var linkTable = new ushort[eventData.EventData.Select(x => x.Length).Sum()];

            // Handle each event link group
            foreach (var linkedEvents in eventData.EventData.SelectMany(x => x).Select((x, i) => new
            {
                Index = i,
                Data = x,
                LinkID = x.LinkGroup == 0xFFFF ? -1 : x.LinkGroup
            }).GroupBy(x => x.LinkID))
            {
                // Get the group
                var group = linkedEvents.ToArray();

                // Handle every event
                for (int i = 0; i < group.Length; i++)
                {
                    // Get the item
                    var item = group[i];

                    if (item.Data.LinkGroup == 0xFFFF)
                        linkTable[item.Index] = (ushort)item.Index;
                    else if (group.Length == i + 1)
                        linkTable[item.Index] = (ushort)group[0].Index;
                    else
                        linkTable[item.Index] = (ushort)group[i + 1].Index;
                }
            }

            var index = 0;

            // Load the events
            for (int i = 0; i < eventData.GraphicsGroupCount; i++)
            {
                var graphics = eventData.GraphicData[i];

                // Add if not found
                if (graphics.ImageDescriptorsPointer != null && !eventDesigns.ContainsKey(graphics.ImageDescriptorsPointer))
                {
                    Common_Design finalDesign = new Common_Design
                    {
                        Sprites = new List<Sprite>(),
                        Animations = new List<Common_Animation>(),
                        FilePath = graphics.ImageDescriptorsPointer.file.filePath
                    };

                    // Get every sprite
                    foreach (Common_ImageDescriptor img in graphics.ImageDescriptors)
                    {
                        // Get the texture for the sprite, or null if not loading textures
                        Texture2D tex = loadTextures ? GetSpriteTexture(context, graphics, img, spritePalette) : null;

                        // Add it to the array
                        finalDesign.Sprites.Add(tex == null ? null : Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), 16, 20));
                    }

                    if (graphics.AnimDescriptors != null)
                        // Add animations
                        finalDesign.Animations.AddRange(graphics.AnimDescriptors.Select(x => x.ToCommonAnimation()));

                    // Add to the designs
                    eventDesigns.Add(graphics.ImageDescriptorsPointer, finalDesign);
                }

                for (int j = 0; j < eventData.EventData[i].Length; j++)
                {
                    var dat = eventData.EventData[i][j];

                    // Add if not found
                    if (dat.ETAPointer != null && !eventETA.ContainsKey(dat.ETAPointer))
                    {
                        // Add to the ETA
                        eventETA.Add(dat.ETAPointer, dat.ETA);
                    }
                    else if (dat.ETAPointer != null)
                    {
                        // Temporary solution - combine ETA
                        var current = eventETA[dat.ETAPointer];

                        if (dat.ETA.Length > current.Length)
                            Array.Resize(ref current, dat.ETA.Length);

                        for (int ii = 0; ii < dat.ETA.Length; ii++)
                        {
                            if (current[ii] == null)
                                current[ii] = new Common_EventState[dat.ETA[ii]?.Length ?? 0];

                            if ((dat.ETA[ii]?.Length ?? 0) > current[ii].Length)
                                Array.Resize(ref current[ii], dat.ETA[ii].Length);

                            for (int jj = 0; jj < (dat.ETA[ii]?.Length ?? 0); jj++)
                                current[ii][jj] = dat.ETA[ii][jj];
                        }
                    }

                    // Add the event
                    commonLev.EventData.Add(new Common_EventData
                    {
                        Type = dat.Type,
                        Etat = dat.Etat,
                        SubEtat = dat.SubEtat,
                        XPosition = dat.XPosition,
                        YPosition = dat.YPosition,
                        DESKey = graphics.ImageDescriptorsPointer?.ToString() ?? String.Empty,
                        ETAKey = dat.ETAPointer?.ToString() ?? String.Empty,
                        OffsetBX = dat.OffsetBX,
                        OffsetBY = dat.OffsetBY,
                        OffsetHY = dat.OffsetHY,
                        FollowSprite = dat.FollowSprite,
                        HitPoints = dat.HitPoints,
                        Layer = dat.Layer,
                        HitSprite = dat.HitSprite,
                        FollowEnabled = dat.FollowEnabled,
                        CommandCollection = dat.Commands,
                        LinkIndex = linkTable[index],
                    });

                    index++;
                }
            }

            Controller.status = $"Loading map";
            await Controller.WaitIfNecessary();

            // Enumerate each cell
            for (int cellY = 0; cellY < map.MapData.Height; cellY++) 
            {
                for (int cellX = 0; cellX < map.MapData.Width; cellX++) 
                {
                    // Get the cell
                    var cell = map.MapData.Tiles[cellY * map.MapData.Width + cellX];

                    // Set the common tile
                    commonLev.Maps[0].Tiles[cellY * map.MapData.Width + cellX] = new Common_Tile() 
                    {
                        TileSetGraphicIndex = cell.TileIndex,
                        CollisionType = cell.CollisionType,
                        PaletteIndex = 1,
                        XPosition = cellX,
                        YPosition = cellY
                    };
                }
            }

            return new PS1EditorManager(commonLev, context, eventDesigns, eventETA);
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
        public virtual async Task LoadFilesAsync(Context context)
        {
            await LoadExtraFile(context, GetROMFilePath, 0x08000000);
        }

        public virtual async Task<GBAMemoryMappedFile> LoadExtraFile(Context context, string path, uint baseAddress)
        {
            await FileSystem.PrepareFile(context.BasePath + path);

            var file = new GBAMemoryMappedFile(context, baseAddress)
            {
                filePath = path,
            };
            context.AddFile(file);

            return file;
        }

        #endregion
    }
}