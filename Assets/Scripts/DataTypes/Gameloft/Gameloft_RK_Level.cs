using System;
using System.Linq;

namespace R1Engine
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
		public RGB888Color Color_bC { get; set; }
		public RGB888Color Color_bD_Road1 { get; set; }
		public RGB888Color Color_bA { get; set; }
		public RGB888Color Color_bE_Road2 { get; set; }
		public RGB888Color Color_bF_Fog { get; set; }
		public short bG_FogAmount { get; set; }
		public short aU_2 { get; set; }
		public byte N { get; set; }
		public byte gF { get; set; }
		public bool bp { get; set; }
		public short bJ { get; set; }
		public short bK { get; set; }
		public RGB888Color Color_afterbK_0 { get; set; }
		public RGB888Color Color_afterbK_1 { get; set; }
		public byte bL { get; set; }
		public RGB888Color Color_bH_Wall0 { get; set; }
		public RGB888Color Color_bI_Wall1 { get; set; }
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
			LumsCount = s.Serialize<byte>(LumsCount, name: nameof(LumsCount));
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
			BackgroundLayersCount = s.Serialize<byte>(BackgroundLayersCount, name: nameof(BackgroundLayersCount));
			bB = s.Serialize<byte>(bB, name: nameof(bB));
			Color_bC = s.SerializeObject<RGB888Color>(Color_bC, name: nameof(Color_bC));
			Color_bD_Road1 = s.SerializeObject<RGB888Color>(Color_bD_Road1, name: nameof(Color_bD_Road1));
			Color_bA = s.SerializeObject<RGB888Color>(Color_bA, name: nameof(Color_bA));
			Color_bE_Road2 = s.SerializeObject<RGB888Color>(Color_bE_Road2, name: nameof(Color_bE_Road2));
			Color_bF_Fog = s.SerializeObject<RGB888Color>(Color_bF_Fog, name: nameof(Color_bF_Fog));
			bG_FogAmount = s.Serialize<short>(bG_FogAmount, name: nameof(bG_FogAmount));
			aU_2 = s.Serialize<short>(aU_2, name: nameof(aU_2));
			N = s.Serialize<byte>(N, name: nameof(N));
			gF = s.Serialize<byte>(gF, name: nameof(gF));
			bp = s.Serialize<bool>(bp, name: nameof(bp));
			bJ = s.Serialize<short>(bJ, name: nameof(bJ));
			bK = s.Serialize<short>(bK, name: nameof(bK));
			Color_afterbK_0 = s.SerializeObject<RGB888Color>(Color_afterbK_0, name: nameof(Color_afterbK_0));
			Color_afterbK_1 = s.SerializeObject<RGB888Color>(Color_afterbK_1, name: nameof(Color_afterbK_1));
			bL = s.Serialize<byte>(bL, name: nameof(bL));
			Color_bH_Wall0 = s.SerializeObject<RGB888Color>(Color_bH_Wall0, name: nameof(Color_bH_Wall0));
			Color_bI_Wall1 = s.SerializeObject<RGB888Color>(Color_bI_Wall1, name: nameof(Color_bI_Wall1));
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
			Lums = s.SerializeObjectArray<Lum>(Lums, LumsCount, name: nameof(Lums));
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

		public class TriggerObject : R1Serializable {
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

		public class Struct2 : R1Serializable {
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

		public class BackgroundGradient : R1Serializable {
			public byte Count { get; set; }
			public Key[] Keys { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Count = s.Serialize<byte>(Count, name: nameof(Count));
				Keys = s.SerializeObjectArray<Key>(Keys, Count, name: nameof(Keys));
			}
			public class Key : R1Serializable {
				public short Position { get; set; } // 100 is top of screen, 80 is middle
				public RGB888Color Color { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					Position = s.Serialize<short>(Position, name: nameof(Position));
					Color = s.SerializeObject<RGB888Color>(Color, name: nameof(Color));
				}
			}
		}

		public class TrackGradientMap : R1Serializable {
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

		public class ObjectType : R1Serializable {
			public byte PuppetIndex { get; set; }
			public byte AnimationIndex { get; set; }
			public byte PaletteIndex { get; set; }
			public short YPosition { get; set; }
			public short Short5 { get; set; }
			public short Short7 { get; set; }
			public byte Byte9 { get; set; }
			public short Short10 { get; set; }
			public byte Byte12 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				PuppetIndex = s.Serialize<byte>(PuppetIndex, name: nameof(PuppetIndex));
				AnimationIndex = s.Serialize<byte>(AnimationIndex, name: nameof(AnimationIndex));
				PaletteIndex = s.Serialize<byte>(PaletteIndex, name: nameof(PaletteIndex));
				YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
				Short5 = s.Serialize<short>(Short5, name: nameof(Short5));
				Short7 = s.Serialize<short>(Short7, name: nameof(Short7));
				Byte9 = s.Serialize<byte>(Byte9, name: nameof(Byte9));
				Short10 = s.Serialize<short>(Short10, name: nameof(Short10));
				Byte12 = s.Serialize<byte>(Byte12, name: nameof(Byte12));
			}
		}

		public class Object3D : R1Serializable {
			public byte Count { get; set; }
			public ushort UShort1 { get; set; }
			public ushort UShort2 { get; set; }
			public ushort UShort3 { get; set; }
			public Command[] Commands { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Count = s.Serialize<byte>(Count, name: nameof(Count));
				s.SerializeBitValues<ushort>(bitFunc => {
					UShort1 = (ushort)bitFunc(UShort1, 7, name: nameof(UShort1));
					UShort2 = (ushort)bitFunc(UShort2, 7, name: nameof(UShort2));
					UShort3 = (ushort)bitFunc(UShort3, 2, name: nameof(UShort3));
				});
				Commands = s.SerializeObjectArray<Command>(Commands, Count, name: nameof(Commands));
			}
			public class Command : R1Serializable {
				public CommandType Type { get; set; }
				public short[] Shorts { get; set; }

				public RGB888Color Color { get; set; }
				public Position[] Positions { get; set; }
				public AABB Rectangle { get; set; }
				public Arc FillArc { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					Type = s.Serialize<CommandType>(Type, name: nameof(Type));
					switch (Type) {
						case CommandType.Color:
							Color = s.SerializeObject<RGB888Color>(Color, name: nameof(Color));
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

				public class Position : R1Serializable {
					public short X { get; set; }
					public short Y { get; set; }
					public short Z { get; set; }
					public override void SerializeImpl(SerializerObject s) {
						X = s.Serialize<short>(X, name: nameof(X));
						s.SerializeBitValues<short>(bitFunc => {
							Y = (short)bitFunc(Y, 14, name: nameof(Y));
							Z = (short)bitFunc(Z, 1, name: nameof(Z));
						});
					}
				}
				public class AABB : R1Serializable {
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
				public class Arc : R1Serializable {
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
			public bool FlipX { get; set; }
			public int ObjType { get; set; }
			public bool FlagUnknown { get; set; }
			public bool DisplaySprite { get; set; }
			public bool HasCollision { get; set; }
			public bool FlagLast { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				s.SerializeBitValues<short>(bitFunc => {
					TrackObjectIndex = (short)bitFunc(TrackObjectIndex, 11, name: nameof(TrackObjectIndex));
					FlipX = bitFunc(FlipX ? 1 : 0, 1, name: nameof(FlipX)) == 1;
					ObjType = bitFunc(ObjType, 3, name: nameof(ObjType));
					/*FlagUnknown = bitFunc(FlagUnknown ? 1 : 0, 1, name: nameof(FlagUnknown)) == 1;
					DisplaySprite = bitFunc(DisplaySprite ? 1 : 0, 1, name: nameof(DisplaySprite)) == 1;
					HasCollision = bitFunc(HasCollision ? 1 : 0, 1, name: nameof(HasCollision)) == 1;*/
					FlagLast = bitFunc(FlagLast ? 1 : 0, 1, name: nameof(FlagLast)) == 1;
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
			public short Flags { get; set; }

			// Lowres only
			public byte RoadTexture0 { get; set; }
			public byte RoadTexture1 { get; set; }
			public Entry[] Entries { get; set; }

			public short Width { get; set; }
			public byte Byte4 { get; set; }
			public RGB888Color ColorGround { get; set; }
			public RGB888Color ColorAbyss { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Flags = s.Serialize<short>(Flags, name: nameof(Flags));
				if (s.GameSettings.GameModeSelection != GameModeSelection.RaymanKartMobile_320x240) {
					RoadTexture0 = s.Serialize<byte>(RoadTexture0, name: nameof(RoadTexture0));
					RoadTexture1 = s.Serialize<byte>(RoadTexture1, name: nameof(RoadTexture1));
					Entries = s.SerializeObjectArray<Entry>(Entries, 3, name: nameof(Entries));
				}
				Width = s.Serialize<short>(Width, name: nameof(Width));
				Byte4 = s.Serialize<byte>(Byte4, name: nameof(Byte4));
				ColorGround = s.SerializeObject<RGB888Color>(ColorGround, name: nameof(ColorGround));
				ColorAbyss = s.SerializeObject<RGB888Color>(ColorAbyss, name: nameof(ColorAbyss));
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


		public class Lum : R1Serializable {
			public byte TrackBlockIndex { get; set; }
			public short XPosition { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				TrackBlockIndex = s.Serialize<byte>(TrackBlockIndex, name: nameof(TrackBlockIndex));
				XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
			}
		}
	}
}