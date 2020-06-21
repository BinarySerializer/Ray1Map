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
    /// The game manager for Rayman 1 (Saturn)
    /// </summary>
    public class Satun_R1_Manager : PS1_Manager
    {
        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public override int TileSetWidth => 16;

        protected override PS1MemoryMappedFile.InvalidPointerMode InvalidPointerMode => PS1MemoryMappedFile.InvalidPointerMode.Allow;

        /// <summary>
        /// Gets the file info to use
        /// </summary>
        /// <param name="settings">The game settings</param>
        protected override Dictionary<string, PS1FileInfo> GetFileInfo(GameSettings settings) => null;

        /// <summary>
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The world folder path</returns>
        public string GetWorldFolderPath(World world) => GetWorldName(world) + "/";

        /// <summary>
        /// Gets the offset for the palettes in the game executable
        /// </summary>
        /// <returns>The offset for the palettes in the game executable</returns>
        public uint GetPalOffsetFilePath(GameSettings settings)
        {
            if (settings.GameModeSelection == GameModeSelection.RaymanSaturnUS)
                return 0x79224;
            else if (settings.GameModeSelection == GameModeSelection.RaymanSaturnEU)
                return 0x78D14;
            else if (settings.GameModeSelection == GameModeSelection.RaymanSaturnJP)
                return 0x791C4;
            else if (settings.GameModeSelection == GameModeSelection.RaymanSaturnProto)
                return 0x87754;
            else
                throw new ArgumentOutOfRangeException(nameof(settings.GameModeSelection), settings.GameModeSelection, "The requested game mode is not supported for Saturn");
        }

        /// <summary>
        /// Gets the executable file path
        /// </summary>
        /// <returns>The executable file path</returns>
        public string GetExeFilePath() => "0";

        /// <summary>
        /// Gets the allfix file path
        /// </summary>
        /// <returns>The allfix file path</returns>
        public string GetAllfixFilePath() => "RAY.DTA";

        /// <summary>
        /// Gets the world data file path
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world data file path</returns>
        public string GetWorldFilePath(GameSettings settings) => GetWorldFolderPath(settings.World) + $"{GetWorldName(settings.World)}.DTA";

        /// <summary>
        /// Gets the level data file path
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level data file path</returns>
        public string GetLevelFilePath(GameSettings settings) => GetWorldFolderPath(settings.World) + $"{GetWorldName(settings.World)}{settings.Level:00}.DTA";

        /// <summary>
        /// Gets the map file path
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The map file path</returns>
        public string GetMapFilePath(Context context) => GetWorldFolderPath(context.Settings.World) + $"{GetWorldName(context.Settings.World)}00{context.Settings.Level}.XMP";

        /// <summary>
        /// Gets the tile-set palette file path
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile-set palette file path</returns>
        public string GetTileSetPaletteFilePath(Context context) => GetWorldFolderPath(context.Settings.World) + $"{GetWorldName(context.Settings.World)}.PAL";

        /// <summary>
        /// Gets the tile-set palette index table file path
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile-set palette index table file path</returns>
        public string GetTileSetPaletteIndexTableFilePath(Context context) => GetWorldFolderPath(context.Settings.World) + $"{GetWorldName(context.Settings.World)}_01.PLT";

        /// <summary>
        /// Gets the tile-set file path
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile-set file path</returns>
        public string GetTileSetFilePath(Context context) => GetWorldFolderPath(context.Settings.World) + $"{GetWorldName(context.Settings.World)}_01.BIT";

        public string GetFixImageFilePath() => "RAY.IMG";
        public string GetWorldImageFilePath(Context context) => GetWorldFolderPath(context.Settings.World) + $"{GetWorldName(context.Settings.World)}.IMG";
        public string GetLevelImageFilePath(Context context) => GetWorldFolderPath(context.Settings.World) + $"{GetWorldName(context.Settings.World)}{context.Settings.Level:00}.IMG";

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => EnumHelpers.GetValues<World>().Select(w => new KeyValuePair<World, int[]>(w, Directory.EnumerateFiles(settings.GameDirectory + GetWorldFolderPath(w), $"*.XMP", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(5)))
            .ToArray())).ToArray();

        public async Task<uint> LoadFile(Context context, string path, uint baseAddress = 0)
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
        public override Common_Tileset GetTileSet(Context context) 
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

            return new Common_Tileset(tex, Settings.CellSize);
        }

        public int GetPaletteIndex(Context context) {
            // Imitates code at 0x0603586c (US executable)
            // After tile palettes: 19
            int part2 = 19;
            int level = context.Settings.Level;
            switch (context.Settings.World) {
                case World.Jungle:
                    if (level == 9 || level == 16) { // Different palette for Moskito boss, but why Swamp 1?
                        return part2 + 1;
                    } else {
                        return 2;
                    }
                case World.Music:
                    return 0;
                case World.Mountain:
                    return 4;
                case World.Image:
                    if (level == 4 || level == 11) { // Different palette for the boss levels
                        return part2 + 4;
                    } else {
                        return 3;
                    }
                case World.Cave:
                    return 1;
                case World.Cake:
                    return part2 + 3;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The filled v-ram</returns>
        public override void FillVRAM(Context context) {
            string fixPath = GetFixImageFilePath();
            string worldPath = GetWorldImageFilePath(context);
            string levelPath = GetLevelImageFilePath(context);
            var fixImg = context.FileExists(fixPath) ? FileFactory.Read<Array<byte>>(fixPath, context, (y, x) => x.Length = y.CurrentLength) : null;
            var worldImg = context.FileExists(worldPath) ? FileFactory.Read<Array<byte>>(GetWorldImageFilePath(context), context, (y, x) => x.Length = y.CurrentLength) : null;
            var levelImg = context.FileExists(levelPath) ? FileFactory.Read<Array<byte>>(levelPath, context, (y, x) => x.Length = y.CurrentLength) : null;
            
            ImageBuffer buf = new ImageBuffer();
            if (fixImg != null) buf.AddData(fixImg.Value);
            if (worldImg != null) buf.AddData(worldImg.Value);
            if (levelImg != null) buf.AddData(levelImg.Value);
            context.StoreObject("vram", buf);
        }

        public override Texture2D GetSpriteTexture(Context context, PS1_R1_Event e, Common_ImageDescriptor img) {
            if (img.ImageType != 2 && img.ImageType != 3) return null;
            if (img.Unknown2 == 0) return null;
            ImageBuffer buf = context.GetStoredObject<ImageBuffer>("vram");

            // Get the image properties
            var width = img.OuterWidth;
            var height = img.OuterHeight;
            var offset = img.ImageBufferOffset;

            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false) {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            //Debug.Log(string.Format("{0:X8}", img.ImageBufferOffset) + " - " + tex.width + " - " + tex.height);
            var pal = FileFactory.Read<ObjectArray<ARGB1555Color>>(context.GetFile(GetExeFilePath()).StartPointer + GetPalOffsetFilePath(context.Settings), context, (s, x) => x.Length = 25 * 256 * 2);

            var palette = pal.Value;
            var paletteOffset = img.PaletteInfo;

            
            //paletteOffset = (ushort)(256 * (img.Unknown2 >> 4));
            if (img.ImageType == 3) {
                //paletteOffset = 20 * 256;
                paletteOffset = (ushort)((GetPaletteIndex(context) * 256));
            } else {
                paletteOffset = (ushort)((GetPaletteIndex(context) * 256) + ((img.PaletteInfo >> 8)) * 16);
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
        /// <returns>The editor manager</returns>
        public override async Task<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            // Get the paths
            var allfixFilePath = GetAllfixFilePath();
            var worldFilePath = GetWorldFilePath(context.Settings);
            var levelFilePath = GetLevelFilePath(context.Settings);
            var tileSetPaletteFilePath = GetTileSetPaletteFilePath(context);
            var tileSetPaletteIndexTableFilePath = GetTileSetPaletteIndexTableFilePath(context);
            var tileSetFilePath = GetTileSetFilePath(context);
            var mapFilePath = GetMapFilePath(context);

            uint baseAddress = 0x00200000;

            // Load the memory mapped files
            baseAddress += await LoadFile(context, allfixFilePath, baseAddress);

            if (FileSystem.FileExists(context.BasePath + worldFilePath))
                baseAddress += await LoadFile(context, worldFilePath, baseAddress);

            baseAddress += await LoadFile(context, levelFilePath, baseAddress);

            // Load the files
            await LoadFile(context, tileSetPaletteFilePath);
            await LoadFile(context, tileSetPaletteIndexTableFilePath);
            await LoadFile(context, tileSetFilePath);
            await LoadFile(context, mapFilePath);

            if (FileSystem.FileExists(context.BasePath + levelFilePath))
            {
                // Load executable to get the palettes
                await LoadFile(context, GetExeFilePath());

                // Load the texture files
                await LoadFile(context, GetFixImageFilePath());
                await LoadFile(context, GetWorldImageFilePath(context));
                await LoadFile(context, GetLevelImageFilePath(context));
            }

            // NOTE: Big ray data is always loaded at 0x00280000

            // Read the map block
            var map = FileFactory.Read<PS1_R1_MapBlock>(mapFilePath, context);

            PS1_R1_EventBlock eventBlock = null;

            if (FileSystem.FileExists(context.BasePath + levelFilePath))
                // Read the event block
                eventBlock = FileFactory.Read<PS1_R1_EventBlock>(levelFilePath, context);

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
            var context = new Context(settings);

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
                        var newTex = new Texture2D((int)sizes[i].x, (int)sizes[i].y);

                        // Set the pixels
                        newTex.SetPixels(pixels.Reverse().Skip(pixelOffset).Take(newTex.width * newTex.height).ToArray());

                        newTex.Apply();

                        pixelOffset += newTex.width * newTex.height;
                    
                        File.WriteAllBytes(outputPath, newTex.EncodeToPNG());
                    }
                }
                else
                {
                    // Get the output file path
                    var outputPath = Path.Combine(outputDir, FileSystem.ChangeFilePathExtension(file, $".png"));

                    File.WriteAllBytes(outputPath, tex.EncodeToPNG());
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
                var tex = new Texture2D(width, (int)(rawData.Length / width));

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

                File.WriteAllBytes(outputPath, tex.EncodeToPNG());
            }

            // TODO: The width isn't right for all of these - this only includes files from the EU release! US/JP are the same.
            // Export the files
            exportBit("CAK/CAK_01.BIT", 320);
            exportBit("CAK/EXPLOSE.BIT", 320);
            exportBit("CAK/GAT1_SP.BIT", 320);
            exportBit("CAK/GAT3_SP.BIT", 320);
            exportBit("CAK/GAT_F01P.BIT", 320);
            exportBit("CAK/GAT_F03.BIT", 320);
            exportBit("CAK/GAT_F04P.BIT", 320);
            exportBit("CAK/MDK_F01P.BIT", 320);
            exportBit("CAK/VITRAUX.BIT", 320);

            exportBit("CAV/CAV1_SP.BIT", 320);
            exportBit("CAV/CAV_01.BIT", 320);
            exportBit("CAV/CAV_F01.BIT", 320);
            exportBit("CAV/CAV_F04P.BIT", 320);
            exportBit("CAV/CAV_F1.BIT", 320);
            exportBit("CAV/CAV_FJOE.BIT", 320);
            exportBit("CAV/CAV_FSKO.BIT", 320);
            exportBit("CAV/FD_JOPAL.BIT", 320);

            exportBit("IMG/IMG2_SP.BIT", 320);
            exportBit("IMG/IMG4_SP.BIT", 320);
            exportBit("IMG/IMG5_SP.BIT", 320);
            exportBit("IMG/IMG_01.BIT", 320);
            exportBit("IMG/IMG_F01B.BIT", 320);
            exportBit("IMG/IMG_F02.BIT", 320);
            exportBit("IMG/IMG_F04.BIT", 320);
            exportBit("IMG/IMG_F05P.BIT", 320);
            exportBit("IMG/IMG_F06P.BIT", 320);

            exportBit("JUN/JUN2_SP.BIT", 320);
            exportBit("JUN/JUN3_SP.BIT", 320);
            exportBit("JUN/JUN_01.BIT", 320);
            exportBit("JUN/JUN_F01P.BIT", 320);
            exportBit("JUN/JUN_F2JM.BIT", 320);
            exportBit("JUN/JUN_F3JM.BIT", 320);
            exportBit("JUN/JUN_MOPA.BIT", 320);

            exportBit("MON/MON1_SP.BIT", 320);
            exportBit("MON/MON2_SP.BIT", 320);
            exportBit("MON/MON7_SP.BIT", 320);
            exportBit("MON/MON_01.BIT", 320);
            exportBit("MON/MON_F01.BIT", 320);
            exportBit("MON/MON_F02.BIT", 320);
            exportBit("MON/MON_F04.BIT", 320);
            exportBit("MON/MON_F05P.BIT", 320);

            exportBit("MUS/MUS2_SP.BIT", 320);
            exportBit("MUS/MUS3_SP.BIT", 320);
            exportBit("MUS/MUS4_SP.BIT", 320);
            exportBit("MUS/MUS_01.BIT", 320);
            exportBit("MUS/MUS_F02.BIT", 320);
            exportBit("MUS/MUS_F04.BIT", 320);
            exportBit("MUS/MUS_F05P.BIT", 320);
            exportBit("MUS/MUS_F1.BIT", 320);
            exportBit("MUS/MUS_F2D.BIT", 320);

            exportBit("VIGNET/CONTINUE.BIT", 320);
            exportBit("VIGNET/END_01.BIT", 320);
            exportBit("VIGNET/FD01SATP.BIT", 320);
            exportBit("VIGNET/LOGO.BIT", 320);
            exportBit("VIGNET/NWORLD.BIT", 384);
            exportBit("VIGNET/RAYMAN.BIT", 320);
            exportBit("VIGNET/SEGA.BIT", 55);

            // Make an export for each size
            exportBit("VIGNET/VRAM_A.BIT", sizes: new []
            {
                // TODO: The height is not right...
                new Vector2(208, 208), 
                new Vector2(182, 208),
                new Vector2(208, 208), 
            });
            // TODO: Add multiple sizes
            exportBit("VIGNET/VRAM_B.BIT", 208);

            exportVig("VIGNET/VIG_01R.VIG", 219);
            exportVig("VIGNET/VIG_02R.VIG", 231);
            exportVig("VIGNET/VIG_03R.VIG", 257);
            exportVig("VIGNET/VIG_04R.VIG", 200);
            exportVig("VIGNET/VIG_05R.VIG", 146);
            exportVig("VIGNET/VIG_06R.VIG", 203);
            exportVig("VIGNET/VIG_DRK.VIG", 180);
            exportVig("VIGNET/VIG_END.VIG", 191);
            exportVig("VIGNET/VIG_JOE.VIG", 162);
            exportVig("VIGNET/VIG_MUS.VIG", 159);
            exportVig("VIGNET/VIG_RAP.VIG", 255);
            exportVig("VIGNET/VIG_TRZ.VIG", 178);
            exportVig("LANGUE.VIG", 320);
        }

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

            if (path == GetAllfixFilePath())
                return $"Allfix/";
            else if (path == GetWorldFilePath(settings))
                return $"{settings.World}/{settings.World} - ";
            else if (path == GetLevelFilePath(settings))
                return $"{settings.World}/{settings.World}{settings.Level} - ";

            return $"Unknown/";
        }
    }
}