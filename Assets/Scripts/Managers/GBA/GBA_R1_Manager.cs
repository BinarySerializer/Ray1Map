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
    /// Base game manager for PC
    /// </summary>
    public class GBA_R1_Manager : IGameManager {
        #region Values and paths

        /// <summary>
        /// The size of one cell
        /// </summary>
        public const int CellSize = 16;

        /// <summary>
        /// Indicates if the game has 3 palettes it swaps between
        /// </summary>
        public bool Has3Palettes => false;

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => GetGBALevels.GroupBy(x => x).Select(x => new KeyValuePair<World, int[]>(x.Key, Enumerable.Range(1, x.Count()).ToArray())).ToArray();

        /// <summary>
        /// Gets the available educational volumes
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The available educational volumes</returns>
        public string[] GetEduVolumes(GameSettings settings) => new string[0];

        /// <summary>
        /// Gets the available levels ordered based on the global level array
        /// </summary>
        public World[] GetGBALevels => new World[]
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

        public virtual string GetROMFilePath => $"ROM.gba";

        /// <summary>
        /// Gets the global level index from the world and level
        /// </summary>
        /// <param name="world">The world</param>
        /// <param name="lvl">The level</param>
        /// <returns>The global index</returns>
        public int GetGlobalLevelIndex(World world, int lvl)
        {
            var lvls = GetGBALevels;
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
                new GameAction("Export Vignette", false, true, async (input, output) => await ExtractVignetteAsync(settings, output)),
            };
        }

        public async Task ExtractVignetteAsync(GameSettings settings, string outputDir)
        {
            // Create a context
            using (var context = new Context(settings))
            {
                // Load the ROM
                await LoadFilesAsync(context);

                // Read data from the ROM
                var rom = FileFactory.Read<GBA_R1_ROM>(GetROMFilePath, context);

                // Extract every vignette
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
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"Vignette_{i}.png"), tex.EncodeToPNG());
                }
            }
        }

        /// <summary>
        /// Auto applies the palette to the tiles in the level
        /// </summary>
        /// <param name="level">The level to auto-apply the palette to</param>
        public void AutoApplyPalette(Common_Lev level) {}


        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="level">The level to get the tile set for</param>
        /// <returns>The tile set to use</returns>
        public Common_Tileset GetTileSet(Context context, GBA_R1_Level level) {
            // Read the tiles
            const int block_size = 0x20;
            ushort maxBlockIndex = level.TileBlockIndices.Max();
            Array<byte> tiles = FileFactory.Read<Array<byte>>(level.TilesPointer, context, (s, a) => a.Length = 0x20 * ((uint)maxBlockIndex + 1));

            uint length = (uint)level.TileBlockIndices.Length * 8 * 8;

            // Get the tile-set texture
            var tex = new Texture2D(256, Mathf.CeilToInt(length / 256f / CellSize) * CellSize) {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            for (int i = 0; i < level.TileBlockIndices.Length; i++) {
                ushort blockIndex = level.TileBlockIndices[i];

                var x = ((i / 4) * 2) % (256/8) + ((i % 2) == 0 ? 0 : 1);
                var y = (((i / 4) * 2) / (256/8)) * 2 + ((i % 4) < 2 ? 0 : 1);

                var curOff = block_size * blockIndex;
                
                FillSpriteTextureBlock(tex, 0, 0, x, y, tiles.Value, curOff, level.TilePalettes, level.TilePaletteIndices[i], false, reverseHeight: false);
            }

            tex.Apply();

            return new Common_Tileset(tex, CellSize);
        }

        /// <summary>
        /// Gets the sprite texture for an event
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="e">The event</param>
        /// <param name="s">The image descriptor to use</param>
        /// <returns>The texture</returns>
        public virtual Texture2D GetSpriteTexture(Context context, PC_Event e, Common_ImageDescriptor s)
        {
            if (s.Index == 0)
                return null;

            // Create the texture
            Texture2D tex = new Texture2D(s.OuterWidth, s.OuterHeight, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            tex.SetPixels(new Color[tex.width * tex.height]);

            var pal = FileFactory.Read<GBA_R1_ROM>(GetROMFilePath, context).SpritePalettes;
            var offset = s.ImageBufferOffset;
            if (offset % 4 != 0) {
                offset += 4 - (offset % 4);
            }
            var curOff = (int)offset;
            int block_size = 0x20;
            while (e.ImageBuffer[curOff] != 0xFF) {
                var structure = e.ImageBuffer[curOff];
                var blockX = e.ImageBuffer[curOff + 1];
                var blockY = e.ImageBuffer[curOff + 2];
                var paletteInd = e.ImageBuffer[curOff + 3];
                bool doubleScale = (structure & 0x10) != 0;
                curOff += 4;
                switch (structure & 0xF) {
                    case 11:
                        for (int y = 0; y < 8; y++) {
                            for (int x = 0; x < 4; x++) {
                                FillSpriteTextureBlock(tex, blockX, blockY, x, y, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                                curOff += block_size;
                            }
                        }
                        break;
                    case 10:
                        for (int y = 0; y < 4; y++) {
                            for (int x = 0; x < 2; x++) {
                                FillSpriteTextureBlock(tex, blockX, blockY, x, y, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                                curOff += block_size;
                            }
                        }
                        break;
                    case 9:
                        for (int y = 0; y < 4; y++) {
                            FillSpriteTextureBlock(tex, blockX, blockY, 0, y, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                            curOff += block_size;
                        }
                        break;
                    case 8:
                        // 2 blocks vertically
                        for (int y = 0; y < 2; y++) {
                            FillSpriteTextureBlock(tex, blockX, blockY, 0, y, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                            curOff += block_size;
                        }
                        break;
                    case 7:
                        for (int y = 0; y < 4; y++) {
                            for (int x = 0; x < 8; x++) {
                                FillSpriteTextureBlock(tex, blockX, blockY, x, y, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                                curOff += block_size;
                            }
                        }
                        break;
                    case 6:
                        for (int y = 0; y < 2; y++) {
                            for (int x = 0; x < 4; x++) {
                                FillSpriteTextureBlock(tex, blockX, blockY, x, y, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                                curOff += block_size;
                            }
                        }
                        break;
                    case 5:
                        for (int x = 0; x < 4; x++) {
                            FillSpriteTextureBlock(tex, blockX, blockY, x, 0, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                            curOff += block_size;
                        }
                        //curOff += block_size * 4;
                        break;
                    case 4:
                        // 2 blocks horizontally
                        for (int x = 0; x < 2; x++) {
                            FillSpriteTextureBlock(tex, blockX, blockY, x, 0, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                            curOff += block_size;
                        }
                        break;
                    case 2:
                        for (int y = 0; y < 4; y++) {
                            for (int x = 0; x < 4; x++) {
                                FillSpriteTextureBlock(tex, blockX, blockY, x, y, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                                curOff += block_size;
                            }
                        }
                        break;
                    case 1:
                        // 4 blocks
                        for (int y = 0; y < 2; y++) {
                            for (int x = 0; x < 2; x++) {
                                FillSpriteTextureBlock(tex, blockX, blockY, x, y, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                                curOff += block_size;
                            }
                        }
                        break;
                    case 0:
                        // 1 block
                        FillSpriteTextureBlock(tex, blockX, blockY, 0,0, e.ImageBuffer, curOff, pal, paletteInd, doubleScale);
                        curOff += block_size;
                        break;
                    default:
                        Controller.print("Didn't recognize command " + structure + " - " + e.ImageBufferPointer_GBA + " - " + curOff + (e.ImageBufferPointer_GBA + offset));
                        break;
                }
            }

            tex.Apply();

            return tex;
        }

        public Texture2D GetVignetteTexture(GBA_R1_BackgroundVignette vig)
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

                    var curOff = 0x20 * blockIndex;

                    new GBA_R1_Manager().FillSpriteTextureBlock(tex, x * 8, y * 8, 0, 0, vig.ImageData, curOff, vig.Palettes, vig.PaletteIndices[index], false);
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
                    if (actualX < tex.width && actualY < tex.height) {
                        int off = y * 8 + x;
                        if (imageBufferOffset + off / 2 > imageBuffer.Length) continue;
                        int b = imageBuffer[imageBufferOffset + off / 2];
                        if (off % 2 == 0) {
                            b = BitHelpers.ExtractBits(b, 4, 0);
                        } else {
                            b = BitHelpers.ExtractBits(b, 4, 4);
                        }
                        Color c = pal[paletteInd * 0x10 + b].GetColor();
                        if (b != 0) {
                            c = new Color(c.r, c.g, c.b, 1f);
                        }
                        if (reverseHeight) {
                            tex.SetPixel(actualX, tex.height - 1 - actualY, c);
                            if (doubleScale) {
                                tex.SetPixel(actualX, tex.height - 1 - actualY - 1, c);
                                tex.SetPixel(actualX + 1, tex.height - 1 - actualY, c);
                                tex.SetPixel(actualX + 1, tex.height - 1 - actualY - 1, c);
                            }
                        } else {
                            tex.SetPixel(actualX, actualY, c);
                            if (doubleScale) {
                                tex.SetPixel(actualX, actualY + 1, c);
                                tex.SetPixel(actualX + 1, actualY, c);
                                tex.SetPixel(actualX + 1, actualY + 1, c);
                            }
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
            // Get the global level index
            int globalLevelIndex = GetGlobalLevelIndex(context.Settings.World, context.Settings.Level);

            // TODO: Parse data directly from ROM
            // Load the memory file for the current level (the WRAM section)
            var memoryFile = await LoadExtraFile(context, "Jungle2.gba", 0x02000000);

            uint eventCount = 146;

            // Read data from the ROM
            var rom = FileFactory.Read<GBA_R1_ROM>(GetROMFilePath, context);

            // Parse memory files
            PC_Event[] events = FileFactory.Read<ObjectArray<PC_Event>>(new Pointer(0x020226B0, memoryFile), context, (ss, o) => o.Length = eventCount, name: $"Events").Value;
            ushort[] linkTable = FileFactory.Read<Array<ushort>>(new Pointer(0x0202D408, memoryFile), context, (ss, o) => o.Length = eventCount, name: $"EventLinks").Value;

            // Get the current level
            var level = rom.Levels[globalLevelIndex];
            level.SerializeLevelData(context.Deserializer);

            Common_Tileset tileset = GetTileSet(context, level);

            // Convert levelData to common level format
            Common_Lev commonLev = new Common_Lev 
            {
                // Create the map
                Maps = new Common_LevelMap[]
                {
                    new Common_LevelMap()
                    {
                        // Set the dimensions
                        Width = level.MapData.Width,
                        Height = level.MapData.Height,

                        // Create the tile arrays
                        TileSet = new Common_Tileset[3],
                        Tiles = new Common_Tile[level.MapData.Width * level.MapData.Height]
                    }
                },

                // Create the events list
                EventData = new List<Common_EventData>(),
            };

            //commonLev.Maps[0].TileSet[0] = new Common_Tileset(Enumerable.Repeat(new ARGBColor(0, 0, 0, 0), 16*16).ToArray(), 1, 16);
            commonLev.Maps[0].TileSet[0] = tileset;

            var eventDesigns = new Dictionary<Pointer, Common_Design>();
            var eventETA = new Dictionary<Pointer, Common_EventState[][]>();

            var index = 0;

            // Load the events
            foreach (PC_Event e in events)
            {
                // Add if not found
                if (e.ImageDescriptorsPointer_GBA != null && !eventDesigns.ContainsKey(e.ImageDescriptorsPointer_GBA))
                {
                    Common_Design finalDesign = new Common_Design
                    {
                        Sprites = new List<Sprite>(),
                        Animations = new List<Common_Animation>(),
                        FilePath = e.ImageDescriptorsPointer_GBA.file.filePath
                    };

                    // Get every sprite
                    foreach (Common_ImageDescriptor i in e.ImageDescriptors)
                    {
                        // Get the texture for the sprite, or null if not loading textures
                        Texture2D tex = loadTextures ? GetSpriteTexture(context, e, i) : null;

                        // Add it to the array
                        finalDesign.Sprites.Add(tex == null ? null : Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), 16, 20));
                    }

                    // TODO: Clean this up - maybe inherit from the PS1 manager?
                    // Add animations
                    finalDesign.Animations.AddRange(e.AnimDescriptors.Select(x => new PS1_R1_Manager().GetCommonAnimation(x)));

                    // Add to the designs
                    eventDesigns.Add(e.ImageDescriptorsPointer_GBA, finalDesign);
                }

                // Add if not found
                if (e.ETAPointer_GBA != null && !eventETA.ContainsKey(e.ETAPointer_GBA))
                {
                    // Add to the ETA
                    eventETA.Add(e.ETAPointer_GBA, e.ETA_GBA);
                }
                else
                {
                    // Temporary solution - combine ETA
                    var current = eventETA[e.ETAPointer_GBA];

                    if (e.ETA_GBA.Length > current.Length)
                        Array.Resize(ref current, e.ETA_GBA.Length);

                    for (int i = 0; i < e.ETA_GBA.Length; i++)
                    {
                        if (current[i] == null)
                            current[i] = new Common_EventState[e.ETA_GBA[i].Length];

                        if (e.ETA_GBA[i].Length > current[i].Length)
                            Array.Resize(ref current[i], e.ETA_GBA[i].Length);

                        for (int j = 0; j < e.ETA_GBA[i].Length; j++)
                            current[i][j] = e.ETA_GBA[i][j];
                    }
                }

                // Add the event
                commonLev.EventData.Add(new Common_EventData
                {
                    Type = e.Type,
                    Etat = e.Etat,
                    SubEtat = e.SubEtat,
                    XPosition = e.XPosition,
                    YPosition = e.YPosition,
                    DESKey = e.ImageDescriptorsPointer_GBA?.ToString() ?? String.Empty,
                    ETAKey = e.ETAPointer_GBA?.ToString() ?? String.Empty,
                    OffsetBX = e.OffsetBX,
                    OffsetBY = e.OffsetBY,
                    OffsetHY = e.OffsetHY,
                    FollowSprite = e.FollowSprite,
                    HitPoints = e.HitPoints,
                    Layer = e.Layer,
                    HitSprite = e.HitSprite,
                    FollowEnabled = e.FollowEnabled,
                    CommandCollection = e.Commands_GBA,
                    LinkIndex = linkTable[index]
                });

                index++;
            }

            await Controller.WaitIfNecessary();

            // Enumerate each cell
            for (int cellY = 0; cellY < level.MapData.Height; cellY++) 
            {
                for (int cellX = 0; cellX < level.MapData.Width; cellX++) 
                {
                    // Get the cell
                    var cell = level.MapData.Tiles[cellY * level.MapData.Width + cellX];

                    // Set the common tile
                    commonLev.Maps[0].Tiles[cellY * level.MapData.Width + cellX] = new Common_Tile() 
                    {
                        // TODO: Fix once we load tile graphics
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