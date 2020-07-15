using R1Engine.Serialize;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        protected override KeyValuePair<uint, int>[] GetVignette => new KeyValuePair<uint, int>[]
        {
            // TODO: Fill out
        };

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
                //new GameAction("Extract Vignette", false, true, (input, output) => ExtractVignetteAsync(settings, output)),
                new GameAction("Extract Data Blocks", false, true, (input, output) => ExportDataBlocks(settings, output)),
                new GameAction("Fix memory dump byte swapping", false, false, (input, output) => FixMemoryDumpByteSwapping(settings)),
                //new GameAction("Export Palettes", false, true, (input, output) => ExportPaletteImage(settings, output)),
            };
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
                        var path = Path.Combine(outputPath, $"{blockRef.String}_{blockRef.DataPointer.StringAbsoluteOffset}");
                        var length = i != (refs.Length - 1) ? refs[i + 1].DataPointer - blockRef.DataPointer : s.CurrentLength - blockRef.DataPointer.FileOffset;

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