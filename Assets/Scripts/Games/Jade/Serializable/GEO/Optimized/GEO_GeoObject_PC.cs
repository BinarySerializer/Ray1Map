using BinarySerializer;
using System.Linq;

namespace Ray1Map.Jade {
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

			if(s.CurrentAbsoluteOffset >= (GeometricObject.GRO.Object.Offset + GeometricObject.GRO.Object.FileSize).AbsoluteOffset) return;

			// TODO: If !IsBinaryData, the following data is in another file (key given by optimizedkey in GeometricObject)...

			VertexDataBufferSize = s.Serialize<uint>(VertexDataBufferSize, name: nameof(VertexDataBufferSize));
			if (VertexDataBufferSize != 0) {
				VertexDataBufferType = s.Serialize<uint>(VertexDataBufferType, name: nameof(VertexDataBufferType));
				if (s.GetR1Settings().Platform == Platform.PS3 && s.GetR1Settings().EngineVersion == EngineVersion.Jade_PoP_SoT) {
					s.DoEndian(Endian.Big, () => {
						VertexData = s.SerializeObject<PointDataBuffer>(VertexData, onPreSerialize: b => b.Geo = this, name: nameof(VertexData));
					});
				} else {
					VertexData = s.SerializeObject<PointDataBuffer>(VertexData, onPreSerialize: b => b.Geo = this, name: nameof(VertexData));
				}

				ElementDataBufferSize = s.Serialize<uint>(ElementDataBufferSize, name: nameof(ElementDataBufferSize));
				if (ElementDataBufferSize != 0) {
					if (s.GetR1Settings().Platform == Platform.PS3 && s.GetR1Settings().EngineVersion == EngineVersion.Jade_PoP_SoT) {
						s.DoEndian(Endian.Big, () => {
							ElementData = s.SerializeObject<ElementDataBuffer>(ElementData, onPreSerialize: b => b.Geo = this, name: nameof(ElementData));
						});
					} else {
						ElementData = s.SerializeObject<ElementDataBuffer>(ElementData, onPreSerialize: b => b.Geo = this, name: nameof(ElementData));
					}
				}
			}

		}

		public class ElementHeader : BinarySerializable {
			public uint MaterialId { get; set; }
			public uint TriCount { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				MaterialId = s.Serialize<uint>(MaterialId, name: nameof(MaterialId));
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_SoT)) {
					TriCount = s.Serialize<uint>(TriCount, name: nameof(TriCount));
				}
			}
		}
		public class PointDataBuffer : BinarySerializable {
			public GEO_GeoObject_PC Geo { get; set; }
			public uint TotalSize => Geo.VertexDataBufferSize;

			public uint Count { get; set; }
			public uint PointDataSize { get; set; }
			public uint PS3PoP_OriginalSize { get; set; }
			public uint PS3PoP_OffsetInBF { get; set; }
			public Point[] Points { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Count = s.Serialize<uint>(Count, name: nameof(Count));
				PointDataSize = s.Serialize<uint>(PointDataSize, name: nameof(PointDataSize));
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_WW_20040920) && s.GetR1Settings().Platform == Platform.PS3) {
					PS3PoP_OriginalSize = s.Serialize<uint>(PS3PoP_OriginalSize, name: nameof(PS3PoP_OriginalSize));
					PS3PoP_OffsetInBF = s.Serialize<uint>(PS3PoP_OffsetInBF, name: nameof(PS3PoP_OffsetInBF));
				} else {
					Points = s.SerializeObjectArray<Point>(Points, Count, onPreSerialize: v => v.Buffer = this, name: nameof(Points));
				}

				if (s.CurrentAbsoluteOffset != Offset.AbsoluteOffset + TotalSize) {
					s.SystemLogger?.LogWarning($"Vertex Buffer at {Offset} wasn't serialized properly.");
					s.Goto(Offset + TotalSize);
				}

			}
			public class Point : BinarySerializable {
				public PointDataBuffer Buffer { get; set; }

				public ushort OriginalVertexIndex => Ponderations[0].Index;

				public Jade_Vector Vertex { get; set; }
				public Jade_Vector Normal { get; set; }
				public Jade_Vector StaticMesh { get; set; }
				public ushort[] BoneIndices { get; set; }
				public GEO_GeometricObject_VertexPonderation[] Ponderations { get; set; }
				public GEO_GeometricObject.UV UV { get; set; }

				public BGRA8888Color Color { get; set; }
				public float[] UnknownFloats { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					Vertex = s.SerializeObject<Jade_Vector>(Vertex, name: nameof(Vertex));
					if (Buffer.Geo.VertexDataBufferType != 0) Normal = s.SerializeObject<Jade_Vector>(Normal, name: nameof(Normal));
					if (Buffer.Geo.VertexDataBufferType == 3) {
						BoneIndices = s.SerializeArray<ushort>(BoneIndices, 3, name: nameof(BoneIndices));
						if (s.GetR1Settings().Platform == Platform.PC || s.GetR1Settings().Platform == Platform.iOS) s.Align(4, Offset);
						Ponderations = s.SerializeObjectArray<GEO_GeometricObject_VertexPonderation>(Ponderations, 3, name: nameof(Ponderations));
					}
					UV = s.SerializeObject<GEO_GeometricObject.UV>(UV, name: nameof(UV));

					if (Buffer.Geo.VertexDataBufferType == 8) {
						Color = s.SerializeObject<BGRA8888Color>(Color, name: nameof(Color));
						UnknownFloats = s.SerializeArray<float>(UnknownFloats, 2, name: nameof(UnknownFloats));
					}

					if (s.CurrentAbsoluteOffset != Offset.AbsoluteOffset + Buffer.PointDataSize) {
						s.SystemLogger?.LogWarning($"Vertex Data at {Offset} wasn't serialized properly.");
						s.Goto(Offset + Buffer.PointDataSize);
					}
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
					uint triCount = 0;

					if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_SoT)) {
						triCount = Geo.ElementHeaders[i].TriCount;
					} else {
						triCount = Geo.GeometricObject.Elements[i].TrianglesCount;
					}
					if(triCount == 0) continue;
					ElementDatas[i] = s.SerializeObject<ElementData>(ElementDatas[i], onPreSerialize: ed => ed.TriCount = triCount, name: $"{nameof(ElementDatas)}[{i}]");
				}

				if (s.CurrentAbsoluteOffset != Offset.AbsoluteOffset + TotalSize) {
					s.SystemLogger?.LogWarning($"Element Data Buffer at {Offset} wasn't serialized properly.");
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

		#region Helpers

		public static float ProcessColorValue(float color) => System.MathF.Min(1f, color * 2f);
		public static SerializableColor ProcessColor(BaseColor color, bool multiplyAlpha = false) => new SerializableColor(
			ProcessColorValue(color.Red), ProcessColorValue(color.Green), ProcessColorValue(color.Blue), multiplyAlpha ? ProcessColorValue(color.Alpha) : color.Alpha);

		#endregion

		public void Unoptimize() {
			if(VertexDataBufferSize == 0) return;
			if (Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_WW_20040920) && Context.GetR1Settings().Platform == Platform.PS3) {
				throw new System.NotImplementedException("Read points data from geometry BF");
			}
			bool restoreOriginalVertexIndices = VertexDataBufferType == 3; // Due to original indices being stored in Ponderations, we can determine the original vertex index ^^

			// TODO: Only keep unique UVs?
			GeometricObject.UVsCount = VertexData.Count;
			GeometricObject.UVs = VertexData.Points.Select(p => p.UV).ToArray();

			GeometricObject.Montreal_HasNormals = VertexDataBufferType != 0 ? 1 : 0;
			GeometricObject.Montreal_HasColors = VertexDataBufferType == 8 ? 1 : 0;

			if (restoreOriginalVertexIndices) {
				uint verticesCount = (uint)VertexData.Points.Max(p => p.Ponderations[0].Index) + 1;
				GeometricObject.Vertices = new Jade_Vector[verticesCount];
				GeometricObject.Normals = new Jade_Vector[verticesCount];
				if (GeometricObject.Montreal_HasColors == 1) GeometricObject.Colors = new SerializableColor[verticesCount];

				foreach (var point in VertexData.Points) {
					GeometricObject.Vertices[point.OriginalVertexIndex] = point.Vertex;
					GeometricObject.Normals[point.OriginalVertexIndex] = point.Normal;
					if (GeometricObject.Montreal_HasColors == 1) GeometricObject.Colors[point.OriginalVertexIndex] = ProcessColor(point.Color);
				}
			} else {
				GeometricObject.Vertices = VertexData.Points.Select(v => v.Vertex).ToArray();
				GeometricObject.Normals = VertexData.Points.Select(v => v.Normal).ToArray();
				if (GeometricObject.Montreal_HasColors == 1) GeometricObject.Colors = VertexData.Points.Select(v => ProcessColor(v.Color)).ToArray();
			}
			GeometricObject.VerticesCount = (uint)GeometricObject.Vertices.Length;
			GeometricObject.Code_00 = GeometricObject.VerticesCount;
			if (GeometricObject.Montreal_HasColors == 1) GeometricObject.ColorsCount = (uint)GeometricObject.Colors.Length;

			GeometricObject.ElementsCount = ElementsCount;
			GeometricObject.Elements = new GEO_GeometricObjectElement[ElementsCount];
			for (int i = 0; i < ElementsCount; i++) {
				GEO_GeometricObjectElement element = new GEO_GeometricObjectElement();
				element.GeometricObject = GeometricObject;
				element.MaterialID = ElementHeaders[i].MaterialId;
				element.TrianglesCount = ElementData.ElementDatas[i]?.TriCount ?? 0;
				if (element.TrianglesCount == 0) {
					element.Triangles = new GEO_GeometricObjectElement.Triangle[0];
				} else {
					element.Triangles = ElementData.ElementDatas[i].Triangles.Select(t => new GEO_GeometricObjectElement.Triangle() {
						UV0 = t.Index0,
						UV1 = t.Index1,
						UV2 = t.Index2,
						Vertex0 = t.Index0,
						Vertex1 = t.Index1,
						Vertex2 = t.Index2,
						SmoothingGroup = (uint)i, // TODO
					}).ToArray();
					if (restoreOriginalVertexIndices) {
						foreach (var tri in element.Triangles) {
							tri.Vertex0 = VertexData.Points[tri.Vertex0].OriginalVertexIndex;
							tri.Vertex1 = VertexData.Points[tri.Vertex1].OriginalVertexIndex;
							tri.Vertex2 = VertexData.Points[tri.Vertex2].OriginalVertexIndex;
						}
					}
				}
				GeometricObject.Elements[i] = element;
			}
		}
	}
}
