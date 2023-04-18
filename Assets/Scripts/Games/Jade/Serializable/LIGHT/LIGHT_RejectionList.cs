using BinarySerializer;

namespace Ray1Map.Jade {
	public class LIGHT_RejectionList : Jade_File {
		public override string Export_Extension => "lrl";
		public uint Version { get; set; }
		public Entry[] V0_Entries { get; set; }
		public uint V2_Count { get; set; }
		public EntryV2[] V2_Objects { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version < 2) {
				V0_Entries = s.SerializeObjectArrayUntil<Entry>(V0_Entries, e => e.LightKey == -1, name: nameof(V0_Entries));
			} else if (Version == 2) {
				V2_Count = s.Serialize<uint>(V2_Count, name: nameof(V2_Count));
				if (V2_Count > 0x2800) return;
				if (V2_Objects == null) V2_Objects = new EntryV2[V2_Count];
				for (int i = 0; i < V2_Objects.Length; i++) {
					V2_Objects[i] = s.SerializeObject<EntryV2>(V2_Objects[i], name: $"{nameof(V2_Objects)}[{i}]");
					if(V2_Objects[i].LightsCount > 0x2800) break;
				}
			}
		}

		public class Entry : BinarySerializable {
			public int LightKey { get; set; }
			public int ObjectKey { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				LightKey = s.Serialize<int>(LightKey, name: nameof(LightKey));
				if (LightKey != -1) {
					ObjectKey = s.Serialize<int>(ObjectKey, name: nameof(ObjectKey));
				}
			}
		}

		public class EntryV2 : BinarySerializable {
			public Jade_Reference<OBJ_GameObject> ObjectKey { get; set; }
			public uint LightsCount { get; set; }
			public Jade_Reference<OBJ_GameObject>[] Lights { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				ObjectKey = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(ObjectKey, name: nameof(ObjectKey));
				LightsCount = s.Serialize<uint>(LightsCount, name: nameof(LightsCount));
				if (LightsCount > 0x2800) return;
				Lights = s.SerializeObjectArray<Jade_Reference<OBJ_GameObject>>(Lights, LightsCount, name: nameof(Lights));
			}
		}
	}
}
