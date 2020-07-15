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
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return new GameAction[]
            {
                //new GameAction("Export Sprites", false, true, (input, output) => ExportAllSpritesAsync(settings, output, false)),
                //new GameAction("Export Animation Frames", false, true, (input, output) => ExportAllSpritesAsync(settings, output, true)),
                new GameAction("Extract Vignette", false, true, (input, output) => ExtractVignetteAsync(settings, output)),
                new GameAction("Extract Data Blocks", false, true, (input, output) => ExportDataBlocks(settings, output)),
                new GameAction("Fix memory dump byte swapping", false, false, (input, output) => FixMemoryDumpByteSwapping(settings)),
                //new GameAction("Export Palettes", false, true, (input, output) => ExportPaletteImage(settings, output)),
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