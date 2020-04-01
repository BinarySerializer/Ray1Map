using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// The editor manager for Mapper (PC)
    /// </summary>
    public class PC_Mapper_EditorManager : PC_RD_EditorManager
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="level">The common level</param>
        /// <param name="context">The context</param>
        /// <param name="manager">The manager</param>
        /// <param name="designs">The common design</param>
        public PC_Mapper_EditorManager(Common_Lev level, Context context, PC_Manager manager, Common_Design[] designs) : base(level, context, manager, designs)
        { }

        /// <summary>
        /// Indicates if the local commands should be used
        /// </summary>
        protected override bool UsesLocalCommands => true;

        /// <summary>
        /// Gets the color index for a colored ting
        /// </summary>
        /// <param name="hitpointsValue">The hitpoints value</param>
        /// <returns>The index</returns>
        public override int GetColoredTingColorIndex(int hitpointsValue)
        {
            switch (hitpointsValue)
            {
                default:
                case 0:
                    return 0;

                case 1:
                    return 1;

                case 2:
                    return 2;

                case 3:
                    return 3;

                case 4:
                    return 4;

                case 5:
                    return 5;
            }
        }
    }
}