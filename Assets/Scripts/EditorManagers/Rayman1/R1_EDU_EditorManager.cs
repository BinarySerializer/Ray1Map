using R1Engine.Serialize;
using System.Collections.Generic;

namespace R1Engine
{
    /// <summary>
    /// The editor manager for Rayman EDU (PC)
    /// </summary>
    public class R1_EDU_EditorManager : R1_PC_EditorManager
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="level">The common level</param>
        /// <param name="context">The context</param>
        /// <param name="manager">The manager</param>
        /// <param name="designs">The common design</param>
        public R1_EDU_EditorManager(Unity_Level level, Context context, R1_PCBaseManager manager, IEnumerable<Unity_ObjGraphics> designs) : base(level, context, manager, designs)
        { }

        /// <summary>
        /// Indicates if the local commands should be used
        /// </summary>
        protected override bool UsesLocalCommands => false;

        /// <summary>
        /// Gets the DES key for the specified event data item
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>The DES key</returns>
        public override string GetDesKey(GeneralEventInfoData eventInfoData)
        {
            return eventInfoData.DesEdu[Settings.R1_World]?.ToString();
        }

        /// <summary>
        /// Gets the ETA key for the specified event data item
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>The ETA key</returns>
        public override string GetEtaKey(GeneralEventInfoData eventInfoData)
        {
            return eventInfoData.EtaEdu[Settings.R1_World]?.ToString();
        }

        /// <summary>
        /// Checks if the event is available in the current world
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>True if it's available, otherwise false</returns>
        public override bool IsAvailableInWorld(GeneralEventInfoData eventInfoData)
        {
            return eventInfoData.DesEdu.ContainsKey(Settings.R1_World) && eventInfoData.DesEdu[Settings.R1_World] != null;
        }
    }
}