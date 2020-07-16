using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// The editor manager for Jaguar
    /// </summary>
    public class JaguarEditorManager : BaseEditorManager
    {
        public JaguarEditorManager(Common_Lev level, Context context, IDictionary<string, Common_Design> des, IDictionary<string, Common_EventState[][]> eta, IDictionary<string, string[][]> etaNames) : base(level, context, new ReadOnlyDictionary<string, Common_Design>(des), new ReadOnlyDictionary<string, Common_EventState[][]>(eta))
        {
            ETANames = etaNames != null ? new ReadOnlyDictionary<string, string[][]>(etaNames) : null;
        }

        public override IReadOnlyDictionary<string, string[][]> ETANames { get; }

        // NOTE: Jaguar doesn't use commands!
        /// <summary>
        /// Indicates if the local commands should be used
        /// </summary>
        protected override bool UsesLocalCommands => false;

        /// <summary>
        /// Gets the DES key for the specified event data item
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>The DES key</returns>
        public override string GetDesKey(GeneralEventInfoData eventInfoData) => throw new NotImplementedException();

        /// <summary>
        /// Gets the ETA key for the specified event data item
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>The ETA key</returns>
        public override string GetEtaKey(GeneralEventInfoData eventInfoData) => throw new NotImplementedException();

        /// <summary>
        /// Checks if the event is available in the current world
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>True if it's available, otherwise false</returns>
        public override bool IsAvailableInWorld(GeneralEventInfoData eventInfoData) => false;
    }
}