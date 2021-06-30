using System;
using BinarySerializer;

namespace R1Engine.Jade {
	// Found in GEO_LoadPCGeoObject
	public class GEO_GeoObject_PC : BinarySerializable {
		public GEO_GeometricObject GeometricObject { get; set; }

		public uint ElementsCount { get; set; }
		public ElementHeader[] ElementHeaders { get; set; }

		public uint VertexDataBufferSize { get; set; }
		public uint VertexDataBufferType { get; set; }
		public PointDataBuffer VertexData { get; set; }

		public uint ElementDataBufferSize { get; set; }
		public ElementDataBuffer ElementData { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			if (Loader.IsBinaryData) {
				ElementsCount = s.Serialize<uint>(ElementsCount, name: nameof(ElementsCount));
				ElementHeaders = s.SerializeObjectArray<ElementHeader>(ElementHeaders, ElementsCount, name: nameof(ElementHeaders));
			}

			if(s.CurrentAbsoluteOffset >=( GeometricObject.GRO.Object.Offset + GeometricObject.GRO.Object.FileSize).AbsoluteOffset) return;

			// TODO: If !IsBinaryData, the following data is in another file (key given by optimizedkey in GeometricObject)...

			VertexDataBufferSize = s.Serialize<uint>(VertexDataBufferSize, name: nameof(VertexDataBufferSize));
			if (VertexDataBufferSize != 0) {
				VertexDataBufferType = s.Serialize<uint>(VertexDataBufferType, name: nameof(VertexDataBufferType));
				VertexData = s.SerializeObject<PointDataBuffer>(VertexData, onPreSerialize: b => b.Geo = this, name: nameof(VertexData));

				ElementDataBufferSize = s.Serialize<uint>(ElementDataBufferSize, name: nameof(ElementDataBufferSize));
				if (ElementDataBufferSize != 0) {
					ElementData = s.SerializeObject<ElementDataBuffer>(ElementData, onPreSerialize: b => b.Geo = this, name: nameof(ElementData));
				}
			}

		}

		public class ElementHeader : BinarySerializable {
			public uint MaterialId { get; set; }
			public uint TriCount { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				MaterialId = s.Serialize<uint>(MaterialId, name: nameof(MaterialId));
				TriCount = s.Serialize<uint>(TriCount, name: nameof(TriCount));
			}
		}
		public class PointDataBuffer : BinarySerializable {
			public GEO_GeoObject_PC Geo { get; set; }
			public uint TotalSize => Geo.VertexDataBufferSize;

			public uint Count { get; set; }
			public uint PointDataSize { get; set; }
			public Point[] Points { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Count = s.Serialize<uint>(Count, name: nameof(Count));
				PointDataSize = s.Serialize<uint>(PointDataSize, name: nameof(PointDataSize));
				Points = s.SerializeObjectArray<Point>(Points, Count, onPreSerialize: v => v.Buffer = this, name: nameof(Points));

				if (s.CurrentAbsoluteOffset != Offset.AbsoluteOffset + TotalSize) {
					s.LogWarning($"Vertex Buffer at {Offset} wasn't serialized properly.");
					s.Goto(Offset + TotalSize);
				}

			}
			public class Point : BinarySerializable {
				public PointDataBuffer Buffer { get; set; }

				public Jade_Vector Vertex { get; set; }
				public Jade_Vector Normal { get; set; }
				public Jade_Vector StaticMesh { get; set; }
				public byte[] PointBytes { get; set; }
				public GEO_GeometricObject.UV UV { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					Vertex = s.SerializeObject<Jade_Vector>(Vertex, name: nameof(Vertex));
					if (Buffer.Geo.VertexDataBufferType != 0) Normal = s.SerializeObject<Jade_Vector>(Normal, name: nameof(Normal));
					PointBytes = s.SerializeArray<byte>(PointBytes, (Buffer.PointDataSize - (s.CurrentAbsoluteOffset - Offset.AbsoluteOffset) - 8), name: nameof(PointBytes));
					UV = s.SerializeObject<GEO_GeometricObject.UV>(UV, name: nameof(UV));
				}
			}
		}
		public class ElementDataBuffer : BinarySerializable {
			public GEO_GeoObject_PC Geo { get; set; }
			public uint TotalSize => Geo.ElementDataBufferSize;

			public ElementData[] ElementDatas { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				if (ElementDatas == null) ElementDatas = new ElementData[Geo.ElementsCount];
				for (int i = 0; i < Geo.ElementsCount; i++) {
					uint triCount = Geo.ElementHeaders[i].TriCount;
					if(triCount == 0) continue;
					ElementDatas[i] = s.SerializeObject<ElementData>(ElementDatas[i], onPreSerialize: ed => ed.TriCount = triCount, name: $"{nameof(ElementDatas)}[{i}]");
				}

				if (s.CurrentAbsoluteOffset != Offset.AbsoluteOffset + TotalSize) {
					s.LogWarning($"Element Data Buffer at {Offset} wasn't serialized properly.");
					s.Goto(Offset + TotalSize);
				}

			}

			public class ElementData : BinarySerializable {
				public uint TriCount { get; set; }
				public Triangle[] Triangles { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					Triangles = s.SerializeObjectArray<Triangle>(Triangles, TriCount, name: nameof(Triangles));
				}

				public class Triangle : BinarySerializable {
					public ushort Index0 { get; set; }
					public ushort Index1 { get; set; }
					public ushort Index2 { get; set; }

					public override void SerializeImpl(SerializerObject s) {
						Index0 = s.Serialize<ushort>(Index0, name: nameof(Index0));
						Index1 = s.Serialize<ushort>(Index1, name: nameof(Index1));
						Index2 = s.Serialize<ushort>(Index2, name: nameof(Index2));
					}
				}
			}
		}
	}
}
