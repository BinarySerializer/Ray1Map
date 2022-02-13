using BinarySerializer;

namespace Ray1Map.Jade {
	public class OBJ_GameObject_GeometricData_Xenon : BinarySerializable {
		public uint Version { get; set; } // Set in on PreSerialize

		public uint UInt_00 { get; set; }
		public Jade_TextureReference Lightmap { get; set; }
		public uint LightmapStructsCount { get; set; }
		public LightmapStruct[] LightmapStructs { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (Version > 2) {
				UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
				Lightmap = s.SerializeObject<Jade_TextureReference>(Lightmap, name: nameof(Lightmap))?.Resolve();
				if (!Lightmap.IsNull) {
					LightmapStructsCount = s.Serialize<uint>(LightmapStructsCount, name: nameof(LightmapStructsCount));
					LightmapStructs = s.SerializeObjectArray<LightmapStruct>(LightmapStructs, LightmapStructsCount, name: nameof(LightmapStructs));
				}
			}
		}

		public class LightmapStruct : BinarySerializable {
			public uint Count { get; set; }
			public Entry[] Entries { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Count = s.Serialize<uint>(Count, name: nameof(Count));
				Entries = s.SerializeArray<Entry>(Entries, Count, name: nameof(Entries));
			}

			public class Entry : BinarySerializable {
				public float Float_00 { get; set; }
				public float Float_04 { get; set; }
				public float Float_08 { get; set; }
				public float Float_0C { get; set; }
				public float Float_10 { get; set; }
				public float Float_14 { get; set; }
				public override void SerializeImpl(SerializerObject s) {
					Float_00 = s.Serialize<float>(Float_00, name: nameof(Float_00));
					Float_04 = s.Serialize<float>(Float_04, name: nameof(Float_04));
					Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
					Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
					Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
					Float_14 = s.Serialize<float>(Float_14, name: nameof(Float_14));
				}
			}
		}
	}
}
