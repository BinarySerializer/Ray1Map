using System;
using BinarySerializer;

namespace R1Engine.Jade {
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
			public uint UInt_00 { get; set; }
			public Jade_Matrix Matrix { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
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
				public uint VIFPrograms1Count { get; set; }
				public VIFProgram1[] VIFPrograms1 { get; set; }
				public uint VIFPrograms2Count { get; set; }
				public VIFProgram2[] VIFPrograms2 { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					BoundingVolumeMin = s.SerializeObject<Jade_Vector>(BoundingVolumeMin, name: nameof(BoundingVolumeMin));
					if(s.GetR1Settings().Platform != Platform.PSP) Float0 = s.Serialize<float>(Float0, name: nameof(Float0));
					BoundingVolumeMax = s.SerializeObject<Jade_Vector>(BoundingVolumeMax, name: nameof(BoundingVolumeMax));
					Float1 = s.Serialize<float>(Float1, name: nameof(Float1));
					UInt2 = s.Serialize<uint>(UInt2, name: nameof(UInt2));
					BonesCount = s.Serialize<uint>(BonesCount, name: nameof(BonesCount));
					Bones = s.SerializeArray<byte>(Bones, BonesCount, name: nameof(Bones));
					VIFPrograms1Count = s.Serialize<uint>(VIFPrograms1Count, name: nameof(VIFPrograms1Count));
					VIFPrograms1 = s.SerializeObjectArray<VIFProgram1>(VIFPrograms1, VIFPrograms1Count, name: nameof(VIFPrograms1));
					if (s.GetR1Settings().Platform != Platform.PSP) {
						VIFPrograms2Count = s.Serialize<uint>(VIFPrograms2Count, name: nameof(VIFPrograms2Count));
						VIFPrograms2 = s.SerializeObjectArray<VIFProgram2>(VIFPrograms2, VIFPrograms2Count, name: nameof(VIFPrograms2));
					}
				}

				public class VIFProgram2 : BinarySerializable {
					public uint ID { get; set; } // Used as address (ADDR = ID << 24)
					public uint DataSize { get; set; }
					public byte[] Bytes { get; set; } // GeometryCommands
					public PS2_GeometryCommand[] Commands { get; set; }
					public override void SerializeImpl(SerializerObject s) {
						ID = s.Serialize<uint>(ID, name: nameof(ID));
						DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
						Bytes = s.SerializeArray<byte>(Bytes, DataSize, name: nameof(Bytes));
						/*Commands = s.SerializeObjectArrayUntil<PS2_GeometryCommand>(Commands, gc => s.CurrentAbsoluteOffset >= Offset.AbsoluteOffset + 8 + DataSize, name: nameof(Commands));
						if(s.CurrentAbsoluteOffset > Offset.AbsoluteOffset + 8 + DataSize) {
							s.LogWarning($"{Offset}: Read too many geometry commands");
						}
						s.Goto(Offset + 8 + DataSize);*/
					}
				}

				public class VIFProgram1 : BinarySerializable {
					public uint ID { get; set; } // Used as address (ADDR = ID << 24)
					public uint DataSize { get; set; }
					public byte[] Bytes { get; set; } // GeometryCommands
					public PS2_GeometryCommand[] Commands { get; set; }
					public override void SerializeImpl(SerializerObject s) {
						if (s.GetR1Settings().Platform == Platform.PSP) {
							ID = s.Serialize<ushort>((ushort)ID, name: nameof(ID));
						} else {
							ID = s.Serialize<uint>(ID, name: nameof(ID));
						}
						DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
						Bytes = s.SerializeArray<byte>(Bytes, DataSize, name: nameof(Bytes));
						/*Commands = s.SerializeObjectArrayUntil<PS2_GeometryCommand>(Commands, gc => s.CurrentAbsoluteOffset >= Offset.AbsoluteOffset + 8 + DataSize, name: nameof(Commands));
						if (s.CurrentAbsoluteOffset > Offset.AbsoluteOffset + 8 + DataSize) {
							s.LogWarning($"{Offset}: Read too many geometry commands");
						}
						s.Goto(Offset + 8 + DataSize);*/
					}
				}
			}
		}
	}
}
