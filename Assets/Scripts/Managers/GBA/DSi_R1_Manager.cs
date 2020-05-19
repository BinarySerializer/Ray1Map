using R1Engine.Serialize;
using System.IO;
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

        /// <summary>
        /// True if colors are 4-bit, false if they're 8-bit
        /// </summary>
        public override bool Is4Bit => false;

        /// <summary>
        /// True if palette indexes are used, false if not
        /// </summary>
        public override bool UsesPaletteIndex => false;

        #endregion

        #region Manager Methods

        public override async Task ExtractVignetteAsync(GameSettings settings, string outputDir) {
            // Create a context
            using (var context = new Context(settings)) {
                // Load the ROM
                await LoadFilesAsync(context);

                // Read data from the ROM
                var data = FileFactory.Read<DSi_R1_DataFile>(GetROMFilePath, context);

                // Extract every background vignette
                for (int i = 0; i < data.BackgroundVignettes.Length; i++) {
                    // Get the vignette
                    var vig = data.BackgroundVignettes[i];

                    // Make sure we have image data
                    if (vig.ImageData == null)
                        continue;

                    // Get the texture
                    var tex = GetVignetteTexture(vig);

                    // Save the texture
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"BG_{i}.png"), tex.EncodeToPNG());
                }

                // Extract every intro vignette
                /*for (int i = 0; i < data.IntroVignettes.Length; i++) {
                    // Get the vignette
                    var vig = rom.IntroVignettes[i];

                    // Get the texture
                    var tex = GetVignetteTexture(vig);

                    // Save the texture
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"Intro_{i}.png"), tex.EncodeToPNG());
                }

                // Extract world map vignette

                // Get the world map texture
                var worldMapTex = GetVignetteTexture(rom.WorldMapVignette);

                // Save the texture
                Util.ByteArrayToFile(Path.Combine(outputDir, $"WorldMap.png"), worldMapTex.EncodeToPNG());*/
            }
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <returns>The editor manager</returns>
        public override async Task<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            Controller.status = $"Loading data";
            await Controller.WaitIfNecessary();

            // Read the data
            var data = FileFactory.Read<DSi_R1_DataFile>(GetROMFilePath, context);
            return await LoadAsync(context, loadTextures, data.LevelMapData, data.LevelEventData, data.SpritePalette);
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