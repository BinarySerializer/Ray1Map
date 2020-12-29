using System;

namespace R1Engine
{
    /// <summary>
    /// Level data for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class R1_R2LevDataFile : R1Serializable
    {
        #region Level Data

        public int Unk1 { get; set; }
        public int Unk2 { get; set; }

        /// <summary>
        /// Pointer to the events
        /// </summary>
        public Pointer LoadedEventsPointer { get; set; }

        /// <summary>
        /// Pointer to the always events
        /// </summary>
        public Pointer AlwaysEventsPointer { get; set; }

        /// <summary>
        /// Pointer to the allfix image descriptors
        /// </summary>
        public Pointer FixImageDescriptorsPointer { get; set; }
        
        public byte[] Unk3 { get; set; }

        public ushort LoadedEventCount { get; set; } // NormalEventCount + AlwaysEventSlotsCount
        public ushort AlwaysEventsCount { get; set; } // These are not in the normal event array
        public ushort NormalEventCount { get; set; } // Normal events
        public ushort AlwaysEventSlotsCount { get; set; } // Dummy slots for always events during runtime

        public ushort UShort_3A { get; set; }

        /// <summary>
        /// The number of allfix image descriptors
        /// </summary>
        public ushort NumFixImageDescriptors { get; set; }

        public ushort UShort_3E { get; set; }
        public ushort UShort_40 { get; set; }
        public ushort UShort_42 { get; set; }
        public ushort UShort_44 { get; set; }
        public ushort UShort_46 { get; set; }
        public uint DevPointer_48 { get; set; } // Dev pointer

        public Pointer ZDCDataPointer { get; set; }
        public Pointer ZDCArray1Pointer { get; set; } // Indexed using ZDCEntry.ZDCIndex
        public Pointer ZDCArray2Pointer { get; set; } // Indexed using ZDCEntry.ZDCIndex and additional value
        public Pointer ZDCArray3Pointer { get; set; } // Indexed using UnkPointer5 values

        /// <summary>
        /// The event link table for all loaded events
        /// </summary>
        public ushort[] EventLinkTable { get; set; }

        #endregion

        #region Parsed Data

        /// <summary>
        /// The events
        /// </summary>
        public R1_R2EventData[] Events { get; set; }
        
        /// <summary>
        /// The always events. These do not have map positions.
        /// </summary>
        public R1_R2EventData[] AlwaysEvents { get; set; }

        /// <summary>
        /// The allfix image descriptors
        /// </summary>
        public R1_ImageDescriptor[] FixImageDescriptors { get; set; }

        public R1_ZDCData[] ZDC { get; set; }
        public ZDC_TriggerFlags[] ZDCTriggerFlags { get; set; }
        public ushort[] ZDCArray2 { get; set; }
        public R1_R2ZDCUnkData[] ZDCArray3 { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Unk1 = s.Serialize<int>(Unk1, name: nameof(Unk1));
            Unk2 = s.Serialize<int>(Unk2, name: nameof(Unk2));

            LoadedEventsPointer = s.SerializePointer(LoadedEventsPointer, name: nameof(LoadedEventsPointer));

            AlwaysEventsPointer = s.SerializePointer(AlwaysEventsPointer, name: nameof(AlwaysEventsPointer));
            FixImageDescriptorsPointer = s.SerializePointer(FixImageDescriptorsPointer, name: nameof(FixImageDescriptorsPointer));

            Unk3 = s.SerializeArray<byte>(Unk3, 30, name: nameof(Unk3));

            LoadedEventCount = s.Serialize<ushort>(LoadedEventCount, name: nameof(LoadedEventCount));

            AlwaysEventsCount = s.Serialize<ushort>(AlwaysEventsCount, name: nameof(AlwaysEventsCount));
            NormalEventCount = s.Serialize<ushort>(NormalEventCount, name: nameof(NormalEventCount));
            AlwaysEventSlotsCount = s.Serialize<ushort>(AlwaysEventSlotsCount, name: nameof(AlwaysEventSlotsCount));
            UShort_3A = s.Serialize<ushort>(UShort_3A, name: nameof(UShort_3A));
            NumFixImageDescriptors = s.Serialize<ushort>(NumFixImageDescriptors, name: nameof(NumFixImageDescriptors));
            UShort_3E = s.Serialize<ushort>(UShort_3E, name: nameof(UShort_3E));
            UShort_40 = s.Serialize<ushort>(UShort_40, name: nameof(UShort_40));
            UShort_42 = s.Serialize<ushort>(UShort_42, name: nameof(UShort_42));
            UShort_44 = s.Serialize<ushort>(UShort_44, name: nameof(UShort_44));
            UShort_46 = s.Serialize<ushort>(UShort_46, name: nameof(UShort_46));
            DevPointer_48 = s.Serialize<uint>(DevPointer_48, name: nameof(DevPointer_48));
            
            ZDCDataPointer = s.SerializePointer(ZDCDataPointer, name: nameof(ZDCDataPointer));
            ZDCArray1Pointer = s.SerializePointer(ZDCArray1Pointer, name: nameof(ZDCArray1Pointer));
            ZDCArray2Pointer = s.SerializePointer(ZDCArray2Pointer, name: nameof(ZDCArray2Pointer));
            ZDCArray3Pointer = s.SerializePointer(ZDCArray3Pointer, name: nameof(ZDCArray3Pointer));

            EventLinkTable = s.SerializeArray<ushort>(EventLinkTable, LoadedEventCount, name: nameof(EventLinkTable));

            s.DoAt(FixImageDescriptorsPointer, () => FixImageDescriptors = s.SerializeObjectArray<R1_ImageDescriptor>(FixImageDescriptors, NumFixImageDescriptors, name: nameof(FixImageDescriptors)));

            s.DoAt(LoadedEventsPointer, () => Events = s.SerializeObjectArray<R1_R2EventData>(Events, LoadedEventCount, name: nameof(Events)));
            s.DoAt(AlwaysEventsPointer, () => AlwaysEvents = s.SerializeObjectArray<R1_R2EventData>(AlwaysEvents, AlwaysEventsCount, name: nameof(AlwaysEvents)));

            s.DoAt(ZDCDataPointer, () => ZDC = s.SerializeObjectArray<R1_ZDCData>(ZDC, 237, name: nameof(ZDC)));
            s.DoAt(ZDCArray1Pointer, () => ZDCTriggerFlags = s.SerializeArray<ZDC_TriggerFlags>(ZDCTriggerFlags, 237, name: nameof(ZDCTriggerFlags)));
            s.DoAt(ZDCArray2Pointer, () => ZDCArray2 = s.SerializeArray<ushort>(ZDCArray2, 474, name: nameof(ZDCArray2)));
            s.DoAt(ZDCArray3Pointer, () => ZDCArray3 = s.SerializeObjectArray<R1_R2ZDCUnkData>(ZDCArray3, 16, name: nameof(ZDCArray3)));
        }

        #endregion

        [Flags]
        public enum ZDC_TriggerFlags : byte
        {
            None = 0,

            Flag_0 = 1 << 0,

            Rayman = 1 << 1,
            Poing_0 = 1 << 2,
            
            Flag_3 = 1 << 3,
            Flag_4 = 1 << 4,
            Flag_5 = 1 << 5,
            Flag_6 = 1 << 6,

            Poing_1 = 1 << 7
        }
    }
}