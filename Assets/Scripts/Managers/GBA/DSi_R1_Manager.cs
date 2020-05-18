using R1Engine.Serialize;
using System.Threading.Tasks;

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

            return await LoadAsync(context, loadTextures, data.LevelMapData, data.LevelEventData, data.SpritePalettes);
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