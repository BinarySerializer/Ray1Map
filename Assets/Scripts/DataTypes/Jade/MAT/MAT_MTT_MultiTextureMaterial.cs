using System;
using BinarySerializer;

namespace R1Engine.Jade {
	// MAT_pst_CreateMultiTextureFromBuffer
	public class MAT_MTT_MultiTextureMaterial : GRO_GraphicRenderObject {
		public Jade_Color Ambiant { get; set; }
		public Jade_Color Diffuse { get; set; }
		public Jade_Color Specular { get; set; }
		public Jade_Color RealSpecular { get; set; } // Color?
		public uint SpecularExp { get; set; } // Read as a float, but it's 0x80000000, ie negative zero
		public float Opacity { get; set; }
		public uint Flags { get; set; }
		public uint FirstLevelPointer { get; set; } // Leftover, this is overwritten
		public uint ValidateMask { get; set; }

		// Float_0C Negative -> NF (Negative Float)
		public byte Version { get; set; }
		public byte Sound { get; set; }
		public short Dummy { get; set; }
		public uint NF_Xenon_UInt { get; set; }

		// Montreal
		public float Float_Editor_Montreal { get; set; }

		public MAT_MTT_Level[] Levels { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_WW) || !Loader.IsBinaryData) {
				Ambiant = s.SerializeObject<Jade_Color>(Ambiant, name: nameof(Ambiant));
				Diffuse = s.SerializeObject<Jade_Color>(Diffuse, name: nameof(Diffuse));
			}
			Specular = s.SerializeObject<Jade_Color>(Specular, name: nameof(Specular));
			if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_WW)) {
				SpecularExp = s.Serialize<uint>(SpecularExp, name: nameof(SpecularExp));
			} else {
				if (ObjectVersion < 8) {
					RealSpecular = new Jade_Color(0xFFFFFFFF);
					if (!Loader.IsBinaryData) SpecularExp = s.Serialize<uint>(SpecularExp, name: nameof(SpecularExp));
				} else {
					RealSpecular = s.SerializeObject<Jade_Color>(RealSpecular, name: nameof(RealSpecular));
					SpecularExp = s.Serialize<uint>(SpecularExp, name: nameof(SpecularExp));
				}
			}
			if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_WW) || !Loader.IsBinaryData) {
				Opacity = s.Serialize<float>(Opacity, name: nameof(Opacity));
			}
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			FirstLevelPointer = s.Serialize<uint>(FirstLevelPointer, name: nameof(FirstLevelPointer));
			ValidateMask = s.Serialize<uint>(ValidateMask, name: nameof(ValidateMask));

			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
				if (BitHelpers.ExtractBits((int)SpecularExp, 1, 31) == 1) {
					Version = s.Serialize<byte>(Version, name: nameof(Version));
					Sound = s.Serialize<byte>(Sound, name: nameof(Sound));
					Dummy = s.Serialize<short>(Dummy, name: nameof(Dummy));
					if (s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon) && Version == 3) {
						NF_Xenon_UInt = s.Serialize<uint>(NF_Xenon_UInt, name: nameof(NF_Xenon_UInt));
					}
				} else {
					Version = 1;
				}
			} else {
				if (ObjectVersion >= 1) {
					if (!Loader.IsBinaryData) Float_Editor_Montreal = s.Serialize<float>(Float_Editor_Montreal, name: nameof(Float_Editor_Montreal));
				}
			}

			if (FirstLevelPointer != 0) {
				Levels = s.SerializeObjectArrayUntil<MAT_MTT_Level>(Levels, t => t.TextureID == 0, 
					onPreSerialize: t => {
						t.Material = this;
					}, name: nameof(Levels));
			}
		}

		public class MAT_MTT_Level : BinarySerializable {
			public MAT_MTT_MultiTextureMaterial Material { get; set; } // Set in onPreSerialize

			public short TextureID { get; set; }
			public uint AdditionalFlags { get; set; }
			public uint Flags { get; set; }
			public float ScaleSpeedPosU { get; set; }
			public float ScaleSpeedPosV { get; set; }
			public Jade_TextureReference Texture { get; set; }

			// Xenon
			public uint HasXenonData { get; set; }
			public XenonData Data { get; set; }

			// Montreal
			public byte DispLayersCount { get; set; }
			public float DispOffset { get; set; }
			public float DispOffsetU { get; set; }
			public float DispOffsetV { get; set; }

			public int AlphaTestValue { get; set; }
			public int UnusedFlags { get; set; }
			public int FrameCycle { get; set; }
			public bool IsRotationAnim { get; set; }
			public int RotationSpeed { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				TextureID = s.Serialize<short>(TextureID, name: nameof(TextureID));
				if (Material.ObjectVersion < 7) {
					AdditionalFlags = s.Serialize<ushort>((ushort)AdditionalFlags, name: nameof(AdditionalFlags));
				} else {
					AdditionalFlags = s.Serialize<uint>(AdditionalFlags, name: nameof(AdditionalFlags));
				}
				Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
				ScaleSpeedPosU = s.Serialize<float>(ScaleSpeedPosU, name: nameof(ScaleSpeedPosU));
				ScaleSpeedPosV = s.Serialize<float>(ScaleSpeedPosV, name: nameof(ScaleSpeedPosV));

				if (Material.ObjectVersion >= 9) {
					DispLayersCount = s.Serialize<byte>(DispLayersCount, name: nameof(DispLayersCount));
					DispOffset = s.Serialize<float>(DispOffset, name: nameof(DispOffset));
					DispOffsetU = s.Serialize<float>(DispOffsetU, name: nameof(DispOffsetU));
					DispOffsetV = s.Serialize<float>(DispOffsetV, name: nameof(DispOffsetV));
				}
				if (Material.ObjectVersion >= 3) {
					s.SerializeBitValues<int>(bitFunc => {
						RotationSpeed = bitFunc(RotationSpeed, 15, name: nameof(RotationSpeed));
						IsRotationAnim = bitFunc(IsRotationAnim ? 1 : 0, 1, name: nameof(IsRotationAnim)) == 1;
						FrameCycle = bitFunc(FrameCycle, 4, name: nameof(FrameCycle));
						UnusedFlags = bitFunc(UnusedFlags, 4, name: nameof(UnusedFlags));
						AlphaTestValue = bitFunc(AlphaTestValue, 8, name: nameof(AlphaTestValue));
					});
				}

				Texture = s.SerializeObject<Jade_TextureReference>(Texture, name: nameof(Texture))?.Resolve(s, RRR2_readBool: true);

				if (Material.Version >= 2 && Material.Version <= 0x12) {
					HasXenonData = s.Serialize<uint>(HasXenonData, name: nameof(HasXenonData));
					if (HasXenonData != 0) {
						Data = s.SerializeObject<XenonData>(Data, name: nameof(Data));
					}
				}
			}

			public class XenonData : BinarySerializable {
				public uint Version { get; set; }
				public uint Flags { get; set; }

				public uint Type6_UInt { get; set; }
				public uint Type4_UInt { get; set; }
				public float Type4_Float { get; set; }
				public Jade_TextureReference Texture0 { get; set; }
				public Jade_TextureReference Texture1 { get; set; }
				public Jade_CubeMapReference CubeMap { get; set; }
				public Jade_TextureReference Texture2 { get; set; }
				public Jade_TextureReference Texture3 { get; set; }

				public float Float_00 { get; set; }
				public float Float_01 { get; set; }
				public float Float_02 { get; set; }
				public float Float_03 { get; set; }
				public float Float_04 { get; set; }
				public float Float_05 { get; set; }
				public float Float_06 { get; set; }
				public float Float_07 { get; set; }
				public float Float_08 { get; set; }
				public float Float_09 { get; set; }
				public float Float_10 { get; set; }
				public float Float_11 { get; set; }
				public float Float_12 { get; set; }

				public float Type2_Float_00 { get; set; }
				public float Type2_Float_01 { get; set; }
				public float Type2_Float_02 { get; set; }
				public float Type2_Float_03 { get; set; }
				public float Type2_Float_04 { get; set; }
				public float Type2_Float_05 { get; set; }

				public uint UInt_78 { get; set; }
				public uint Type10_UInt { get; set; }
				public float Type15_Float { get; set; }
				public byte Type2_Byte_0 { get; set; }
				public byte Type2_Byte_1 { get; set; }
				public float Type3_Float { get; set; }
				public float Type7_Float_00 { get; set; }
				public float Type7_Float_01 { get; set; }
				public float Type8_Float_00 { get; set; }
				public float Type8_Float_01 { get; set; }
				public float Type11_Float_00 { get; set; }
				public float Type11_Float_01 { get; set; }
				public float Type11_Float_02 { get; set; }
				public float Type11_Float_03 { get; set; }
				public uint Type13_UInt_00 { get; set; }
				public float Type13_Float_01 { get; set; }
				public float Type13_Float_02 { get; set; }
				public float Type13_Float_03 { get; set; }
				public float Type14_Float { get; set; }
				public float Type16_Float_00 { get; set; }
				public float Type16_Float_01 { get; set; }
				public float Type17_Float { get; set; }
				public float Type18_Float { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					Version = s.Serialize<uint>(Version, name: nameof(Version));
					if (Version == 0) return;

					Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
					if (Version >= 6) Type6_UInt = s.Serialize<uint>(Type6_UInt, name: nameof(Type6_UInt));
					if (Version >= 4) {
						Type4_UInt = s.Serialize<uint>(Type4_UInt, name: nameof(Type4_UInt));
						Type4_Float = s.Serialize<float>(Type4_Float, name: nameof(Type4_Float));
					}
					Texture0 = s.SerializeObject<Jade_TextureReference>(Texture0, name: nameof(Texture0));
					if ((Flags & 0x1000) == 0) Texture0?.Resolve();
					Texture1 = s.SerializeObject<Jade_TextureReference>(Texture1, name: nameof(Texture1));
					if ((Flags & 0x2000) == 0) Texture1?.Resolve();

					CubeMap = s.SerializeObject<Jade_CubeMapReference>(CubeMap, name: nameof(CubeMap));
					if ((Flags & 0x8000) == 0) CubeMap?.Resolve();

					if (Version >= 10) {
						Texture2 = s.SerializeObject<Jade_TextureReference>(Texture2, name: nameof(Texture2));
						if ((Flags & 0x200000) == 0) Texture2?.Resolve();
					}
					if (Version >= 2) {
						Texture3 = s.SerializeObject<Jade_TextureReference>(Texture3, name: nameof(Texture3));
						if ((Flags & 0x4000) == 0) Texture3?.Resolve();
					}

					Float_00 = s.Serialize<float>(Float_00, name: nameof(Float_00));
					Float_01 = s.Serialize<float>(Float_01, name: nameof(Float_01));
					Float_02 = s.Serialize<float>(Float_02, name: nameof(Float_02));
					Float_03 = s.Serialize<float>(Float_03, name: nameof(Float_03));
					Float_04 = s.Serialize<float>(Float_04, name: nameof(Float_04));
					Float_05 = s.Serialize<float>(Float_05, name: nameof(Float_05));
					Float_06 = s.Serialize<float>(Float_06, name: nameof(Float_06));
					Float_07 = s.Serialize<float>(Float_07, name: nameof(Float_07));
					Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
					Float_09 = s.Serialize<float>(Float_09, name: nameof(Float_09));
					Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
					Float_11 = s.Serialize<float>(Float_11, name: nameof(Float_11));
					Float_12 = s.Serialize<float>(Float_12, name: nameof(Float_12));

					if (Version >= 2) {
						Type2_Float_00 = s.Serialize<float>(Type2_Float_00, name: nameof(Type2_Float_00));
						Type2_Float_01 = s.Serialize<float>(Type2_Float_01, name: nameof(Type2_Float_01));
						Type2_Float_02 = s.Serialize<float>(Type2_Float_02, name: nameof(Type2_Float_02));
						Type2_Float_03 = s.Serialize<float>(Type2_Float_03, name: nameof(Type2_Float_03));
						if (Version <= 4) {
							Type2_Float_04 = s.Serialize<float>(Type2_Float_04, name: nameof(Type2_Float_04));
							Type2_Float_05 = s.Serialize<float>(Type2_Float_05, name: nameof(Type2_Float_05));
						}
					}
					UInt_78 = s.Serialize<uint>(UInt_78, name: nameof(UInt_78));
					if (Version >= 10) Type10_UInt = s.Serialize<uint>(Type10_UInt, name: nameof(Type10_UInt));
					if (Version >= 15) Type15_Float = s.Serialize<float>(Type15_Float, name: nameof(Type15_Float));

					if (Version >= 2) {
						Type2_Byte_0 = s.Serialize<byte>(Type2_Byte_0, name: nameof(Type2_Byte_0));
						Type2_Byte_1 = s.Serialize<byte>(Type2_Byte_1, name: nameof(Type2_Byte_1));
					}
					if (Version >= 3) Type3_Float = s.Serialize<float>(Type3_Float, name: nameof(Type3_Float));
					if (Version >= 7) {
						Type7_Float_00 = s.Serialize<float>(Type7_Float_00, name: nameof(Type7_Float_00));
						Type7_Float_01 = s.Serialize<float>(Type7_Float_01, name: nameof(Type7_Float_01));
					}
					if (Version >= 8) {
						Type8_Float_00 = s.Serialize<float>(Type8_Float_00, name: nameof(Type8_Float_00));
						Type8_Float_01 = s.Serialize<float>(Type8_Float_01, name: nameof(Type8_Float_01));
					}
					if (Version >= 11) {
						Type11_Float_00 = s.Serialize<float>(Type11_Float_00, name: nameof(Type11_Float_00));
						Type11_Float_01 = s.Serialize<float>(Type11_Float_01, name: nameof(Type11_Float_01));
						Type11_Float_02 = s.Serialize<float>(Type11_Float_02, name: nameof(Type11_Float_02));
						Type11_Float_03 = s.Serialize<float>(Type11_Float_03, name: nameof(Type11_Float_03));
					}
					if (Version >= 13) {
						Type13_UInt_00 = s.Serialize<uint>(Type13_UInt_00, name: nameof(Type13_UInt_00));
						Type13_Float_01 = s.Serialize<float>(Type13_Float_01, name: nameof(Type13_Float_01));
						Type13_Float_02 = s.Serialize<float>(Type13_Float_02, name: nameof(Type13_Float_02));
						Type13_Float_03 = s.Serialize<float>(Type13_Float_03, name: nameof(Type13_Float_03));
					}
					if (Version >= 14) Type14_Float = s.Serialize<float>(Type14_Float, name: nameof(Type14_Float));
					if (Version >= 16) {
						Type16_Float_00 = s.Serialize<float>(Type16_Float_00, name: nameof(Type16_Float_00));
						Type16_Float_01 = s.Serialize<float>(Type16_Float_01, name: nameof(Type16_Float_01));
					}
					if (Version >= 17) Type17_Float = s.Serialize<float>(Type17_Float, name: nameof(Type17_Float));
					if (Version >= 18) Type18_Float = s.Serialize<float>(Type18_Float, name: nameof(Type18_Float));
				}
			}
		}
	}
}
