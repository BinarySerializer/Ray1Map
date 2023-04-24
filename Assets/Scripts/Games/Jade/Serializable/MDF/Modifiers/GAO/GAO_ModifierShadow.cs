using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierShadow : MDF_Modifier {
		public uint Version { get; set; }
		public uint Flags { get; set; }
		public Jade_TextureReference Texture { get; set; }
		public float XSizeFactor { get; set; }
		public float YSizeFactor { get; set; }
		public uint TextureIndex { get; set; }
		public uint NextShadowModifierPointer { get; set; }
		public uint GameObjectPointer { get; set; }
		public float ZAttenuationFactor { get; set; }
		public Jade_Color ShadowColor { get; set; }
		public float ZStart { get; set; }
		public Jade_Vector Center { get; set; }
		public float ZSizeFactor { get; set; }
		public uint ProjectionMethod { get; set; }
		public byte TextureTiling { get; set; }
		public byte KK_Version { get; set; }
		public short Dummy { get; set; }
		public Jade_Reference<OBJ_GameObject> LightGameObject { get; set; }

		public uint T2T_UInt { get; set; }
		public float T2T_Float { get; set; }

		public float CPP_Float0 { get; set; }
		public float CPP_Float1 { get; set; }
		public float CPP_Float2 { get; set; }
		public float CPP_Float3 { get; set; }
		public float CPP_Float4 { get; set; }
		public float CPP_Float5 { get; set; }
		public float CPP_Float6_V3 { get; set; }
		public uint Flags_TFS { get; set; }
		public float FadeInStart { get; set; }
		public float FadeInLength { get; set; }
		public float FadeOutStart { get; set; }
		public float FadeOutLength { get; set; }
		public float MinimumIntensity { get; set; }
		public float CullDistance { get; set; }
		public uint HighRes { get; set; } // Boolean
		public float TVP_Float0 { get; set; }
		public float TVP_Float1 { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CPP)) {
				Version = s.Serialize<uint>(Version, name: nameof(Version));
				if (Version < 4) {
					Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
					Texture = s.SerializeObject<Jade_TextureReference>(Texture, name: nameof(Texture));
					XSizeFactor = s.Serialize<float>(XSizeFactor, name: nameof(XSizeFactor));
					YSizeFactor = s.Serialize<float>(YSizeFactor, name: nameof(YSizeFactor));
					TextureIndex = s.Serialize<uint>(TextureIndex, name: nameof(TextureIndex));
					ZAttenuationFactor = s.Serialize<float>(ZAttenuationFactor, name: nameof(ZAttenuationFactor));
					ShadowColor = s.SerializeObject<Jade_Color>(ShadowColor, name: nameof(ShadowColor));
					ZStart = s.Serialize<float>(ZStart, name: nameof(ZStart));
					Center = s.SerializeObject<Jade_Vector>(Center, name: nameof(Center));
					ZSizeFactor = s.Serialize<float>(ZSizeFactor, name: nameof(ZSizeFactor));
					ProjectionMethod = s.Serialize<uint>(ProjectionMethod, name: nameof(ProjectionMethod));
					TextureTiling = s.Serialize<byte>(TextureTiling, name: nameof(TextureTiling));
					CPP_Float0 = s.Serialize<float>(CPP_Float0, name: nameof(CPP_Float0));
					CPP_Float1 = s.Serialize<float>(CPP_Float1, name: nameof(CPP_Float1));
					CPP_Float2 = s.Serialize<float>(CPP_Float2, name: nameof(CPP_Float2));
					CPP_Float3 = s.Serialize<float>(CPP_Float3, name: nameof(CPP_Float3));
					CPP_Float4 = s.Serialize<float>(CPP_Float4, name: nameof(CPP_Float4));
					CPP_Float5 = s.Serialize<float>(CPP_Float5, name: nameof(CPP_Float5));
					if(Version >= 3) CPP_Float6_V3 = s.Serialize<float>(CPP_Float6_V3, name: nameof(CPP_Float6_V3));
				} else {
					if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_TFS)) {
						Flags_TFS = s.Serialize<uint>(Flags_TFS, name: nameof(Flags_TFS));
						ShadowColor = s.SerializeObject<Jade_Color>(ShadowColor, name: nameof(ShadowColor));
						if (Version >= 5) {
							FadeInStart = s.Serialize<float>(FadeInStart, name: nameof(FadeInStart));
							FadeInLength = s.Serialize<float>(FadeInLength, name: nameof(FadeInLength));
							FadeOutStart = s.Serialize<float>(FadeOutStart, name: nameof(FadeOutStart));
							FadeOutLength = s.Serialize<float>(FadeOutLength, name: nameof(FadeOutLength));
							if (Version >= 6) {
								MinimumIntensity = s.Serialize<float>(MinimumIntensity, name: nameof(MinimumIntensity));
								if (Version >= 7) {
									CullDistance = s.Serialize<float>(CullDistance, name: nameof(CullDistance));
									HighRes = s.Serialize<uint>(HighRes, name: nameof(HighRes));
								}
							}
						}
					} else {
						Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
						Texture = s.SerializeObject<Jade_TextureReference>(Texture, name: nameof(Texture));
						TextureIndex = s.Serialize<uint>(TextureIndex, name: nameof(TextureIndex));
						ShadowColor = s.SerializeObject<Jade_Color>(ShadowColor, name: nameof(ShadowColor));
						Center = s.SerializeObject<Jade_Vector>(Center, name: nameof(Center));
						TVP_Float0 = s.Serialize<float>(TVP_Float0, name: nameof(TVP_Float0));
						TVP_Float1 = s.Serialize<float>(TVP_Float1, name: nameof(TVP_Float1));
						Texture?.Resolve(s, RRR2_readBool: true);
					}
				}
			} else {
				if (!Loader.IsBinaryData) Version = s.Serialize<uint>(Version, name: nameof(Version));
				Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
				Texture = s.SerializeObject<Jade_TextureReference>(Texture, name: nameof(Texture));
				XSizeFactor = s.Serialize<float>(XSizeFactor, name: nameof(XSizeFactor));
				YSizeFactor = s.Serialize<float>(YSizeFactor, name: nameof(YSizeFactor));
				TextureIndex = s.Serialize<uint>(TextureIndex, name: nameof(TextureIndex));
				if (!Loader.IsBinaryData) {
					NextShadowModifierPointer = s.Serialize<uint>(NextShadowModifierPointer, name: nameof(NextShadowModifierPointer));
					GameObjectPointer = s.Serialize<uint>(GameObjectPointer, name: nameof(GameObjectPointer));
				}
				ZAttenuationFactor = s.Serialize<float>(ZAttenuationFactor, name: nameof(ZAttenuationFactor));
				ShadowColor = s.SerializeObject<Jade_Color>(ShadowColor, name: nameof(ShadowColor));
				ZStart = s.Serialize<float>(ZStart, name: nameof(ZStart));
				Center = s.SerializeObject<Jade_Vector>(Center, name: nameof(Center));
				ZSizeFactor = s.Serialize<float>(ZSizeFactor, name: nameof(ZSizeFactor));
				ProjectionMethod = s.Serialize<uint>(ProjectionMethod, name: nameof(ProjectionMethod));
				TextureTiling = s.Serialize<byte>(TextureTiling, name: nameof(TextureTiling));
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong) || !Loader.IsBinaryData) {
					KK_Version = s.Serialize<byte>(KK_Version, name: nameof(KK_Version));
					Dummy = s.Serialize<short>(Dummy, name: nameof(Dummy));
					if (KK_Version >= 1) {
						LightGameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(LightGameObject, name: nameof(LightGameObject))?.Resolve();
					}
				}
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_T2T_20051002)) {
					T2T_UInt = s.Serialize<uint>(T2T_UInt, name: nameof(T2T_UInt));
					if (T2T_UInt > 1) T2T_Float = s.Serialize<float>(T2T_Float, name: nameof(T2T_Float));
				}
				Texture?.Resolve(s, RRR2_readBool: true);
			}
		}
	}
}
