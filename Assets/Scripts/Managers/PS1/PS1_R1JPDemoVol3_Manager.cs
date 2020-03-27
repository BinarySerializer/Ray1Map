using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (PS1 - Japan Demo Vol3)
    /// </summary>
    public class PS1_R1JPDemoVol3_Manager : PS1_R1JP_Manager
    {
        /// <summary>
        /// Reads the tile set for the specified world
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The tile set</returns>
        public override Common_Tileset ReadTileSet(Context context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The editor manager</returns>
        public override Task<BaseEditorManager> LoadAsync(Context context)
        {
            throw new NotImplementedException();
        }
    }
}