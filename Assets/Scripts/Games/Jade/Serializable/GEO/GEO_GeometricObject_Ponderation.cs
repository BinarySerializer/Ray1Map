using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in GEO_SKN_Load
	public class GEO_GeometricObject_Ponderation : BinarySerializable {
		public short Flags { get; set; }
		public ushort PonderationListsCount { get; set; } // BonesCount
		public VertexPonderationList[] PonderationLists { get; set; } // Bones

		public override void SerializeImpl(SerializerObject s) {
			Flags = s.Serialize<short>(Flags, name: nameof(Flags));
			PonderationListsCount = s.Serialize<ushort>(PonderationListsCount, name: nameof(PonderationListsCount));
			PonderationLists = s.SerializeObjectArray<VertexPonderationList>(PonderationLists, PonderationListsCount, name: nameof(PonderationLists));
		}

		public class VertexPonderationList : BinarySerializable {
			public short IndexOfMatrix { get; set; } // BoneID
			public ushort PonderatedVerticesCount { get; set; } // WeightsCount
			public Jade_Matrix FlashedMatrix { get; set; } // BindPose
			public float[] Ponderations { get; set; } // Weights
			public int[] Indices { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				IndexOfMatrix = s.Serialize<short>(IndexOfMatrix, name: nameof(IndexOfMatrix));
				PonderatedVerticesCount = s.Serialize<ushort>(PonderatedVerticesCount, name: nameof(PonderatedVerticesCount));
				FlashedMatrix = s.SerializeObject<Jade_Matrix>(FlashedMatrix, name: nameof(FlashedMatrix));
				Ponderations = s.SerializeArray<float>(Ponderations, PonderatedVerticesCount, name: nameof(Ponderations));
				if (Indices == null) {
					s.DoAt(s.CurrentPointer - 4 * PonderatedVerticesCount, () => {
						Indices = new int[PonderatedVerticesCount];
						for (int i = 0; i < PonderatedVerticesCount; i++) {
							s.DoBits<uint>(b => {
								Indices[i] = b.SerializeBits<int>(Indices[i], 16, name: $"{nameof(Indices)}[{i}]");
							});
						}
					});
				}
			}
		}
	}
}
