using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class LIGHT_Rejection : Jade_File {
		public uint Version { get; set; }
		public Entry[] V0_Entries { get; set; }
		public uint V2_Count { get; set; }
		public EntryV2[] V2_Entries { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version < 2) {
				V0_Entries = s.SerializeObjectArrayUntil<Entry>(V0_Entries, e => e.Int_00 == -1, name: nameof(V0_Entries));
			} else if (Version == 2) {
				V2_Count = s.Serialize<uint>(V2_Count, name: nameof(V2_Count));
				if (V2_Count > 0x2800) return;
				if (V2_Entries == null) V2_Entries = new EntryV2[V2_Count];
				for (int i = 0; i < V2_Entries.Length; i++) {
					V2_Entries[i] = s.SerializeObject<EntryV2>(V2_Entries[i], name: $"{nameof(V2_Entries)}[{i}]");
					if(V2_Entries[i].Count > 0x2800) break;
				}
			}
		}

		public class Entry : BinarySerializable {
			public int Int_00 { get; set; }
			public int Int_04 { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
				if (Int_00 != -1) {
					Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));
				}
			}
		}

		public class EntryV2 : BinarySerializable {
			public int Int_00 { get; set; }
			public uint Count { get; set; }
			public uint[] UInts { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
				Count = s.Serialize<uint>(Count, name: nameof(Count));
				if (Count > 0x2800) return;
				UInts = s.SerializeArray<uint>(UInts, Count, name: nameof(UInts));
			}
		}
	}
}
