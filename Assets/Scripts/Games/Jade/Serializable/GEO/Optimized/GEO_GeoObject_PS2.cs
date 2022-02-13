using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in GEO_LoadPS2GeoObject
	public class GEO_GeoObject_PS2 : BinarySerializable {
		public uint ElementsCount { get; set; }
		public uint[] Elements_MaterialId { get; set; }
		public uint BonesCount { get; set; }
		public Bone[] Bones { get; set; }
		public ElementDataBuffer ElementData { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if (Loader.IsBinaryData) {
				ElementsCount = s.Serialize<uint>(ElementsCount, name: nameof(ElementsCount));
				Elements_MaterialId = s.SerializeArray<uint>(Elements_MaterialId, ElementsCount, name: nameof(Elements_MaterialId));
				BonesCount = s.Serialize<uint>(BonesCount, name: nameof(BonesCount));
				Bones = s.SerializeObjectArray<Bone>(Bones, BonesCount, name: nameof(Bones));
			}
			// TODO: If !IsBinaryData, the following data is in another file (key given by optimizedkey in GeometricObject)...
			ElementData = s.SerializeObject<ElementDataBuffer>(ElementData, onPreSerialize: b => b.Geo = this, name: nameof(ElementData));

		}

		public class Bone : BinarySerializable {
			public uint Index { get; set; }
			public Jade_Matrix Matrix { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Index = s.Serialize<uint>(Index, name: nameof(Index));
				Matrix = s.SerializeObject<Jade_Matrix>(Matrix, name: nameof(Matrix));
			}
		}

		public class ElementDataBuffer : BinarySerializable {
			public GEO_GeoObject_PS2 Geo { get; set; }

			public uint ElementDataBufferSize { get; set; }
			public ElementData[] ElementDatas { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				ElementDataBufferSize = s.Serialize<uint>(ElementDataBufferSize, name: nameof(ElementDataBufferSize));
				if (ElementDataBufferSize > 4) {
					ElementDatas = s.SerializeObjectArray<ElementData>(ElementDatas, Geo.ElementsCount, name: nameof(ElementDatas));
				}
				if (s.CurrentAbsoluteOffset != Offset.AbsoluteOffset + ElementDataBufferSize && ElementDataBufferSize > 4) {
					s.LogWarning($"Element Data Buffer at {Offset} wasn't serialized properly: current:{s.CurrentPointer} vs expected:{Offset + ElementDataBufferSize}");
					s.Goto(Offset + ElementDataBufferSize);
				}

			}
			public class ElementData : BinarySerializable {
				public Jade_Code FoceFace { get; set; } // 0xF0CEFACE
				public uint Count { get; set; }
				public MeshElement[] MeshElements { get; set; }
				public Jade_Code Deadbeef { get; set; } // 0xDEADBEEF

				public override void SerializeImpl(SerializerObject s) {
					FoceFace = s.Serialize<Jade_Code>(FoceFace, name: nameof(FoceFace));
					Count = s.Serialize<uint>(Count, name: nameof(Count));
					MeshElements = s.SerializeObjectArray<MeshElement>(MeshElements, Count, name: nameof(MeshElements));
					Deadbeef = s.Serialize<Jade_Code>(Deadbeef, name: nameof(Deadbeef));
				}
			}
			public class MeshElement : BinarySerializable {
				public Jade_Vector BoundingVolumeMin { get; set; }
				public float Float0 { get; set; }
				public Jade_Vector BoundingVolumeMax { get; set; }
				public float Float1 { get; set; }
				public uint UInt2 { get; set; } // Unused
				public uint BonesCount { get; set; }
				public byte[] Bones { get; set; }
				public uint VIFProgramsCount { get; set; }
				public PS2_DMAChainData[] VIFPrograms { get; set; }
				public uint DMAChainProgramsCount { get; set; }
				public PS2_DMAChainProgram[] DMAChainPrograms { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					BoundingVolumeMin = s.SerializeObject<Jade_Vector>(BoundingVolumeMin, name: nameof(BoundingVolumeMin));
					if(s.GetR1Settings().Platform != Platform.PSP) Float0 = s.Serialize<float>(Float0, name: nameof(Float0));
					BoundingVolumeMax = s.SerializeObject<Jade_Vector>(BoundingVolumeMax, name: nameof(BoundingVolumeMax));
					Float1 = s.Serialize<float>(Float1, name: nameof(Float1));
					UInt2 = s.Serialize<uint>(UInt2, name: nameof(UInt2));
					BonesCount = s.Serialize<uint>(BonesCount, name: nameof(BonesCount));
					Bones = s.SerializeArray<byte>(Bones, BonesCount, name: nameof(Bones));
					VIFProgramsCount = s.Serialize<uint>(VIFProgramsCount, name: nameof(VIFProgramsCount));
					VIFPrograms = s.SerializeObjectArray<PS2_DMAChainData>(VIFPrograms, VIFProgramsCount, onPreSerialize: p => p.Pre_IsInstance = false, name: nameof(VIFPrograms));
					if (s.GetR1Settings().Platform == Platform.PS2) {
						DMAChainProgramsCount = s.Serialize<uint>(DMAChainProgramsCount, name: nameof(DMAChainProgramsCount));
						DMAChainPrograms = s.SerializeObjectArray<PS2_DMAChainProgram>(DMAChainPrograms, DMAChainProgramsCount, name: nameof(DMAChainPrograms));
					}
				}
			}
		}
	}
}
