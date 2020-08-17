using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// The editor manager for PS1
    /// </summary>
    public class R1_PS1_EditorManager : BaseEditorManager
    {
        public R1_PS1_EditorManager(Unity_Level level, Context context, IDictionary<Pointer, Unity_ObjGraphics> des, IDictionary<Pointer, R1_EventState[][]> eta, R1_EventData[] events) 
            : base(level, context, new ReadOnlyDictionary<string, Unity_ObjGraphics>(des.ToDictionary(x => x.Key.ToString(), x => x.Value)), new ReadOnlyDictionary<string, R1_EventState[][]>(eta.ToDictionary(x => x.Key.ToString(), x => x.Value)))
        {
            if (events == null)
                return;

            DESCollection = des.ToDictionary(x => x.Key.ToString(), x =>
            {
                // Find matching event
                var e = events.First(y => y.ImageDescriptorsPointer == x.Key);

                // Save the DES data
                return new DESData(e.ImageDescriptorsPointer, e.AnimDescriptorsPointer, e.ImageBufferPointer, e.ImageDescriptors, e.AnimDescriptors, e.ImageBuffer);
            });
            ETACollection = eta.ToDictionary(x => x.Key.ToString(), x =>
            {
                // Find matching event
                var e = events.First(y => y.ETAPointer == x.Key);

                // Save the ETA data    
                return new ETAData(e.ETAPointer, e.ETA);
            });
        }

        /// <summary>
        /// Indicates if the local commands should be used
        /// </summary>
        protected override bool UsesLocalCommands => Settings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 || Settings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol6;

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

        public override Enum GetCollisionTypeAsEnum(byte collisionType) => (R1_TileCollisionType)collisionType;

        public override Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(byte collisionType) => ((R1_TileCollisionType)collisionType).GetCollisionTypeGraphic();

        public Dictionary<string, DESData> DESCollection { get; }
        public Dictionary<string, ETAData> ETACollection { get; }

        public class DESData
        {
            public DESData(Pointer imageDescriptorsPointer, Pointer animDescriptorsPointer, Pointer imageBufferPointer, R1_ImageDescriptor[] imageDescriptors, R1_PS1_AnimationDescriptor[] animDescriptors, byte[] imageBuffer)
            {
                ImageDescriptorsPointer = imageDescriptorsPointer;
                AnimDescriptorsPointer = animDescriptorsPointer;
                ImageBufferPointer = imageBufferPointer;
                ImageDescriptors = imageDescriptors;
                AnimDescriptors = animDescriptors;
                ImageBuffer = imageBuffer;
            }

            public Pointer ImageDescriptorsPointer { get; }
            public Pointer AnimDescriptorsPointer { get; }
            public Pointer ImageBufferPointer { get; }

            public R1_ImageDescriptor[] ImageDescriptors { get; }
            public R1_PS1_AnimationDescriptor[] AnimDescriptors { get; }
            public byte[] ImageBuffer { get; }
        }

        public class ETAData
        {
            public ETAData(Pointer etaPointer, R1_PS1_ETA eta)
            {
                ETAPointer = etaPointer;
                ETA = eta;
            }

            public Pointer ETAPointer { get; }
            
            public R1_PS1_ETA ETA { get; }
        }
    }
}