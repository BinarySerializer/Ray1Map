using System;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierBeamGen : MDF_Modifier {
		public uint Version { get; set; }
		public uint Flags { get; set; }
		public Pattern GenPattern { get; set; }
		public BeamGenParams Params { get; set; }
		public float Length { get; set; }
		public float LengthVariation { get; set; }
		public float LengthVariationSpeed { get; set; }
		public float Width { get; set; }
		public float FlareWidth { get; set; }
		public float WidthVariation { get; set; }
		public float WidthVariationSpeed { get; set; }
		public float SourcePointsVariation { get; set; }
		public float SourcePointsVariationSpeed { get; set; }
		public float EndPointsVariation { get; set; }
		public float EndPointsVariationSpeed { get; set; }
		public float Intensity { get; set; }
		public float IntensityVariation { get; set; }
		public float IntensityVariationSpeed { get; set; }
		public Jade_Color BeamStartColor { get; set; }
		public Jade_Color BeamEndColor { get; set; }
		public Jade_Color FlareColor { get; set; }

		public int UseBeamCutting { get; set; } // a bool
		public float BeamFOVX { get; set; }
		public float BeamFOVY { get; set; }
		public float BeamNear { get; set; }
		public float BeamFar { get; set; }
		public Jade_Matrix ProjectionToLocal { get; set; }
		
		public Jade_Reference<GEO_Object> Material { get; set; }

		public uint UserID { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version >= 1) {
				Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
				GenPattern = s.Serialize<Pattern>(GenPattern, name: nameof(GenPattern));

				Params = GenPattern switch
				{
					Pattern.Rectangular => s.SerializeObject<RectangularParams>((RectangularParams)Params, p => p.BeamGen = this, name: nameof(Params)),
					Pattern.Random => s.SerializeObject<RandomParams>((RandomParams)Params, p => p.BeamGen = this, name: nameof(Params)),
					Pattern.Circular => s.SerializeObject<CircularParams>((CircularParams)Params, p => p.BeamGen = this, name: nameof(Params)),
					Pattern.Texture => s.SerializeObject<TextureParams>((TextureParams)Params, p => p.BeamGen = this, name: nameof(Params)),
					Pattern.SkinAndMaterial => s.SerializeObject<SkinAndMaterialParams>((SkinAndMaterialParams)Params, p => p.BeamGen = this, name: nameof(Params)),
					Pattern.CheckerBoard => s.SerializeObject<CheckerBoardParams>((CheckerBoardParams)Params, p => p.BeamGen = this, name: nameof(Params)),
					_ => throw new NotImplementedException($"TODO: Unhandled ModifierBeamGen pattern {GenPattern}")
				};
				Length = s.Serialize<float>(Length, name: nameof(Length));
				LengthVariation = s.Serialize<float>(LengthVariation, name: nameof(LengthVariation));
				LengthVariationSpeed = s.Serialize<float>(LengthVariationSpeed, name: nameof(LengthVariationSpeed));
				Width = s.Serialize<float>(Width, name: nameof(Width));
				if (Version >= 9) {
					FlareWidth = s.Serialize<float>(FlareWidth, name: nameof(FlareWidth));
				} else {
					FlareWidth = Width;
				}
				WidthVariation = s.Serialize<float>(WidthVariation, name: nameof(WidthVariation));
				WidthVariationSpeed = s.Serialize<float>(WidthVariationSpeed, name: nameof(WidthVariationSpeed));
				SourcePointsVariation = s.Serialize<float>(SourcePointsVariation, name: nameof(SourcePointsVariation));
				SourcePointsVariationSpeed = s.Serialize<float>(SourcePointsVariationSpeed, name: nameof(SourcePointsVariationSpeed));
				EndPointsVariation = s.Serialize<float>(EndPointsVariation, name: nameof(EndPointsVariation));
				EndPointsVariationSpeed = s.Serialize<float>(EndPointsVariationSpeed, name: nameof(EndPointsVariationSpeed));
				Intensity = s.Serialize<float>(Intensity, name: nameof(Intensity));
				IntensityVariation = s.Serialize<float>(IntensityVariation, name: nameof(IntensityVariation));
				IntensityVariationSpeed = s.Serialize<float>(IntensityVariationSpeed, name: nameof(IntensityVariationSpeed));
				BeamStartColor = s.SerializeObject<Jade_Color>(BeamStartColor, name: nameof(BeamStartColor));
				BeamEndColor = s.SerializeObject<Jade_Color>(BeamEndColor, name: nameof(BeamEndColor));
				FlareColor = s.SerializeObject<Jade_Color>(FlareColor, name: nameof(FlareColor));

				if (Version >= 8) {
					UseBeamCutting = s.Serialize<int>(UseBeamCutting, name: nameof(UseBeamCutting));
					BeamFOVX = s.Serialize<float>(BeamFOVX, name: nameof(BeamFOVX));
					BeamFOVY = s.Serialize<float>(BeamFOVY, name: nameof(BeamFOVY));
					BeamNear = s.Serialize<float>(BeamNear, name: nameof(BeamNear));
					BeamFar = s.Serialize<float>(BeamFar, name: nameof(BeamFar));
					ProjectionToLocal = s.SerializeObject<Jade_Matrix>(ProjectionToLocal, name: nameof(ProjectionToLocal));
				}

				Material = s.SerializeObject<Jade_Reference<GEO_Object>>(Material, name: nameof(Material))?.Resolve();
			}
			if (Version >= 2) {
				UserID = s.Serialize<uint>(UserID, name: nameof(UserID));
			}
		}

		public enum Pattern : uint {
			Rectangular = 0,
			Random = 1,
			Circular = 2,
			Texture = 3,
			SkinAndMaterial = 4,
			CheckerBoard = 5
		}

		public abstract class BeamGenParams : BinarySerializable {
			public GAO_ModifierBeamGen BeamGen { get; set; }
		}
		public class RectangularParams : BeamGenParams {
			public int BeamCountX { get; set; }
			public int BeamCountY { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				BeamCountX = s.Serialize<int>(BeamCountX, name: nameof(BeamCountX));
				BeamCountY = s.Serialize<int>(BeamCountY, name: nameof(BeamCountY));
			}
		}
		public class CheckerBoardParams : BeamGenParams {
			public int BeamCountX { get; set; }
			public int BeamCountY { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				BeamCountX = s.Serialize<int>(BeamCountX, name: nameof(BeamCountX));
				BeamCountY = s.Serialize<int>(BeamCountY, name: nameof(BeamCountY));
			}
		}
		public class RandomParams : BeamGenParams {
			public int BeamsCount { get; set; }
			public int Seed { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				BeamsCount = s.Serialize<int>(BeamsCount, name: nameof(BeamsCount));
				Seed = s.Serialize<int>(Seed, name: nameof(Seed));
			}
		}
		public class CircularParams : BeamGenParams {
			public int ScanPointsCountX { get; set; }
			public int ScanPointsCountY { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				ScanPointsCountX = s.Serialize<int>(ScanPointsCountX, name: nameof(ScanPointsCountX));
				ScanPointsCountY = s.Serialize<int>(ScanPointsCountY, name: nameof(ScanPointsCountY));
			}
		}
		public class TextureParams : BeamGenParams {
			public uint UInt_Editor_00 { get; set; }
			public uint GenPointsCount { get; set; }
			public Point[] GenPoints { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

				if(!Loader.IsBinaryData) UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));

				GenPointsCount = s.Serialize<uint>(GenPointsCount, name: nameof(GenPointsCount));
				GenPoints = s.SerializeObjectArray<Point>(GenPoints, GenPointsCount, name: nameof(GenPoints));
			}

			public class Point : BinarySerializable {
				public Jade_Vector Position { get; set; }
				public Jade_Vector Direction { get; set; }
				public override void SerializeImpl(SerializerObject s) {
					Position = s.SerializeObject<Jade_Vector>(Position, name: nameof(Position));
					Direction = s.SerializeObject<Jade_Vector>(Direction, name: nameof(Direction));
				}
			}
		}
		public class SkinAndMaterialParams : BeamGenParams {
			public short SubMatNumber { get; set; }
			public float Float_Editor_02 { get; set; }
			public Jade_Reference<OBJ_GameObject> CenterObject { get; set; }
			public bool UseGizmoForCenter { get; set; }
			public bool UseObjectForCenter { get; set; }
			public bool UseTextureForGenPoints { get; set; }
			public bool OneBeamPerValidTexel { get; set; }
			public ushort UnusedFlags { get; set; }
			public byte Byte_Editor_03 { get; set; }
			public uint UInt_Editor_04 { get; set; }
			public ushort GizmoNumber { get; set; }


			public uint GenPointsCount { get; set; }
			public Point[] GenPoints { get; set; }
			public uint[] GenPointsEarly_Editor { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

				SubMatNumber = s.Serialize<short>(SubMatNumber, name: nameof(SubMatNumber));
				if (!Loader.IsBinaryData) Float_Editor_02 = s.Serialize<float>(Float_Editor_02, name: nameof(Float_Editor_02));
				CenterObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(CenterObject, name: nameof(CenterObject))?.Resolve();

				if (BeamGen.Version >= 3) {
					s.DoBits<ushort>(b => {
						UseGizmoForCenter = b.SerializeBits<int>(UseGizmoForCenter ? 1 : 0, 1, name: nameof(UseGizmoForCenter)) == 1;
						UseObjectForCenter = b.SerializeBits<int>(UseObjectForCenter ? 1 : 0, 1, name: nameof(UseObjectForCenter)) == 1;
						UseTextureForGenPoints = b.SerializeBits<int>(UseTextureForGenPoints ? 1 : 0, 1, name: nameof(UseTextureForGenPoints)) == 1;
						OneBeamPerValidTexel = b.SerializeBits<int>(OneBeamPerValidTexel ? 1 : 0, 1, name: nameof(OneBeamPerValidTexel)) == 1;
						UnusedFlags = (ushort)b.SerializeBits<int>(UnusedFlags, 12, name: nameof(UnusedFlags));
					});
				} else {
					if (!Loader.IsBinaryData) Byte_Editor_03 = s.Serialize<byte>(Byte_Editor_03, name: nameof(Byte_Editor_03));
				}
				if (!Loader.IsBinaryData) UInt_Editor_04 = s.Serialize<uint>(UInt_Editor_04, name: nameof(UInt_Editor_04));

				if (BeamGen.Version >= 6) {
					GizmoNumber = s.Serialize<ushort>(GizmoNumber, name: nameof(GizmoNumber));
				}
				GenPointsCount = s.Serialize<uint>(GenPointsCount, name: nameof(GenPointsCount));
				if (BeamGen.Version >= 7) {
					GenPoints = s.SerializeObjectArray<Point>(GenPoints, GenPointsCount, name: nameof(GenPoints));
				} else if (!Loader.IsBinaryData) {
					GenPointsEarly_Editor = s.SerializeArray<uint>(GenPointsEarly_Editor, GenPointsCount, name: nameof(GenPointsEarly_Editor));
				}
			}

			public class Point : BinarySerializable {
				public Jade_Vector VertexPosition { get; set; }
				public byte[] BoneIndex { get; set; }
				public byte[] BoneWeight { get; set; }
				public override void SerializeImpl(SerializerObject s) {
					VertexPosition = s.SerializeObject<Jade_Vector>(VertexPosition, name: nameof(VertexPosition));
					BoneIndex = s.SerializeArray<byte>(BoneIndex, 4, name: nameof(BoneIndex));
					BoneWeight = s.SerializeArray<byte>(BoneWeight, 4, name: nameof(BoneWeight));
				}
			}
		}
	}
}
