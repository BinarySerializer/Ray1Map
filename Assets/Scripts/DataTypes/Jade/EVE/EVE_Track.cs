using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public class EVE_Track : BinarySerializable
    {
        public TrackFlags Flags { get; set; }
        public ushort Ushort_02 { get; set; }
        public uint Uint_04 { get; set; }
        public int Int_08 { get; set; }
        public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
        public string String_0C_Editor { get; set; }
        public byte Byte_Editor { get; set; }
        public EVE_ListEvents ListEvents { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            Flags = s.Serialize<TrackFlags>(Flags, name: nameof(Flags));
            Ushort_02 = s.Serialize<ushort>(Ushort_02, name: nameof(Ushort_02));
            Uint_04 = s.Serialize<uint>(Uint_04, name: nameof(Uint_04));

            if (Flags.HasFlag(TrackFlags.Flag_15))
            {
                if (((ushort)Flags & 0x3F00) == 0)
                    Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));
            }
            else
            {
                GameObject = s.SerializeObject(GameObject, name: nameof(GameObject))?.Resolve();

                if (!Loader.IsBinaryData)
                {
                    String_0C_Editor = s.SerializeString(String_0C_Editor, 15, name: nameof(String_0C_Editor));
                    Byte_Editor = s.Serialize<byte>(Byte_Editor, name: nameof(Byte_Editor));
                }
            }

            ListEvents = s.SerializeObject<EVE_ListEvents>(ListEvents, x => x.Track = this, name: nameof(ListEvents));
        }

        [Flags]
        public enum TrackFlags : ushort
        {
            None = 0,
            Flag_0 = 1 << 0,
            Flag_1 = 1 << 1,
            Flag_2 = 1 << 2,
            Flag_3 = 1 << 3,
            Flag_4 = 1 << 4,
            Flag_5 = 1 << 5,
            Flag_6 = 1 << 6,
            Flag_7 = 1 << 7,
            Flag_8 = 1 << 8,
            Flag_9 = 1 << 9,
            Flag_10 = 1 << 10,
            Flag_11 = 1 << 11,
            Flag_12 = 1 << 12,
            Flag_13 = 1 << 13,
            Flag_14 = 1 << 14,
            Flag_15 = 1 << 15,
        }
    }
}