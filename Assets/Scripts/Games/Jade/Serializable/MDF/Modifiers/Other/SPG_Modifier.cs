using BinarySerializer;

namespace Ray1Map.Jade {
	public class SPG_Modifier : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public uint Version { get; set; }
		public float GlobalSize { get; set; }
		public float GlobalRatio { get; set; }
		public uint MaterialID { get; set; }

		public float Extraction { get; set; }
		public float ThresholdMin { get; set; }
		public uint MaxDepth { get; set; }
		public uint SubMaterialMask { get; set; }
		public float Noise { get; set; }
		public SPG_SpriteMapper[] SpriteMappers { get; set; }
		public SPG_SpriteElementDescriptor[] SpriteElementDescriptors { get; set; }
		public float LODCorrectionFactor { get; set; }
		public uint Flags { get; set; }
		public float SpecialFogNear { get; set; }
		public float SpecialFogFar { get; set; }
		public SerializableColor SpecialFogColor { get; set; }

		public float V8_Montreal_Float { get; set; }
		public float V9_Montreal_Float0 { get; set; }
		public float V9_Montreal_Float1 { get; set; }
		public uint  V9_Montreal_UInt2  { get; set; }
		public float V9_Montreal_Float3 { get; set; }
		public float V9_Montreal_Float4 { get; set; }
		public float V9_Montreal_Float5 { get; set; }
		public float V9_Montreal_Float6 { get; set; }
		public float V10_Montreal_Float0 { get; set; }
		public float V10_Montreal_Float1 { get; set; }
		public float V10_Montreal_Float2 { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			GlobalSize = s.Serialize<float>(GlobalSize, name: nameof(GlobalSize));
			GlobalRatio = s.Serialize<float>(GlobalRatio, name: nameof(GlobalRatio));
			MaterialID = s.Serialize<uint>(MaterialID, name: nameof(MaterialID));
			if (Version >= 1) {
				Extraction = s.Serialize<float>(Extraction, name: nameof(Extraction));
				ThresholdMin = s.Serialize<float>(ThresholdMin, name: nameof(ThresholdMin));
				MaxDepth = s.Serialize<uint>(MaxDepth, name: nameof(MaxDepth));
			}
			if (Version >= 2) SubMaterialMask = s.Serialize<uint>(SubMaterialMask, name: nameof(SubMaterialMask));
			if (Version >= 3) Noise = s.Serialize<float>(Noise, name: nameof(Noise));
			if (Version >= 4) SpriteMappers = s.SerializeObjectArray<SPG_SpriteMapper>(SpriteMappers, 4, name: nameof(SpriteMappers));
			if (Version >= 5) SpriteElementDescriptors = s.SerializeObjectArray<SPG_SpriteElementDescriptor>(SpriteElementDescriptors, 4, name: nameof(SpriteElementDescriptors));
			if (Version >= 6) LODCorrectionFactor = s.Serialize<float>(LODCorrectionFactor, name: nameof(LODCorrectionFactor));
			if (Version >= 7) Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
				if (Version >= 8) {
					SpecialFogNear = s.Serialize<float>(SpecialFogNear, name: nameof(SpecialFogNear));
					SpecialFogFar = s.Serialize<float>(SpecialFogFar, name: nameof(SpecialFogFar));
					SpecialFogColor = s.SerializeInto<SerializableColor>(SpecialFogColor, BitwiseColor.RGBA8888, name: nameof(SpecialFogColor));
				}
			} else {
				if (Version >= 8) V8_Montreal_Float = s.Serialize<float>(V8_Montreal_Float, name: nameof(V8_Montreal_Float));
				if (Version >= 9) {
					V9_Montreal_Float0 = s.Serialize<float>(V9_Montreal_Float0, name: nameof(V9_Montreal_Float0));
					V9_Montreal_Float1 = s.Serialize<float>(V9_Montreal_Float1, name: nameof(V9_Montreal_Float1));
					V9_Montreal_UInt2 = s.Serialize<uint>(V9_Montreal_UInt2, name: nameof(V9_Montreal_UInt2));
					V9_Montreal_Float3 = s.Serialize<float>(V9_Montreal_Float3, name: nameof(V9_Montreal_Float3));
					V9_Montreal_Float4 = s.Serialize<float>(V9_Montreal_Float4, name: nameof(V9_Montreal_Float4));
					V9_Montreal_Float5 = s.Serialize<float>(V9_Montreal_Float5, name: nameof(V9_Montreal_Float5));
					V9_Montreal_Float6 = s.Serialize<float>(V9_Montreal_Float6, name: nameof(V9_Montreal_Float6));
				}
				if (Version >= 10) {
					V10_Montreal_Float0 = s.Serialize<float>(V10_Montreal_Float0, name: nameof(V10_Montreal_Float0));
					V10_Montreal_Float1 = s.Serialize<float>(V10_Montreal_Float1, name: nameof(V10_Montreal_Float1));
					V10_Montreal_Float2 = s.Serialize<float>(V10_Montreal_Float2, name: nameof(V10_Montreal_Float2));
				}
			}
		}

		public class SPG_SpriteMapper : BinarySerializable {
			public sbyte UShift { get; set; }
			public sbyte UAdd { get; set; }
			public sbyte VShift { get; set; }
			public sbyte VAdd { get; set; }
			public sbyte USubFrameShift { get; set; }
			public sbyte VSubFrameShift { get; set; }
			public sbyte AnimationFrameDT { get; set; }
			public sbyte AnimOffset { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				UShift = s.Serialize<sbyte>(UShift, name: nameof(UShift));
				UAdd = s.Serialize<sbyte>(UAdd, name: nameof(UAdd));
				VShift = s.Serialize<sbyte>(VShift, name: nameof(VShift));
				VAdd = s.Serialize<sbyte>(VAdd, name: nameof(VAdd));
				USubFrameShift = s.Serialize<sbyte>(USubFrameShift, name: nameof(USubFrameShift));
				VSubFrameShift = s.Serialize<sbyte>(VSubFrameShift, name: nameof(VSubFrameShift));
				AnimationFrameDT = s.Serialize<sbyte>(AnimationFrameDT, name: nameof(AnimationFrameDT));
				AnimOffset = s.Serialize<sbyte>(AnimOffset, name: nameof(AnimOffset));
			}
		}

		public class SPG_SpriteElementDescriptor : BinarySerializable {
			public uint SubElementMaterialNumber { get; set; }
			public float SizeFactor { get; set; }
			public float SizeNoiseFactor { get; set; }
			public float RatioFactor { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				SubElementMaterialNumber = s.Serialize<uint>(SubElementMaterialNumber, name: nameof(SubElementMaterialNumber));
				SizeFactor = s.Serialize<float>(SizeFactor, name: nameof(SizeFactor));
				SizeNoiseFactor = s.Serialize<float>(SizeNoiseFactor, name: nameof(SizeNoiseFactor));
				RatioFactor = s.Serialize<float>(RatioFactor, name: nameof(RatioFactor));
			}
		}
	}
}
