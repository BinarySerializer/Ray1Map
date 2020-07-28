using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Asyncoroutine;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Base .xxx game manager for PS1
    /// </summary>
    public abstract class PS1_BaseXXX_Manager : PS1_Manager
    {
        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public virtual string GetLevelFilePath(GameSettings settings) => GetWorldFolderPath(settings.World) + $"{GetWorldName(settings.World)}{settings.Level:00}.XXX";

        /// <summary>
        /// Gets the file path for the allfix file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The allfix file path</returns>
        public virtual string GetAllfixFilePath(GameSettings settings) => GetDataPath() + $"RAY.XXX";

        /// <summary>
        /// Gets the file path for the big ray file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The big ray file path</returns>
        public virtual string GetBigRayFilePath(GameSettings settings) => GetDataPath() + $"INI.XXX";

        /// <summary>
        /// Gets the file path for the font file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The font file path</returns>
        public virtual string GetFontFilePath(GameSettings settings) => GetDataPath() + $"LET2.IMG";

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public virtual string GetWorldFilePath(GameSettings settings) => GetWorldFolderPath(settings.World) + $"{GetWorldName(settings.World)}.XXX";

        /// <summary>
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The world folder path</returns>
        public virtual string GetWorldFolderPath(World world) => GetDataPath() + GetWorldName(world) + "/";

        /// <summary>
        /// Gets the base path for the game data
        /// </summary>
        /// <returns>The data path</returns>
        public virtual string GetDataPath() => "RAY/";

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => EnumHelpers.GetValues<World>().Select(w => new KeyValuePair<World, int[]>(w, Directory.EnumerateFiles(settings.GameDirectory + GetWorldFolderPath(w), $"{GetWorldName(w)}**.XXX", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Where(x => x.Length == 5)
            .Select(x => Int32.Parse(x.Substring(3)))
            .ToArray())).Where(x => x.Value.Any()).ToArray();

        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return base.GetGameActions(settings).Concat(new GameAction[]
            {
                new GameAction("Export Palettes", false, true, (input, output) => ExportPaletteImageAsync(settings, output)),
                new GameAction("Export Menu Sprites", false, true, (input, output) => ExportMenuSpritesAsync(settings, output, false)),
                new GameAction("Export Menu Animation Frames", false, true, (input, output) => ExportMenuSpritesAsync(settings, output, true)),
            }).ToArray();
        }

        /// <summary>
        /// Gets the vignette file info
        /// </summary>
        /// <returns>The vignette file info</returns>
        protected override PS1VignetteFileInfo[] GetVignetteInfo() => new PS1VignetteFileInfo[]
        {
            new PS1VignetteFileInfo("RAY/IMA/CRD/END_01.R16", 320),
            new PS1VignetteFileInfo("RAY/IMA/CRD/VAC.XXX", 206, 199, 182, 195, 214, 187),
            new PS1VignetteFileInfo("RAY/IMA/CRD/VAC_CLOR.R16", 206),
            new PS1VignetteFileInfo("RAY/IMA/CRD/VAC_MAR.R16 ", 195),
            new PS1VignetteFileInfo("RAY/IMA/CRD/VAC_MOSR.R16", 182),
            new PS1VignetteFileInfo("RAY/IMA/CRD/VAC_RAYR.R16", 187),
            new PS1VignetteFileInfo("RAY/IMA/CRD/VAC_SKOR.R16", 199),
            new PS1VignetteFileInfo("RAY/IMA/CRD/VAC_TOOR.R16", 214),

            // EU only
            new PS1VignetteFileInfo("RAY/IMA/CRD/LANGUE.R16", 320),
            new PS1VignetteFileInfo("RAY/IMA/CRD/PIRACY.R16", 320),

            new PS1VignetteFileInfo("RAY/IMA/FND/NWORLD.R16"),
            new PS1VignetteFileInfo("RAY/IMA/FND/IMGF2.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/IMGF3.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/IMGF4.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/IMGF5.XXX"),
            // Ignore US exclusive duplicate
            //new PS1VignetteFileInfo("RAY/IMA/FND/IMGF21.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/JUNF1.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/JUNF2.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/JUNF3.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/JUNF4.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/JUNF5.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/JUNF6.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/MONF1.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/MONF2.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/MONF3.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/MONF4.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/MONF5.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/MUSF1.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/MUSF2.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/MUSF3.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/MUSF4.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/MUSF5.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/CAVF1.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/CAVF2.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/CAVF3.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/CAVF4.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/CAVF5.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/CAVF6.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/GATF1.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/GATF2.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/GATF3.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/IMGF1.XXX"),

            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_JOE.R16", 162),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_MUS.R16", 159),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_PR1.R16", 254),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_PR2.R16", 208),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_PR3.R16", 200),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_PR4.R16", 200),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_PR5.R16", 146),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_RAP.R16", 171),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_TRZ.R16", 178),
            new PS1VignetteFileInfo("RAY/IMA/VIG/CONTINUE.R16", 320),
            new PS1VignetteFileInfo("RAY/IMA/VIG/FND01.R16", 320),
            new PS1VignetteFileInfo("RAY/IMA/VIG/FND02.R16", 320),
            new PS1VignetteFileInfo("RAY/IMA/VIG/LOGO_UBI.R16", 640),
            new PS1VignetteFileInfo("RAY/IMA/VIG/PRE.XXX", 254, 208, 200, 200, 146),
            new PS1VignetteFileInfo("RAY/IMA/VIG/PRESENT.R16", 279),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_01R.R16", 219),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_02R.R16", 231),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_03R.R16", 257),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_04R.R16", 200),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_05R.R16", 146),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_06R.R16", 203),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_DRK.R16", 168),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_END.R16", 306),

            // JP only
            new PS1VignetteFileInfo("RAY/IMA/VIG/PRES01A.R16", 640),
            new PS1VignetteFileInfo("RAY/IMA/VIG/PRES01B.R16", 640),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_CAK.R16", 203),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_CAV.R16", 146),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_HLO.R16", 320),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_IMG.R16", 200),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_JUN.R16", 219),
            // This one seems broken or using some weird encoding
            //new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_MON.R16", ???),
        };

        /// <summary>
        /// Gets the base directory name for exporting a common design
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="des">The design to export</param>
        /// <returns>The base directory name</returns>
        protected override string GetExportDirName(GameSettings settings, Common_Design des)
        {
            // Get the file path
            var path = des.FilePath;

            if (path == null)
                throw new Exception("Path can not be null");

            if (path == GetAllfixFilePath(settings))
                return $"Allfix/";
            else if (path == GetWorldFilePath(settings))
                return $"{settings.World}/{settings.World} - ";
            else if (path == GetLevelFilePath(settings))
                return $"{settings.World}/{settings.World}{settings.Level} - ";

            return $"Unknown/";
        }

        public async Task ExportPaletteImageAsync(GameSettings settings, string outputPath)
        {
            var spritePals = new List<ARGB1555Color[]>();
            var tilePals = new List<ARGB1555Color[]>();

            void Add(ICollection<ARGB1555Color[]> pals, ARGB1555Color[] pal)
            {
                if (pal != null && !pals.Any(x => x.SequenceEqual(pal)))
                    pals.Add(pal);
            }

            // Enumerate every world
            foreach (var world in GetLevels(settings))
            {
                settings.World = world.Key;
                settings.Level = 1;

                using (var context = new Context(settings))
                {
                    // Read the allfix file
                    await LoadExtraFile(context, GetAllfixFilePath(context.Settings));
                    var allfix = FileFactory.Read<PS1_R1_AllfixFile>(GetAllfixFilePath(context.Settings), context);

                    // Read the BigRay file
                    await LoadExtraFile(context, GetBigRayFilePath(context.Settings));
                    var br = FileFactory.Read<PS1_R1_BigRayFile>(GetBigRayFilePath(context.Settings), context);

                    Add(spritePals, allfix.Palette1);
                    Add(spritePals, allfix.Palette2);
                    Add(spritePals, allfix.Palette3);
                    Add(spritePals, allfix.Palette4);
                    Add(spritePals, allfix.Palette5);
                    Add(spritePals, allfix.Palette6);
                    Add(spritePals, br.Palette1);
                    Add(spritePals, br.Palette2);

                    // Read the world file
                    await LoadExtraFile(context, GetWorldFilePath(context.Settings));
                    var wld = FileFactory.Read<PS1_R1_WorldFile>(GetWorldFilePath(context.Settings), context);

                    Add(spritePals, wld.EventPalette1);
                    Add(spritePals, wld.EventPalette2);

                    foreach (var tilePal in wld.TilePalettes ?? new ARGB1555Color[0][])
                        Add(tilePals, tilePal);
                }
            }

            // Export
            PaletteHelpers.ExportPalette(Path.Combine(outputPath, $"{settings.GameModeSelection}.png"), spritePals.Concat(tilePals).SelectMany(x => x).ToArray(), optionalWrap: 256);
        }

        public async Task ExportMenuSpritesAsync(GameSettings settings, string outputPath, bool exportAnimFrames)
        {
            using (var context = new Context(settings))
            {
                // Read the allfix file
                await LoadExtraFile(context, GetAllfixFilePath(context.Settings));
                var fix = FileFactory.Read<PS1_R1_AllfixFile>(GetAllfixFilePath(context.Settings), context);

                // Read the BigRay file
                await LoadExtraFile(context, GetBigRayFilePath(context.Settings));
                var br = FileFactory.Read<PS1_R1_BigRayFile>(GetBigRayFilePath(context.Settings), context);

                // Load the font file
                await LoadExtraFile(context, GetFontFilePath(context.Settings));

                // Fill the v-ram
                FillVRAM(context, VRAMMode.Menu);

                // Export each font DES
                if (!exportAnimFrames)
                {
                    for (int fontIndex = 0; fontIndex < fix.FontData.Length; fontIndex++)
                    {
                        // Export every sprite
                        for (int spriteIndex = 0; spriteIndex < fix.FontData[fontIndex].ImageDescriptorsCount; spriteIndex++)
                        {
                            // Get the sprite texture
                            var tex = GetSpriteTexture(context, null, fix.FontData[fontIndex].ImageDescriptors[spriteIndex]);

                            // Make sure it's not null
                            if (tex == null)
                                continue;

                            // Export the font sprite
                            Util.ByteArrayToFile(Path.Combine(outputPath, "Font", $"{fontIndex} - {spriteIndex}.png"), tex.EncodeToPNG());
                        }
                    }
                }

                // Export menu sprites from allfix
                var exportedImgDescr = new List<Pointer>();
                var index = 0;

                foreach (EventData t in fix.MenuEvents)
                {
                    if (exportedImgDescr.Contains(t.ImageDescriptorsPointer))
                        continue;

                    exportedImgDescr.Add(t.ImageDescriptorsPointer);

                    await ExportEventSpritesAsync(t, Path.Combine(outputPath, "Menu"), index);

                    index++;
                }

                // Export BigRay
                await ExportEventSpritesAsync(br.BigRay, Path.Combine(outputPath, "BigRay"), 0);

                async Task ExportEventSpritesAsync(EventData e, string eventOutputDir, int desIndex)
                {
                    var sprites = e.ImageDescriptors.Select(x => GetSpriteTexture(context, e, x)).ToArray();

                    if (!exportAnimFrames)
                    {
                        for (int i = 0; i < sprites.Length; i++)
                        {
                            if (sprites[i] == null)
                                continue;

                            Util.ByteArrayToFile(Path.Combine(eventOutputDir, $"{desIndex} - {i}.png"), sprites[i].EncodeToPNG());
                        }
                    }
                    else
                    {
                        // Enumerate the animations
                        for (var j = 0; j < e.AnimDescriptors.Length; j++)
                        {
                            // Get the animation descriptor
                            var anim = e.AnimDescriptors[j];

                            // Get the speed
                            var speed = String.Join("-", e.ETA.EventStates.SelectMany(x => x).Where(x => x.AnimationIndex == j).Select(x => x.AnimationSpeed).Distinct());

                            // Get the folder
                            var animFolderPath = Path.Combine(eventOutputDir, desIndex.ToString(), $"{j}-{speed}");

                            int? frameWidth = null;
                            int? frameHeight = null;

                            for (int dummyFrame = 0; dummyFrame < anim.FrameCount; dummyFrame++)
                            {
                                for (int dummyLayer = 0; dummyLayer < anim.LayersPerFrame; dummyLayer++)
                                {
                                    var l = anim.Layers[dummyFrame * anim.LayersPerFrame + dummyLayer];

                                    if (l.ImageIndex < sprites.Length)
                                    {
                                        var s = sprites[l.ImageIndex];

                                        if (s != null)
                                        {
                                            var w = s.width + l.XPosition;
                                            var h = s.height + l.YPosition;

                                            if (frameWidth == null || frameWidth < w)
                                                frameWidth = w;

                                            if (frameHeight == null || frameHeight < h)
                                                frameHeight = h;
                                        }
                                    }
                                }
                            }

                            // Create each animation frame
                            for (int frameIndex = 0; frameIndex < anim.FrameCount; frameIndex++)
                            {
                                Texture2D tex = new Texture2D(frameWidth ?? 1, frameHeight ?? 1, TextureFormat.RGBA32, false)
                                {
                                    filterMode = FilterMode.Point,
                                    wrapMode = TextureWrapMode.Clamp
                                };

                                // Default to fully transparent
                                tex.SetPixels(Enumerable.Repeat(new Color(0, 0, 0, 0), tex.width * tex.height).ToArray());

                                bool hasLayers = false;

                                // Write each layer
                                for (var layerIndex = 0; layerIndex < anim.LayersPerFrame; layerIndex++)
                                {
                                    var animationLayer = anim.Layers[frameIndex * anim.LayersPerFrame + layerIndex];

                                    if (animationLayer.ImageIndex >= sprites.Length)
                                        continue;

                                    // Get the sprite
                                    var sprite = sprites[animationLayer.ImageIndex];

                                    if (sprite == null)
                                        continue;

                                    // Set every pixel
                                    for (int y = 0; y < sprite.height; y++)
                                    {
                                        for (int x = 0; x < sprite.width; x++)
                                        {
                                            var c = sprite.GetPixel(x, sprite.height - y - 1);

                                            var xPosition = (animationLayer.IsFlippedHorizontally ? (sprite.width - 1 - x) : x) + animationLayer.XPosition;
                                            var yPosition = y + animationLayer.YPosition;

                                            if (xPosition >= tex.width)
                                                throw new Exception("Horizontal overflow!");

                                            if (c.a != 0)
                                                tex.SetPixel(xPosition, tex.height - 1 - yPosition, c);
                                        }
                                    }

                                    hasLayers = true;
                                }

                                tex.Apply();

                                if (!hasLayers)
                                    continue;

                                // Save the file
                                Util.ByteArrayToFile(Path.Combine(animFolderPath, $"{frameIndex}.png"), tex.EncodeToPNG());
                            }
                        }
                    }

                    // Unload textures
                    await Resources.UnloadUnusedAssets();
                }
            }
        }
    }
}