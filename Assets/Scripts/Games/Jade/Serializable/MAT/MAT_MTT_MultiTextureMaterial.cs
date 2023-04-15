using System;
using BinarySerializer;

namespace Ray1Map.Jade {
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
			if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_SoT) || !Loader.IsBinaryData) {
				Ambiant = s.SerializeObject<Jade_Color>(Ambiant, name: nameof(Ambiant));
				Diffuse = s.SerializeObject<Jade_Color>(Diffuse, name: nameof(Diffuse));
			}
			Specular = s.SerializeObject<Jade_Color>(Specular, name: nameof(Specular));
			if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_SoT)) {
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
			if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_SoT) || !Loader.IsBinaryData) {
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
					onPreSerialize: (t, _) => {
						t.Material = this;
					}, name: nameof(Levels));
			}
		}

		public class MAT_MTT_Level : BinarySerializable {
			public MAT_MTT_MultiTextureMaterial Material { get; set; } // Set in onPreSerialize

			public short TextureID { get; set; }
			public CompressedUV ScaleSpeedPosU { get; set; } = new CompressedUV();
			public CompressedUV ScaleSpeedPosV { get; set; } = new CompressedUV();
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

			// Flags
			public MaterialFlags Flags { get; set; }
			public ColorOp ColorOperation { get; set; }
			public Blending BlendMode { get; set; }
			public UVSource Source { get; set; }
			public int AlphaTreshold { get; set; }
			public UVDynamicTransformation UVTransformation { get; set; }

			// Additional Flags
			public ushort AdditionalFlags { get; set; }
			public SFlags SpecialFlags { get; set; }
			public XYZ Axis { get; set; }
			public Matrix MatrixFrom { get; set; }
			public byte GizmoNumber { get; set; }
			public byte LocalAlpha { get; set; }


			public override void SerializeImpl(SerializerObject s) {
				TextureID = s.Serialize<short>(TextureID, name: nameof(TextureID));
				if (Material.ObjectVersion < 7) {
					s.DoBits<ushort>(b => {
						SpecialFlags = b.SerializeBits<SFlags>(SpecialFlags, 4, name: nameof(SpecialFlags));
						Axis = b.SerializeBits<XYZ>(Axis, 2, name: nameof(Axis));
						MatrixFrom = b.SerializeBits<Matrix>(MatrixFrom, 2, name: nameof(MatrixFrom));
						GizmoNumber = b.SerializeBits<byte>(GizmoNumber, 7, name: nameof(GizmoNumber));
						LocalAlpha = b.SerializeBits<byte>(LocalAlpha, 9, name: nameof(LocalAlpha));
					});
				} else {
					s.DoBits<uint>(b => {
						SpecialFlags = b.SerializeBits<SFlags>(SpecialFlags, 4, name: nameof(SpecialFlags));
						Axis = b.SerializeBits<XYZ>(Axis, 2, name: nameof(Axis));
						MatrixFrom = b.SerializeBits<Matrix>(MatrixFrom, 2, name: nameof(MatrixFrom));
						GizmoNumber = b.SerializeBits<byte>(GizmoNumber, 7, name: nameof(GizmoNumber));
						LocalAlpha = b.SerializeBits<byte>(LocalAlpha, 9, name: nameof(LocalAlpha));
						AdditionalFlags = b.SerializeBits<ushort>(AdditionalFlags, 16, name: nameof(AdditionalFlags));
					});
				}
				s.DoBits<uint>(b => {
					Flags = b.SerializeBits<MaterialFlags>(Flags, 12, name: nameof(Flags));
					ColorOperation = b.SerializeBits<ColorOp>(ColorOperation, 4, name: nameof(ColorOperation));
					BlendMode = b.SerializeBits<Blending>(BlendMode, 4, name: nameof(BlendMode));
					Source = b.SerializeBits<UVSource>(Source, 4, name: nameof(Source));
					AlphaTreshold = b.SerializeBits<int>(AlphaTreshold, 6, name: nameof(AlphaTreshold));
					UVTransformation = b.SerializeBits<UVDynamicTransformation>(UVTransformation, 2, name: nameof(UVTransformation));
				});
				//Flags = s.Serialize<MaterialFlags>(Flags, name: nameof(Flags));
				ScaleSpeedPosU = s.SerializeObject<CompressedUV>(ScaleSpeedPosU, name: nameof(ScaleSpeedPosU));
				ScaleSpeedPosV = s.SerializeObject<CompressedUV>(ScaleSpeedPosV, name: nameof(ScaleSpeedPosV));
				if (s.IsSerializerLoggerEnabled) {
					s.Log($"Rotation: {0}", UVRotation);
				}

				if (Material.ObjectVersion >= 9) {
					DispLayersCount = s.Serialize<byte>(DispLayersCount, name: nameof(DispLayersCount));
					DispOffset = s.Serialize<float>(DispOffset, name: nameof(DispOffset));
					DispOffsetU = s.Serialize<float>(DispOffsetU, name: nameof(DispOffsetU));
					DispOffsetV = s.Serialize<float>(DispOffsetV, name: nameof(DispOffsetV));
				}
				if (Material.ObjectVersion >= 3) {
					s.DoBits<uint>(b => {
						RotationSpeed = b.SerializeBits<int>(RotationSpeed, 15, name: nameof(RotationSpeed));
						IsRotationAnim = b.SerializeBits<int>(IsRotationAnim ? 1 : 0, 1, name: nameof(IsRotationAnim)) == 1;
						FrameCycle = b.SerializeBits<int>(FrameCycle, 4, name: nameof(FrameCycle));
						UnusedFlags = b.SerializeBits<int>(UnusedFlags, 4, name: nameof(UnusedFlags));
						AlphaTestValue = b.SerializeBits<int>(AlphaTestValue, 8, name: nameof(AlphaTestValue));
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

			public float UVRotation {
				get {
					int rotation = 0;
					BitHelpers.SetBits(rotation, ScaleSpeedPosU.RotationBitUpper ? 1 : 0, 1, 3);
					BitHelpers.SetBits(rotation, ScaleSpeedPosU.RotationBitLower ? 1 : 0, 1, 2);
					BitHelpers.SetBits(rotation, ScaleSpeedPosV.RotationBitUpper ? 1 : 0, 1, 1);
					BitHelpers.SetBits(rotation, ScaleSpeedPosV.RotationBitLower ? 1 : 0, 1, 0);
					return rotation / 63f;
				}
				set {
					int rotation = (int)MathF.Round(value * 63);
					ScaleSpeedPosU.RotationBitUpper = BitHelpers.ExtractBits(rotation, 1, 3) == 1;
					ScaleSpeedPosU.RotationBitLower = BitHelpers.ExtractBits(rotation, 1, 2) == 1;
					ScaleSpeedPosV.RotationBitUpper = BitHelpers.ExtractBits(rotation, 1, 1) == 1;
					ScaleSpeedPosV.RotationBitLower = BitHelpers.ExtractBits(rotation, 1, 0) == 1;
				}
			}

			// Flags
			[Flags] // From Horsez
			public enum MaterialFlags : ushort {
				TilingU              = 0b0000000000000001,
				TilingV              = 0b0000000000000010,
				BilinearFiltering    = 0b0000000000000100,
				TrilinearFiltering   = 0b0000000000001000,
				//ShiftUsingNormal_Bug = 0b0000000000001000,
				AlphaTest            = 0b0000000000010000,
				HideAlpha            = 0b0000000000100000,
				HideColor            = 0b0000000001000000,
				InvertAlpha          = 0b0000000010000000,
				WriteOnlyOnSameZ     = 0b0000000100000000, // "WConATF" in Montreal (Spree)
				NoZBuffer            = 0b0000001000000000,
				UseLocalAlpha        = 0b0000010000000000,
				ActiveLayer          = 0b0000100000000000,
				OnlyAdditionalLayer  = 0b0001000000000000,
			}
			public enum ColorOp : byte {
				Diffuse       = 0,
				Specular      = 1,
				Disable       = 2,
				RLI           = 3,
				FullLight     = 4,
				InvertDiffuse = 5,
				Diffuse2X     = 6,
				SpecularColor = 7,
				DiffuseColor  = 8,
				ConstantColor = 9,
				XenonAlphaAdd = 10,
				XenonModulateColor = 11,
			}
			public enum Blending : byte {
				Copy              = 0,
				Alpha             = 1,
				AlphaPremult      = 2,
				AlphaDest         = 3,
				AlphaDestPremult  = 4,
				Add               = 5,
				Sub               = 6,
				Glow              = 7,
				PS2ShadowSpecific = 8,
				SpecialContrast   = 9,
			}
			public enum UVSource : byte {
				Object1 = 0,
				Object2,
				Chrome,
				DF_GIZMO,
				Phong_GIZMO,
				Previous,
				Planar_GIZMO,
				FaceMap,
				FogZZ,
				WaterHole,
				Default10,
				Default11,
				Default12,
				Default13,
				Default14,
				Default15,
			}

			// Additional Flags
			[Flags]
			public enum SFlags : byte {
				UseScale     = 0b00000001,
				UseSymmetric = 0b00000010,
				UseNegative  = 0b00000100,
				DeductAlpha  = 0b00001000,
				//ShiftUsingNormal_Fix = 0b00001000,
			}
			public enum XYZ : byte {
				X = 0,
				Y = 1,
				Z = 2,
				XYZ = 3
			}
			public enum Matrix : byte {
				Object = 0,
				World = 1,
				Camera = 2,
				Gizmo = 3
			}

			public const MaterialFlags ShiftUsingNormal_Bug = MaterialFlags.TrilinearFiltering;
			public const SFlags ShiftUsingNormal_Fix = SFlags.DeductAlpha;

			protected override void OnChangeContext(Context oldContext, Context newContext) {
				base.OnChangeContext(oldContext, newContext);

				bool old_isFurSupported = oldContext.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong);
				bool new_isFurSupported = newContext.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong);

				if (old_isFurSupported || new_isFurSupported) {
					bool old_hasFurBug = oldContext.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong) && oldContext.GetR1Settings().EngineVersion != EngineVersion.Jade_RRRPrototype;
					bool new_hasFurBug = newContext.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong) && newContext.GetR1Settings().EngineVersion != EngineVersion.Jade_RRRPrototype;

					if (!new_isFurSupported) {
						if (!old_hasFurBug) {
							SpecialFlags &= ~ShiftUsingNormal_Fix; // Remove fix
						}
						// If we come from a mode with the fur flag issue, don't remove it, as we may want to keep trilinear rendering
					} else {
						if (new_hasFurBug) {
							if (!old_isFurSupported) {
								// Pre-fur materials should not have the fur flag
								Flags &= ~ShiftUsingNormal_Bug;
							} else if (!old_hasFurBug) {
								// Fur was supported and fur bug did not exist
								if (SpecialFlags.HasFlag(ShiftUsingNormal_Fix)) {
									SpecialFlags &= ~ShiftUsingNormal_Fix; // Remove fix
									Flags |= ShiftUsingNormal_Bug; // Add bug
								}
							}
						} else if (old_hasFurBug) {
							if (Flags.HasFlag(ShiftUsingNormal_Bug))
								SpecialFlags |= ShiftUsingNormal_Fix;
							// Keep bug flag intact in case trilinear filtering is still desired
						}

					}
				}
			}

			public class CompressedUV : BinarySerializable, ISerializerShortLog {
				public float Scale {
					get {
						if (!RotationBitLower && !RotationBitUpper) {
							if (ScaleBits == 0 && SpeedBits == 0 && !RotationBitUpper) // 0 value is treated as identity
								return 1f;
							if(SpeedBits == 0x800 && ScaleBits == 0x400) // Old identity value
								return 1f;
						}
						return BitConverter.Int32BitsToSingle(ScaleBits << 17);
					}
					set {
						ScaleBits = (short)(BitConverter.SingleToInt32Bits(value) >> 17);
					}
				}
				public float Speed {
					get => BitConverter.Int32BitsToSingle(SpeedBits << 17);
					set {
						SpeedBits = (short)(BitConverter.SingleToInt32Bits(value) >> 17);
					}
				}
				public bool RotationBitUpper { get; set; }
				public bool RotationBitLower { get; set; }
				public short SpeedBits { get; set; }
				public short ScaleBits { get; set; }

				public override string ToString() => $"CompressedUV(Position/Speed: {Speed}, Scale: {Scale})";
				public string ShortLog => ToString();

				public override void SerializeImpl(SerializerObject s) {
					s.DoBits<uint>(b => {
						RotationBitLower = b.SerializeBits<bool>(RotationBitLower, 1, name: nameof(RotationBitLower));
						SpeedBits = b.SerializeBits<short>(SpeedBits, 15, name: nameof(SpeedBits));
						RotationBitUpper = b.SerializeBits<bool>(RotationBitUpper, 1, name: nameof(RotationBitUpper));
						ScaleBits = b.SerializeBits<short>(ScaleBits, 15, name: nameof(ScaleBits));
					});
				}
			}

			[Flags]
			public enum UVDynamicTransformation : byte {
				None = 0,
				U = 1 << 0,
				V = 1 << 1
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
