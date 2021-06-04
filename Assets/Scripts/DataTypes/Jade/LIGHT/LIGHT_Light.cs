using System;
using BinarySerializer;

namespace R1Engine.Jade {
	// Found in LIGHT_p_CreateFromBuffer
	public class LIGHT_Light : GRO_GraphicRenderObject {
		public LightType Type { get; set; }
		public int Flags { get; set; }
		public Jade_Color Color { get; set; }
		public LIGHT_XenonData1 XenonData1 { get; set; }
		public uint UInt_Editor_00 { get; set; }
		public Jade_Color SpecularColor { get; set; }
		public float Near { get; set; }
		public float Far { get; set; }
		public float LittleAlpha { get; set; }
		public float BigAlpha { get; set; }
		public float Intensity { get; set; }
		public float OcclusionFactor { get; set; }
		public float HighIntensity { get; set; }
		public float LowIntensity { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
		public LIGHT_XenonData2 XenonData2 { get; set; }

		public uint FogLinksCount { get; set; }
		public FogLink[] FogLinks { get; set; }
		public uint FogSubBVsCount { get; set; }
		public FogSubBV[] FogSubBVs { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			s.SerializeBitValues<int>(bitFunc => {
				Type = (LightType)bitFunc((int)Type, 4, name: nameof(Type));
				Flags = bitFunc(Flags, 28, name: nameof(Flags));
			});
			Color = s.SerializeObject<Jade_Color>(Color, name: nameof(Color));
			if (s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon) && BitHelpers.ExtractBits((int)Flags, 3, 0) == 7) {
				XenonData1 = s.SerializeObject<LIGHT_XenonData1>(XenonData1, name: nameof(XenonData1));
			}
			if (!Loader.IsBinaryData && ObjectVersion >= 5)
				UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
			if (ObjectVersion >= 10) SpecularColor = s.SerializeObject<Jade_Color>(SpecularColor, name: nameof(SpecularColor));
			Near = s.Serialize<float>(Near, name: nameof(Near));
			Far = s.Serialize<float>(Far, name: nameof(Far));
			LittleAlpha = s.Serialize<float>(LittleAlpha, name: nameof(LittleAlpha));
			BigAlpha = s.Serialize<float>(BigAlpha, name: nameof(BigAlpha));
			if (ObjectVersion >= 1) Intensity = s.Serialize<float>(Intensity, name: nameof(Intensity));
			if (ObjectVersion >= 11) OcclusionFactor = s.Serialize<float>(OcclusionFactor, name: nameof(OcclusionFactor));
			if (ObjectVersion >= 13) {
				HighIntensity = s.Serialize<float>(HighIntensity, name: nameof(HighIntensity));
				LowIntensity = s.Serialize<float>(LowIntensity, name: nameof(LowIntensity));
			}
			if (ObjectVersion < 6 && (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier) || !Loader.IsBinaryData)) {
				GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject));
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) GameObject?.Resolve();
			}
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
				if (Type == LightType.Fog) {
					if (ObjectVersion >= 8) {
						FogLinksCount = s.Serialize<uint>(FogLinksCount, name: nameof(FogLinksCount));
						FogLinks = s.SerializeObjectArray<FogLink>(FogLinks, FogLinksCount, name: nameof(FogLinks));
					}
					if (ObjectVersion >= 9) {
						FogSubBVsCount = s.Serialize<uint>(FogSubBVsCount, name: nameof(FogSubBVsCount));
						FogSubBVs = s.SerializeObjectArray<FogSubBV>(FogSubBVs, FogSubBVsCount, name: nameof(FogSubBVs));
					}
				}
			}

			if ((XenonData1 != null && BitHelpers.ExtractBits((int)XenonData1.LightFlags, 3, 0) == 5)
				|| (XenonData1 == null && s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon) && BitHelpers.ExtractBits((int)Flags, 3, 0) == 5)) {
				XenonData2 = s.SerializeObject<LIGHT_XenonData2>(XenonData2, name: nameof(XenonData2));
			}
		}

		public enum LightType : int {
			Type0 = 0,
			Type1 = 1,
			Type2 = 2,
			Fog = 3,
			Type4 = 4,
			Type5 = 5,
			Type6 = 6,
			Type7 = 7,
			Type8 = 8,
			Type9 = 9,
			Type10 = 10,
			Type11 = 11,
			Type12 = 12,
			Type13 = 13,
			Type14 = 14,
			Type15 = 15,
		}

		public class FogLink : BinarySerializable {
			public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
			public Jade_Vector LinkPosition { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject));
				LinkPosition = s.SerializeObject<Jade_Vector>(LinkPosition, name: nameof(LinkPosition));
			}
		}

		public class FogSubBV : BinarySerializable {
			public Jade_Vector Min { get; set; }
			public Jade_Vector Max { get; set; }
			public Jade_Matrix InvFogMatrix { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Min = s.SerializeObject<Jade_Vector>(Min, name: nameof(Min));
				Max = s.SerializeObject<Jade_Vector>(Max, name: nameof(Max));
				InvFogMatrix = s.SerializeObject<Jade_Matrix>(InvFogMatrix, name: nameof(InvFogMatrix));
			}
		}
	}
}
