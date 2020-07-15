using R1Engine.Serialize;
using System.Collections.Generic;

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

        public Pointer GetDataPointer(Context context, Jaguar_R1Proto_References reference) => FileFactory.Read<Jaguar_R1_ROM>(GetROMFilePath, context).GetProtoDataPointer(reference);

        #endregion
    }
}