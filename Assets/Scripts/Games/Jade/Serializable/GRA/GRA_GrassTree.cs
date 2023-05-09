using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in GRA_GrassField_CreateFromBuffer
	public class GRA_GrassTree : BinarySerializable {
		public uint Pre_ObjectVersion { get; set; }

		public Jade_Vector BoxOverhead { get; set; }
		public uint NodesCount { get; set; }
		public Node[] Nodes { get; set; }
		public uint PatchesCount { get; set; }
		public Patch[] Patches { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			BoxOverhead = s.SerializeObject<Jade_Vector>(BoxOverhead, name: nameof(BoxOverhead));
			NodesCount = s.Serialize<uint>(NodesCount, name: nameof(NodesCount));
			Nodes = s.SerializeObjectArray<Node>(Nodes, NodesCount, name: nameof(Nodes));
			PatchesCount = s.Serialize<uint>(PatchesCount, name: nameof(PatchesCount));
			Patches = s.SerializeObjectArray<Patch>(Patches, PatchesCount, onPreSerialize: p => p.Pre_ObjectVersion = Pre_ObjectVersion, name: nameof(Patches));
		}

		public class Node : BinarySerializable {
			public ushort UShort0 { get; set; }
			public ushort UShort1 { get; set; }
			public Jade_Vector Vector0 { get; set; }
			public Jade_Vector Vector1 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				UShort0 = s.Serialize<ushort>(UShort0, name: nameof(UShort0));
				UShort1 = s.Serialize<ushort>(UShort1, name: nameof(UShort1));
				Vector0 = s.SerializeObject<Jade_Vector>(Vector0, name: nameof(Vector0));
				Vector1 = s.SerializeObject<Jade_Vector>(Vector1, name: nameof(Vector1));
			}
		}
		public class Patch : BinarySerializable {
			public uint Pre_ObjectVersion { get; set; }

			public Jade_Vector Min { get; set; }
			public Jade_Vector Max { get; set; }
			public GRA_GrassSample[] Samples { get; set; }
			public GRA_GrassTile[] Tiles { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

				Min = s.SerializeObject<Jade_Vector>(Min, name: nameof(Min));
				Max = s.SerializeObject<Jade_Vector>(Max, name: nameof(Max));
				Samples = s.SerializeObjectArray<GRA_GrassSample>(Samples, 17 * 17, onPreSerialize: smp => smp.Pre_ObjectVersion = Pre_ObjectVersion, name: nameof(Samples));
				Tiles = s.SerializeObjectArray<GRA_GrassTile>(Tiles, 16 * 16, name: nameof(Tiles));
				if (!Loader.IsBinaryData) {
					foreach (var samp in Samples) samp.SerializeEditorData(s);
				}
			}
		}
	}
}
