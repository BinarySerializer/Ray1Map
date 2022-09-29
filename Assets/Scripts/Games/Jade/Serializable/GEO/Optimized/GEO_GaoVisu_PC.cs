using BinarySerializer;
using System.Linq;

namespace Ray1Map.Jade {
	public class GEO_GaoVisu_PC : BinarySerializable {
		public uint VertexDataBufferType { get; set; }
		public uint VertexDataBufferSize { get; set; }
		public PointDataBuffer VertexData { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			VertexDataBufferType = s.Serialize<uint>(VertexDataBufferType, name: nameof(VertexDataBufferType));
			if (VertexDataBufferType != 0) {
				VertexDataBufferSize = s.Serialize<uint>(VertexDataBufferSize, name: nameof(VertexDataBufferSize));
				if (s.GetR1Settings().Platform == Platform.PS3 && s.GetR1Settings().EngineVersion == EngineVersion.Jade_PoP_SoT) {
					s.DoEndian(Endian.Big, () => {
						VertexData = s.SerializeObject<PointDataBuffer>(VertexData, onPreSerialize: b => b.Geo = this, name: nameof(VertexData));
					});
				} else {
					VertexData = s.SerializeObject<PointDataBuffer>(VertexData, onPreSerialize: b => b.Geo = this, name: nameof(VertexData));
				}
			}
		}

		public class PointDataBuffer : BinarySerializable {
			public GEO_GaoVisu_PC Geo { get; set; }
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
					s.SystemLogger?.LogWarning($"VISU Vertex Buffer at {Offset} wasn't serialized properly.");
					s.Goto(Offset + TotalSize);
				}
			}

			public class Point : BinarySerializable {
				public PointDataBuffer Buffer { get; set; }

				public BGRA8888Color Color { get; set; }
				public float[] UnknownFloats { get; set; }
				public byte[] PointBytes { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					Color = s.SerializeObject<BGRA8888Color>(Color, name: nameof(Color));
					UnknownFloats = s.SerializeArray<float>(UnknownFloats, 2, name: nameof(UnknownFloats));
					PointBytes = s.SerializeArray<byte>(PointBytes, Buffer.PointDataSize - (s.CurrentAbsoluteOffset - Offset.AbsoluteOffset), name: nameof(PointBytes));

					if (s.CurrentAbsoluteOffset != Offset.AbsoluteOffset + Buffer.PointDataSize) {
						s.SystemLogger?.LogWarning($"VISU Vertex Data at {Offset} wasn't serialized properly.");
						s.Goto(Offset + Buffer.PointDataSize);
					}
				}
			}

		}

		public void Unoptimize(OBJ_GameObject_Visual visu, GEO_GeometricObject geo, GEO_GeoObject_PC geoPC) {
			if (VertexDataBufferSize == 0) return;
			if (Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_WW_20040920) && Context.GetR1Settings().Platform == Platform.PS3) {
				throw new System.NotImplementedException("Read points data from geometry BF");
			}
			bool restoreOriginalVertexIndices = geoPC.VertexDataBufferType == 3; // Due to original indices being stored in Ponderations, we can determine the original vertex index ^^

			if (restoreOriginalVertexIndices) {
				uint verticesCount = (uint)geoPC.VertexData.Points.Max(p => p.Ponderations[0].Index) + 1;
				visu.VertexColors = new Jade_Color[verticesCount];

				for (int i = 0; i < geoPC.VertexData.Points.Length; i++) {
					var point = geoPC.VertexData.Points[i];
					var visuPoint = VertexData.Points[i];
					visu.VertexColors[point.OriginalVertexIndex] = GEO_GeoObject_PC.ProcessColor(visuPoint.Color);
				}
			} else {
				visu.VertexColors = VertexData.Points.Select(v => GEO_GeoObject_PC.ProcessColor(v.Color)).ToArray();
			}
			visu.VertexColorsCount = (uint)visu.VertexColors.Length;
		}
	}
}
