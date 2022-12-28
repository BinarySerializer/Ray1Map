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

            if (Flags.HasFlag(TrackFlags.Anims))
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
            RunningInit = 1 << 0,
            AutoLoop = 1 << 1,
            AutoStop = 1 << 2,
            Time = 1 << 3,
            MustResolve = 1 << 4,
            Hidden = 1 << 5,
            Selected = 1 << 6,
            Curve = 1 << 7,
            FirstEvent = 1 << 8,
            Under256 = 1 << 9,
            Flash = 1 << 10,
            SameFlags = 1 << 11,
            SameType = 1 << 12,
            SameSize = 1 << 13,
            Optimized = 1 << 14,
            Anims = 1 << 15,
        }
    }
}