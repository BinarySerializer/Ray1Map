using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Base game manager for DSi
    /// </summary>
    public class DSi_R1_Manager : GBA_R1_Manager
    {
        #region Values and paths

        /// <summary>
        /// The amount of levels in the game
        /// </summary>
        public new const int LevelCount = 22 + 18 + 13 + 13 + 12 + 4;

        /// <summary>
        /// Gets the available levels ordered based on the global level array
        /// </summary>
        public override World[] GetGlobalLevels => new World[]
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
        };

        /// <summary>
        /// Gets the file path to the ROM file
        /// </summary>
        public override string GetROMFilePath => $"0.bin";

        #endregion

        #region Manager Methods

        /// <summary>
        /// Exports every sprite from the game
        /// </summary>
        /// <param name="baseGameSettings">The game settings</param>
        /// <param name="outputDir">The output directory</param>
        /// <returns>The task</returns>
        public override Task ExportAllSpritesAsync(GameSettings baseGameSettings, string outputDir) => throw new NotImplementedException();

        public override Task ExportUnusedSpritesAsync(GameSettings baseGameSettings, string outputDir) => throw new NotImplementedException();

        public override Task ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new NotImplementedException();


        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="levelMapData">The level to get the tile set for</param>
        /// <returns>The tile set to use</returns>
        public Common_Tileset GetTileSet(Context context, DSi_R1_LevelMapData levelMapData) {
            // Read the tiles
            const int block_size = 0x40;
            //Array<byte> tiles = FileFactory.Read<Array<byte>>(levelMapData.TileData, context, (s, a) => a.Length = block_size * ((uint)maxBlockIndex + 1));

            uint length = (uint)levelMapData.TileBlockIndices.Length * 8 * 8;

            // Get the tile-set texture
            var tex = new Texture2D(256, Mathf.CeilToInt(length / 256f / Settings.CellSize) * Settings.CellSize) {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            for (int i = 0; i < levelMapData.TileBlockIndices.Length; i++) {
                ushort blockIndex = levelMapData.TileBlockIndices[i];

                var x = ((i / 4) * 2) % (256 / 8) + ((i % 2) == 0 ? 0 : 1);
                var y = (((i / 4) * 2) / (256 / 8)) * 2 + ((i % 4) < 2 ? 0 : 1);

                var curOff = block_size * blockIndex;
                /*if (levelMapData.TilePaletteIndices[i] >= 10) {
                    Debug.LogWarning("Tile palette index exceeded 9: " + i + " - " + levelMapData.TilePaletteIndices[i]);
                }*/
                FillSpriteTextureBlock(tex, 0, 0, x, y, levelMapData.TileData, curOff, levelMapData.TilePalette, 0, false, reverseHeight: false, is4Bit: false);
            }

            tex.Apply();

            return new Common_Tileset(tex, Settings.CellSize);
        }


        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <returns>The editor manager</returns>
        public override async Task<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            // TODO: Eventually merge this with the LoadAsync method in the GBA manager

            var data = FileFactory.Read<DSi_R1_DataFile>(GetROMFilePath, context);
            data.LevelMapData.SerializeLevelData(context.Deserializer);

            var map = data.LevelMapData.MapData;

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
            commonLev.Maps[0].TileSet[0] = GetTileSet(context, data.LevelMapData);
            //commonLev.Maps[0].TileSet[0] = new Common_Tileset(Enumerable.Repeat(new ARGBColor(0, 0, 0, 0), 16 * 16).ToArray(), 1, 16);

            Controller.status = $"Loading events";
            await Controller.WaitIfNecessary();

            var eventDesigns = new Dictionary<Pointer, Common_Design>();
            var eventETA = new Dictionary<Pointer, Common_EventState[][]>();

            var eventData = data.LevelEventData;

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
                        // TODO: Fix and get palette
                        // Get the texture for the sprite, or null if not loading textures
                        //Texture2D tex = loadTextures ? GetSpriteTexture(context, graphics, img, Enumerable.Repeat(new ARGB1555Color(50, 50, 50), 512).ToArray()) : null;
                        Texture2D tex = null;

                        // Add it to the array
                        finalDesign.Sprites.Add(tex == null ? null : Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), 16, 20));
                    }

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
                                current[ii] = new Common_EventState[dat.ETA[ii].Length];

                            if (dat.ETA[ii].Length > current[ii].Length)
                                Array.Resize(ref current[ii], dat.ETA[ii].Length);

                            for (int jj = 0; jj < dat.ETA[ii].Length; jj++)
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
            for (int cellY = 0; cellY < map.Height; cellY++)
            {
                for (int cellX = 0; cellX < map.Width; cellX++)
                {
                    // Get the cell
                    var cell = map.Tiles[cellY * map.Width + cellX];

                    // Set the common tile
                    commonLev.Maps[0].Tiles[cellY * map.Width + cellX] = new Common_Tile()
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
        /// Preloads all the necessary files into the context
        /// </summary>
        /// <param name="context">The serialization context</param>
        public override async Task LoadFilesAsync(Context context)
        {
            await LoadExtraFile(context, GetROMFilePath, 0x21E0F00);
        }

        #endregion
    }
}