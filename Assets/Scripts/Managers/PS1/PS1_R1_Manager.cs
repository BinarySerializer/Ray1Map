using System;
using R1Engine.Serialize;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_Manager : PS1_BaseXXX_Manager
    {
        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public override int TileSetWidth => 16;

        /// <summary>
        /// Gets the file info to use
        /// </summary>
        /// <param name="settings">The game settings</param>
        protected override Dictionary<string, PS1FileInfo> GetFileInfo(GameSettings settings) => settings.GameModeSelection == GameModeSelection.RaymanPS1US ? PS1FileInfo.fileInfoUS : PS1FileInfo.fileInfoPAL;

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override Common_Tileset GetTileSet(Context context)
        {
            // Get the file name
            var filename = GetWorldFilePath(context.Settings);

            // Read the file
            var worldFile = FileFactory.Read<PS1_R1_WorldFile>(filename, context);

            int tileCount = worldFile.TilePaletteIndexTable.Length;
            int width = TileSetWidth * Settings.CellSize;
            int height = (worldFile.PalettedTiles.Length) / width;

            var pixels = new ARGB1555Color[width * height];

            int tile = 0;

            for (int yB = 0; yB < height; yB += 16)
            for (int xB = 0; xB < width; xB += 16, tile++)
            for (int y = 0; y < Settings.CellSize; y++)
            for (int x = 0; x < Settings.CellSize; x++)
            {
                int pixel = x + xB + (y + yB) * width;
                
                if (tile >= tileCount)
                {
                    // Set dummy data
                    pixels[pixel] = new ARGB1555Color();
                }
                else
                {
                    byte tileIndex1 = worldFile.TilePaletteIndexTable[tile];
                    byte tileIndex2 = worldFile.PalettedTiles[pixel];
                    pixels[pixel] = worldFile.TilePalettes[tileIndex1][tileIndex2];
                }
            }

            return new Common_Tileset(pixels, TileSetWidth, Settings.CellSize);
        }

        // TODO: Fix & support for JP version
        public string GetLevelBackgroundFilePath(GameSettings settings, bool returnNullIfNoParallax)
        {
            return null;

            var index = -1;

            // TODO: Add bonus levels
            if (settings.World == World.Jungle)
            {
                switch (settings.Level)
                {
                    case 2:
                    case 3:
                    case 4:
                    case 10:
                    case 11:
                    case 12:
                    case 14:
                    case 15:
                    case 17:
                        index = returnNullIfNoParallax ? -1 : 1;
                        break;

                    case 1:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 13:
                        index = 2;
                        break;

                    case 9:
                        index = returnNullIfNoParallax ? -1 : 3;
                        break;

                    case 16:
                        index = returnNullIfNoParallax ? -1 : 4;
                        break;
                }
            }
            else if (settings.World == World.Music)
            {
                switch (settings.Level)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 5:
                        index = 2;
                        break;

                    case 4:
                        index = returnNullIfNoParallax ? -1 : 3;
                        break;
                }
            }

            return index == -1 ? null : $"RAY/IMA/FND/{GetWorldName(settings.World)}F{index}.XXX";
        }

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The filled v-ram</returns>
        public override void FillVRAM(Context context)
        {
            // Read the files
            var allFix = FileFactory.Read<PS1_R1_AllfixFile>(GetAllfixFilePath(context.Settings), context);
            var world = FileFactory.Read<PS1_R1_WorldFile>(GetWorldFilePath(context.Settings), context);
            var levelTextureBlock = FileFactory.Read<PS1_R1_LevFile>(GetLevelFilePath(context.Settings), context).TextureBlock;

            //var bgPath = GetLevelBackgroundFilePath(context.Settings, true);
            //ARGB1555Color[][] bgPalette = new ARGB1555Color[0][];
            //if (bgPath != null)
            //    bgPalette = FileFactory.Read<PS1_R1_BackgroundVignetteFile>(bgPath, context).ParallaxPalettes;

            PS1_VRAM vram = new PS1_VRAM();

            // skip loading the backgrounds for now. They take up 320 (=5*64) x 256 per background
            // 2 backgrounds are stored underneath each other vertically, so this takes up 10 pages in total
            vram.currentXPage = 5;

            // Since skippedPagesX is uneven, and all other data takes up 2x2 pages, the game corrects this by
            // storing the first bit of sprites we load as 1x2
            byte[] cageSprites = new byte[128 * (256 * 2 - 8)];
            Array.Copy(allFix.TextureBlock, 0, cageSprites, 0, cageSprites.Length);
            byte[] allFixSprites = new byte[allFix.TextureBlock.Length - cageSprites.Length];
            Array.Copy(allFix.TextureBlock, cageSprites.Length, allFixSprites, 0, allFixSprites.Length);
            byte[] unknown = new byte[128 * 8];
            vram.AddData(unknown, 128);
            vram.AddData(cageSprites, 128);
            vram.AddData(allFixSprites, 256);

            vram.AddData(world.TextureBlock, 256);
            vram.AddData(levelTextureBlock, 256);

            // Palettes start at y = 256 + 234 (= 490), so page 1 and y=234
            int paletteY = 234;
            /*vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette3.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette4.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);*/
            vram.AddDataAt(12, 1, 0, paletteY++, world.EventPalette1.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, world.EventPalette2.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette1.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette5.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette6.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette2.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);

            // TODO: How are these aligned? Seems different for every background...
            //// Add background parallax palettes
            //foreach (var p in bgPalette.Reverse())
            //    vram.AddDataAt(12, 1, 0, paletteY++, p.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            
            paletteY += 13 - world.TilePalettes.Length;
            
            // Add tile palettes
            foreach (var p in world.TilePalettes)
                vram.AddDataAt(12, 1, 0, paletteY++, p.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);

            context.StoreObject("vram", vram);
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <returns>The editor manager</returns>
        public override async Task<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            // Read the allfix file
            await LoadExtraFile(context, GetAllfixFilePath(context.Settings));
            FileFactory.Read<PS1_R1_AllfixFile>(GetAllfixFilePath(context.Settings), context);

            Controller.status = $"Loading world file";

            await Controller.WaitIfNecessary();

            // Read the world file
            await LoadExtraFile(context, GetWorldFilePath(context.Settings));
            FileFactory.Read<PS1_R1_WorldFile>(GetWorldFilePath(context.Settings), context);

            Controller.status = $"Loading map data";

            // Read the level data
            await LoadExtraFile(context, GetLevelFilePath(context.Settings));
            var level = FileFactory.Read<PS1_R1_LevFile>(GetLevelFilePath(context.Settings), context);

            // Load the background
            var bgPath = GetLevelBackgroundFilePath(context.Settings, true);
            if (bgPath != null)
                await LoadExtraFile(context, bgPath);

            // Load the level
            return await LoadAsync(context, level.MapData, level.EventData.Events, level.EventData.EventLinkingTable.Select(x => (ushort)x).ToArray(), loadTextures, bgPath == null ? null : level.BackgroundData);
        }

        /// <summary>
        /// Saves the specified level
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="editorManager">The editor manager</param>
        public override void SaveLevel(Context context, BaseEditorManager editorManager)
        {
            var em = (PS1EditorManager)editorManager;
            var commonLevelData = editorManager.Level;

            // Get the level file path
            var lvlPath = GetLevelFilePath(context.Settings);

            // Get the level data
            var lvlData = context.GetMainFileObject<PS1_R1_LevFile>(lvlPath);

            // Update the tiles
            for (int y = 0; y < lvlData.MapData.Height; y++)
            {
                for (int x = 0; x < lvlData.MapData.Width; x++)
                {
                    // Get the tiles
                    var tile = lvlData.MapData.Tiles[y * lvlData.MapData.Width + x];
                    var commonTile = commonLevelData.Maps[0].Tiles[y * lvlData.MapData.Width + x];

                    // Update the tile
                    tile.CollisionType = commonTile.CollisionType;
                    tile.TileMapY = (int)Math.Floor(commonTile.TileSetGraphicIndex / (double)TileSetWidth);
                    tile.TileMapX = commonTile.TileSetGraphicIndex - (Settings.CellSize * tile.TileMapY);
                }
            }

            var newEvents = commonLevelData.EventData.Select(e =>
            {
                // Get the DES data
                var des = em.DESCollection[e.DESKey];
                var eta = em.ETACollection[e.ETAKey];

                var newEvent = new EventData
                {
                    ImageDescriptorsPointer = des.ImageDescriptorsPointer,
                    AnimDescriptorsPointer = des.AnimDescriptorsPointer,
                    ImageBufferPointer = des.ImageBufferPointer,
                    ETAPointer = eta.ETAPointer,

                    // Ignore since these get set automatically later...
                    //CommandsPointer = null,
                    //LabelOffsetsPointer = null,

                    PS1Demo_Unk1 = new byte[40],

                    XPosition = (ushort)e.XPosition,
                    YPosition = (ushort)e.YPosition,

                    ImageDescriptorCount = (ushort)des.ImageDescriptors.Length,

                    Unk_98 = new byte[5],

                    OffsetBX = (byte)e.OffsetBX,
                    OffsetBY = (byte)e.OffsetBY,

                    Etat = (byte)e.Etat,
                    SubEtat = (byte)e.SubEtat,

                    OffsetHY = (byte)e.OffsetHY,
                    FollowSprite = (byte)e.FollowSprite,
                    HitPoints = (byte)e.HitPoints,

                    Layer = (byte)e.Layer,
                    Type = (EventType)e.Type,
                    HitSprite = (byte)e.HitSprite,

                    AnimDescriptorCount = (byte)des.AnimDescriptors.Length,

                    ImageDescriptors = des.ImageDescriptors,
                    AnimDescriptors = des.AnimDescriptors,
                    Commands = e.CommandCollection,
                    LabelOffsets = e.LabelOffsets,
                    ETA = eta.ETA,
                    ImageBuffer = des.ImageBuffer
                };

                newEvent.SetFollowEnabled(context.Settings, e.FollowEnabled);

                return newEvent;
            }).ToArray();

            var newEventLinkTable = commonLevelData.EventData.Select(x => (byte)x.LinkIndex).ToArray();

            // Create the edited block which we append to the file
            lvlData.EditedBlock = new PS1_R1_EditedLevelBlock();

            lvlData.EditedBlock.UpdateAndFillDataBlock(lvlData.Offset + lvlData.FileSize, lvlData.EventData, newEvents, newEventLinkTable, context.Settings);

            // TODO: When writing make sure that ONLY the level file gets recreated - do not touch the other files (ignore DoAt if the file needs to be switched based on some setting?)
            // Save the file
            FileFactory.Write<PS1_R1_LevFile>(lvlPath, context);
        }
    }
}