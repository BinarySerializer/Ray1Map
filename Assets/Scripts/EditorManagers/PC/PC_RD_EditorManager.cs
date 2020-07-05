using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// The editor manager for Rayman Designer (PC)
    /// </summary>
    public class PC_RD_EditorManager : BaseEditorManager
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="level">The common level</param>
        /// <param name="context">The context</param>
        /// <param name="manager">The manager</param>
        /// <param name="designs">The common design</param>
        public PC_RD_EditorManager(Common_Lev level, Context context, PC_RD_Manager manager, IEnumerable<Common_Design> designs) : base(level, context, new ReadOnlyDictionary<string, Common_Design>(designs.Select((x, i) => new
        {
            FileName = manager.GetDESFileName(context, i),
            Item = x
        }).ToDictionary(x => x.FileName, x => x.Item)), new ReadOnlyDictionary<string, Common_EventState[][]>(manager.GetCurrentEventStates(context).Select((x, i) => new
        {
            FileName = manager.GetETAFileName(context, i),
            Item = x
        }).ToDictionary(x => x.FileName, x => x.Item.States)))
        {
            // Read the world data
            var worldData = FileFactory.Read<PC_WorldFile>(manager.GetWorldFilePath(context.Settings), context);

            // Get file names if available
            DESFileIndex = worldData.DESFileNames ?? new string[0];
            ETAFileIndex = worldData.ETAFileNames ?? new string[0];
        }

        /// <summary>
        /// Indicates if the local commands should be used
        /// </summary>
        protected override bool UsesLocalCommands => false;

        /// <summary>
        /// The DES file index
        /// </summary>
        public string[] DESFileIndex { get; }
        
        /// <summary>
        /// The ETA file index
        /// </summary>
        public string[] ETAFileIndex { get; }

        /// <summary>
        /// Gets the DES key for the specified event data item
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>The DES key</returns>
        public override string GetDesKey(GeneralEventInfoData eventInfoData)
        {
            return eventInfoData.DesKit[Settings.World] + ".DES";
        }

        /// <summary>
        /// Gets the ETA key for the specified event data item
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>The ETA key</returns>
        public override string GetEtaKey(GeneralEventInfoData eventInfoData)
        {
            return eventInfoData.EtaKit[Settings.World] + ".ETA";
        }

        /// <summary>
        /// Checks if the event is available in the current world
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>True if it's available, otherwise false</returns>
        public override bool IsAvailableInWorld(GeneralEventInfoData eventInfoData)
        {
            return eventInfoData.DesKit.ContainsKey(Settings.World) && !String.IsNullOrWhiteSpace(eventInfoData.DesKit[Settings.World]);
        }
    }
}