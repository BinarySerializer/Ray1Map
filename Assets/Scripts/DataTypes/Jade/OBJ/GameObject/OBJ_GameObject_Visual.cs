using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class OBJ_GameObject_Visual : BinarySerializable {
		public uint Version { get; set; } // Set in on PreSerialize

		public Jade_Reference<GEO_Object> GeometricObject { get; set; }
		public Jade_Reference<GEO_Object> Material { get; set; }

		// Montreal
		public DM DrawMask { get; set; }
		public byte AdditionalFlags { get; set; }
		public byte LightSetMask { get; set; }
		public sbyte DisplayOrder { get; set; }
		public byte[] Padding { get; set; }
		public uint VertexColorsCount { get; set; }
		public Jade_Color[] VertexColors { get; set; }

		public uint MontrealHasEditorData { get; set; }
		public uint Montreal_EditorData_UInt { get; set; }
		public float Montreal_EditorData_Float { get; set; }

		public uint AmbientUnknown { get; set; }
		public Jade_TextureReference AmbientTexture { get; set; }
		public uint AmbientElementCount { get; set; }
		public OBJ_GameObject_Visual_AmbientElement[] AmbientElements { get; set; }

		public Jade_Reference<OBJ_GameObject> AmbientOfGAO { get; set; }
		public Jade_Reference<OBJ_GameObject> LocalFog { get; set; }

		public short V4_Short { get; set; }
		public uint PlatformOptimizedMeshKey { get; set; }
		public uint V7_Editor_UInt_1 { get; set; }
		public uint V7_Editor_UInt_2 { get; set; }
		public uint V7_Editor_UInt_3 { get; set; }
		public uint V13_Editor_UInt_0 { get; set; }
		public uint V13_Editor_UInt_1 { get; set; }
		public uint V13_Editor_UInt_2 { get; set; }
		public GEO_GaoVisu_PS2 VisuPS2 { get; set; }

		// Montpellier
		public int Int_7A { get; set; }
		public uint UInt_7E { get; set; } // Seem to be flags of some sort
		public OBJ_GameObject_GeometricData_Xenon Xenon { get; set; }

		// RLI
		public uint Code { get; set; }
		public Jade_Reference<OBJ_GameObjectRLI> RLI { get; set; }
		public uint[] RLI_UInts { get; set; }

		public OBJ_GameObject_GeometricData_Xenon2 Xenon2 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			GeometricObject = s.SerializeObject<Jade_Reference<GEO_Object>>(GeometricObject, name: nameof(GeometricObject))?.Resolve(onPostSerialize: (_, f) => {
				if (f.RenderObject.Type != GRO_Type.GEO
				&& f.RenderObject.Type != GRO_Type.PAG
				&& f.RenderObject.Type != GRO_Type.GEO_StaticLOD
				&& f.RenderObject.Type != GRO_Type.CAM
				&& f.RenderObject.Type != GRO_Type.STR
				&& f.RenderObject.Type != GRO_Type.Unknown) {
					throw new Exception($"{f.Key}: Expected GEO, got {f.RenderObject.Type}");
				}
			});
			Material = s.SerializeObject<Jade_Reference<GEO_Object>>(Material, name: nameof(Material))?.Resolve(onPostSerialize: (_,f) => {
				if(f.RenderObject.Type != GRO_Type.MAT_MSM
				&& f.RenderObject.Type != GRO_Type.MAT_MTT
				&& f.RenderObject.Type != GRO_Type.MAT_SIN) {
					throw new Exception($"{f.Key}: Expected material, got {f.RenderObject.Type}");
				}
			});
			if (s.GetR1Settings().Jade_Version >= Jade_Version.Montreal) {
				DrawMask = s.Serialize<DM>(DrawMask, name: nameof(DrawMask));
				if (Version >= 3) AdditionalFlags = s.Serialize<byte>(AdditionalFlags, name: nameof(AdditionalFlags));
				if (Version >= 5) LightSetMask = s.Serialize<byte>(LightSetMask, name: nameof(LightSetMask));
				DisplayOrder = s.Serialize<sbyte>(DisplayOrder, name: nameof(DisplayOrder));
				Padding = s.SerializeArray<byte>(Padding, 3, name: nameof(Padding));
				VertexColorsCount = s.Serialize<uint>(VertexColorsCount, name: nameof(VertexColorsCount));
				VertexColors = s.SerializeObjectArray<Jade_Color>(VertexColors, VertexColorsCount, name: nameof(VertexColors));

				bool hasEditorData = true;
				bool hasLightMap = true;
				bool hasAmbientOfGAO = true;
				bool hasLocalFog = true;
				if (Version < 4) {
					uint drawMaskTemp = (uint)DrawMask;
					if (Version < 10) drawMaskTemp = (uint)BitHelpers.SetBits((int)drawMaskTemp, 0, 1, 9);
					if (Version < 11) drawMaskTemp = (uint)BitHelpers.SetBits((int)drawMaskTemp, 0, 1, 10);
					hasEditorData = BitHelpers.ExtractBits((int)drawMaskTemp, 1, 8) == 1;
					hasLightMap = BitHelpers.ExtractBits((int)drawMaskTemp, 1, 9) == 1;
					hasAmbientOfGAO = BitHelpers.ExtractBits((int)drawMaskTemp, 1, 10) == 1;
					hasLocalFog = BitHelpers.ExtractBits((int)drawMaskTemp, 1, 13) == 1;
					UnityEngine.Debug.LogWarning($"TODO: Check OBJ_GameObject_Visual format @ {s.CurrentPointer}");
				}
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
				if (hasEditorData && !Loader.IsBinaryData) {
					MontrealHasEditorData = s.Serialize<uint>(MontrealHasEditorData, name: nameof(MontrealHasEditorData));
					if (MontrealHasEditorData == 1) {
						Montreal_EditorData_UInt = s.Serialize<uint>(Montreal_EditorData_UInt, name: nameof(Montreal_EditorData_UInt));
						Montreal_EditorData_Float = s.Serialize<float>(Montreal_EditorData_Float, name: nameof(Montreal_EditorData_Float));
					}
				}
				if (hasLightMap) {
					AmbientUnknown = s.Serialize<uint>(AmbientUnknown, name: nameof(AmbientUnknown));
					AmbientTexture = s.SerializeObject<Jade_TextureReference>(AmbientTexture, name: nameof(AmbientTexture))?.Resolve();
					if (!AmbientTexture.IsNull) {
						AmbientElementCount = s.Serialize<uint>(AmbientElementCount, name: nameof(AmbientElementCount));
						if ((AmbientElementCount & 0xFFFF) != 0) {
							AmbientElements = s.SerializeObjectArray<OBJ_GameObject_Visual_AmbientElement>(AmbientElements, AmbientElementCount, name: nameof(AmbientElements));
						}
					}
				}
				if (hasAmbientOfGAO) {
					AmbientOfGAO = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(AmbientOfGAO, name: nameof(AmbientOfGAO))?.Resolve();
				}
				if (hasLocalFog) {
					LocalFog = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(LocalFog, name: nameof(LocalFog))?.Resolve();
				}
				if (Version >= 4 && Version < 7) {
					V4_Short = s.Serialize<short>(V4_Short, name: nameof(V4_Short));
				}
				if (Version >= 7) {
					if (!Loader.IsBinaryData) {
						PlatformOptimizedMeshKey = s.Serialize<uint>(PlatformOptimizedMeshKey, name: nameof(PlatformOptimizedMeshKey));
						V7_Editor_UInt_1 = s.Serialize<uint>(V7_Editor_UInt_1, name: nameof(V7_Editor_UInt_1));
						V7_Editor_UInt_2 = s.Serialize<uint>(V7_Editor_UInt_2, name: nameof(V7_Editor_UInt_2));
						V7_Editor_UInt_3 = s.Serialize<uint>(V7_Editor_UInt_3, name: nameof(V7_Editor_UInt_3));
					}
					if (Version >= 13) {
						if (!Loader.IsBinaryData) {
							V13_Editor_UInt_0 = s.Serialize<uint>(V13_Editor_UInt_0, name: nameof(V13_Editor_UInt_0));
							V13_Editor_UInt_1 = s.Serialize<uint>(V13_Editor_UInt_1, name: nameof(V13_Editor_UInt_1));
							V13_Editor_UInt_2 = s.Serialize<uint>(V13_Editor_UInt_2, name: nameof(V13_Editor_UInt_2));
						}
					}
					switch (s.GetR1Settings().Platform) {
						case Platform.PS2:
							VisuPS2 = s.SerializeObject<GEO_GaoVisu_PS2>(VisuPS2, name: nameof(VisuPS2));
							break;
						case Platform.Wii: break;
						default:
							UnityEngine.Debug.LogWarning($"{GetType()}: Skipping unimplemented platform {s.GetR1Settings().Platform}. In case of errors, check this");
							break;
					}
				}

			} else {
				Int_7A = s.Serialize<int>(Int_7A, name: nameof(Int_7A));
				UInt_7E = s.Serialize<uint>(UInt_7E, name: nameof(UInt_7E));

				if (s.GetR1Settings().Jade_Version == Jade_Version.Xenon) {
					Xenon = s.SerializeObject<OBJ_GameObject_GeometricData_Xenon>(Xenon, onPreSerialize: x => x.Version = Version, name: nameof(Xenon));
				}

				// RLI
				Code = s.Serialize<uint>(Code, name: nameof(Code));
				if (Code == (uint)Jade_Code.RLI) {
					RLI = s.SerializeObject<Jade_Reference<OBJ_GameObjectRLI>>(RLI, name: nameof(RLI))?.Resolve();
				} else {
					RLI_UInts = s.SerializeArray<uint>(RLI_UInts, Code, name: nameof(RLI_UInts));
				}

				if (s.GetR1Settings().Jade_Version == Jade_Version.Xenon) {
					Xenon2 = s.SerializeObject<OBJ_GameObject_GeometricData_Xenon2>(Xenon2, onPreSerialize: x => x.Version = Version, name: nameof(Xenon2));
				}
			}
		}

		[Flags]
		public enum DM : uint {
			None					= 0,
			UseTexture				= 0x00000001,
			DontForceColor			= 0x00000002,
			DontShowBV				= 0x00000004,
			Draw					= 0x00000008,
			NotWired				= 0x00000010,
			Lighted					= 0x00000020,
			UseRLI					= 0x00000040,
			MaterialColor			= 0x00000080,
			UseAmbient				= 0x00000100,
			DoNotSort				= 0x00000200,
			ComputeSpecular			= 0x00000400,
			TestBackFace			= 0x00000800,
			HidePoint				= 0x00001000,
			NotInvertBF				= 0x00002000,
			Fogged					= 0x00004000,
			Symetric				= 0x00008000,
			ReceiveDynSdw			= 0x00010000,
			ActiveSkin				= 0x00020000,
			RadioCut				= 0x00040000,
			EmitRadShadows			= 0x00080000,
			UseNormalMaterial		= 0x00100000,
			UseBVForLightRejection	= 0x00200000,
			DontUseAmbient2			= 0x00400000,
			DontRecomputeNormales	= 0x00800000,
			ZTest					= 0x01000000,
			DontScaleRLI			= 0x02000000,

			// Not in Phoenix data
			Unknown1				= 0x04000000,
			Unknown2				= 0x08000000,
			Unknown3				= 0x10000000,
			Unknown4				= 0x20000000,
			Unknown5				= 0x40000000,
			Unknown6				= 0x80000000,

			All						= 0xFFFFFFFF,
		}
	}
}
