using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public class EVE_Event_InterpolationKey : BinarySerializable
    {
        public EVE_Event Event { get; set; } // Set in onPreSerialize

        public ushort UShort_00 { get; set; }
        public ushort Type { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (Event.Index == 0 || !Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_13)) {
                UShort_00 = s.Serialize<ushort>(UShort_00, name: nameof(UShort_00));
            } else {
                var firstEvent = Event.ListEvents.Events[0];
                if ((firstEvent.Ushort_00 & 0x8000) == 0) {
                    if (firstEvent.InterpolationKey_UInt.HasValue) {
                        UShort_00 = (ushort)BitHelpers.ExtractBits((int)firstEvent.InterpolationKey_UInt, 15, 1);
                    } else if(firstEvent.InterpolationKey != null) {
                        throw new NotImplementedException($"TODO: Implement {GetType()}: Figure out InterpolationKey");
                    }
                }
            }
            if (Event.Index == 0 || !Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_8)) {
                Type = s.Serialize<ushort>(Type, name: nameof(Type));
            } else if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_13)) {
                var firstEvent = Event.ListEvents.Events[0];
                if ((firstEvent.Ushort_00 & 0x8000) != 0) {
                    Type = 144;
                } else if (firstEvent.InterpolationKey_UInt.HasValue) {
                    Type = (ushort)BitHelpers.ExtractBits((int)firstEvent.InterpolationKey_UInt, 16, 16);
                } else if (firstEvent.InterpolationKey != null) {
                    throw new NotImplementedException($"TODO: Implement {GetType()}: Figure out InterpolationKey");
                } else {
                    throw new NotImplementedException($"TODO: Implement {GetType()}: Figure out InterpolationKey");
                }
            } else {
                var firstEvent = Event.ListEvents.Events[0];
                if ((firstEvent.Ushort_00 & 0x8000) != 0) {
                    Type = 144;
                } else if (firstEvent.InterpolationKey_UInt.HasValue) {
                    Type = (ushort)BitHelpers.ExtractBits((int)firstEvent.InterpolationKey_UInt, 16, 16);
                } else if (firstEvent.InterpolationKey != null) {
                    throw new NotImplementedException($"TODO: Implement {GetType()}: Figure out InterpolationKey");
                } else {
                    throw new NotImplementedException($"TODO: Implement {GetType()}: Figure out InterpolationKey");
                }
            }
            if ((Type & 0x80) != 0) {
                throw new NotImplementedException($"TODO: Implement {GetType()}: Type & 0x80");
            }
            throw new NotImplementedException($"TODO: Implement {GetType()}");
        }
    }
}