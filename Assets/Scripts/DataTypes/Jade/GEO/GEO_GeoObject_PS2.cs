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
				public Jade_Code Element_StartCode { get; set; } // 0xF0CEFACE
				public uint Count { get; set; }
				public MeshElement[] MeshElements { get; set; }
				public Jade_Code Element_EndCode { get; set; } // 0xDEADBEEF

				public override void SerializeImpl(SerializerObject s) {
					Element_StartCode = s.Serialize<Jade_Code>(Element_StartCode, name: nameof(Element_StartCode));
					Count = s.Serialize<uint>(Count, name: nameof(Count));
					MeshElements = s.SerializeObjectArray<MeshElement>(MeshElements, Count, name: nameof(MeshElements));
					Element_EndCode = s.Serialize<Jade_Code>(Element_EndCode, name: nameof(Element_EndCode));
				}
			}
			public class MeshElement : BinarySerializable {
				public Jade_Vector Vector0 { get; set; }
				public float Float0 { get; set; }
				public Jade_Vector Vector1 { get; set; }
				public float Float1 { get; set; }
				public uint UInt2 { get; set; }
				public uint Count0 { get; set; }
				public byte[] Bytes0 { get; set; }
				public uint Count1 { get; set; }
				public Struct1[] Structs1 { get; set; }
				public uint Count2 { get; set; }
				public Struct2[] Structs2 { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					Vector0 = s.SerializeObject<Jade_Vector>(Vector0, name: nameof(Vector0));
					Float0 = s.Serialize<float>(Float0, name: nameof(Float0));
					Vector1 = s.SerializeObject<Jade_Vector>(Vector1, name: nameof(Vector1));
					Float1 = s.Serialize<float>(Float1, name: nameof(Float1));
					UInt2 = s.Serialize<uint>(UInt2, name: nameof(UInt2));
					Count0 = s.Serialize<uint>(Count0, name: nameof(Count0));
					Bytes0 = s.SerializeArray<byte>(Bytes0, Count0, name: nameof(Bytes0));
					Count1 = s.Serialize<uint>(Count1, name: nameof(Count1));
					Structs1 = s.SerializeObjectArray<Struct1>(Structs1, Count1, name: nameof(Structs1));
					Count2 = s.Serialize<uint>(Count2, name: nameof(Count2));
					Structs2 = s.SerializeObjectArray<Struct2>(Structs2, Count2, name: nameof(Structs2));
				}

				public class Struct2 : BinarySerializable {
					public uint UInt_00 { get; set; }
					public uint DataSize { get; set; }
					public byte[] Bytes { get; set; }
					public override void SerializeImpl(SerializerObject s) {
						UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
						DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
						Bytes = s.SerializeArray<byte>(Bytes, DataSize, name: nameof(Bytes));
					}
				}

				public class Struct1 : BinarySerializable {
					public uint UInt_00 { get; set; }
					public uint DataSize { get; set; }
					public byte[] Bytes { get; set; }
					public override void SerializeImpl(SerializerObject s) {
						UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
						DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
						Bytes = s.SerializeArray<byte>(Bytes, DataSize, name: nameof(Bytes));
					}
				}
			}
		}
	}
}
