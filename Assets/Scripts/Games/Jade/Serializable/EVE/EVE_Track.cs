using System;
using BinarySerializer;

namespace Ray1Map.Jade
{
    public class EVE_Track : BinarySerializable {
        public EVE_ListTracks ListTracks { get; set; } // Set in onPreSerialize
        
        public TrackFlags Flags { get; set; }
        public ushort Gizmo { get; set; }
        public uint DataLength { get; set; }
        public int Type { get; set; }
        public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
        public string String_0C_Editor { get; set; }
        public byte Byte_Editor { get; set; }
        public EVE_ListEvents ListEvents { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            Flags = s.Serialize<TrackFlags>(Flags, name: nameof(Flags));
            Gizmo = s.Serialize<ushort>(Gizmo, name: nameof(Gizmo));
            DataLength = s.Serialize<uint>(DataLength, name: nameof(DataLength));

            if (Flags.HasFlag(TrackFlags.Flag_15))
            {
                if (((ushort)Flags & 0x3F00) == 0)
                    Type = s.Serialize<int>(Type, name: nameof(Type));
            }
            else
            {
                GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();

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