using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (Saturn)
    /// </summary>
    public abstract class R1_Saturn_Manager : R1_PS1BaseManager
    {
        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public override int TileSetWidth => 16;

        public virtual string GetLanguageFilePath(string langCode) => $"RAY{langCode}.TXT";

        protected override PS1MemoryMappedFile.InvalidPointerMode InvalidPointerMode => PS1MemoryMappedFile.InvalidPointerMode.Allow;

        public uint BaseAddress => 0x00200000;

        public override BinaryFile.Endian Endianness { get; } = BinaryFile.Endian.Big;

        /// <summary>
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The world folder path</returns>
        public string GetWorldFolderPath(R1_World world) => GetWorldName(world) + "/";

        /// <summary>
        /// Gets the offset for the palettes in the game executable
        /// </summary>
        public abstract uint GetPalOffset { get; }
        public abstract uint GetFndFileTableOffset { get; }
        public abstract uint GetFndSPFileTableOffset { get; }
        public abstract uint GetFndIndexTableOffset { get; }

        /// <summary>
        /// Gets the allfix file path
        /// </summary>
        /// <returns>The allfix file path</returns>
        public string GetAllfixFilePath() => "RAY.DTA";

        public string GetBigRayFilePath() => "BIG.DTA";

        /// <summary>
        /// Gets the world data file path
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world data file path</returns>
        public string GetWorldFilePath(GameSettings settings) => GetWorldFolderPath(settings.R1_World) + $"{GetWorldName(settings.R1_World)}.DTA";

        /// <summary>
        /// Gets the level data file path
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level data file path</returns>
        public string GetLevelFilePath(GameSettings settings) => GetWorldFolderPath(settings.R1_World) + $"{GetWorldName(settings.R1_World)}{settings.Level:00}.DTA";

        /// <summary>
        /// Gets the map file path
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The map file path</returns>
        public string GetMapFilePath(Context context) => (GetWorldFolderPath(context.Settings.R1_World) + (context.Settings.Level == 140 ? $"JUNNT14.XMP" : $"{GetWorldName(context.Settings.R1_World)}00{context.Settings.Level}.XMP"));

        /// <summary>
        /// Gets the tile-set palette file path
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile-set palette file path</returns>
        public string GetTileSetPaletteFilePath(Context context) => GetWorldFolderPath(context.Settings.R1_World) + $"{GetWorldName(context.Settings.R1_World)}.PAL";

        /// <summary>
        /// Gets the tile-set palette index table file path
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile-set palette index table file path</returns>
        public string GetTileSetPaletteIndexTableFilePath(Context context) => GetWorldFolderPath(context.Settings.R1_World) + $"{GetWorldName(context.Settings.R1_World)}_01.PLT";

        /// <summary>
        /// Gets the tile-set file path
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile-set file path</returns>
        public string GetTileSetFilePath(Context context) => GetWorldFolderPath(context.Settings.R1_World) + $"{GetWorldName(context.Settings.R1_World)}_01.BIT";

        public string GetBigRayImageFilePath() => "BIG.IMG";
        public string GetFixImageFilePath() => "RAY.IMG";
        public string GetWorldImageFilePath(Context context) => GetWorldFolderPath(context.Settings.R1_World) + $"{GetWorldName(context.Settings.R1_World)}.IMG";
        public string GetLevelImageFilePath(Context context) => GetWorldFolderPath(context.Settings.R1_World) + $"{GetWorldName(context.Settings.R1_World)}{context.Settings.Level:00}.IMG";

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(WorldHelpers.GetR1Worlds().Select(w => new GameInfo_World((int)w, Directory.EnumerateFiles(settings.GameDirectory + GetWorldFolderPath(w), $"*.XMP", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(5)))
            .ToArray())).Select(x => x.Index == 1 ? new GameInfo_World(x.Index, x.Maps.Append(140).ToArray()) : x).ToArray());

        public override string ExeFilePath => "0";
        public override uint? ExeBaseAddress => null;

        public override FileTableInfo[] FileTableInfos => new FileTableInfo[0];

        public async UniTask<uint> LoadFile(Context context, string path, uint baseAddress = 0)
        {
            await FileSystem.PrepareFile(context.BasePath + path);
            if (!FileSystem.FileExists(context.BasePath + path)) {
                return 0;
            }
            if (baseAddress != 0)
            {
                PS1MemoryMappedFile file = new PS1MemoryMappedFile(context, baseAddress, InvalidPointerMode)
                {
                    filePath = path,
                    Endianness = BinaryFile.Endian.Big
                };
                context.AddFile(file);

                return file.Length;
            }
            else
            {
                LinearSerializedFile file = new LinearSerializedFile(context)
                {
                    filePath = path,
                    Endianness = BinaryFile.Endian.Big
                };
                context.AddFile(file);
                return 0;
            }
        }

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override Unity_MapTileMap GetTileSet(Context context) 
        {
            // Read the files
            var tileSetPalette = FileFactory.Read<ObjectArray<ARGB1555Color>>(GetTileSetPaletteFilePath(context), context, onPreSerialize: (s, x) => x.Length = s.CurrentLength / 2);
            var tileSetPaletteIndexTable = FileFactory.Read<Array<byte>>(GetTileSetPaletteIndexTableFilePath(context), context, onPreSerialize: (s, x) => x.Length = s.CurrentLength);
            var tileSet = FileFactory.Read<BIT>(GetTileSetFilePath(context), context, onPreSerialize: (s, b) =>
            {
                b.PAL = tileSetPalette;
                b.PLT = tileSetPaletteIndexTable;
            });

            // Get the tile-set texture
            var tex = tileSet.ToTexture(Settings.CellSize * TileSetWidth);

            // Add transparency
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    var color = tex.GetPixel(x, y);

                    if (color.r == 0 && color.g == 0 && color.b == 0)
                        tex.SetPixel(x, y, new Color(color.r, color.g, color.b, 0f));
                }
            }

            tex.Apply();

            return new Unity_MapTileMap(tex, Settings.CellSize);
        }

        public int GetPaletteIndex(Context context) {
            // Imitates code at 0x0603586c (US executable)
            // After tile palettes: 19
            int part2 = 19;
            int level = context.Settings.Level;
            switch (context.Settings.R1_World) {
                case R1_World.Jungle:
                    if (level == 9 || level == 16) { // Different palette for Moskito boss, but why Swamp 1?
                        return part2 + 1;
                    } else {
                        return 2;
                    }
                case R1_World.Music:
                    return 0;
                case R1_World.Mountain:
                    return 4;
                case R1_World.Image:
                    if (level == 4 || level == 11) { // Different palette for the boss levels
                        return part2 + 4;
                    } else {
                        return 3;
                    }
                case R1_World.Cave:
                    return 1;
                case R1_World.Cake:
                    return part2 + 3;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="mode">The blocks to fill</param>
        /// <returns>The filled v-ram</returns>
        protected override void FillVRAM(Context context, VRAMMode mode)
        {
            string fixPath = GetFixImageFilePath();
            string worldPath = GetWorldImageFilePath(context);
            string levelPath = GetLevelImageFilePath(context);
            string bigRayPath = GetBigRayImageFilePath();

            var fixImg = context.FileExists(fixPath) && mode != VRAMMode.BigRay ? FileFactory.Read<Array<byte>>(fixPath, context, (y, x) => x.Length = y.CurrentLength) : null;
            var worldImg = context.FileExists(worldPath) && mode == VRAMMode.Level ? FileFactory.Read<Array<byte>>(GetWorldImageFilePath(context), context, (y, x) => x.Length = y.CurrentLength) : null;
            var levelImg = context.FileExists(levelPath) && mode == VRAMMode.Level ? FileFactory.Read<Array<byte>>(levelPath, context, (y, x) => x.Length = y.CurrentLength) : null;
            var bigRayImg = context.FileExists(bigRayPath) && mode == VRAMMode.BigRay ? FileFactory.Read<Array<byte>>(bigRayPath, context, (y, x) => x.Length = y.CurrentLength) : null;

            ImageBuffer buf = new ImageBuffer();
            if (fixImg != null) buf.AddData(fixImg.Value);
            if (worldImg != null) buf.AddData(worldImg.Value);
            if (levelImg != null) buf.AddData(levelImg.Value);
            if (bigRayImg != null) buf.AddData(bigRayImg.Value);
            context.StoreObject("vram", buf);
        }

        /// <summary>
        /// Gets the sprite texture for an event
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="imgBuffer">The image buffer, if available</param>
        /// <param name="s">The image descriptor to use</param>
        /// <returns>The texture</returns>
        public override Texture2D GetSpriteTexture(Context context, byte[] imgBuffer, R1_ImageDescriptor img)
        {
            if (img.ImageType != 2 && img.ImageType != 3) return null;
            if (img.Unknown2 == 0) return null;
            ImageBuffer buf = context.GetStoredObject<ImageBuffer>("vram");

            // Get the image properties
            var width = img.Width;
            var height = img.Height;
            var offset = img.ImageBufferOffset;

            Texture2D tex = TextureHelpers.CreateTexture2D(width, height);

            var palette = FileFactory.Read<R1_PS1_Executable>(ExeFilePath, context).Saturn_Palettes;
            var paletteOffset = img.PaletteInfo;

            var isBigRay = img.Offset.file.filePath == GetBigRayFilePath();
            var isFont = context.GetStoredObject<R1_PS1_FontData[]>("Font")?.SelectMany(x => x.ImageDescriptors).Contains(img) == true;
            
            //paletteOffset = (ushort)(256 * (img.Unknown2 >> 4));
            if (img.ImageType == 3) {
                //paletteOffset = 20 * 256;
                paletteOffset = isBigRay ? (ushort)(21 * 256) : isFont ? (ushort)(19 * 256) : (ushort)((GetPaletteIndex(context) * 256));
            } else {
                paletteOffset = isBigRay ? (ushort)(21 * 256) : isFont ? (ushort)(19 * 256) : (ushort)((GetPaletteIndex(context) * 256) + ((img.PaletteInfo >> 8)) * 16);
                //paletteOffset = (ushort)((GetPaletteIndex(context) * 256) + ((img.Unknown2 >> 4) - 1) * 16);
                //paletteOffset = (ushort)(19 * 256 + ((img.Unknown2 >> 4) - 1) * 16);
            }

            // Set every pixel
            if (img.ImageType == 3) {
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        var paletteIndex = buf.GetPixel8((uint)(offset + width * y + x));

                        // Set the pixel
                        var color = palette[paletteOffset + paletteIndex].GetColor();
                        if (paletteIndex == 0) {
                            color = new Color(color.r, color.g, color.b, 0f);
                        } else {
                            color = new Color(color.r, color.g, color.b, 1f);
                        }
                        tex.SetPixel(x, height - 1 - y, color);
                    }
                }
            } else if (img.ImageType == 2) {
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        var paletteIndex = buf.GetPixel8((uint)(offset + (width * y + x) / 2));
                        if (x % 2 == 0)
                            paletteIndex = (byte)BitHelpers.ExtractBits(paletteIndex, 4, 4);
                        else
                            paletteIndex = (byte)BitHelpers.ExtractBits(paletteIndex, 4, 0);

                        // Set the pixel
                        var color = palette[paletteOffset + paletteIndex].GetColor();
                        if (paletteIndex == 0) {
                            color = new Color(color.r, color.g, color.b, 0f);
                        } else {
                            color = new Color(color.r, color.g, color.b, 1f);
                        }
                        tex.SetPixel(x, height - 1 - y, color);
                    }
                }
            }


            //tex.SetPixels(Enumerable.Repeat(Color.blue, tex.width * tex.height).ToArray());
            tex.Apply();

            return tex;
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <returns>The level</returns>
        public override async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            // Get the paths
            var allfixFilePath = GetAllfixFilePath();
            var worldFilePath = GetWorldFilePath(context.Settings);
            var levelFilePath = GetLevelFilePath(context.Settings);
            var tileSetPaletteFilePath = GetTileSetPaletteFilePath(context);
            var tileSetPaletteIndexTableFilePath = GetTileSetPaletteIndexTableFilePath(context);
            var tileSetFilePath = GetTileSetFilePath(context);
            var mapFilePath = GetMapFilePath(context);

            uint baseAddress = BaseAddress;

            // Load the memory mapped files
            baseAddress += await LoadFile(context, allfixFilePath, baseAddress);

            baseAddress += await LoadFile(context, worldFilePath, baseAddress);

            baseAddress += await LoadFile(context, levelFilePath, baseAddress);

            // Load the files
            await LoadFile(context, tileSetPaletteFilePath);
            await LoadFile(context, tileSetPaletteIndexTableFilePath);
            await LoadFile(context, tileSetFilePath);
            await LoadFile(context, mapFilePath);

            // Load executable to get the palettes and tables
            await LoadFile(context, ExeFilePath);

            // Load the texture files
            await LoadFile(context, GetFixImageFilePath());

            if (FileSystem.FileExists(context.BasePath + levelFilePath))
            {
                await LoadFile(context, GetWorldImageFilePath(context));
                await LoadFile(context, GetLevelImageFilePath(context));
            }

            // NOTE: Big ray data is always loaded at 0x00280000

            // Read the map block
            var map = FileFactory.Read<MapData>(mapFilePath, context);

            R1_PS1_EventBlock eventBlock = null;

            if (FileSystem.FileExists(context.BasePath + levelFilePath))
                // Read the event block
                eventBlock = FileFactory.Read<R1_PS1_EventBlock>(levelFilePath, context);

            // Load the level
            return await LoadAsync(context, map, eventBlock?.Events, eventBlock?.EventLinkingTable.Select(b => (ushort)b).ToArray(), loadTextures);
        }

        /// <summary>
        /// Gets the vignette file info
        /// </summary>
        /// <returns>The vignette file info</returns>
        protected override PS1VignetteFileInfo[] GetVignetteInfo() => throw new InvalidOperationException("Saturn does not use PS1 vignette files!");

        /// <summary>
        /// Exports all vignette textures to the specified output directory
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="outputDir">The output directory</param>
        public override void ExportVignetteTextures(GameSettings settings, string outputDir)
        {
            // Create a new context
            using (var context = new Context(settings))
            {
                void exportBit(string file, int width = 16, bool swizzled = true, int blockWidth = 8, int blockHeight = 8, IList<Vector2> sizes = null)
                {
                    // Add the file to the context
                    context.AddFile(new LinearSerializedFile(context)
                    {
                        filePath = file,
                        Endianness = BinaryFile.Endian.Big
                    });

                    // Read the file
                    BIT bit = FileFactory.Read<BIT>(file, context);

                    // Get the texture
                    var tex = bit.ToTexture(width, swizzled: swizzled, blockWidth: blockWidth, blockHeight: blockHeight, invertYAxis: true);

                    if (sizes?.Any() == true)
                    {
                        // Get the pixels
                        var pixels = tex.GetPixels();

                        var pixelOffset = 0;

                        // TODO: This doesn't work...
                        for (int i = 0; i < sizes.Count; i++)
                        {
                            // Get the output file path
                            var outputPath = Path.Combine(outputDir, FileSystem.ChangeFilePathExtension(file, $" - {i}.png"));

                            // Create a new texture
                            var newTex = TextureHelpers.CreateTexture2D((int)sizes[i].x, (int)sizes[i].y);

                            // Set the pixels
                            newTex.SetPixels(pixels.Reverse().Skip(pixelOffset).Take(newTex.width * newTex.height).ToArray());

                            newTex.Apply();

                            pixelOffset += newTex.width * newTex.height;

                            Util.ByteArrayToFile(outputPath, newTex.EncodeToPNG());
                        }
                    }
                    else
                    {
                        // Get the output file path
                        var outputPath = Path.Combine(outputDir, FileSystem.ChangeFilePathExtension(file, $".png"));

                        Util.ByteArrayToFile(outputPath, tex.EncodeToPNG());
                    }
                }

                void exportVig(string file, int width)
                {
                    // Add the file to the context
                    context.AddFile(new LinearSerializedFile(context)
                    {
                        filePath = file,
                        Endianness = BinaryFile.Endian.Big
                    });

                    // Read the raw data
                    var rawData = FileFactory.Read<ObjectArray<ARGB1555Color>>(file, context, onPreSerialize: (s, x) => x.Length = s.CurrentLength / 2);

                    // Create the texture
                    var tex = TextureHelpers.CreateTexture2D(width, (int)(rawData.Length / width));

                    // Set the pixels
                    for (int y = 0; y < tex.height; y++)
                    {
                        for (int x = 0; x < tex.width; x++)
                        {
                            var c = rawData.Value[y * tex.width + x];

                            tex.SetPixel(x, tex.height - y - 1, c.GetColor());
                        }
                    }

                    // Apply the pixels
                    tex.Apply();

                    // Get the output file path
                    var outputPath = Path.Combine(outputDir, FileSystem.ChangeFilePathExtension(file, $".png"));

                    Util.ByteArrayToFile(outputPath, tex.EncodeToPNG());
                }

                foreach (var bitFile in Directory.GetFiles(settings.GameDirectory, "*.bit", SearchOption.AllDirectories))
                {
                    var relativePath = bitFile.Substring(context.BasePath.Length).Replace('\\', '/');

                    if (!VigWidths.ContainsKey(relativePath))
                    {
                        Debug.LogWarning($"Vignette file {relativePath} has no width");
                        continue;
                    }

                    exportBit(relativePath, width: VigWidths[relativePath]);
                }
                foreach (var vigFile in Directory.GetFiles(settings.GameDirectory, "*.vig", SearchOption.AllDirectories))
                {
                    var relativePath = vigFile.Substring(context.BasePath.Length).Replace('\\', '/');

                    if (!VigWidths.ContainsKey(relativePath))
                    {
                        Debug.LogWarning($"Vignette file {relativePath} has no width");
                        continue;
                    }

                    exportVig(relativePath, width: VigWidths[relativePath]);
                }
            }
        }

        /// <summary>
        /// Gets the base directory name for exporting a common design
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="des">The design to export</param>
        /// <returns>The base directory name</returns>
        protected override string GetExportDirName(GameSettings settings, Unity_ObjGraphics des)
        {
            // Get the file path
            var path = des.FilePath;

            if (path == null)
                throw new Exception("Path can not be null");

            if (path == GetAllfixFilePath())
                return $"Allfix/";
            else if (path == GetWorldFilePath(settings))
                return $"{settings.World}/{settings.World} - ";
            else if (path == GetLevelFilePath(settings))
                return $"{settings.World}/{settings.World}{settings.Level} - ";

            return $"Unknown/";
        }

        public override async UniTask ExportMenuSpritesAsync(GameSettings settings, string outputPath, bool exportAnimFrames)
        {
            using (var menuContext = new Context(settings))
            {
                using (var bigRayContext = new Context(settings))
                {
                    // Load allfix
                    await LoadFile(menuContext, GetAllfixFilePath(), BaseAddress);
                    var fix = FileFactory.Read<R1_PS1_AllfixBlock>(GetAllfixFilePath(), menuContext, onPreSerialize: (s, o) => o.Length = s.CurrentLength);
                    await LoadFile(menuContext, GetFixImageFilePath());
                    
                    // Load exe
                    await LoadFile(menuContext, ExeFilePath);
                    await LoadFile(bigRayContext, ExeFilePath);

                    // Save font
                    menuContext.StoreObject("Font", fix.FontData);

                    // Read the BigRay file
                    await LoadFile(bigRayContext, GetBigRayFilePath(), 0x00280000);
                    await LoadFile(bigRayContext, GetBigRayImageFilePath());
                    var br = FileFactory.Read<R1_PS1_BigRayBlock>(GetBigRayFilePath(), bigRayContext, onPreSerialize: (s, o) => o.Length = s.CurrentLength);

                    // Export
                    await ExportMenuSpritesAsync(menuContext, bigRayContext, outputPath, exportAnimFrames, fix.FontData, fix.MenuEvents, br);
                }
            }
        }

        public override R1_EventData GetRaymanEvent(Context context) => FileFactory.Read<R1_PS1_AllfixBlock>(GetAllfixFilePath(), context, onPreSerialize: (s, o) => o.Length = s.CurrentLength).MenuEvents[0];

        public override async UniTask<Texture2D> LoadLevelBackgroundAsync(Context context)
        {
            var exe = FileFactory.Read<R1_PS1_Executable>(ExeFilePath, context);
            var worldIndex = context.Settings.World - 1;
            var lvlIndex = context.Settings.Level - 1;

            if (lvlIndex >= 25)
                return null;

            var bgIndex = exe.Saturn_FNDIndexTable[context.Settings.World][lvlIndex];
            var bgFile = exe.Saturn_FNDFileTable[worldIndex][bgIndex];

            if (String.IsNullOrEmpty(bgFile))
                return null;

            var bgFilePath = GetWorldFolderPath(context.Settings.R1_World) + bgFile;
            
            await LoadFile(context, bgFilePath);

            var bit = FileFactory.Read<BIT>(bgFilePath, context);

            return bit.ToTexture(VigWidths[bgFilePath], invertYAxis: true);
        }

        public Dictionary<string, int> VigWidths { get; } = new Dictionary<string, int>()
        {
            //["CAK/CAK_01.BIT"] = ,
            //["CAK/EXPLOSE.BIT"] = ,
            //["CAK/GAT1_SP.BIT"] = ,
            //["CAK/GAT3_SP.BIT"] = ,
            ["CAK/GAT_F01P.BIT"] = 384,
            ["CAK/GAT_F01E.BIT"] = 384,
            ["CAK/GAT_F03.BIT"] = 384,
            ["CAK/GAT_F04.BIT"] = 320,
            ["CAK/GAT_F04P.BIT"] = 320,
            ["CAK/MDK_F01P.BIT"] = 320,
            ["CAK/MBLK_F01.BIT"] = 320,
            //["CAK/VITRAUX.BIT"] = ,

            //["CAV/CAV1_SP.BIT"] = ,
            //["CAV/CAV_01.BIT"] = ,
            ["CAV/CAV_F01.BIT"] = 384,
            ["CAV/CAV_F01M.BIT"] = 384,
            ["CAV/CAV_F04P.BIT"] = 320,
            ["CAV/CAV_F04.BIT"] = 320,
            ["CAV/CAV_F1.BIT"] = 384,
            ["CAV/CAV_FJOE.BIT"] = 192,
            //["CAV/JOE_01.BIT"] = ,
            ["CAV/CAV_FSKO.BIT"] = 384,
            ["CAV/FD_JOPAL.BIT"] = 320,
            ["CAV/FD_PBLJO.BIT"] = 320,

            //["IMG/IMG2_SP.BIT"] = ,
            //["IMG/IMG4_SP.BIT"] = ,
            //["IMG/IMG5_SP.BIT"] = ,
            //["IMG/IMG_01.BIT"] = ,
            ["IMG/IMG_F01.BIT"] = 192,
            ["IMG/IMG_F01B.BIT"] = 192,
            ["IMG/IMG_F02.BIT"] = 384,
            ["IMG/IMG_F04.BIT"] = 384,
            ["IMG/IMG_F05.BIT"] = 320,
            ["IMG/IMG_F05P.BIT"] = 320,
            ["IMG/IMG_F06P.BIT"] = 320,
            ["IMG/IMG_F06.BIT"] = 320,

            //["JUN/JUN2_SP.BIT"] = ,
            //["JUN/JUN3_SP.BIT"] = ,
            //["JUN/JUN_01.BIT"] = ,
            ["JUN/JUN_F01P.BIT"] = 192,
            ["JUN/JUN_F01.BIT"] = 192,
            ["JUN/JUN_F2JM.BIT"] = 384,
            ["JUN/JUN_F3JM.BIT"] = 384,
            ["JUN/JUN_MOPA.BIT"] = 320,
            ["JUN/JUN_MOSK.BIT"] = 320,

            //["MON/MON1_SP.BIT"] = 320,
            //["MON/MON2_SP.BIT"] = 320,
            //["MON/MON7_SP.BIT"] = 320,
            //["MON/MON_01.BIT"] = 320,
            ["MON/MON_F01.BIT"] = 192,
            ["MON/MON_F02.BIT"] = 384,
            ["MON/MON_F2W.BIT"] = 384,
            ["MON/MON_F04.BIT"] = 384,
            ["MON/MON_F05.BIT"] = 192,
            ["MON/MON_F05P.BIT"] = 192,

            //["MUS/MUS2_SP.BIT"] = 320,
            //["MUS/MUS3_SP.BIT"] = 320,
            //["MUS/MUS4_SP.BIT"] = 320,
            //["MUS/MUS_01.BIT"] = 320,
            ["MUS/MUS_F02.BIT"] = 384,
            ["MUS/MUS_F03.BIT"] = 320,
            ["MUS/MUS_F04.BIT"] = 384,
            ["MUS/MUS_F05P.BIT"] = 320,
            ["MUS/MUS_F1.BIT"] = 384,
            ["MUS/MUS_F2D.BIT"] = 384,

            ["VIGNET/CONTINUE.BIT"] = 320,
            ["VIGNET/END_01.BIT"] = 320,
            ["VIGNET/FD01SATP.BIT"] = 320,
            ["VIGNET/FD01SAT.BIT"] = 320,
            ["VIGNET/FND02.BIT"] = 320,
            ["VIGNET/LOGO.BIT"] = 320,
            ["VIGNET/NWORLD.BIT"] = 384,
            ["VIGNET/RAYMAN.BIT"] = 320,
            //["VIGNET/SEGA.BIT"] = 55,

            ["VIGNET/VIG_PR1.VIG"] = 254,
            ["VIGNET/VIG_PR2.VIG"] = 208,
            ["VIGNET/VIG_PR3.VIG"] = 200,
            ["VIGNET/VIG_PR4.VIG"] = 200,
            ["VIGNET/VIG_PR5.VIG"] = 146,
            ["VIGNET/VIG_01R.VIG"] = 219,
            ["VIGNET/VIG_02R.VIG"] = 231,
            ["VIGNET/VIG_03R.VIG"] = 257,
            ["VIGNET/VIG_04R.VIG"] = 200,
            ["VIGNET/VIG_05R.VIG"] = 146,
            ["VIGNET/VIG_06R.VIG"] = 203,
            ["VIGNET/VIG_DRK.VIG"] = 251,
            ["VIGNET/VIG_END.VIG"] = 191,
            ["VIGNET/VIG_JOE.VIG"] = 162,
            ["VIGNET/VIG_MUS.VIG"] = 159,
            ["VIGNET/VIG_RAP.VIG"] = 255,
            ["VIGNET/VIG_TRZ.VIG"] = 178,
            ["LANGUE.VIG"] = 320,

            // VIGNET/VRAM_A.BIT and VIGNET/VRAM_B.BIT contain multiple images
        };
    }
}