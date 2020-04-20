using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// The editor manager for Rayman 1 (PC)
    /// </summary>
    public class PC_R1_EditorManager : BaseEditorManager
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="level">The common level</param>
        /// <param name="context">The context</param>
        /// <param name="manager">The manager</param>
        /// <param name="designs">The common design</param>
        public PC_R1_EditorManager(Common_Lev level, Context context, PC_Manager manager, IEnumerable<Common_Design> designs) : base(level, context, new ReadOnlyDictionary<string, Common_Design>(designs.Select((x, i) => new
        {
            Index = i,
            Item = x
        }).ToDictionary(x => x.Index.ToString(), x => x.Item)), new ReadOnlyDictionary<string, Common_EventState[][]>(manager.GetCurrentEventStates(context).Select((x, i) => new
        {
            Index = i,
            Item = x
        }).ToDictionary(x => x.Index.ToString(), x => x.Item.States)))
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
            return eventInfoData.DesR1[Settings.World]?.ToString();
        }

        /// <summary>
        /// Gets the ETA key for the specified event data item
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>The ETA key</returns>
        public override string GetEtaKey(GeneralEventInfoData eventInfoData)
        {
            return eventInfoData.EtaR1[Settings.World]?.ToString();
        }

        /// <summary>
        /// Checks if the event is available in the current world
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>True if it's available, otherwise false</returns>
        public override bool IsAvailableInWorld(GeneralEventInfoData eventInfoData)
        {
            return eventInfoData.DesR1.ContainsKey(Settings.World) && eventInfoData.DesR1[Settings.World] != null;
        }
    }
}