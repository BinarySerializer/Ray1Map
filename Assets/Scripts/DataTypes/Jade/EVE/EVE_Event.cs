using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public class EVE_Event : BinarySerializable
    {
        public EVE_ListEvents ListEvents { get; set; } // Set before serializing
        public int Index { get; set; } // Set before serializing

        public ushort Ushort_00 { get; set; }
        public ushort Ushort_00_InGame { get; set; }
        public ushort Flags_00 { get; set; }
        public ushort TypeCode { get; set; }
        public ushort Flags_01 { get; set; }

        public uint? InterpolationKey_UInt { get; set; }
        public EVE_Event_InterpolationKey InterpolationKey { get; set; }
        public EVE_Event_AIFunction AIFunction { get; set; }
        public EVE_Event_MagicKey MagicKey { get; set; }
        public EVE_Event_PlaySynchro PlaySynchro { get; set; }
        public EVE_Event_MorphKey MorphKey { get; set; }
        public EVE_Event_MorphKey_Old MorphKey_Old { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            if (ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_9))
                Ushort_00 = s.Serialize<byte>((byte)Ushort_00, name: nameof(Ushort_00));
            else
                Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));
            Ushort_00_InGame = Ushort_00;

            if (ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_11) && Index > 0) {
                if (ListEvents.Track.UInt_04 > 0 &&
                    ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_12) &&
                    ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_13) &&
                    (ListEvents.Header_Flags & 0x80) != 0) {
                    Flags_00 = 0;
                    Flags_01 = 0;
                    TypeCode = 2; // InterpolationKey
                } else {
                    Flags_00 = ListEvents.Events[0].Flags_00;
                    TypeCode = ListEvents.Events[0].TypeCode;
                    Flags_01 = ListEvents.Events[0].Flags_01;
                }

                //Ushort_02 &= 0xFFFB;
            } else {
                s.SerializeBitValues<ushort>(bitFunc => {
                    Flags_00 = (ushort)bitFunc(Flags_00, 6, name: nameof(Flags_00));
                    TypeCode = (ushort)bitFunc(TypeCode, 5, name: nameof(TypeCode));
                    Flags_01 = (ushort)bitFunc(Flags_01, 5, name: nameof(Flags_01));
                });
            }

            switch (TypeCode) {
                case 0x1:
                    AIFunction = s.SerializeObject<EVE_Event_AIFunction>(AIFunction, name: nameof(AIFunction));
                    break;
                case 0x2:
                    if ((Flags_00 & 0x20) != 0) {
                        InterpolationKey_UInt = s.Serialize<uint>(InterpolationKey_UInt ?? 0, name: nameof(InterpolationKey_UInt));
                    } else {
                        InterpolationKey = s.SerializeObject<EVE_Event_InterpolationKey>(InterpolationKey, onPreSerialize: k => k.Event = this, name: nameof(InterpolationKey));
                    }
                    break;
                case 0x4:
                    MorphKey_Old = s.SerializeObject<EVE_Event_MorphKey_Old>(MorphKey_Old, name: nameof(MorphKey_Old));
                    break;
                case 0xC:
                    MagicKey = s.SerializeObject<EVE_Event_MagicKey>(MagicKey, name: nameof(MagicKey));
                    break;
                case 0x14:
                    PlaySynchro = s.SerializeObject<EVE_Event_PlaySynchro>(PlaySynchro, name: nameof(PlaySynchro));
                    break;
                case 0x18:
                    MorphKey = s.SerializeObject<EVE_Event_MorphKey>(MorphKey, name: nameof(MorphKey));
                    break;
            }
        }
    }
}