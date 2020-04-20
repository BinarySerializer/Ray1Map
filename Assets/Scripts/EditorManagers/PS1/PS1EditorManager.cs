using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// The editor manager for PS1
    /// </summary>
    public class PS1EditorManager : BaseEditorManager
    {
        public PS1EditorManager(Common_Lev level, Context context, IDictionary<Pointer, Common_Design> des, IDictionary<Pointer, Common_EventState[][]> eta) : base(level, context, new ReadOnlyDictionary<string, Common_Design>(des.ToDictionary(x => x.Key.ToString(), x => x.Value)), new ReadOnlyDictionary<string, Common_EventState[][]>(eta.ToDictionary(x => x.Key.ToString(), x => x.Value)))
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the ETA key for the specified event data item
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>The ETA key</returns>
        public override string GetEtaKey(GeneralEventInfoData eventInfoData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if the event is available in the current world
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>True if it's available, otherwise false</returns>
        public override bool IsAvailableInWorld(GeneralEventInfoData eventInfoData)
        {
            return false;
        }
    }
}