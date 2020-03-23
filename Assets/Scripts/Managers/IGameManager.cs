using R1Engine.Serialize;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R1Engine
{
    /// <summary>
    /// Defines a generic game manager
    /// </summary>
    public interface IGameManager
    {
        /// <summary>
        /// Indicates if the game has 3 palettes it swaps between
        /// </summary>
        bool Has3Palettes { get; }

        /// <summary>
        /// Gets the levels for the specified world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        int[] GetLevels(GameSettings settings);

        /// <summary>
        /// Gets the available educational volumes
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The available educational volumes</returns>
        string[] GetEduVolumes(GameSettings settings);
        
        /// <summary>
        /// Gets the available game export options
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game export options</returns>
        string[] GetExportOptions(GameSettings settings);

        /// <summary>
        /// Exports the specified content
        /// </summary>
        /// <param name="exportIndex">The export index</param>
        /// <param name="outputDir">The output directory</param>
        /// <param name="settings">The game settings</param>
        void Export(int exportIndex, string outputDir, GameSettings settings);


        ///// <summary>
        ///// Exports all vignette textures to the specified output directory
        ///// </summary>
        ///// <param name="context">The context</param>
        ///// <param name="outputDir">The output directory</param>
        //void ExportVignetteTextures(Context context, string outputDir);

        ///// <summary>
        ///// Exports all sprite textures to the specified output directory
        ///// </summary>
        ///// <param name="context">The context</param>
        ///// <param name="outputDir">The output directory</param>
        //void ExportSpriteTextures(Context context, string outputDir);

        ///// <summary>
        ///// Exports all animation frames to the specified directory
        ///// </summary>
        ///// <param name="context">The context</param>
        ///// <param name="outputDir">The directory to export to</param>
        //void ExportAnimationFrames(Context context, string outputDir);

        /// <summary>
        /// Auto applies the palette to the tiles in the level
        /// </summary>
        /// <param name="level">The level to auto-apply the palette to</param>
        void AutoApplyPalette(Common_Lev level);

        /// <summary>
        /// Loads the specified level
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="eventDesigns">The list of event designs to populate</param>
        /// <returns>The level</returns>
        Task<Common_Lev> LoadLevelAsync(Context context, List<Common_Design> eventDesigns);

        /// <summary>
        /// Saves the specified level
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="commonLevelData">The common level data</param>
        void SaveLevel(Context context, Common_Lev commonLevelData);

        /// <summary>
        /// Preloads all the necessary files into the context
        /// </summary>
        /// <param name="context">The serialization context</param>
        Task LoadFilesAsync(Context context);

        /// <summary>
        /// Gets the common editor event info for an event
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="e">The event</param>
        /// <returns>The common editor event info</returns>
        Common_EditorEventInfo GetEditorEventInfo(GameSettings settings, Common_Event e);

        /// <summary>
        /// Gets the animation info for an event
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="e">The event</param>
        /// <returns>The animation info</returns>
        Common_AnimationInfo GetAnimationInfo(Context context, Common_Event e);

        /// <summary>
        /// Gets the available event names to add for the current world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The names of the available events to add</returns>
        string[] GetEvents(GameSettings settings);

        /// <summary>
        /// Adds a new event to the controller and returns it
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="eventController">The event controller to add to</param>
        /// <param name="index">The event index from the available events</param>
        /// <param name="xPos">The x position</param>
        /// <param name="yPos">The y position</param>
        /// <returns></returns>
        Common_Event AddEvent(GameSettings settings, LevelEventController eventController, int index, uint xPos, uint yPos);
    }
}