using BinarySerializer;
using System;

namespace Ray1Map.Jade
{
    public class EVE_Event : BinarySerializable
    {
        public EVE_ListEvents ListEvents { get; set; } // Set before serializing
        public int Index { get; set; } // Set before serializing

        public ushort NumFrames { get; set; }
        public bool HasUglyOptimizationFlag { get; set; }
        public Flags_Basic BasicFlags { get; set; }
        public Flags_Type Type { get; set; }
        public int OtherFlags { get; set; }

        public uint? InterpolationKey_T { get; set; }
        public EVE_Event_InterpolationKey InterpolationKey { get; set; }
        public EVE_Event_AIFunction AIFunction { get; set; }
        public EVE_Event_MagicKey MagicKey { get; set; }
        public EVE_Event_PlaySynchro PlaySynchro { get; set; }
        public EVE_Event_MorphKey MorphKey { get; set; }
        public EVE_Event_MorphKey_Old MorphKey_Old { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            if (ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Under256))
                NumFrames = s.Serialize<byte>((byte)NumFrames, name: nameof(NumFrames));
            else
                NumFrames = s.Serialize<ushort>(NumFrames, name: nameof(NumFrames));
            HasUglyOptimizationFlag = (NumFrames & 0x8000) != 0;

            if (ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameFlags) && Index > 0) {
                if (ListEvents.Track.DataLength > 0 &&
                    ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameType) &&
                    ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameSize) &&
                    ListEvents.Events[0].HasUglyOptimizationFlag) {
                    BasicFlags = 0;
                    OtherFlags = 0;
                    Type = Flags_Type.InterpolationKey; // InterpolationKey
                } else {
                    BasicFlags = ListEvents.Events[0].BasicFlags;
                    Type = ListEvents.Events[0].Type;
                    OtherFlags = ListEvents.Events[0].OtherFlags;
                }

                //Ushort_02 &= 0xFFFB;
            } else {
                if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && ListEvents.Track.ListTracks.Montreal_Version >= 0x8000) {
                    s.DoBits<int>(b => {
                        BasicFlags = b.SerializeBits<Flags_Basic>(BasicFlags, 6, name: nameof(BasicFlags));
                        Type = b.SerializeBits<Flags_Type>(Type, 5, name: nameof(Type));
                        OtherFlags = b.SerializeBits<int>(OtherFlags, 5 + 16, name: nameof(OtherFlags));
                    });
                } else {
                    s.DoBits<ushort>(b => {
                        BasicFlags = b.SerializeBits<Flags_Basic>(BasicFlags, 6, name: nameof(BasicFlags));
                        Type = b.SerializeBits<Flags_Type>(Type, 5, name: nameof(Type));
                        OtherFlags = b.SerializeBits<int>(OtherFlags, 5, name: nameof(OtherFlags));
                    });
                }
            }

            switch (Type) {
                case Flags_Type.AIFunction:
                    AIFunction = s.SerializeObject<EVE_Event_AIFunction>(AIFunction, name: nameof(AIFunction));
                    break;
                case Flags_Type.InterpolationKey:
                    if (BasicFlags.HasFlag(Flags_Basic.Symmetric)) {
                        InterpolationKey_T = s.Serialize<uint>(InterpolationKey_T ?? 0, name: nameof(InterpolationKey_T));
                    } else {
                        InterpolationKey = s.SerializeObject<EVE_Event_InterpolationKey>(InterpolationKey, onPreSerialize: k => k.Event = this, name: nameof(InterpolationKey));
                    }
                    break;
                case Flags_Type.MorphKey:
                    MorphKey_Old = s.SerializeObject<EVE_Event_MorphKey_Old>(MorphKey_Old, name: nameof(MorphKey_Old));
                    break;
                case Flags_Type.MagicKey:
                    MagicKey = s.SerializeObject<EVE_Event_MagicKey>(MagicKey, name: nameof(MagicKey));
                    break;
				case Flags_Type.PlaySynchro:
                    PlaySynchro = s.SerializeObject<EVE_Event_PlaySynchro>(PlaySynchro, name: nameof(PlaySynchro));
                    break;
                case Flags_Type.MorphKeyNew:
                    MorphKey = s.SerializeObject<EVE_Event_MorphKey>(MorphKey, name: nameof(MorphKey));
                    break;
            }
		}
		[Flags]
		public enum Flags_Basic : byte {
            Empty = 0,
			DoOnce = 1 << 0,
			Selected = 1 << 1,
			Flash = 1 << 2,
			WaitFalse = 1 << 3,
			WaitTrue = 1 << 4,
            Symmetric = 1 << 5,
		}

		[Flags]
        public enum Flags_Type : byte {
            Empty = 0,
            AIFunction       = 1 << 0,
            InterpolationKey = 1 << 1,
            MorphKey         = 1 << 2,
            GotoLabel        = 1 << 3,
			SetTimeToLabel   = 1 << 4,

            // New types since RRR
			MagicKey = GotoLabel | MorphKey,
            PlaySynchro = SetTimeToLabel | MorphKey,
            MorphKeyNew = SetTimeToLabel | GotoLabel,
		}
    }
}