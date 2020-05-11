using R1Engine.Serialize;
using System;
using System.Collections.Generic;
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
        public KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => new KeyValuePair<World, int[]>[]
        {
            new KeyValuePair<World, int[]>(World.Jungle, new int[]
            {
                1
            }), 
        };

        /// <summary>
        /// Gets the available educational volumes
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The available educational volumes</returns>
        public string[] GetEduVolumes(GameSettings settings) => new string[0];

        #endregion

        #region Manager Methods

        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[0];
        
        /// <summary>
        /// Auto applies the palette to the tiles in the level
        /// </summary>
        /// <param name="level">The level to auto-apply the palette to</param>
        public void AutoApplyPalette(Common_Lev level) {}

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
            var paletteFile = "Jungle1Palettes.gba";
            ObjectArray<ARGB1555Color> pal = FileFactory.Read<ObjectArray<ARGB1555Color>>(paletteFile, context, (y, x) => x.Length = y.CurrentLength / 2);
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

        public void FillSpriteTextureBlock(Texture2D tex,
            int blockX, int blockY,
            int relX, int relY,
            byte[] imageBuffer, int imageBufferOffset,
            ObjectArray<ARGB1555Color> pal, int paletteInd, bool doubleScale) {
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
                        Color c = pal.Value[(16 + paletteInd) * 0x10 + b].GetColor();
                        if (b != 0) {
                            c = new Color(c.r, c.g, c.b, 1f);
                        }
                        tex.SetPixel(actualX, tex.height - 1 - actualY, c);
                        if (doubleScale) {
                            tex.SetPixel(actualX, tex.height - 1 - actualY - 1, c);
                            tex.SetPixel(actualX + 1, tex.height - 1 - actualY, c);
                            tex.SetPixel(actualX + 1, tex.height - 1 - actualY - 1, c);
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
            // Load the rom
            var romFile = await LoadExtraFile(context, "ROM.gba", 0x08000000);

            // TODO: Parse data directly from ROM
            // Load the memory file for the current level (the WRAM section)
            var memoryFile = await LoadExtraFile(context, "Jungle2.gba", 0x02000000);

            // Load the palette file for the current level
            var paletteFile = await LoadExtraFile(context, "Jungle1Palettes.gba", 0x05000000);

            uint eventCount = 146;
            uint levelCount = 22 + 18 + 13 + 13 + 12 + 4 + 6;

            // Read data from the ROM
            GBA_R1_Level[] levels = FileFactory.Read<ObjectArray<GBA_R1_Level>>(new Pointer(0x085485B4, romFile), context, (ss, o) => o.Length = levelCount, name: $"Levels").Value;
            GBA_R1_UnkStruct[] unkStruct = FileFactory.Read<ObjectArray<GBA_R1_UnkStruct>>(new Pointer(0x086D4D60, romFile), context, (ss, o) => o.Length = 48, name: $"UnkStruct").Value;

            // Parse memory files
            GBA_R1_Map map = FileFactory.Read<GBA_R1_Map>(new Pointer(0x02002230, memoryFile), context, name: $"Map");
            PC_Event[] events = FileFactory.Read<ObjectArray<PC_Event>>(new Pointer(0x020226B0, memoryFile), context, (ss, o) => o.Length = eventCount, name: $"Events").Value;

            // Doesn't seem correct
            ushort[] linkTable = FileFactory.Read<Array<ushort>>(new Pointer(0x0202BB00, memoryFile), context, (ss, o) => o.Length = eventCount, name: $"EventLinks").Value;

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

            // Load a dummy tile for now
            commonLev.Maps[0].TileSet[0] = new Common_Tileset(Enumerable.Repeat(new ARGBColor(0, 0, 0, 0), 16*16).ToArray(), 1, 16);

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
                        // TODO: Remove try/catch
                        try
                        {
                            // Get the texture for the sprite, or null if not loading textures
                            Texture2D tex = loadTextures ? GetSpriteTexture(context, e, i) : null;

                            // Add it to the array
                            finalDesign.Sprites.Add(tex == null ? null : Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), 16, 20));
                        }
                        catch (Exception ex)
                        {
                            finalDesign.Sprites.Add(null);
                            Debug.LogError($"{ex.Message}");
                        }
                    }

                    // TODO: Clean this up - maybe inherit from the PS1 manager?
                    // Add animations
                    finalDesign.Animations.AddRange(e.AnimDescriptors.Select(x => new PS1_R1_Manager().GetCommonAnimation(x)));

                    // Add to the designs
                    eventDesigns.Add(e.ImageDescriptorsPointer_GBA, finalDesign);
                }

                // Add if not found
                if (e.ETAPointer_GBA != null && !eventETA.ContainsKey(e.ETAPointer_GBA))
                    // Add to the ETA
                    eventETA.Add(e.ETAPointer_GBA, e.ETA_GBA);

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
            for (int cellY = 0; cellY < map.Height; cellY++) 
            {
                for (int cellX = 0; cellX < map.Width; cellX++) 
                {
                    // Get the cell
                    var cell = map.Tiles[cellY * map.Width + cellX];

                    // Set the common tile
                    commonLev.Maps[0].Tiles[cellY * map.Width + cellX] = new Common_Tile() 
                    {
                        // TODO: Fix once we load tile graphics
                        //TileSetGraphicIndex = textureIndex,
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
        public virtual Task LoadFilesAsync(Context context) => Task.CompletedTask;

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