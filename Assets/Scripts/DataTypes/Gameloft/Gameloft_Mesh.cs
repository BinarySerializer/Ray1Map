using System;
using System.Linq;

namespace R1Engine
{
	public class Gameloft_Mesh : Gameloft_Resource {
		public ushort VerticesCount { get; set; }
		public Vertex[] Vertices { get; set; }
		public byte ObjectsCount { get; set; }
		public byte BM_Byte0 { get; set; }
		public bool BM_Bool1 { get; set; }
		public byte BM_Byte2 { get; set; }
		public byte BM_Byte3 { get; set; }
		public byte[] BM_VertexBytes { get; set; } // verticesCount - 1. Related to edges?
		public byte cd { get; set; }
		public byte ce { get; set; }
		public byte aW { get; set; }
		public byte aUCount { get; set; }
		public byte bB { get; set; }
		public RGB888Color Color_bC { get; set; }
		public RGB888Color Color_bD { get; set; }
		public RGB888Color Color_bA { get; set; }
		public RGB888Color Color_bE { get; set; }
		public RGB888Color Color_bF { get; set; }
		public short bG { get; set; }
		public short aU_2 { get; set; }
		public byte N { get; set; }
		public byte gF { get; set; }
		public bool bp { get; set; }
		public short bJ { get; set; }
		public short bK { get; set; }
		public byte[] bytes_afterBK { get; set; }
		public byte bL { get; set; }
		public RGB888Color Color_bH { get; set; }
		public RGB888Color Color_bI { get; set; }
		public byte byte_afterBI { get; set; }

		public RGB888Color Color_dj { get; set; }
		public RGB888Color Color_dk { get; set; }
		public byte dl { get; set; }
		public byte dm { get; set; }

		public LastStruct[] Objects { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			VerticesCount = s.Serialize<ushort>(VerticesCount, name: nameof(VerticesCount));
			Vertices = s.SerializeObjectArray<Vertex>(Vertices, VerticesCount, name: nameof(Vertices));
			ObjectsCount = s.Serialize<byte>(ObjectsCount, name: nameof(ObjectsCount));
			BM_Byte0 = s.Serialize<byte>(BM_Byte0, name: nameof(BM_Byte0));
			BM_Bool1 = s.Serialize<bool>(BM_Bool1, name: nameof(BM_Bool1));
			BM_Byte2 = s.Serialize<byte>(BM_Byte2, name: nameof(BM_Byte2));
			BM_Byte3 = s.Serialize<byte>(BM_Byte3, name: nameof(BM_Byte3));
			BM_VertexBytes = s.SerializeArray<byte>(BM_VertexBytes, VerticesCount - 1, name: nameof(BM_VertexBytes));
			cd = s.Serialize<byte>(cd, name: nameof(cd));
			ce = s.Serialize<byte>(ce, name: nameof(ce));
			aW = s.Serialize<byte>(aW, name: nameof(aW));
			aUCount = s.Serialize<byte>(aUCount, name: nameof(aUCount));
			bB = s.Serialize<byte>(bB, name: nameof(bB));
			Color_bC = s.SerializeObject<RGB888Color>(Color_bC, name: nameof(Color_bC));
			Color_bD = s.SerializeObject<RGB888Color>(Color_bD, name: nameof(Color_bD));
			Color_bA = s.SerializeObject<RGB888Color>(Color_bA, name: nameof(Color_bA));
			Color_bE = s.SerializeObject<RGB888Color>(Color_bE, name: nameof(Color_bE));
			Color_bF = s.SerializeObject<RGB888Color>(Color_bF, name: nameof(Color_bF));
			bG = s.Serialize<short>(bG, name: nameof(bG));
			aU_2 = s.Serialize<short>(aU_2, name: nameof(aU_2));
			N = s.Serialize<byte>(N, name: nameof(N));
			gF = s.Serialize<byte>(gF, name: nameof(gF));
			bp = s.Serialize<bool>(bp, name: nameof(bp));
			bJ = s.Serialize<short>(bJ, name: nameof(bJ));
			bK = s.Serialize<short>(bK, name: nameof(bK));
			bytes_afterBK = s.SerializeArray<byte>(bytes_afterBK, 6, name: nameof(bytes_afterBK));
			bL = s.Serialize<byte>(bL, name: nameof(bL));
			Color_bH = s.SerializeObject<RGB888Color>(Color_bH, name: nameof(Color_bH));
			Color_bI = s.SerializeObject<RGB888Color>(Color_bI, name: nameof(Color_bI));
			byte_afterBI = s.Serialize<byte>(byte_afterBI, name: nameof(byte_afterBI));
			if (BitHelpers.ExtractBits(aW, 1, 2) == 1) {
				Color_dj = s.SerializeObject<RGB888Color>(Color_dj, name: nameof(Color_dj));
				Color_dk = s.SerializeObject<RGB888Color>(Color_dk, name: nameof(Color_dk));
				dl = s.Serialize<byte>(dl, name: nameof(dl));
				dm = s.Serialize<byte>(dm, name: nameof(dm));
			}
			/* TODO:
			bk();
			bg();
			ab(i);
			bh();
			bi();
			bj();
			bn();
			bl();
			bo();
			bp();*/

			//Objects = s.SerializeObjectArray<LastStruct>(Objects, ObjectsCount, name: nameof(Objects));
		}

		public class Vertex : R1Serializable {
			public sbyte XPosition { get; set; }
			public sbyte YPosition { get; set; }
			public byte Flags1 { get; set; }
			public byte Flags2 { get; set; }
			public short Unknown { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				XPosition = s.Serialize<sbyte>(XPosition, name: nameof(XPosition));
				YPosition = s.Serialize<sbyte>(YPosition, name: nameof(YPosition));
				Flags1 = s.Serialize<byte>(Flags1, name: nameof(Flags1));
				Flags2 = s.Serialize<byte>(Flags2, name: nameof(Flags2));
				Unknown = s.Serialize<short>(Unknown, name: nameof(Unknown));
			}
		}



		public class LastStruct : R1Serializable {
			public byte VertexIndex { get; set; }
			public short Unknown { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				VertexIndex = s.Serialize<byte>(VertexIndex, name: nameof(VertexIndex));
				Unknown = s.Serialize<short>(Unknown, name: nameof(Unknown));
			}
		}
	}
}