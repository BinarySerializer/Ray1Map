using System;
using System.Linq;

namespace R1Engine
{
	public class Gameloft_Level3D : Gameloft_Resource {
		public ushort VerticesCount { get; set; }
		public Vertex[] Vertices { get; set; }
		public byte ObjectsCount { get; set; }
		public byte BM_Byte0 { get; set; }
		public bool BM_Bool1 { get; set; }
		public byte BM_Byte2 { get; set; }
		public byte BM_Byte3 { get; set; }
		public byte[] BM_VertexBytes { get; set; } // verticesCount - 1. Related to edges?
		public byte RoadTextureID_Night { get; set; } // Resource ID in the RoadTexturesID
		public byte RoadTextureID_Day { get; set; }
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

		public ushort Structs1Count { get; set; }
		public Struct1[] Structs1 { get; set; }
		public byte Structs2Count { get; set; }
		public Struct2[] Structs2 { get; set; }
		public BackgroundLayer[] BackgroundLayers { get; set; }
		public byte[][] bs_Vertex_BackgroundIndex { get; set; } // Each byte is an index to a Struct3/aU
		public byte Structs4Count { get; set; }
		public Struct4[] Structs4 { get; set; }
		public byte Structs5Count { get; set; }
		public Struct5[] Structs5 { get; set; }
		public ushort Structs6Count { get; set; }
		public Struct6[] Structs6 { get; set; }

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
			RoadTextureID_Night = s.Serialize<byte>(RoadTextureID_Night, name: nameof(RoadTextureID_Night));
			RoadTextureID_Day = s.Serialize<byte>(RoadTextureID_Day, name: nameof(RoadTextureID_Day));
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
			Structs1Count = s.Serialize<ushort>(Structs1Count, name: nameof(Structs1Count));
			Structs1 = s.SerializeObjectArray<Struct1>(Structs1, Structs1Count, name: nameof(Structs1));
			Structs2Count = s.Serialize<byte>(Structs2Count, name: nameof(Structs2Count));
			Structs2 = s.SerializeObjectArray<Struct2>(Structs2, Structs2Count, name: nameof(Structs2));
			BackgroundLayers = s.SerializeObjectArray<BackgroundLayer>(BackgroundLayers, aUCount, name: nameof(BackgroundLayers));
			bs_Vertex_BackgroundIndex = s.SerializeArraySize<byte[],byte>(bs_Vertex_BackgroundIndex, name: nameof(bs_Vertex_BackgroundIndex));
			for (int i = 0; i < bs_Vertex_BackgroundIndex.Length; i++) {
				bs_Vertex_BackgroundIndex[i] = s.SerializeArray<byte>(bs_Vertex_BackgroundIndex[i], VerticesCount, name: $"{nameof(bs_Vertex_BackgroundIndex)}[{i}]");
			}
			Structs4Count = s.Serialize<byte>(Structs4Count, name: nameof(Structs4Count));
			Structs4 = s.SerializeObjectArray<Struct4>(Structs4, Structs4Count, name: nameof(Structs4));
			Structs5Count = s.Serialize<byte>(Structs5Count, name: nameof(Structs5Count));
			Structs5 = s.SerializeObjectArray<Struct5>(Structs5, Structs5Count, name: nameof(Structs5));
			Structs6Count = s.Serialize<ushort>(Structs6Count, name: nameof(Structs6Count));
			Structs6 = s.SerializeObjectArray<Struct6>(Structs6, Structs6Count, name: nameof(Structs6));
			/* TODO:
			bl();
			bo();
			bp();*/
			/*
			
  private static void bn() throws Exception {
    int i;
    if ((i = b.readShort()) > 0) {
      cy = new int[i];
      cz = new int[i];
      cA = new short[i];
      cB = new byte[i];
      cx = new short[i][];
      cC = new short[i];
      cD = new byte[i];
      for (byte b = 0; b < i; b++) {
        byte b1 = (byte)b.readUByte();
        byte b2 = (byte)b.readUByte();
        short s = (short)(byte)b.readUByte();
        if (b1 != -1 || b2 != -1) {
          cx[b] = new short[3];
          cx[b][0] = (short)b1;
          cx[b][1] = (short)b2;
          cx[b][2] = (s >= 0) ? s : 0;
        } 
        cy[b] = b.readShort();
        cz[b] = b.readShort();
        cA[b] = (short)b.readShort();
        cB[b] = (byte)b.readUByte();
        cC[b] = (short)b.readShort();
        cD[b] = (byte)b.readUByte();
      } 
    } 
    ao = new Image[20 + i][][];
    an = new byte[20 + i][][];
  }
  
  private static void bo() throws Exception {
    cE = (short)b.readShort();
    if (cE > 0) {
      cG = new byte[cE];
      cH = new short[cE];
      for (byte b = 0; b < cE; b++) {
        cG[b] = (byte)b.readUByte();
        cH[b] = (short)(1 * b.readShort());
      } 
    } 
    cF = (short)b.readShort();
    if (cF > 0) {
      cI = new short[cF];
      cK = new int[cF];
      byte b;
      for (b = 0; b < cF; b++) {
        cI[b] = (short)b.readShort();
        if (S[5] == 2 && (cI[b] & 0x7000) == 24576)
          cK[b] = 12288; 
        if (S[5] == 3 && (cI[b] & 0x7000) == 24576)
          cK[b] = 12288; 
      } 
      cJ = new short[VerticesCount];
      for (b = 0; b < VerticesCount; b++)
        cJ[b] = (short)b.readShort(); 
    } 
  }
  
  private static void bp() throws Exception {
    cf = (byte)b.readUByte();
    cg = new short[cf];
    ch = new int[cf];
    ci = new int[cf];
    cj = new byte[cf];
    ck = new short[cf];
    for (byte b = 0; b < cf; b++) {
      cg[b] = (short)b.readShort();
      ck[b] = (short)b.readShort();
      cj[b] = (byte)b.readUByte();
      ch[b] = 0xFF000000 | b.readUByte() << 16 | (b.readUByte() & 0xFF) << 8 | b.readUByte() & 0xFF;
      ci[b] = 0xFF000000 | b.readUByte() << 16 | (b.readUByte() & 0xFF) << 8 | b.readUByte() & 0xFF;
    } 
  }
			*/

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

		public class Struct1 : R1Serializable {
			public short XPosition { get; set; }
			public short YPosition { get; set; }
			public byte Byte2 { get; set; }
			public byte Count { get; set; }
			public short[] Unknown { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
				YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
				Byte2 = s.Serialize<byte>(Byte2, name: nameof(Byte2));
				Count = s.Serialize<byte>(Count, name: nameof(Count));
				Unknown = s.SerializeArray<short>(Unknown, Count, name: nameof(Unknown));
			}
		}

		public class Struct2 : R1Serializable {
			public byte Byte0 { get; set; }
			public byte Byte1 { get; set; }
			public short Short2 { get; set; }
			public short Short4 { get; set; }
			public short Short6 { get; set; }
			public short Short8 { get; set; }
			public short ShortA { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Byte0 = s.Serialize<byte>(Byte0, name: nameof(Byte0));
				Byte1 = s.Serialize<byte>(Byte1, name: nameof(Byte1));
				Short2 = s.Serialize<short>(Short2, name: nameof(Short2));
				Short4 = s.Serialize<short>(Short4, name: nameof(Short4));
				Short6 = s.Serialize<short>(Short6, name: nameof(Short6));
				Short8 = s.Serialize<short>(Short8, name: nameof(Short8));
				ShortA = s.Serialize<short>(ShortA, name: nameof(ShortA));
			}
		}

		public class BackgroundLayer : R1Serializable {
			public byte ImageResourceIndex { get; set; }
			public short Short1 { get; set; }
			public byte Byte3 { get; set; }
			public short Short4 { get; set; }
			public short Short6 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				ImageResourceIndex = s.Serialize<byte>(ImageResourceIndex, name: nameof(ImageResourceIndex));
				Short1 = s.Serialize<short>(Short1, name: nameof(Short1));
				Byte3 = s.Serialize<byte>(Byte3, name: nameof(Byte3));
				Short4 = s.Serialize<short>(Short4, name: nameof(Short4));
				Short6 = s.Serialize<short>(Short6, name: nameof(Short6));
			}
		}

		public class Struct4 : R1Serializable {
			public byte Count { get; set; }
			public KeyValue[] KeyValues { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Count = s.Serialize<byte>(Count, name: nameof(Count));
				KeyValues = s.SerializeObjectArray<KeyValue>(KeyValues, Count, name: nameof(KeyValues));
			}
			public class KeyValue : R1Serializable {
				public short Short { get; set; }
				public RGB888Color Color { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					Short = s.Serialize<short>(Short, name: nameof(Short));
					Color = s.SerializeObject<RGB888Color>(Color, name: nameof(Color));
				}
			}
		}

		public class Struct5 : R1Serializable {
			public byte Struct4Index { get; set; }
			public byte Byte1 { get; set; }
			public byte Byte2 { get; set; }
			public byte Byte3 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Struct4Index = s.Serialize<byte>(Struct4Index, name: nameof(Struct4Index));
				Byte1 = s.Serialize<byte>(Byte1, name: nameof(Byte1));
				Byte2 = s.Serialize<byte>(Byte2, name: nameof(Byte2));
				Byte3 = s.Serialize<byte>(Byte3, name: nameof(Byte3));
			}
		}

		public class Struct6 : R1Serializable {
			public byte Byte0 { get; set; }
			public byte Byte1 { get; set; }
			public byte Byte2 { get; set; }
			public short Short3 { get; set; }
			public short Short5 { get; set; }
			public short Short7 { get; set; }
			public byte Byte9 { get; set; }
			public short Short10 { get; set; }
			public byte Byte12 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Byte0 = s.Serialize<byte>(Byte0, name: nameof(Byte0));
				Byte1 = s.Serialize<byte>(Byte1, name: nameof(Byte1));
				Byte2 = s.Serialize<byte>(Byte2, name: nameof(Byte2));
				Short3 = s.Serialize<short>(Short3, name: nameof(Short3));
				Short5 = s.Serialize<short>(Short5, name: nameof(Short5));
				Short7 = s.Serialize<short>(Short7, name: nameof(Short7));
				Byte9 = s.Serialize<byte>(Byte9, name: nameof(Byte9));
				Short10 = s.Serialize<short>(Short10, name: nameof(Short10));
				Byte12 = s.Serialize<byte>(Byte12, name: nameof(Byte12));
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