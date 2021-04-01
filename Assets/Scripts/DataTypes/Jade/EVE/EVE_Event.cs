using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public class EVE_Event : BinarySerializable
    {
        public EVE_ListEvents ListEvents { get; set; } // Set before serializing
        public int Index { get; set; } // Set before serializing

        public ushort Ushort_00 { get; set; }
        public ushort Ushort_02 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            if (ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_9))
                Ushort_00 = s.Serialize<byte>((byte)Ushort_00, name: nameof(Ushort_00));
            else
                Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));

            if (ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_11) && Index != 0)
            {
                if (ListEvents.Track.Uint_04 > 0 &&
                    ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_11) &&
                    ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_12) &&
                    ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_13) &&
                    (ListEvents.Header_Flags & 0x80) != 0)
                    Ushort_02 = 128;
                else
                    throw new NotImplementedException($"TODO: Implement {GetType()}"); // Ushort_02 = *(_WORD *)(*(_DWORD *)(trackStruct + 4) + 2);

                Ushort_02 &= 0xFFFB;
            }
            else
            {
                Ushort_02 = s.Serialize<ushort>(Ushort_02, name: nameof(Ushort_02));
            }

            var v24 = Ushort_02 & 0x7C0;

            throw new NotImplementedException($"TODO: Implement {GetType()}");
        }
    }
}