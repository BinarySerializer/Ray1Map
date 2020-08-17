using System.Collections.Generic;
using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// The editor manager for Mapper (PC)
    /// </summary>
    public class R1_Mapper_EditorManager : R1_Kit_EditorManager
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="level">The common level</param>
        /// <param name="context">The context</param>
        /// <param name="manager">The manager</param>
        /// <param name="designs">The common design</param>
        public R1_Mapper_EditorManager(Unity_Level level, Context context, R1_Kit_Manager manager, IEnumerable<Unity_ObjGraphics> designs) : base(level, context, manager, designs)
        { }

        /// <summary>
        /// Indicates if the local commands should be used
        /// </summary>
        protected override bool UsesLocalCommands => true;
    }
}