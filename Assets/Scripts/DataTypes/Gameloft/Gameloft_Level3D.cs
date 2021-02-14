using System;
using System.Linq;

namespace R1Engine
{
	public class Gameloft_Level3D : Gameloft_Resource {
		public ushort TrackLength { get; set; }
		public TrackBlock[] TrackBlocks { get; set; }
		public byte Structs12Count { get; set; }
		public byte BM_Byte0 { get; set; }
		public bool BM_Bool1 { get; set; }
		public byte BM_Byte2 { get; set; }
		public byte BM_Byte3 { get; set; }
		public TrackMapping[] MapSpriteMapping { get; set; } // Struct0Count - 1
		public byte RoadTextureID_0 { get; set; } // Resource ID in the RoadTexturesID
		public byte RoadTextureID_1 { get; set; }
		public byte aW { get; set; }
		public byte aUCount { get; set; }
		public byte bB { get; set; }
		public RGB888Color Color_bC { get; set; }
		public RGB888Color Color_bD_Road1 { get; set; }
		public RGB888Color Color_bA { get; set; }
		public RGB888Color Color_bE_Road2 { get; set; }
		public RGB888Color Color_bF { get; set; }
		public short bG { get; set; }
		public short aU_2 { get; set; }
		public byte N { get; set; }
		public byte gF { get; set; }
		public bool bp { get; set; }
		public short bJ { get; set; }
		public short bK { get; set; }
		public RGB888Color Color_afterbK_0 { get; set; }
		public RGB888Color Color_afterbK_1 { get; set; }
		public byte bL { get; set; }
		public RGB888Color Color_bH_TunnelDark { get; set; }
		public RGB888Color Color_bI_TunnelLight { get; set; }
		public byte byte_afterBI { get; set; }

		public RGB888Color Color_dj_BridgeDark { get; set; }
		public RGB888Color Color_dk_BridgeLight { get; set; }
		public byte dl { get; set; }
		public byte dm { get; set; }

		// Lowres only?
		public RGB888Color Color_dq { get; set; }
		public RGB888Color Color_dr { get; set; }
		public RGB888Color Color_ds { get; set; }
		public RGB888Color Color_dt { get; set; }
		public RGB888Color Color_du { get; set; }
		public short dx { get; set; }
		public short dw { get; set; }
		public short dv { get; set; }

		public ushort Structs1Count { get; set; }
		public Struct1[] Structs1 { get; set; }
		public byte Structs2Count { get; set; }
		public Struct2[] Structs2 { get; set; }
		public BackgroundLayer[] BackgroundLayers { get; set; }
		public byte[][] bs_Struct0_BackgroundIndex { get; set; } // Each byte is an index to a Struct3/aU
		public byte Structs4Count { get; set; }
		public Struct4[] Structs4 { get; set; }
		public byte Structs5Count { get; set; }
		public Struct5[] Structs5 { get; set; }
		public ushort Structs6Count { get; set; }
		public Struct6[] Structs6 { get; set; }
		public ushort Structs7Count { get; set; }
		public Struct7[] Structs7 { get; set; }
		public ushort TrackObjectsCount { get; set; }
		public TrackObject[] TrackObjects { get; set; }
		public ushort cF { get; set; }
		public TrackObjectInstance[] TrackObjectInstances { get; set; }
		public TrackObjectCollection[] TrackObjectCollections { get; set; }
		public byte Structs11Count { get; set; }
		public Type[] Types { get; set; }
		public Struct12[] Structs12 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			TrackLength = s.Serialize<ushort>(TrackLength, name: nameof(TrackLength));
			TrackBlocks = s.SerializeObjectArray<TrackBlock>(TrackBlocks, TrackLength, name: nameof(TrackBlocks));
			Structs12Count = s.Serialize<byte>(Structs12Count, name: nameof(Structs12Count));
			BM_Byte0 = s.Serialize<byte>(BM_Byte0, name: nameof(BM_Byte0));
			BM_Bool1 = s.Serialize<bool>(BM_Bool1, name: nameof(BM_Bool1));
			BM_Byte2 = s.Serialize<byte>(BM_Byte2, name: nameof(BM_Byte2));
			BM_Byte3 = s.Serialize<byte>(BM_Byte3, name: nameof(BM_Byte3));
			MapSpriteMapping = s.SerializeObjectArray<TrackMapping>(MapSpriteMapping, TrackLength - 1, name: nameof(MapSpriteMapping));
			if (s.GameSettings.GameModeSelection == GameModeSelection.RaymanKartMobile_320x240) {
				RoadTextureID_0 = s.Serialize<byte>(RoadTextureID_0, name: nameof(RoadTextureID_0));
				RoadTextureID_1 = s.Serialize<byte>(RoadTextureID_1, name: nameof(RoadTextureID_1));
			}
			aW = s.Serialize<byte>(aW, name: nameof(aW));
			aUCount = s.Serialize<byte>(aUCount, name: nameof(aUCount));
			bB = s.Serialize<byte>(bB, name: nameof(bB));
			Color_bC = s.SerializeObject<RGB888Color>(Color_bC, name: nameof(Color_bC));
			Color_bD_Road1 = s.SerializeObject<RGB888Color>(Color_bD_Road1, name: nameof(Color_bD_Road1));
			Color_bA = s.SerializeObject<RGB888Color>(Color_bA, name: nameof(Color_bA));
			Color_bE_Road2 = s.SerializeObject<RGB888Color>(Color_bE_Road2, name: nameof(Color_bE_Road2));
			Color_bF = s.SerializeObject<RGB888Color>(Color_bF, name: nameof(Color_bF));
			bG = s.Serialize<short>(bG, name: nameof(bG));
			aU_2 = s.Serialize<short>(aU_2, name: nameof(aU_2));
			N = s.Serialize<byte>(N, name: nameof(N));
			gF = s.Serialize<byte>(gF, name: nameof(gF));
			bp = s.Serialize<bool>(bp, name: nameof(bp));
			bJ = s.Serialize<short>(bJ, name: nameof(bJ));
			bK = s.Serialize<short>(bK, name: nameof(bK));
			Color_afterbK_0 = s.SerializeObject<RGB888Color>(Color_afterbK_0, name: nameof(Color_afterbK_0));
			Color_afterbK_1 = s.SerializeObject<RGB888Color>(Color_afterbK_1, name: nameof(Color_afterbK_1));
			bL = s.Serialize<byte>(bL, name: nameof(bL));
			Color_bH_TunnelDark = s.SerializeObject<RGB888Color>(Color_bH_TunnelDark, name: nameof(Color_bH_TunnelDark));
			Color_bI_TunnelLight = s.SerializeObject<RGB888Color>(Color_bI_TunnelLight, name: nameof(Color_bI_TunnelLight));
			byte_afterBI = s.Serialize<byte>(byte_afterBI, name: nameof(byte_afterBI));
			if (BitHelpers.ExtractBits(aW, 1, 2) == 1) {
				Color_dj_BridgeDark = s.SerializeObject<RGB888Color>(Color_dj_BridgeDark, name: nameof(Color_dj_BridgeDark));
				Color_dk_BridgeLight = s.SerializeObject<RGB888Color>(Color_dk_BridgeLight, name: nameof(Color_dk_BridgeLight));
				dl = s.Serialize<byte>(dl, name: nameof(dl));
				dm = s.Serialize<byte>(dm, name: nameof(dm));
			}
			if (s.GameSettings.GameModeSelection != GameModeSelection.RaymanKartMobile_320x240) {
				Color_dq = s.SerializeObject<RGB888Color>(Color_dq, name: nameof(Color_dq));
				Color_dr = s.SerializeObject<RGB888Color>(Color_dr, name: nameof(Color_dr));
				Color_ds = s.SerializeObject<RGB888Color>(Color_ds, name: nameof(Color_ds));
				Color_dt = s.SerializeObject<RGB888Color>(Color_dt, name: nameof(Color_dt));
				Color_du = s.SerializeObject<RGB888Color>(Color_du, name: nameof(Color_du));
				dx = s.Serialize<short>(dx, name: nameof(dx));
				dw = s.Serialize<short>(dw, name: nameof(dw));
				dv = s.Serialize<short>(dv, name: nameof(dv));
			}
			Structs1Count = s.Serialize<ushort>(Structs1Count, name: nameof(Structs1Count));
			Structs1 = s.SerializeObjectArray<Struct1>(Structs1, Structs1Count, name: nameof(Structs1));
			Structs2Count = s.Serialize<byte>(Structs2Count, name: nameof(Structs2Count));
			Structs2 = s.SerializeObjectArray<Struct2>(Structs2, Structs2Count, name: nameof(Structs2));
			BackgroundLayers = s.SerializeObjectArray<BackgroundLayer>(BackgroundLayers, aUCount, name: nameof(BackgroundLayers));
			bs_Struct0_BackgroundIndex = s.SerializeArraySize<byte[],byte>(bs_Struct0_BackgroundIndex, name: nameof(bs_Struct0_BackgroundIndex));
			for (int i = 0; i < bs_Struct0_BackgroundIndex.Length; i++) {
				bs_Struct0_BackgroundIndex[i] = s.SerializeArray<byte>(bs_Struct0_BackgroundIndex[i], TrackLength, name: $"{nameof(bs_Struct0_BackgroundIndex)}[{i}]");
			}
			Structs4Count = s.Serialize<byte>(Structs4Count, name: nameof(Structs4Count));
			Structs4 = s.SerializeObjectArray<Struct4>(Structs4, Structs4Count, name: nameof(Structs4));
			Structs5Count = s.Serialize<byte>(Structs5Count, name: nameof(Structs5Count));
			Structs5 = s.SerializeObjectArray<Struct5>(Structs5, Structs5Count, name: nameof(Structs5));
			Structs6Count = s.Serialize<ushort>(Structs6Count, name: nameof(Structs6Count));
			Structs6 = s.SerializeObjectArray<Struct6>(Structs6, Structs6Count, name: nameof(Structs6));
			Structs7Count = s.Serialize<ushort>(Structs7Count, name: nameof(Structs7Count));
			Structs7 = s.SerializeObjectArray<Struct7>(Structs7, Structs7Count, name: nameof(Structs7));
			TrackObjectsCount = s.Serialize<ushort>(TrackObjectsCount, name: nameof(TrackObjectsCount));
			TrackObjects = s.SerializeObjectArray<TrackObject>(TrackObjects, TrackObjectsCount, name: nameof(TrackObjects));
			cF = s.Serialize<ushort>(cF, name: nameof(cF));
			TrackObjectInstances = s.SerializeObjectArray<TrackObjectInstance>(TrackObjectInstances, cF, name: nameof(TrackObjectInstances));
			if(cF > 0) TrackObjectCollections = s.SerializeObjectArray<TrackObjectCollection>(TrackObjectCollections, TrackLength, name: nameof(TrackObjectCollections));
			Structs11Count = s.Serialize<byte>(Structs11Count, name: nameof(Structs11Count));
			Types = s.SerializeObjectArray<Type>(Types, Structs11Count, name: nameof(Types));
			Structs12 = s.SerializeObjectArray<Struct12>(Structs12, Structs12Count, name: nameof(Structs12));
		}

		public class TrackBlock : R1Serializable {
			public sbyte DeltaRotation { get; set; }
			public sbyte DeltaHeight { get; set; }
			public byte Type { get; set; }
			public TurnFlags Flags { get; set; }
			public short Unknown { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				DeltaRotation = s.Serialize<sbyte>(DeltaRotation, name: nameof(DeltaRotation));
				DeltaHeight = s.Serialize<sbyte>(DeltaHeight, name: nameof(DeltaHeight));
				Type = s.Serialize<byte>(Type, name: nameof(Type));
				Flags = s.Serialize<TurnFlags>(Flags, name: nameof(Flags));
				Unknown = s.Serialize<short>(Unknown, name: nameof(Unknown));
			}

			[Flags]
			public enum TurnFlags : byte {
				None,
				Flag0 = 1 << 0,
				Effect = 1 << 1, // Snow in Frozen Highway, Rain in Murky Swamp, Bubbles in Shipwreck Track
				DrawRocksOnRight = 1 << 2,
				DrawRocksOnLeft = 1 << 3,
				Flag4 = 1 << 4,
				TurnRight = 1 << 5,
				TurnLeft = 1 << 6,
				Flag7 = 1 << 7
			}
		}

		public class TrackMapping : R1Serializable {
			public byte YDelta { get; set; }
			public byte XDelta { get; set; }
			public bool YDeltaSign { get; set; }
			public bool XDeltaSign { get; set; }

			public UnityEngine.Vector2Int Vector2 {
				get {
					int y = YDelta;
					int x = XDelta;
					if (XDeltaSign) x = -x;
					if (YDeltaSign) y = -y;
					return new UnityEngine.Vector2Int(x,y);
				}
			}

			public override void SerializeImpl(SerializerObject s) {
				s.SerializeBitValues<byte>(bitFunc => {
					YDelta = (byte)bitFunc(YDelta, 3, name: nameof(YDelta));
					XDelta = (byte)bitFunc(XDelta, 3, name: nameof(XDelta));
					YDeltaSign = bitFunc(YDeltaSign ? 1 : 0, 1, name: nameof(YDeltaSign)) == 1;
					XDeltaSign = bitFunc(XDeltaSign ? 1 : 0, 1, name: nameof(XDeltaSign)) == 1;
				});
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
			public byte Min { get; set; }
			public byte Max { get; set; } // If between min & max, or if max < min && smaller than max or larger than min, it uses Struct4 with that index
			public byte Byte3 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Struct4Index = s.Serialize<byte>(Struct4Index, name: nameof(Struct4Index));
				Min = s.Serialize<byte>(Min, name: nameof(Min));
				Max = s.Serialize<byte>(Max, name: nameof(Max));
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

		public class Struct7 : R1Serializable {
			public byte Count { get; set; }
			public ushort UShort1 { get; set; }
			public ushort UShort2 { get; set; }
			public ushort UShort3 { get; set; }
			public Entry[] Entries { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Count = s.Serialize<byte>(Count, name: nameof(Count));
				s.SerializeBitValues<ushort>(bitFunc => {
					UShort1 = (ushort)bitFunc(UShort1, 7, name: nameof(UShort1));
					UShort2 = (ushort)bitFunc(UShort2, 7, name: nameof(UShort2));
					UShort3 = (ushort)bitFunc(UShort3, 2, name: nameof(UShort3));
				});
				Entries = s.SerializeObjectArray<Entry>(Entries, Count, name: nameof(Entries));
			}
			public class Entry : R1Serializable {
				public byte Type { get; set; }
				public short[] Shorts { get; set; }
				public byte[] Bytes { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					Type = s.Serialize<byte>(Type, name: nameof(Type));
					if (Type == 0) {
						Bytes = s.SerializeArray<byte>(Bytes, GetLength()-1, name: nameof(Bytes));
					} else {
						Shorts = s.SerializeArray<short>(Shorts, GetLength()-1, name: nameof(Shorts));
					}
				}

				public int GetLength() {
					switch (Type) {
						case 0: return 4;
						case 1: return 5;
						case 2: return 7;
						case 3: return 5;
						case 5: return 7;
						default: return 0;
					}
				}
			}
		}


		public class TrackObject : R1Serializable {
			public byte ObjectType { get; set; }
			public short XPosition { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				ObjectType = s.Serialize<byte>(ObjectType, name: nameof(ObjectType));
				XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
			}
		}
		public class TrackObjectInstance : R1Serializable {
			public short TrackObjectIndex { get; set; }
			public int Int0 { get; set; }
			public int Int1 { get; set; }
			public int Int2 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				s.SerializeBitValues<short>(bitFunc => {
					TrackObjectIndex = (short)bitFunc(TrackObjectIndex, 11, name: nameof(TrackObjectIndex));
					Int0 = bitFunc(Int0, 5, name: nameof(Int0));
					//Int1 = bitFunc(Int1, 3, name: nameof(Int1));
					//Int2 = bitFunc(Int2, 1, name: nameof(Int2));
				});
			}
		}
		public class TrackObjectCollection : R1Serializable {
			public short InstanceIndex { get; set; }
			public int Count { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				s.SerializeBitValues<short>(bitFunc => {
					InstanceIndex = (short)bitFunc(InstanceIndex, 11, name: nameof(InstanceIndex));
					Count = bitFunc(Count, 5, name: nameof(Count));
				});
			}
		}
		public class Type : R1Serializable {
			public short Short0 { get; set; }

			// Lowres only
			public byte cg0 { get; set; }
			public byte cg1 { get; set; }
			public Entry[] Entries { get; set; }

			public short Short2 { get; set; }
			public byte Byte4 { get; set; }
			public RGB888Color ColorGround { get; set; }
			public RGB888Color Color8 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Short0 = s.Serialize<short>(Short0, name: nameof(Short0));
				if (s.GameSettings.GameModeSelection != GameModeSelection.RaymanKartMobile_320x240) {
					cg0 = s.Serialize<byte>(cg0, name: nameof(cg0));
					cg1 = s.Serialize<byte>(cg1, name: nameof(cg1));
					Entries = s.SerializeObjectArray<Entry>(Entries, 3, name: nameof(Entries));
				}
				Short2 = s.Serialize<short>(Short2, name: nameof(Short2));
				Byte4 = s.Serialize<byte>(Byte4, name: nameof(Byte4));
				ColorGround = s.SerializeObject<RGB888Color>(ColorGround, name: nameof(ColorGround));
				Color8 = s.SerializeObject<RGB888Color>(Color8, name: nameof(Color8));
			}

			public class Entry : R1Serializable {
				public byte ci { get; set; }
				public short cj { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					ci = s.Serialize<byte>(ci, name: nameof(ci));
					cj = s.Serialize<short>(cj, name: nameof(cj));
				}
			}
		}


		public class Struct12 : R1Serializable {
			public byte Struct0Index { get; set; }
			public short Unknown { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Struct0Index = s.Serialize<byte>(Struct0Index, name: nameof(Struct0Index));
				Unknown = s.Serialize<short>(Unknown, name: nameof(Unknown));
			}
		}
	}
}