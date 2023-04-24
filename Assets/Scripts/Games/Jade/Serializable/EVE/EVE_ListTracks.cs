using BinarySerializer;
using System;

namespace Ray1Map.Jade
{
    public class EVE_ListTracks : Jade_File {
		public override string Export_Extension => "trl";
		public override bool HasHeaderBFFile => true;
		public bool Pre_IsEmbedded { get; set; } = false;

        public Jade_Reference<EVE_ListTracks> ListTracks_TRS { get; set; }
        public ushort TracksCount { get; set; }
        public ushort TracksCount2 { get; set; }
        public TracksFlags Flags { get; set; }
        public EVE_Track[] Tracks { get; set; }
        public ushort Montreal_Version { get; set; }

		public byte[] FastInterpolationKeyTracks { get; set; }

		public uint TotalTracksCount => (uint)Tracks.Length + (uint)(ListTracks_TRS?.Value?.Tracks?.Length ?? 0);

        protected override void SerializeFile(SerializerObject s)
        {
            if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_TMNT)) {
				ListTracks_TRS = s.SerializeObject<Jade_Reference<EVE_ListTracks>>(ListTracks_TRS, name: nameof(ListTracks_TRS));
                if (Loader.IsBinaryData) {
                    ListTracks_TRS?.ResolveEmbedded(s, onPreSerialize: (_,e) => e.Pre_IsEmbedded = true, flags: LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.DontUseCachedFile | LOA_Loader.ReferenceFlags.MustExist, unknownFileSize: true);
                } else {
                    ListTracks_TRS?.Resolve();
                }
			}
            bool useCount2 = false;
            TracksCount = s.Serialize<ushort>(TracksCount, name: nameof(TracksCount));
            if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && TracksCount >= 0x8000) {
                Montreal_Version = TracksCount;
                TracksCount2 = s.Serialize<ushort>(TracksCount2, name: nameof(TracksCount2));
                useCount2 = true;
            }
			Flags = s.Serialize<TracksFlags>(Flags, name: nameof(Flags));
            Tracks = s.SerializeObjectArray<EVE_Track>(Tracks, useCount2 ? TracksCount2 : TracksCount, onPreSerialize: trk => trk.ListTracks = this, name: nameof(Tracks));

			if (!Pre_IsEmbedded && s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CPP)) {
				FastInterpolationKeyTracks = s.SerializeArray<byte>(FastInterpolationKeyTracks, (TotalTracksCount / 8) + (TotalTracksCount % 8 == 0 ? 0 : 1), name: nameof(FastInterpolationKeyTracks));
			}
        }

		protected override void OnChangeContext(Context oldContext, Context newContext) {
			base.OnChangeContext(oldContext, newContext);
            if (newContext.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier) && oldContext.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
                TracksCount = 0;
                TracksCount2 = 0;
                Tracks = new EVE_Track[0];
            }
		}

		[Flags]
		public enum TracksFlags : ushort {
			None = 0,
			NoFlash = 1 << 0, // Do never matrix flash
			Resolved = 1 << 1, // List of tracks has been resolved
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
			ForceARAM = 1 << 14,
			Anims = 1 << 15, // List of tracks is part of an anim (Load/Save only)
		}
	}
}