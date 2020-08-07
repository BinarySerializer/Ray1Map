using System;
using R1Engine.Serialize;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Game manager for the Jaguar prototype
    /// </summary>
    public class Jaguar_R1Proto_Manager : Jaguar_R1Demo_Manager
    {
        #region Values and paths

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) =>
            new KeyValuePair<World, int[]>[]
            {
                new KeyValuePair<World, int[]>(World.Jungle, new int[]
                {
                    1
                }), 
            };

        public override uint EventCount => 28;

        /// <summary>
        /// Gets the vignette addresses and widths
        /// </summary>
        protected override KeyValuePair<uint, int>[] GetVignette => null;

        protected override Dictionary<SpecialEventType, Pointer> GetSpecialEventPointers(Context context) => new Dictionary<SpecialEventType, Pointer>();

        public override uint[] AdditionalEventDefinitionPointers => null;

        #endregion

        #region Manager Methods

        /// <summary>
        /// Exports every sprite from the game
        /// </summary>
        /// <param name="baseGameSettings">The game settings</param>
        /// <param name="outputDir">The output directory</param>
        /// <param name="exportAnimFrames">True if animation frames should be exported, false if sprites should be exported</param>
        /// <returns>The task</returns>
        public override async Task ExportAllSpritesAsync(GameSettings baseGameSettings, string outputDir, bool exportAnimFrames)
        {
            // Create the context
            using (var context = new Context(baseGameSettings))
            {
                // Load the game data
                await LoadFilesAsync(context);

                // Serialize the rom
                var rom = FileFactory.Read<Jaguar_R1_ROM>(GetROMFilePath, context);

                var usedNames = new List<string>();
                // Helper method to get the name for a pointer
                string GetPointerName(Pointer p)
                {
                    string name = rom.References.FirstOrDefault(x => x.DataPointer == p && !usedNames.Contains(x.String))
                                   ?.String ?? $"{rom.References.First(x => x.DataPointer == p)?.String}";

                    usedNames.Add(name);

                    return name;
                }

                // Enumerate every graphics
                foreach (var sprKey in rom.ImageBuffers.Keys)
                {
                    // Get data
                    var imgBuffer = rom.ImageBuffers[sprKey];
                    var imgDescr = rom.ImageBufferDescriptors[sprKey];
                    var pal = rom.SpritePalette;
                    var desName = rom.References.Last(x => x.DataValue == sprKey).String.Substring(4);

                    if (exportAnimFrames)
                    {
                        // Enumerate every event definition which uses this image buffer
                        foreach (var ed in rom.EventDefinitions.Where(x => x.ImageBufferMemoryPointerPointer >> 8 == sprKey))
                        {
                            // Create template
                            var animations = new List<ExportAnim>();

                            // Add single animation
                            if (ed.AnimationLayers != null) 
                                animations.Add(new ExportAnim()
                                {
                                    Anim = ed.ToCommonAnimation(),
                                    AnimationSpeed = 1,
                                    Pointer = ed.AnimationPointer - 4
                                });

                            // Add state animations
                            if (ed.States != null)
                                 animations.AddRange(ed.States.Where(x => x.Animation?.Layers != null).Select(x => new ExportAnim()
                                 {
                                     Anim = x.Animation.ToCommonAnimation(ed),
                                     AnimationSpeed = (byte)(x.AnimationSpeed & 0b1111),
                                     Pointer = x.Animation.Offset
                                 }));

                            // Add complex data animations
                            HashSet<Pointer> complexDataSeen = new HashSet<Pointer>();
                            void AddComplexData(Jaguar_R1_EventComplexData cd)
                            {
                                if (cd == null || complexDataSeen.Contains(cd.Offset)) 
                                    return;
                                
                                complexDataSeen.Add(cd.Offset);
                                var animComplex = cd.States?.Where(x => x.Layers != null).Select(x => new ExportAnim()
                                {
                                    Anim = x.ToCommonAnimation(ed),
                                    AnimationSpeed = (byte)(x.UnkBytes[0] & 0b1111),
                                    Pointer = x.AnimationPointer - 4
                                });

                                if (animComplex != null && cd.ImageDescriptorsPointer == imgDescr.First().Offset) 
                                    animations.AddRange(animComplex);

                                if (cd.Transitions != null)
                                {
                                    foreach (Jaguar_R1_EventComplexDataTransition t in cd.Transitions)
                                        AddComplexData(t.ComplexData);
                                }
                            }
                            AddComplexData(ed.ComplexData);

                            if (ed.ComplexData?.States != null)
                                animations.AddRange(ed.ComplexData.States.Where(x => x.Layers != null).Select(x => new ExportAnim()
                                {
                                    Anim = x.ToCommonAnimation(ed),
                                    AnimationSpeed = (byte)(x.UnkBytes[0] & 0b1111),
                                    Pointer = x.AnimationPointer - 4
                                }));

                            // Skip if no animations
                            if (animations.Count == 0)
                                continue;

                            // Get every sprite
                            var sprites = imgDescr.Select(x => GetSpriteTexture(x, pal, imgBuffer)).ToArray();

                            // Export every animation
                            foreach (var anim in animations.Where(x => x.Anim.Frames.Any()))
                            {
                                var animName = GetPointerName(anim.Pointer);

                                // Get the folder
                                var animFolderPath = Path.Combine(outputDir, "Jungle", desName, $"{animName}-{anim.AnimationSpeed}");

                                int? frameWidth = null;
                                int? frameHeight = null;

                                var layersPerFrame = anim.Anim.Frames.First().Layers.Length;
                                var frameCount = anim.Anim.Frames.Length;

                                for (int dummyFrame = 0; dummyFrame < frameCount; dummyFrame++)
                                {
                                    for (int dummyLayer = 0; dummyLayer < layersPerFrame; dummyLayer++)
                                    {
                                        var l = anim.Anim.Frames[dummyFrame].Layers[dummyLayer];

                                        if (l.ImageIndex < sprites.Length)
                                        {
                                            var sprite = sprites[l.ImageIndex];

                                            if (sprite != null)
                                            {
                                                var w = sprite.width + l.XPosition;
                                                var h = sprite.height + l.YPosition;

                                                if (frameWidth == null || frameWidth < w)
                                                    frameWidth = w;

                                                if (frameHeight == null || frameHeight < h)
                                                    frameHeight = h;
                                            }
                                        }
                                    }
                                }

                                // Create each animation frame
                                for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
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
                                    for (var layerIndex = 0; layerIndex < layersPerFrame; layerIndex++)
                                    {
                                        var animationLayer = anim.Anim.Frames[frameIndex].Layers[layerIndex];

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
                    }
                    else
                    {
                        var imgIndex = 0;
                        foreach (var d in imgDescr)
                        {
                            string filename = Path.Combine(outputDir, desName, $"{imgIndex}.png");

                            // Get the texture
                            var tex = GetSpriteTexture(d, pal, imgBuffer);

                            // Export if not null
                            if (tex != null)
                                Util.ByteArrayToFile(filename, tex.EncodeToPNG());

                            imgIndex++;
                        }
                    }
                }
            }
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
                new GameAction("Export Sprites", false, true, (input, output) => ExportAllSpritesAsync(settings, output, false)),
                new GameAction("Export Animation Frames", false, true, (input, output) => ExportAllSpritesAsync(settings, output, true)),
                new GameAction("Export Vignette", false, true, (input, output) => ExtractVignetteAsync(settings, output)),
                new GameAction("Extract Data Blocks", false, true, (input, output) => ExportDataBlocks(settings, output)),
                new GameAction("Fix memory dump byte swapping", false, false, (input, output) => FixMemoryDumpByteSwapping(settings)),
                new GameAction("Export Palettes", false, true, (input, output) => ExportPaletteImage(settings, output)),
            };
        }

        /// <summary>
        /// Extracts all vignette
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="outputPath">The path to extract to</param>
        /// <returns>The task</returns>
        public override async Task ExtractVignetteAsync(GameSettings settings, string outputPath)
        {
            // Create a context
            using (var context = new Context(settings))
            {
                // Get a deserializer
                var s = context.Deserializer;

                // Add the file
                await LoadExtraFile(context, GetROMFilePath, GetROMBaseAddress);

                var vigRefs = new []
                {
                    new
                    {
                        Key = Jaguar_R1Proto_References.jun_plan0,
                        Width = 192,
                        Height = 246
                    },
                    new
                    {
                        Key = Jaguar_R1Proto_References.jun_roseau,
                        Width = 48,
                        Height = 99
                    },
                    new
                    {
                        Key = Jaguar_R1Proto_References.jun_feuilles,
                        Width = 144,
                        Height = 67
                    },
                };

                // Export every vignette
                foreach (var vig in vigRefs)
                {
                    s.DoAt(GetDataPointer(context, vig.Key), () =>
                    {
                        var tex = new Texture2D(vig.Width, vig.Height)
                        {
                            filterMode = FilterMode.Point,
                            wrapMode = TextureWrapMode.Clamp
                        };

                        var values = s.SerializeObjectArray<RGB556Color>(default, tex.width * vig.Height);
                        
                        for (int y = 0; y < tex.height; y++)
                        {
                            for (int x = 0; x < tex.width; x++)
                            {
                                tex.SetPixel(x, tex.height - y - 1, values[y * tex.width + x].GetColor());
                            }
                        }

                        tex.Apply();

                        Util.ByteArrayToFile(Path.Combine(outputPath, $"{vig.Key}.png"), tex.EncodeToPNG());
                    });
                }
            }
        }

        public override async Task ExportPaletteImage(GameSettings settings, string outputPath)
        {
            using (var context = new Context(settings))
            {
                // Load the files
                await LoadFilesAsync(context);

                // Serialize the rom
                var rom = FileFactory.Read<Jaguar_R1_ROM>(GetROMFilePath, context);

                // Export
                PaletteHelpers.ExportPalette(Path.Combine(outputPath, $"{settings.GameModeSelection}.png"), rom.SpritePalette, optionalWrap: 256);
            }
        }

        public async Task ExportDataBlocks(GameSettings settings, string outputPath)
        {
            // Create the context
            using (var context = new Context(settings))
            {
                // Load the rom
                await LoadFilesAsync(context);

                // Parse the rom
                var rom = FileFactory.Read<Jaguar_R1_ROM>(GetROMFilePath, context);

                // Get and order all references
                var refs = rom.References.Where(x => x.DataPointer != null).OrderBy(x => x.DataPointer.FileOffset).ToArray();

                var s = context.Deserializer;

                for (int i = 0; i < refs.Length; i++)
                {
                    var blockRef = refs[i];
                    s.DoAt(blockRef.DataPointer, () =>
                    {
                        var path = Path.Combine(outputPath, $"{blockRef.DataPointer.FileOffset}_0x{blockRef.DataPointer.StringAbsoluteOffset}_{blockRef.String}");
                        var length = i != (refs.Length - 1) ? refs[i + 1].DataPointer - blockRef.DataPointer : 0xB96A8 - blockRef.DataPointer.FileOffset;

                        var buffer = s.SerializeArray<byte>(default, length, name: blockRef.String);

                        Util.ByteArrayToFile(path, buffer);
                    });
                }
            }
        }

        public Pointer GetDataPointer(Context context, Jaguar_R1Proto_References reference) => FileFactory.Read<Jaguar_R1_ROM>(GetROMFilePath, context).GetProtoDataPointer(reference);

        #endregion
    }
}