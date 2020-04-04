using System;
using System.IO;
using System.Linq;
using R1Engine.Serialize;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (PS1 - Japan Demo Vol3)
    /// </summary>
    public class PS1_R1JPDemoVol3_Manager : PS1_Manager
    {
        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public override int TileSetWidth => 40;

        /// <summary>
        /// The file info to use
        /// </summary>
        protected override Dictionary<string, PS1FileInfo> FileInfo => PS1FileInfo.fileInfoDemoVol3;

        /// <summary>
        /// Gets the file path for the allfix file
        /// </summary>
        /// <returns>The allfix file path</returns>
        public virtual string GetAllfixFilePath() => $"RAY.FXS";

        public string GetPalettePath(GameSettings settings, int i) => $"RAY{i}_{(settings.World == World.Jungle ? 1 : 2)}.PAL";

        /// <summary>
        /// Gets the file path for the world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public virtual string GetWorldFilePath(GameSettings settings) => $"RAY.WL{(settings.World == World.Jungle ? 1 : 2)}";

        /// <summary>
        /// Gets the file path for the level file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public virtual string GetLevelFilePath(GameSettings settings) => $"RAY.LV{(settings.World == World.Jungle ? 1 : 2)}";
        
        /// <summary>
        /// Gets the file path for the level tile set file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level tile set file path</returns>
        public virtual string GetTileSetFilePath(GameSettings settings) => $"_{GetWorldName(settings.World)}_{settings.Level:00}.R16";

        /// <summary>
        /// Gets the file path for the level map file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level map file path</returns>
        public virtual string GetMapFilePath(GameSettings settings) => $"_{GetWorldName(settings.World)}_{settings.Level:00}.MAP";

        /// <summary>
        /// Gets the name for the world
        /// </summary>
        /// <returns>The world name</returns>
        public override string GetWorldName(World world) => base.GetWorldName(world).Substring(1);

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => EnumHelpers.GetValues<World>().Select(w => new KeyValuePair<World, int[]>(w, Directory.EnumerateFiles(settings.GameDirectory, $"_{GetWorldName(w)}_*.MAP", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(4)))
            .ToArray())).ToArray();

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override IList<ARGBColor> GetTileSet(Context context)
        {
            // Get the file name
            var filename = GetTileSetFilePath(context.Settings);

            // Read the file
            var tileSet = FileFactory.Read<ObjectArray<ARGB1555Color>>(filename, context, (s, x) => x.Length = s.CurrentLength / 2);

            // Return the tile set
            return tileSet.Value;
        }

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The filled v-ram</returns>
        public override PS1_VRAM FillVRAM(Context context)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets the texture for a sprite
        /// </summary>
        /// <param name="img">The image descriptor</param>
        /// <param name="vram">The loaded v-ram</param>
        /// <returns>The sprite texture</returns>
        public Texture2D GetSpriteTexture(Context context, PS1_R1_ImageDescriptor img, byte[] buffer) {
            if (img.ImageType != 2 && img.ImageType != 3) return null;

            // Get the image properties
            var width = img.OuterWidth;
            var height = img.OuterHeight;
            var offset = img.OffsetInBuffer;
            
            var texturePageInfo = img.TexturePageInfo;
            var paletteInfo = img.PaletteInfo;

            // see http://hitmen.c02.at/files/docs/psx/psx.pdf page 37
            int pageX = BitHelpers.ExtractBits(texturePageInfo, 4, 0);
            int pageY = BitHelpers.ExtractBits(texturePageInfo, 1, 4);
            int abr = BitHelpers.ExtractBits(texturePageInfo, 2, 5);
            int tp = BitHelpers.ExtractBits(texturePageInfo, 2, 7); // 0: 4-bit, 1: 8-bit, 2: 15-bit direct


            var pal4 = FileFactory.Read<ObjectArray<ARGB1555Color>>(GetPalettePath(context.Settings, 4), context, (s, x) => x.Length = 256);
            var pal8 = FileFactory.Read<ObjectArray<ARGB1555Color>>(GetPalettePath(context.Settings, 8), context, (s, x) => x.Length = 256);


            // Get palette coordinates
            /*int paletteX = BitHelpers.ExtractBits(paletteInfo, 6, 0);
            int paletteY = BitHelpers.ExtractBits(paletteInfo, 10, 6);

            Debug.Log(paletteX + " - " + paletteY + " - " + pageX + " - " + pageY + " - " + tp + " - " + img.ImageType);*/

            // Select correct palette
            var palette = img.ImageType == 3 ? pal8.Value : pal4.Value;
            var paletteOffset = 16 * img.Unknown2;

            // Create the texture
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false) {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                
            };

            // Default to fully transparent
            //tex.SetPixels(new Color[width * height]);

            //try {
            // Set every pixel
            if (img.ImageType == 3) {
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        var paletteIndex = buffer[offset + width * y + x];

                        // Set the pixel
                        tex.SetPixel(x, height - 1 - y, palette[paletteIndex].GetColor());
                    }
                }
            } else if (img.ImageType == 2) {
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        int actualX = (img.ImageOffsetInPageX + x) / 2;
                        var paletteIndex = buffer[offset + (width * y + x) / 2];
                        if (x % 2 == 0)
                            paletteIndex = (byte)BitHelpers.ExtractBits(paletteIndex, 4, 0);
                        else
                            paletteIndex = (byte)BitHelpers.ExtractBits(paletteIndex, 4, 4);

                        // Set the pixel
                        tex.SetPixel(x, height - 1 - y, palette[paletteOffset + paletteIndex].GetColor());
                    }
                }
            }

            // Apply the changes
            tex.Apply();

            // Return the texture
            return tex;
        }

        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return new GameAction[]
            {

            };
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The editor manager</returns>
        public override async Task<BaseEditorManager> LoadAsync(Context context)
        {
            // Get the file paths
            var allfixPath = GetAllfixFilePath();
            var worldPath = GetWorldFilePath(context.Settings);
            var levelPath = GetLevelFilePath(context.Settings);
            var mapPath = GetMapFilePath(context.Settings);
            var tileSetPath = GetTileSetFilePath(context.Settings);
            var pal4Path = GetPalettePath(context.Settings, 4);
            var pal8Path = GetPalettePath(context.Settings, 8);

            // Load the files
            await LoadExtraFile(context, allfixPath);
            await LoadExtraFile(context, worldPath);
            await LoadExtraFile(context, levelPath);
            await LoadExtraFile(context, mapPath);
            await LoadExtraFile(context, tileSetPath);
            await LoadExtraFile(context, pal4Path);
            await LoadExtraFile(context, pal8Path);


            // Read the files
            var map = FileFactory.Read<PS1_R1_MapBlock>(mapPath, context);
            var lvl = FileFactory.Read<PS1_R1JPDemoVol3_LevFile>(levelPath, context);

            // Load the level
            var editorManager = await LoadAsync(context, map, null);

            // Super hacky event testing
            var index = 0;
            var eventDesigns = new List<KeyValuePair<Pointer, Common_Design>>();
            // Add every event
            foreach (PS1_R1_Event e in lvl.Events)
            {
                Controller.status = $"Loading DES {index}/{lvl.Events.Length}";

                await Controller.WaitIfNecessary();

                // Attempt to find existing DES
                var desIndex = eventDesigns.FindIndex(x => x.Key == e.ImageDescriptorsPointer);

                // Add if not found
                if (desIndex == -1)
                {
                    Common_Design finalDesign = new Common_Design
                    {
                        Sprites = new List<Sprite>(),
                        Animations = new List<Common_Animation>()
                    };

                    // Get every sprite
                    foreach (PS1_R1_ImageDescriptor i in e.ImageDescriptors)
                    {
                        /*Texture2D tex = new Texture2D(i.OuterWidth, i.OuterHeight);

                        for (int y = 0; y < tex.height; y++)
                        {
                            for (int x = 0; x < tex.width; x++)
                            {
                                tex.SetPixel(x, y, Color.blue);
                            }
                        }

                        tex.Apply();*/
                        Texture2D tex = GetSpriteTexture(context, i, e.ImageBuffer);

                        // Add it to the array
                        finalDesign.Sprites.Add(tex == null ? null : Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), 16, 20));
                    }

                    // Animations
                    foreach (var a in e.AnimDescriptors)
                    {
                        // Create the animation
                        var animation = new Common_Animation
                        {
                            Frames = new Common_AnimationPart[a.FrameCount, a.LayersPerFrame],
                            DefaultFrameXPosition = a.Frames.FirstOrDefault()?.XPosition ?? -1,
                            DefaultFrameYPosition = a.Frames.FirstOrDefault()?.YPosition ?? -1,
                            DefaultFrameWidth = a.Frames.FirstOrDefault()?.Width ?? -1,
                            DefaultFrameHeight = a.Frames.FirstOrDefault()?.Height ?? -1,
                        };

                        // The layer index
                        var layer = 0;

                        // Create each frame
                        for (int i = 0; i < a.FrameCount; i++)
                        {
                            // Create each layer
                            for (var layerIndex = 0; layerIndex < a.LayersPerFrame; layerIndex++)
                            {
                                var animationLayer = a.Layers[layer];
                                layer++;

                                // Create the animation part
                                var part = new Common_AnimationPart
                                {
                                    SpriteIndex = animationLayer.ImageIndex,
                                    X = animationLayer.XPosition,
                                    Y = animationLayer.YPosition,
                                    Flipped = animationLayer.IsFlipped
                                };

                                // Add the texture
                                animation.Frames[i, layerIndex] = part;
                            }
                        }
                        // Add the animation to list
                        finalDesign.Animations.Add(animation);
                    }

                    // Add to the designs
                    eventDesigns.Add(new KeyValuePair<Pointer, Common_Design>(e.ImageDescriptorsPointer, finalDesign));
                    
                    // Set the index
                    desIndex = eventDesigns.Count - 1;
                }

                // Instantiate event prefab using LevelEventController
                var ee = Controller.obj.levelEventController.AddEvent(e.Type, e.Etat, e.SubEtat, e.XPosition, e.YPosition, desIndex, 0, e.OffsetBX, e.OffsetBY, e.OffsetHY, e.FollowSprite, e.Hitpoints, e.Layer, e.HitSprite, e.FollowEnabled, null, null, lvl.EventLinkTable[index]);

                // Add the event
                editorManager.Level.Events.Add(ee);

                index++;
            }

            return new PS1EditorManager(editorManager.Level, editorManager.Context, this, eventDesigns.Select(x => x.Value).ToArray(), new Common_EventState[0][][]);
        }
    }
}