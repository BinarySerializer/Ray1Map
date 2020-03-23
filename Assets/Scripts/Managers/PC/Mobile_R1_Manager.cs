using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman Classic (Mobile)
    /// </summary>
    public class Mobile_R1_Manager : PC_R1_Manager
    {
        #region Values and paths

        /// <summary>
        /// Gets the base path for the game data
        /// </summary>
        /// <returns>The data path</returns>
        public override string GetDataPath() => "Media/PCMAP/";

        #endregion

        #region Manager Methods

        /// <summary>
        /// Gets a binary file to add to the context
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="filePath">The file path</param>
        /// <returns>The binary file</returns>
        protected override BinaryFile GetFile(Context context, string filePath) => new LinearSerializedFile(context)
        {
            filePath = filePath
        };

        #endregion
    }
}