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
        /// The file info to use
        /// </summary>
        protected override Dictionary<string, PS1FileInfo> FileInfo => null;

        /// <summary>
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The world folder path</returns>
        public string GetWorldFolderPath(World world) => GetWorldName(world) + "/";

        /// <summary>
        /// Gets the allfix file path
        /// </summary>
        /// <returns>The allfix file path</returns>
        public string GetAllfixFilePath() => "RAY.DTA";

        /// <summary>
        /// Gets the world data file path
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The world data file path</returns>
        public string GetWorldFilePath(Context context) => GetWorldFolderPath(context.Settings.World) + $"{GetWorldName(context.Settings.World)}.DTA";

        /// <summary>
        /// Gets the level data file path
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The level data file path</returns>
        public string GetLevelFilePath(Context context) => GetWorldFolderPath(context.Settings.World) + $"{GetWorldName(context.Settings.World)}{context.Settings.Level:00}.DTA";

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
            var tex = tileSet.ToTexture(CellSize * TileSetWidth);

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

            return new Common_Tileset(tex, CellSize);
        }

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The filled v-ram</returns>
        public override void FillVRAM(Context context) {
            string worldPath = GetWorldImageFilePath(context);
            var fixImg = FileFactory.Read<Array<byte>>(GetFixImageFilePath(), context, (y, x) => x.Length = y.CurrentLength);
            var worldImg = context.FileExists(worldPath) ? FileFactory.Read<Array<byte>>(GetWorldImageFilePath(context), context, (y, x) => x.Length = y.CurrentLength) : null;
            var levelImg = FileFactory.Read<Array<byte>>(GetLevelImageFilePath(context), context, (y, x) => x.Length = y.CurrentLength);
            
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
            //var pal = FileFactory.Read<ObjectArray<ARGB1555Color>>("0.PAL", context, (s, x) => x.Length = s.CurrentLength / 2);
            var pal = FileFactory.Read<ObjectArray<ARGB1555Color>>(GetTileSetPaletteFilePath(context), context, (s, x) => x.Length = s.CurrentLength / 2);
            var palette = pal.Value;
            var paletteOffset = img.PaletteInfo % palette.Length; // TODO: Load palettes from "0", don't do modulo

            // Set every pixel
            if (img.ImageType == 3) {
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        var paletteIndex = buf.GetPixel8((uint)(offset + width * y + x));

                        // Set the pixel
                        var color = palette[paletteOffset + paletteIndex].GetColor();
                        if (color.r == 0 && color.g == 0 && color.b == 0) {
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
                        if (color.r == 0 && color.g == 0 && color.b == 0) {
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
        /// <returns>The editor manager</returns>
        public override async Task<BaseEditorManager> LoadAsync(Context context)
        {
            // Get the paths
            var allfixFilePath = GetAllfixFilePath();
            var worldFilePath = GetWorldFilePath(context);
            var levelFilePath = GetLevelFilePath(context);
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
                await LoadFile(context, "0.PAL"); // TODO: read the palettes from the '0' executable file
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
            return await LoadAsync(context, map, eventBlock?.Events, eventBlock?.EventLinkingTable.Select(b => (ushort)b).ToArray());
        }
    }
}