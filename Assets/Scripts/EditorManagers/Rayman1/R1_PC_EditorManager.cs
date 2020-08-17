using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// The editor manager for Rayman 1 (PC)
    /// </summary>
    public class R1_PC_EditorManager : BaseEditorManager
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="level">The common level</param>
        /// <param name="context">The context</param>
        /// <param name="manager">The manager</param>
        /// <param name="designs">The common design</param>
        public R1_PC_EditorManager(Unity_Level level, Context context, R1_PCBaseManager manager, IEnumerable<Unity_ObjGraphics> designs) : base(level, context, new ReadOnlyDictionary<string, Unity_ObjGraphics>(designs.Select((x, i) => new
        {
            Index = i,
            Item = x
        }).ToDictionary(x => x.Index.ToString(), x => x.Item)), new ReadOnlyDictionary<string, R1_EventState[][]>(manager.GetCurrentEventStates(context).Select((x, i) => new
        {
            Index = i,
            Item = x
        }).ToDictionary(x => x.Index.ToString(), x => x.Item.States)))
        { }

        /// <summary>
        /// Indicates if the game has 3 palettes it swaps between
        /// </summary>
        public override bool Has3Palettes => true;

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
            return eventInfoData.DesR1[Settings.R1_World]?.ToString();
        }

        /// <summary>
        /// Gets the ETA key for the specified event data item
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>The ETA key</returns>
        public override string GetEtaKey(GeneralEventInfoData eventInfoData)
        {
            return eventInfoData.EtaR1[Settings.R1_World]?.ToString();
        }

        /// <summary>
        /// Checks if the event is available in the current world
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>True if it's available, otherwise false</returns>
        public override bool IsAvailableInWorld(GeneralEventInfoData eventInfoData)
        {
            return eventInfoData.DesR1.ContainsKey(Settings.R1_World) && eventInfoData.DesR1[Settings.R1_World] != null;
        }

        public override Enum GetCollisionTypeAsEnum(byte collisionType) => (R1_TileCollisionType)collisionType;

        public override Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(byte collisionType) => ((R1_TileCollisionType)collisionType).GetCollisionTypeGraphic();
    }
}