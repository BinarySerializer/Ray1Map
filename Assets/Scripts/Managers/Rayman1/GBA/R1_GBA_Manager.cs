using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Base game manager for GBA
    /// </summary>
    public class R1_GBA_Manager : IGameManager {
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
        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(GetLevelCounts.Select(x => new GameInfo_World((int)x.Key, Enumerable.Range(0, x.Value).ToArray())).ToArray());

        public virtual KeyValuePair<R1_World, int>[] GetLevelCounts => new KeyValuePair<R1_World, int>[]
        {
            new KeyValuePair<R1_World, int>(R1_World.Jungle, 22), 
            new KeyValuePair<R1_World, int>(R1_World.Music, 18), 
            new KeyValuePair<R1_World, int>(R1_World.Mountain, 13), 
            new KeyValuePair<R1_World, int>(R1_World.Image, 13), 
            new KeyValuePair<R1_World, int>(R1_World.Cave, 12), 
            new KeyValuePair<R1_World, int>(R1_World.Cake, 4), 
            new KeyValuePair<R1_World, int>(R1_World.Multiplayer, 6),
        };

        /// <summary>
        /// Gets the file path to the ROM file
        /// </summary>
        public virtual string GetROMFilePath => $"ROM.gba";

        /// <summary>
        /// Gets the base address for the ROM file
        /// </summary>
        protected virtual uint GetROMBaseAddress => 0x08000000;

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
        /// Loads the game data
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The game data</returns>
        public virtual IR1_GBAData LoadData(Context context) => FileFactory.Read<R1_GBA_ROM>(GetROMFilePath, context);

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
                new GameAction("Export Palettes", false, true, (input, output) => ExportPaletteImage(settings, output)),
            };
        }

        /// <summary>
        /// Exports every sprite from the game
        /// </summary>
        /// <param name="baseGameSettings">The game settings</param>
        /// <param name="outputDir">The output directory</param>
        /// <returns>The task</returns>
        public virtual async UniTask ExportAllSpritesAsync(GameSettings baseGameSettings, string outputDir)
        {
            // Lazy check if we're on GBA or DSi
            var isGBA = baseGameSettings.EngineVersion == EngineVersion.R1_GBA;

            // Create the context
            using (var context = new Context(baseGameSettings))
            {
                // Load the game data
                await LoadFilesAsync(context);

                // Serialize the rom
                var data = LoadData(context);

                // Get a pointer tables
                var gbaPointerTable = isGBA ? PointerTables.R1_GBA_PointerTable(baseGameSettings.GameModeSelection, ((R1Serializable)data).Offset.file) : null;
                var dsiPointerTable = !isGBA ? PointerTables.R1_DSi_PointerTable(baseGameSettings.GameModeSelection, ((R1Serializable)data).Offset.file) : null;

                var graphics = new Dictionary<Pointer, List<KeyValuePair<R1_World, ARGB1555Color[]>>>();

                // Enumerate every world
                foreach (var world in GetLevels(baseGameSettings).First().Worlds)
                {
                    baseGameSettings.World = world.Index;

                    // Enumerate every level
                    foreach (var lvl in world.Maps)
                    {
                        baseGameSettings.Level = lvl;

                        // Serialize the event data
                        var eventData = new R1_GBA_LevelEventData();

                        if (isGBA)
                            eventData.SerializeData(context.Deserializer, gbaPointerTable[R1_GBA_ROMPointer.EventGraphicsPointers], gbaPointerTable[R1_GBA_ROMPointer.EventDataPointers], gbaPointerTable[R1_GBA_ROMPointer.EventGraphicsGroupCountTablePointers], gbaPointerTable[R1_GBA_ROMPointer.LevelEventGraphicsGroupCounts]);
                        else
                            eventData.SerializeData(context.Deserializer, dsiPointerTable[R1_DSi_Pointer.EventGraphicsPointers], dsiPointerTable[R1_DSi_Pointer.EventDataPointers], dsiPointerTable[R1_DSi_Pointer.EventGraphicsGroupCountTablePointers], dsiPointerTable[R1_DSi_Pointer.LevelEventGraphicsGroupCounts]);

                        // Get the event graphics
                        for (var i = 0; i < eventData.GraphicData.Length; i++)
                        {
                            var key = eventData.GraphicDataPointers[i];

                            if (!graphics.ContainsKey(key))
                                graphics.Add(key, new List<KeyValuePair<R1_World, ARGB1555Color[]>>());

                            if (graphics[key].All(x => x.Key != (R1_World)world.Index))
                                graphics[key].Add(new KeyValuePair<R1_World, ARGB1555Color[]>((R1_World)world.Index, data.GetSpritePalettes(baseGameSettings)));
                        }
                    }
                }

                // Add unused graphics
                if (isGBA)
                {
                    graphics.Add(gbaPointerTable[R1_GBA_ROMPointer.DrumWalkerGraphics], new List<KeyValuePair<R1_World, ARGB1555Color[]>>());
                    graphics.Add(gbaPointerTable[R1_GBA_ROMPointer.ClockGraphics], new List<KeyValuePair<R1_World, ARGB1555Color[]>>());
                    graphics.Add(gbaPointerTable[R1_GBA_ROMPointer.InkGraphics], new List<KeyValuePair<R1_World, ARGB1555Color[]>>());
                    graphics.Add(gbaPointerTable[R1_GBA_ROMPointer.FontSmallGraphics], new List<KeyValuePair<R1_World, ARGB1555Color[]>>());
                    graphics.Add(gbaPointerTable[R1_GBA_ROMPointer.FontLargeGraphics], new List<KeyValuePair<R1_World, ARGB1555Color[]>>());
                    graphics.Add(gbaPointerTable[R1_GBA_ROMPointer.PinsGraphics], new List<KeyValuePair<R1_World, ARGB1555Color[]>>());
                }
                else
                {
                    // TODO: Where is the font?
                    graphics.Add(dsiPointerTable[R1_DSi_Pointer.ClockGraphics], new List<KeyValuePair<R1_World, ARGB1555Color[]>>());
                }

                var desIndex = 0;

                // Enumerate every graphics
                foreach (var gp in graphics)
                {
                    var imgIndex = 0;

                    // Get the graphic data
                    var g = FileFactory.Read<R1_GBA_EventGraphicsData>(gp.Key, context);

                    // Get the world name
                    var worldName = gp.Value.Count > 1 ? "Allfix" : (!gp.Value.Any() ? "Other" : gp.Value.First().Key.ToString());

                    // Enumerate every image descriptor
                    foreach (var img in g.ImageDescriptors)
                    {
                        // Get the texture
                        var tex = GetSpriteTexture(context, g, img, gp.Value?.FirstOrDefault().Value ?? data.GetSpritePalettes(baseGameSettings));

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
        public virtual async UniTask ExportUnusedSpritesAsync(GameSettings baseGameSettings, string outputDir)
        {
            // Lazy check if we're on GBA or DSi
            var isGBA = baseGameSettings.EngineVersion == EngineVersion.R1_GBA;

            // Create the context
            using (var context = new Context(baseGameSettings))
            {
                // Load the rom
                await LoadFilesAsync(context);

                // Load the data
                var data = LoadData(context);

                // Get used graphics
                var graphics = new List<Pointer>();

                // Get a pointer tables
                var gbaPointerTable = isGBA ? PointerTables.R1_GBA_PointerTable(baseGameSettings.GameModeSelection, ((R1Serializable)data).Offset.file) : null;
                var dsiPointerTable = !isGBA ? PointerTables.R1_DSi_PointerTable(baseGameSettings.GameModeSelection, ((R1Serializable)data).Offset.file) : null;

                // Enumerate every world
                foreach (var world in GetLevels(baseGameSettings).First().Worlds)
                {
                    baseGameSettings.World = world.Index;

                    // Enumerate every level
                    foreach (var lvl in world.Maps)
                    {
                        baseGameSettings.Level = lvl;

                        // Serialize the event data
                        var eventData = new R1_GBA_LevelEventData();

                        if (isGBA)
                            eventData.SerializeData(context.Deserializer, gbaPointerTable[R1_GBA_ROMPointer.EventGraphicsPointers], gbaPointerTable[R1_GBA_ROMPointer.EventDataPointers], gbaPointerTable[R1_GBA_ROMPointer.EventGraphicsGroupCountTablePointers], gbaPointerTable[R1_GBA_ROMPointer.LevelEventGraphicsGroupCounts]);
                        else
                            eventData.SerializeData(context.Deserializer, dsiPointerTable[R1_DSi_Pointer.EventGraphicsPointers], dsiPointerTable[R1_DSi_Pointer.EventDataPointers], dsiPointerTable[R1_DSi_Pointer.EventGraphicsGroupCountTablePointers], dsiPointerTable[R1_DSi_Pointer.LevelEventGraphicsGroupCounts]);

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
                    s.Goto(((R1Serializable)data).Offset + i);

                    File.WriteAllText(Path.Combine(outputDir, "log.txt"), $"{s.CurrentPointer.FileOffset} / {s.CurrentLength}");

                    try
                    {
                        // Make sure the graphic isn't referenced already
                        if (graphics.Any(x => x == s.CurrentPointer))
                            continue;

                        // Serialize it
                        var g = s.SerializeObject<R1_GBA_EventGraphicsData>(default);

                        var imgIndex = 0;

                        // Enumerate every image descriptor
                        foreach (var img in g.ImageDescriptors)
                        {
                            // Get the texture
                            var tex = GetSpriteTexture(context, g, img, data.GetSpritePalettes(baseGameSettings));

                            // Make sure it's not null
                            if (tex == null)
                            {
                                imgIndex++;
                                continue;
                            }

                            Util.ByteArrayToFile(Path.Combine(outputDir, $"{(((R1Serializable)data).Offset + i).FileOffset} - {imgIndex}.png"), tex.EncodeToPNG());

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

        public virtual async UniTask ExtractVignetteAsync(GameSettings settings, string outputDir)
        {
            // Create a context
            using (var context = new Context(settings))
            {
                // Load the ROM
                await LoadFilesAsync(context);

                // Load the game data
                var data = LoadData(context);

                // Extract background vignettes
                if (data.BackgroundVignettes != null)
                {
                    // Extract every background vignette
                    for (int i = 0; i < data.BackgroundVignettes.Length; i++)
                    {
                        // Get the vignette
                        var vig = data.BackgroundVignettes[i];

                        // Make sure we have image data
                        if (vig.ImageData == null)
                            continue;

                        // Get the texture
                        var tex = GetVignetteTexture(vig);

                        // Save the texture
                        Util.ByteArrayToFile(Path.Combine(outputDir, $"BG_{i}.png"), tex.EncodeToPNG());
                    }
                }

                // Extract intro vignettes
                if (data.IntroVignettes != null)
                {
                    // Extract every intro vignette
                    for (int i = 0; i < data.IntroVignettes.Length; i++)
                    {
                        // Get the vignette
                        var vig = data.IntroVignettes[i];

                        // Get the texture
                        var tex = GetVignetteTexture(vig);

                        // Save the texture
                        Util.ByteArrayToFile(Path.Combine(outputDir, $"Intro_{i}.png"), tex.EncodeToPNG());
                    }
                }

                // Extract world map vignette
                if (data.WorldMapVignette != null)
                {
                    // Get the world map texture
                    var worldMapTex = GetVignetteTexture(data.WorldMapVignette);

                    // Save the texture
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"WorldMap.png"), worldMapTex.EncodeToPNG());
                }
            }
        }

        public async UniTask ExportPaletteImage(GameSettings settings, string outputPath)
        {
            var spritePals = new List<ARGB1555Color[]>();
            var tilePals = new List<ARGB1555Color[]>();

            // Enumerate every world
            foreach (var world in GetLevels(settings).First().Worlds)
            {
                settings.World = world.Index;

                // Enumerate every level
                foreach (var lvl in world.Maps)
                {
                    settings.Level = lvl;

                    using (var context = new Context(settings))
                    {
                        // Load the game data
                        await LoadFilesAsync(context);

                        // Serialize the rom
                        var data = LoadData(context);
                        data.LevelMapData.SerializeLevelData(context.Deserializer);

                        // Add the tile palette
                        if (data.LevelMapData.TilePalettes != null && !tilePals.Any(x => x.SequenceEqual(data.LevelMapData.TilePalettes)))
                            tilePals.Add(data.LevelMapData.TilePalettes);

                        // Add the sprite palette
                        var spritePal = data.GetSpritePalettes(settings);

                        if (spritePal != null && !spritePals.Any(x => x.SequenceEqual(spritePal)))
                            spritePals.Add(spritePal);
                    }
                }
            }

            foreach (ARGB1555Color c in spritePals.Concat(tilePals).SelectMany(p => p))
                c.Alpha = Byte.MaxValue;

            // Export
            PaletteHelpers.ExportPalette(Path.Combine(outputPath, $"{settings.GameModeSelection}.png"), spritePals.Concat(tilePals).SelectMany(x => x).ToArray(), optionalWrap: settings.EngineVersion == EngineVersion.R1_GBA ? 16 : 256);
        }


        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="levelMapData">The level to get the tile set for</param>
        /// <returns>The tile set to use</returns>
        public virtual Unity_MapTileMap GetTileSet(Context context, R1_GBA_LevelMapData levelMapData) {
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

                return new Unity_MapTileMap(dummy, Settings.CellSize);
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

            return new Unity_MapTileMap(tex, Settings.CellSize);
        }

        /// <summary>
        /// Gets the sprite texture for an event
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="e">The event</param>
        /// <param name="s">The image descriptor to use</param>
        /// <param name="pal">The sprite palette</param>
        /// <returns>The texture</returns>
        public virtual Texture2D GetSpriteTexture(Context context, R1_GBA_EventGraphicsData e, R1_ImageDescriptor s, ARGB1555Color[] pal)
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

        public Texture2D GetVignetteTexture(R1_GBA_BaseVignette vig)
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
        public Texture2D GetVignetteTexture(R1_GBA_IntroVignette vig)
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
        public virtual async UniTask<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            Controller.status = $"Loading data";
            await Controller.WaitIfNecessary();

            // Read data from the ROM
            var data = LoadData(context);

            // Get properties
            var map = data.LevelMapData;
            var eventData = data.LevelEventData;
            var spritePalette = data.GetSpritePalettes(context.Settings);

            Controller.status = $"Loading level data";
            await Controller.WaitIfNecessary();

            map.SerializeLevelData(context.Deserializer);

            Controller.status = $"Loading tile set";
            await Controller.WaitIfNecessary();

            Unity_MapTileMap tileset = GetTileSet(context, map);

            // Convert levelData to common level format
            Unity_Level level = new Unity_Level 
            {
                // Create the map
                Maps = new Unity_Map[]
                {
                    new Unity_Map()
                    {
                        // Set the dimensions
                        Width = map.MapData.Width,
                        Height = map.MapData.Height,

                        // Create the tile arrays
                        TileSet = new Unity_MapTileMap[1],
                        MapTiles = map.MapData.Tiles.Select(x => new Unity_Tile(x)).ToArray(),
                        TileSetWidth = 1
                    }
                },

                // Create the events list
                EventData = new List<Unity_Obj>(),
            };

            level.Maps[0].TileSet[0] = tileset;

            Controller.status = $"Loading events";
            await Controller.WaitIfNecessary();

            var eventDesigns = new Dictionary<Pointer, Unity_ObjGraphics>();
            var eventETA = new Dictionary<Pointer, R1_EventState[][]>();

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
                    Unity_ObjGraphics finalDesign = new Unity_ObjGraphics
                    {
                        Sprites = new List<Sprite>(),
                        Animations = new List<Unity_ObjAnimation>(),
                        FilePath = graphics.ImageDescriptorsPointer.file.filePath
                    };

                    // Get every sprite
                    foreach (R1_ImageDescriptor img in graphics.ImageDescriptors)
                    {
                        // Get the texture for the sprite, or null if not loading textures
                        Texture2D tex = loadTextures ? GetSpriteTexture(context, graphics, img, spritePalette) : null;

                        // Add it to the array
                        finalDesign.Sprites.Add(tex == null ? null : Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), Settings.PixelsPerUnit, 20));
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
                    else if (dat.ETAPointer != null && context.Settings.EngineVersion == EngineVersion.R1_DSi)
                    {
                        // Temporary solution - combine ETA
                        var current = eventETA[dat.ETAPointer];

                        if (dat.ETA.Length > current.Length)
                            Array.Resize(ref current, dat.ETA.Length);

                        for (int ii = 0; ii < dat.ETA.Length; ii++)
                        {
                            if (current[ii] == null)
                                current[ii] = new R1_EventState[dat.ETA[ii]?.Length ?? 0];

                            if ((dat.ETA[ii]?.Length ?? 0) > current[ii].Length)
                                Array.Resize(ref current[ii], dat.ETA[ii].Length);

                            for (int jj = 0; jj < (dat.ETA[ii]?.Length ?? 0); jj++)
                                current[ii][jj] = dat.ETA[ii][jj];
                        }

                        eventETA[dat.ETAPointer] = current;
                    }

                    var editorEventData = new Unity_Obj(new R1_EventData()
                    {
                        Type = dat.Type,
                        Etat = dat.Etat,
                        SubEtat = dat.SubEtat,
                        XPosition = dat.XPosition,
                        YPosition = dat.YPosition,
                        OffsetBX = dat.OffsetBX,
                        OffsetBY = dat.OffsetBY,
                        OffsetHY = dat.OffsetHY,
                        FollowSprite = dat.FollowSprite,
                        ActualHitPoints = dat.HitPoints,
                        Layer = (byte)dat.Layer,
                        HitSprite = dat.HitSprite,
                    })
                    {
                        Type = dat.Type,
                        DESKey = graphics.ImageDescriptorsPointer?.ToString() ?? String.Empty,
                        ETAKey = dat.ETAPointer?.ToString() ?? String.Empty,
                        CommandCollection = dat.Commands,
                        LinkIndex = linkTable[index],
                    };

                    editorEventData.Data.SetFollowEnabled(context.Settings, dat.FollowEnabled);

                    // Add the event
                    level.EventData.Add(editorEventData);

                    index++;
                }
            }

            if (context.Settings.EngineVersion == EngineVersion.R1_GBA)
            {
                // Add the localization data
                level.Localization = new Dictionary<string, string[]>()
                {
                    ["English"] = data.Strings[0],
                    ["French"] = data.Strings[1],
                    ["German"] = data.Strings[2],
                    ["Spanish"] = data.Strings[3],
                    ["Italian"] = data.Strings[4],
                };
            }
            else
            {
                // Add the localization data
                level.Localization = new Dictionary<string, string[]>()
                {
                    ["English"] = data.Strings[1],
                    ["French"] = data.Strings[2],
                    ["German"] = data.Strings[4],
                    ["Spanish"] = data.Strings[0],
                    ["Italian"] = data.Strings[3],
                };
            }

            return new R1_PS1_EditorManager(level, context, eventDesigns, eventETA, null);
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
        public async UniTask LoadFilesAsync(Context context) => await LoadExtraFile(context, GetROMFilePath, GetROMBaseAddress);

        public virtual async UniTask<GBAMemoryMappedFile> LoadExtraFile(Context context, string path, uint baseAddress)
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