using BinarySerializer;
using System;

namespace Ray1Map.Gameloft
{
    public class Gameloft_RK_Level : Gameloft_Resource {
		public ushort TrackLength { get; set; }
		public TrackBlock[] TrackBlocks { get; set; }
		public byte LumsCount { get; set; }
		public byte BM_Byte0 { get; set; }
		public bool BM_Bool1 { get; set; }
		public byte BM_Byte2 { get; set; }
		public byte BM_Byte3 { get; set; }
		public TrackMapping[] MapSpriteMapping { get; set; } // Struct0Count - 1
		public byte RoadTextureID_0 { get; set; } // Resource ID in the RoadTexturesID
		public byte RoadTextureID_1 { get; set; }
		public byte aW { get; set; }
		public byte BackgroundLayersCount { get; set; }
		public byte bB { get; set; }
		public SerializableColor Color_bC { get; set; }
		public SerializableColor Color_bD_Road1 { get; set; }
		public SerializableColor Color_bA { get; set; }
		public SerializableColor Color_bE_Road2 { get; set; }
		public SerializableColor Color_bF_Fog { get; set; }
		public short bG_FogAmount { get; set; }
		public short aU_2 { get; set; }
		public byte N { get; set; }
		public byte gF { get; set; }
		public bool bp { get; set; }
		public short DefaultRoadWidth { get; set; }
		public short bK { get; set; }
		public SerializableColor Color_afterbK_0 { get; set; }
		public SerializableColor Color_afterbK_1 { get; set; }
		public byte bL { get; set; }
		public byte LowResUnusedByteAfterbL { get; set; }
		public SerializableColor Color_bH_Wall0 { get; set; }
		public SerializableColor Color_bI_Wall1 { get; set; }
		public byte byte_afterBI { get; set; }

		public SerializableColor Color_dj_BridgeDark { get; set; }
		public SerializableColor Color_dk_BridgeLight { get; set; }
		public byte dl { get; set; }
		public byte dm { get; set; }

		// Lowres only?
		public SerializableColor Color_dq { get; set; }
		public SerializableColor Color_Tunnel_0 { get; set; }
		public SerializableColor Color_ds { get; set; }
		public SerializableColor Color_Tunnel_1 { get; set; }
		public SerializableColor Color_Tunnel_Front { get; set; }
		public short Tunnel_Height { get; set; }
		public short Tunnel_Height2 { get; set; }
		public short Tunnel_WallThickness { get; set; }

		public ushort TriggerObjectsCount { get; set; }
		public TriggerObject[] TriggerObjects { get; set; }
		public byte Structs2Count { get; set; }
		public Struct2[] Structs2 { get; set; }
		public BackgroundLayer[] BackgroundLayers { get; set; }
		public byte[][] TrackBackgroundIndices { get; set; } // Each byte is an index to a Struct3/aU
		public byte BackgroundGradientsCount { get; set; }
		public BackgroundGradient[] BackgroundGradients { get; set; }
		public byte TrackGradientMapsCount { get; set; }
		public TrackGradientMap[] TrackGradientMaps { get; set; }
		public ushort ObjectTypesCount { get; set; }
		public ObjectType[] ObjectTypes { get; set; }
		public ushort Objects3DCount { get; set; }
		public Object3D[] Objects3D { get; set; }
		public ushort TrackObjectsCount { get; set; }
		public TrackObject[] TrackObjects { get; set; }
		public ushort TrackObjectInstancesCount { get; set; }
		public TrackObjectInstance[] TrackObjectInstances { get; set; }
		public TrackObjectCollection[] TrackObjectCollections { get; set; }
		public byte TypesCount { get; set; }
		public Type[] Types { get; set; }
		public Lum[] Lums { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			TrackLength = s.Serialize<ushort>(TrackLength, name: nameof(TrackLength));
			TrackBlocks = s.SerializeObjectArray<TrackBlock>(TrackBlocks, TrackLength, name: nameof(TrackBlocks));
			if (s.GetR1Settings().GameModeSelection != GameModeSelection.RaymanKartMobile_128x128) {
				LumsCount = s.Serialize<byte>(LumsCount, name: nameof(LumsCount));
				if (s.GetR1Settings().GameModeSelection != GameModeSelection.RaymanKartMobile_128x128_s40v2
					&& s.GetR1Settings().GameModeSelection != GameModeSelection.RaymanKartMobile_128x160_s40v2a_N6101) {
					BM_Byte0 = s.Serialize<byte>(BM_Byte0, name: nameof(BM_Byte0));
					BM_Bool1 = s.Serialize<bool>(BM_Bool1, name: nameof(BM_Bool1));
					BM_Byte2 = s.Serialize<byte>(BM_Byte2, name: nameof(BM_Byte2));
					BM_Byte3 = s.Serialize<byte>(BM_Byte3, name: nameof(BM_Byte3));
					MapSpriteMapping = s.SerializeObjectArray<TrackMapping>(MapSpriteMapping, TrackLength - 1, name: nameof(MapSpriteMapping));
				}
			}
			if (Gameloft_RK_Manager.UseSingleRoadTexture(s.GetR1Settings())) {
				RoadTextureID_0 = s.Serialize<byte>(RoadTextureID_0, name: nameof(RoadTextureID_0));
				RoadTextureID_1 = s.Serialize<byte>(RoadTextureID_1, name: nameof(RoadTextureID_1));
			}
			aW = s.Serialize<byte>(aW, name: nameof(aW));
			BackgroundLayersCount = s.Serialize<byte>(BackgroundLayersCount, name: nameof(BackgroundLayersCount));
			bB = s.Serialize<byte>(bB, name: nameof(bB));
			Color_bC = s.SerializeInto<SerializableColor>(Color_bC, BytewiseColor.RGB888, name: nameof(Color_bC));
			Color_bD_Road1 = s.SerializeInto<SerializableColor>(Color_bD_Road1, BytewiseColor.RGB888, name: nameof(Color_bD_Road1));
			Color_bA = s.SerializeInto<SerializableColor>(Color_bA, BytewiseColor.RGB888, name: nameof(Color_bA));
			Color_bE_Road2 = s.SerializeInto<SerializableColor>(Color_bE_Road2, BytewiseColor.RGB888, name: nameof(Color_bE_Road2));
			Color_bF_Fog = s.SerializeInto<SerializableColor>(Color_bF_Fog, BytewiseColor.RGB888, name: nameof(Color_bF_Fog));
			bG_FogAmount = s.Serialize<short>(bG_FogAmount, name: nameof(bG_FogAmount));
			aU_2 = s.Serialize<short>(aU_2, name: nameof(aU_2));
			N = s.Serialize<byte>(N, name: nameof(N));
			gF = s.Serialize<byte>(gF, name: nameof(gF));
			bp = s.Serialize<bool>(bp, name: nameof(bp));
			DefaultRoadWidth = s.Serialize<short>(DefaultRoadWidth, name: nameof(DefaultRoadWidth));
			bK = s.Serialize<short>(bK, name: nameof(bK));
			Color_afterbK_0 = s.SerializeInto<SerializableColor>(Color_afterbK_0, BytewiseColor.RGB888, name: nameof(Color_afterbK_0));
			Color_afterbK_1 = s.SerializeInto<SerializableColor>(Color_afterbK_1, BytewiseColor.RGB888, name: nameof(Color_afterbK_1));
			bL = s.Serialize<byte>(bL, name: nameof(bL));
			if (s.GetR1Settings().GameModeSelection == GameModeSelection.RaymanKartMobile_128x128) {
				LowResUnusedByteAfterbL = s.Serialize<byte>(LowResUnusedByteAfterbL, name: nameof(LowResUnusedByteAfterbL));
			}
			Color_bH_Wall0 = s.SerializeInto<SerializableColor>(Color_bH_Wall0, BytewiseColor.RGB888, name: nameof(Color_bH_Wall0));
			Color_bI_Wall1 = s.SerializeInto<SerializableColor>(Color_bI_Wall1, BytewiseColor.RGB888, name: nameof(Color_bI_Wall1));
			byte_afterBI = s.Serialize<byte>(byte_afterBI, name: nameof(byte_afterBI));
			if (s.GetR1Settings().GameModeSelection != GameModeSelection.RaymanKartMobile_128x128 &&
				s.GetR1Settings().GameModeSelection != GameModeSelection.RaymanKartMobile_128x128_s40v2 &&
				s.GetR1Settings().GameModeSelection != GameModeSelection.RaymanKartMobile_128x160_s40v2a_N6101) {
				if (BitHelpers.ExtractBits(aW, 1, 2) == 1) {
					Color_dj_BridgeDark = s.SerializeInto<SerializableColor>(Color_dj_BridgeDark, BytewiseColor.RGB888, name: nameof(Color_dj_BridgeDark));
					Color_dk_BridgeLight = s.SerializeInto<SerializableColor>(Color_dk_BridgeLight, BytewiseColor.RGB888, name: nameof(Color_dk_BridgeLight));
					dl = s.Serialize<byte>(dl, name: nameof(dl));
					dm = s.Serialize<byte>(dm, name: nameof(dm));
				}
				if (s.GetR1Settings().GameModeSelection != GameModeSelection.RaymanKartMobile_320x240_Broken) {
					Color_dq = s.SerializeInto<SerializableColor>(Color_dq, BytewiseColor.RGB888, name: nameof(Color_dq));
					Color_Tunnel_0 = s.SerializeInto<SerializableColor>(Color_Tunnel_0, BytewiseColor.RGB888, name: nameof(Color_Tunnel_0));
					Color_ds = s.SerializeInto<SerializableColor>(Color_ds, BytewiseColor.RGB888, name: nameof(Color_ds));
					Color_Tunnel_1 = s.SerializeInto<SerializableColor>(Color_Tunnel_1, BytewiseColor.RGB888, name: nameof(Color_Tunnel_1));
					Color_Tunnel_Front = s.SerializeInto<SerializableColor>(Color_Tunnel_Front, BytewiseColor.RGB888, name: nameof(Color_Tunnel_Front));
					Tunnel_Height = s.Serialize<short>(Tunnel_Height, name: nameof(Tunnel_Height));
					Tunnel_Height2 = s.Serialize<short>(Tunnel_Height2, name: nameof(Tunnel_Height2));
					Tunnel_WallThickness = s.Serialize<short>(Tunnel_WallThickness, name: nameof(Tunnel_WallThickness));
				}
			}
			TriggerObjectsCount = s.Serialize<ushort>(TriggerObjectsCount, name: nameof(TriggerObjectsCount));
			TriggerObjects = s.SerializeObjectArray<TriggerObject>(TriggerObjects, TriggerObjectsCount, name: nameof(TriggerObjects));
			Structs2Count = s.Serialize<byte>(Structs2Count, name: nameof(Structs2Count));
			Structs2 = s.SerializeObjectArray<Struct2>(Structs2, Structs2Count, name: nameof(Structs2));
			BackgroundLayers = s.SerializeObjectArray<BackgroundLayer>(BackgroundLayers, BackgroundLayersCount, name: nameof(BackgroundLayers));
			TrackBackgroundIndices = s.SerializeArraySize<byte[],byte>(TrackBackgroundIndices, name: nameof(TrackBackgroundIndices));
			for (int i = 0; i < TrackBackgroundIndices.Length; i++) {
				TrackBackgroundIndices[i] = s.SerializeArray<byte>(TrackBackgroundIndices[i], TrackLength, name: $"{nameof(TrackBackgroundIndices)}[{i}]");
			}
			BackgroundGradientsCount = s.Serialize<byte>(BackgroundGradientsCount, name: nameof(BackgroundGradientsCount));
			BackgroundGradients = s.SerializeObjectArray<BackgroundGradient>(BackgroundGradients, BackgroundGradientsCount, name: nameof(BackgroundGradients));
			TrackGradientMapsCount = s.Serialize<byte>(TrackGradientMapsCount, name: nameof(TrackGradientMapsCount));
			TrackGradientMaps = s.SerializeObjectArray<TrackGradientMap>(TrackGradientMaps, TrackGradientMapsCount, name: nameof(TrackGradientMaps));
			ObjectTypesCount = s.Serialize<ushort>(ObjectTypesCount, name: nameof(ObjectTypesCount));
			ObjectTypes = s.SerializeObjectArray<ObjectType>(ObjectTypes, ObjectTypesCount, name: nameof(ObjectTypes));
			Objects3DCount = s.Serialize<ushort>(Objects3DCount, name: nameof(Objects3DCount));
			Objects3D = s.SerializeObjectArray<Object3D>(Objects3D, Objects3DCount, name: nameof(Objects3D));
			TrackObjectsCount = s.Serialize<ushort>(TrackObjectsCount, name: nameof(TrackObjectsCount));
			TrackObjects = s.SerializeObjectArray<TrackObject>(TrackObjects, TrackObjectsCount, name: nameof(TrackObjects));
			TrackObjectInstancesCount = s.Serialize<ushort>(TrackObjectInstancesCount, name: nameof(TrackObjectInstancesCount));
			TrackObjectInstances = s.SerializeObjectArray<TrackObjectInstance>(TrackObjectInstances, TrackObjectInstancesCount, name: nameof(TrackObjectInstances));
			if(TrackObjectInstancesCount > 0) TrackObjectCollections = s.SerializeObjectArray<TrackObjectCollection>(TrackObjectCollections, TrackLength, name: nameof(TrackObjectCollections));
			TypesCount = s.Serialize<byte>(TypesCount, name: nameof(TypesCount));
			Types = s.SerializeObjectArray<Type>(Types, TypesCount, name: nameof(Types));
			if (s.GetR1Settings().GameModeSelection == GameModeSelection.RaymanKartMobile_128x128) {
				LumsCount = s.Serialize<byte>(LumsCount, name: nameof(LumsCount));
			}
			Lums = s.SerializeObjectArray<Lum>(Lums, LumsCount, name: nameof(Lums));
		}

		public class TrackBlock : BinarySerializable {
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
				WallRight = 1 << 2, // Collision only
				WallLeft = 1 << 3, // Collision only
				Flag4 = 1 << 4,
				TurnRight = 1 << 5,
				TurnLeft = 1 << 6,
				Flag7 = 1 << 7
			}
		}

		public class TrackMapping : BinarySerializable {
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
				s.DoBits<byte>(b => {
					YDelta = (byte)b.SerializeBits<int>(YDelta, 3, name: nameof(YDelta));
					XDelta = (byte)b.SerializeBits<int>(XDelta, 3, name: nameof(XDelta));
					YDeltaSign = b.SerializeBits<int>(YDeltaSign ? 1 : 0, 1, name: nameof(YDeltaSign)) == 1;
					XDeltaSign = b.SerializeBits<int>(XDeltaSign ? 1 : 0, 1, name: nameof(XDeltaSign)) == 1;
				});
			}
		}

		public class TriggerObject : BinarySerializable {
			public short Width { get; set; }
			public short Height { get; set; }
			public byte Flags { get; set; } // 0 = jump, 1 = speed boost, 2 = Water, 5 = also speedup apparently?
			public byte Count { get; set; }
			public short[] Parameters { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Width = s.Serialize<short>(Width, name: nameof(Width));
				Height = s.Serialize<short>(Height, name: nameof(Height));
				Flags = s.Serialize<byte>(Flags, name: nameof(Flags));
				Count = s.Serialize<byte>(Count, name: nameof(Count));
				Parameters = s.SerializeArray<short>(Parameters, Count, name: nameof(Parameters));
			}
		}

		public class Struct2 : BinarySerializable {
			public byte Byte0 { get; set; }
			public byte Byte1 { get; set; }
			public short Short2 { get; set; }
			public short Short4 { get; set; }
			public short Short6 { get; set; }
			public short Min { get; set; }
			public short Max { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Byte0 = s.Serialize<byte>(Byte0, name: nameof(Byte0));
				Byte1 = s.Serialize<byte>(Byte1, name: nameof(Byte1));
				Short2 = s.Serialize<short>(Short2, name: nameof(Short2));
				Short4 = s.Serialize<short>(Short4, name: nameof(Short4));
				Short6 = s.Serialize<short>(Short6, name: nameof(Short6));
				Min = s.Serialize<short>(Min, name: nameof(Min));
				Max = s.Serialize<short>(Max, name: nameof(Max));
			}
		}

		public class BackgroundLayer : BinarySerializable {
			public byte ImageResourceIndex { get; set; }
			public short PixelsOffset { get; set; } // vertical position = Flags & 0x2 ? (screenBottom-height-PixelsOffset) : PixelsOffset
			public byte Flags { get; set; }
			public short Short4 { get; set; }
			public short Short6 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				ImageResourceIndex = s.Serialize<byte>(ImageResourceIndex, name: nameof(ImageResourceIndex));
				PixelsOffset = s.Serialize<short>(PixelsOffset, name: nameof(PixelsOffset));
				Flags = s.Serialize<byte>(Flags, name: nameof(Flags));
				Short4 = s.Serialize<short>(Short4, name: nameof(Short4));
				Short6 = s.Serialize<short>(Short6, name: nameof(Short6));
			}
		}

		public class BackgroundGradient : BinarySerializable {
			public byte Count { get; set; }
			public Key[] Keys { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Count = s.Serialize<byte>(Count, name: nameof(Count));
				Keys = s.SerializeObjectArray<Key>(Keys, Count, name: nameof(Keys));
			}
			public class Key : BinarySerializable {
				public short Position { get; set; } // 100 is top of screen, 80 is middle
				public SerializableColor Color { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					Position = s.Serialize<short>(Position, name: nameof(Position));
					Color = s.SerializeInto<SerializableColor>(Color, BytewiseColor.RGB888, name: nameof(Color));
				}
			}
		}

		public class TrackGradientMap : BinarySerializable {
			public byte BackgroundGradientIndex { get; set; }
			public byte Min { get; set; }
			public byte Max { get; set; } // If between min & max, or if max < min && smaller than max or larger than min, it uses Struct4 with that index
			public byte Byte3 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				BackgroundGradientIndex = s.Serialize<byte>(BackgroundGradientIndex, name: nameof(BackgroundGradientIndex));
				Min = s.Serialize<byte>(Min, name: nameof(Min));
				Max = s.Serialize<byte>(Max, name: nameof(Max));
				Byte3 = s.Serialize<byte>(Byte3, name: nameof(Byte3));
			}
		}

		public class ObjectType : BinarySerializable {
			public byte PuppetIndex { get; set; }
			public byte AnimationIndex { get; set; }
			public byte PaletteIndex { get; set; }
			public byte LowResbE0 { get; set; }
			public byte LowResbE1 { get; set; }
			public short YPosition { get; set; }
			public short Width { get; set; }
			public short Short7 { get; set; }
			public byte Byte9 { get; set; }
			public short Short10 { get; set; }
			public byte Byte12 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				PuppetIndex = s.Serialize<byte>(PuppetIndex, name: nameof(PuppetIndex));
				AnimationIndex = s.Serialize<byte>(AnimationIndex, name: nameof(AnimationIndex));
				PaletteIndex = s.Serialize<byte>(PaletteIndex, name: nameof(PaletteIndex));
				if (s.GetR1Settings().GameModeSelection == GameModeSelection.RaymanKartMobile_128x128) {
					LowResbE0 = s.Serialize<byte>(LowResbE0, name: nameof(LowResbE0));
					LowResbE1 = s.Serialize<byte>(LowResbE1, name: nameof(LowResbE1));
				}
				YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
				Width = s.Serialize<short>(Width, name: nameof(Width));
				Short7 = s.Serialize<short>(Short7, name: nameof(Short7));
				Byte9 = s.Serialize<byte>(Byte9, name: nameof(Byte9));
				Short10 = s.Serialize<short>(Short10, name: nameof(Short10));
				Byte12 = s.Serialize<byte>(Byte12, name: nameof(Byte12));
			}
		}

		public class Object3D : BinarySerializable {
			public byte Count { get; set; }
			public ushort UShort1 { get; set; }
			public ushort UShort2 { get; set; }
			public ushort UShort3 { get; set; }
			public Command[] Commands { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Count = s.Serialize<byte>(Count, name: nameof(Count));
				s.DoBits<ushort>(b => {
					UShort1 = (ushort)b.SerializeBits<int>(UShort1, 7, name: nameof(UShort1));
					UShort2 = (ushort)b.SerializeBits<int>(UShort2, 7, name: nameof(UShort2));
					UShort3 = (ushort)b.SerializeBits<int>(UShort3, 2, name: nameof(UShort3));
				});
				Commands = s.SerializeObjectArray<Command>(Commands, Count, name: nameof(Commands));
			}
			public class Command : BinarySerializable {
				public CommandType Type { get; set; }
				public short[] Shorts { get; set; }

				public SerializableColor Color { get; set; }
				public Position[] Positions { get; set; }
				public AABB Rectangle { get; set; }
				public Arc FillArc { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					Type = s.Serialize<CommandType>(Type, name: nameof(Type));
					switch (Type) {
						case CommandType.Color:
							Color = s.SerializeInto<SerializableColor>(Color, BytewiseColor.RGB888, name: nameof(Color));
							break;
						case CommandType.DrawRectangle:
							Rectangle = s.SerializeObject<AABB>(Rectangle, name: nameof(Rectangle));
							break;
						case CommandType.DrawTriangle:
							Positions = s.SerializeObjectArray<Position>(Positions, 3, name: nameof(Positions));
							break;
						case CommandType.DrawLine:
							Positions = s.SerializeObjectArray<Position>(Positions, 2, name: nameof(Positions));
							break;
						case CommandType.FillArc:
							FillArc = s.SerializeObject<Arc>(FillArc, name: nameof(FillArc));
							break;
					}
				}

				public int GetLength() {
					switch (Type) {
						case CommandType.Color: return 3;
						case CommandType.DrawRectangle: return 4;
						case CommandType.DrawTriangle: return 6;
						case CommandType.DrawLine: return 4;
						case CommandType.FillArc: return 6;
						default: return 0;
					}
				}

				public enum CommandType : byte {
					Color = 0,
					DrawRectangle = 1,
					DrawTriangle = 2,
					DrawLine = 3,
					FillArc = 5,
				}

				public class Position : BinarySerializable {
					public short X { get; set; }
					public short Y { get; set; }
					public short Z { get; set; }
					public override void SerializeImpl(SerializerObject s) {
						X = s.Serialize<short>(X, name: nameof(X));
						s.DoBits<short>(b => {
							Y = (short)b.SerializeBits<int>(Y, 14, name: nameof(Y));
							Z = (short)b.SerializeBits<int>(Z, 1, name: nameof(Z));
						});
					}
				}
				public class AABB : BinarySerializable {
					public short X { get; set; }
					public short Y { get; set; }
					public short Width { get; set; }
					public short Height { get; set; }

					public override void SerializeImpl(SerializerObject s) {
						X = s.Serialize<short>(X, name: nameof(X));
						Y = s.Serialize<short>(Y, name: nameof(Y));
						Width = s.Serialize<short>(Width, name: nameof(Width));
						Height = s.Serialize<short>(Height, name: nameof(Height));
					}
				}
				public class Arc : BinarySerializable {
					// http://jdrawing.sourceforge.net/doc/0.3/api/org/jdrawing/graphics/FillArc.html
					public short XPosition { get; set; }
					public short YPosition { get; set; }
					public short Width { get; set; }
					public short Height { get; set; }
					public short StartAngle { get; set; }
					public short ArcAngle { get; set; }

					public override void SerializeImpl(SerializerObject s) {
						XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
						YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
						Width = s.Serialize<short>(Width, name: nameof(Width));
						Height = s.Serialize<short>(Height, name: nameof(Height));
						StartAngle = s.Serialize<short>(StartAngle, name: nameof(StartAngle));
						ArcAngle = s.Serialize<short>(ArcAngle, name: nameof(ArcAngle));
					}
				}
			}
		}


		public class TrackObject : BinarySerializable {
			public byte ObjectType { get; set; }
			public short XPosition { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				ObjectType = s.Serialize<byte>(ObjectType, name: nameof(ObjectType));
				XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
			}
		}
		public class TrackObjectInstance : BinarySerializable {
			public short TrackObjectIndex { get; set; }
			public bool FlipX { get; set; }
			public int ObjType { get; set; }
			public bool FlagLast { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				s.DoBits<short>(b => {
					TrackObjectIndex = (short)b.SerializeBits<int>(TrackObjectIndex, 11, name: nameof(TrackObjectIndex));
					FlipX = b.SerializeBits<int>(FlipX ? 1 : 0, 1, name: nameof(FlipX)) == 1;
					ObjType = b.SerializeBits<int>(ObjType, 3, name: nameof(ObjType));
					/*FlagUnknown = b.SerializeBit<int>(FlagUnknown ? 1 : 0, 1, name: nameof(FlagUnknown)) == 1;
					DisplaySprite = b.SerializeBit<int>(DisplaySprite ? 1 : 0, 1, name: nameof(DisplaySprite)) == 1;
					HasCollision = b.SerializeBit<int>(HasCollision ? 1 : 0, 1, name: nameof(HasCollision)) == 1;*/
					FlagLast = b.SerializeBits<int>(FlagLast ? 1 : 0, 1, name: nameof(FlagLast)) == 1;
				});
			}
		}
		public class TrackObjectCollection : BinarySerializable {
			public short InstanceIndex { get; set; }
			public int Count { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				s.DoBits<short>(b => {
					InstanceIndex = (short)b.SerializeBits<int>(InstanceIndex, 11, name: nameof(InstanceIndex));
					Count = b.SerializeBits<int>(Count, 5, name: nameof(Count));
				});
			}
		}
		public class Type : BinarySerializable {
			public short Flags { get; set; }

			// Lowres only
			public byte RoadTexture0 { get; set; }
			public byte RoadTexture1 { get; set; }
			public Entry[] Entries { get; set; }

			public short Width { get; set; }
			public byte Byte4 { get; set; }
			public SerializableColor ColorGround { get; set; }
			public SerializableColor ColorAbyss { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Flags = s.Serialize<short>(Flags, name: nameof(Flags));
				if (s.GetR1Settings().GameModeSelection != GameModeSelection.RaymanKartMobile_320x240_Broken) {
					if (!Gameloft_RK_Manager.UseSingleRoadTexture(s.GetR1Settings())) {
						RoadTexture0 = s.Serialize<byte>(RoadTexture0, name: nameof(RoadTexture0));
						RoadTexture1 = s.Serialize<byte>(RoadTexture1, name: nameof(RoadTexture1));
					}
					Entries = s.SerializeObjectArray<Entry>(Entries, 3, name: nameof(Entries));
				}
				Width = s.Serialize<short>(Width, name: nameof(Width));
				Byte4 = s.Serialize<byte>(Byte4, name: nameof(Byte4));
				ColorGround = s.SerializeInto<SerializableColor>(ColorGround, BytewiseColor.RGB888, name: nameof(ColorGround));
				ColorAbyss = s.SerializeInto<SerializableColor>(ColorAbyss, BytewiseColor.RGB888, name: nameof(ColorAbyss));
			}

			public class Entry : BinarySerializable {
				public byte ci { get; set; }
				public short cj { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					ci = s.Serialize<byte>(ci, name: nameof(ci));
					cj = s.Serialize<short>(cj, name: nameof(cj));
				}
			}
		}


		public class Lum : BinarySerializable {
			public byte TrackBlockIndex { get; set; }
			public short XPosition { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				TrackBlockIndex = s.Serialize<byte>(TrackBlockIndex, name: nameof(TrackBlockIndex));
				XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
			}
		}
	}
}