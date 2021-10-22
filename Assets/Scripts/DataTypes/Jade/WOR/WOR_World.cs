using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;
using Cysharp.Threading.Tasks;

namespace R1Engine.Jade {
	public class WOR_World : Jade_File {
		public override bool HasHeaderBFFile => true;
		public override string Export_Extension => "wow";
		public override string Export_FileBasename => Name;

		public Jade_FileType FileType { get; set; }
		public uint Version { get; set; }
		public uint TotalGameObjectsCount { get; set; }
		public Jade_Color AmbientColor { get; set; } // a key?
		public string Name { get; set; }
		public uint UInt_AfterName { get; set; }
		public Jade_Reference<DARE_InaudibleSector> InaudibleSector { get; set; }
		public Jade_Matrix CameraPosSave { get; set; }
		public float FieldOfVision { get; set; }
		public Jade_Color BackgroundColor { get; set; }
		public Jade_Color AmbientColor2 { get; set; }
		public uint Editor_UInt_Montreal_AfterBackgroundColor_V4 { get; set; }
		public uint Editor_UInt_Montreal_AfterBackgroundColor { get; set; }
		public uint UInt_9C_Version5 { get; set; }
		public uint LODCut { get; set; }
		public byte[] Bytes_A4 { get; set; }
		public XenonStruct Xenon { get; set; }
		public Jade_Reference<GRID_WorldGrid> Grid0 { get; set; }
		public Jade_Reference<GRID_WorldGrid> Grid1 { get; set; }
		public Jade_Reference<GRID_GridGroup> AllGrids0 { get; set; }
		public Jade_Reference<GRID_GridGroup> AllGrids1 { get; set; }
		public Jade_Reference<WOR_GameObjectGroup> GameObjects { get; set; }
		public Jade_Reference<WAY_AllNetworks> AllNetworks { get; set; }
		public Jade_TextReference Text { get; set; }
		public Secto[] AllSectos { get; set; }
		public Jade_Reference<LIGHT_Rejection> LightRejection { get; set; }
		public Jade_Reference<WOR_MagmaGroup> MagmaGroup { get; set; }

		public List<OBJ_GameObject> SerializedGameObjects { get; set; } = new List<OBJ_GameObject>();
		public TEX_GlobalList TextureList_Montreal { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			FileType = s.SerializeObject<Jade_FileType>(FileType, name: nameof(FileType));
			if (FileType.Type != Jade_FileType.FileType.WOR_World)
				throw new Exception($"Parsing failed: File at {Offset} was not of type {Jade_FileType.FileType.WOR_World}");
			Loader.WorldToLoadIn = this;
			Loader.LoadedWorlds.Add(this);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			TotalGameObjectsCount = s.Serialize<uint>(TotalGameObjectsCount, name: nameof(TotalGameObjectsCount));
			if (!Loader.IsBinaryData || s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
				AmbientColor = s.SerializeObject<Jade_Color>(AmbientColor, name: nameof(AmbientColor));
			}
			Name = s.SerializeString(Name, length: 60, encoding: Jade_BaseManager.Encoding, name: nameof(Name));

			if (!Loader.IsBinaryData && s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
				UInt_AfterName = s.Serialize<uint>(UInt_AfterName, name: nameof(UInt_AfterName));
			} else if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
				InaudibleSector = s.SerializeObject<Jade_Reference<DARE_InaudibleSector>>(InaudibleSector, name: nameof(InaudibleSector))?.Resolve();
			}
			CameraPosSave = s.SerializeObject<Jade_Matrix>(CameraPosSave, name: nameof(CameraPosSave));
			FieldOfVision = s.Serialize<float>(FieldOfVision, name: nameof(FieldOfVision));
			BackgroundColor = s.SerializeObject<Jade_Color>(BackgroundColor, name: nameof(BackgroundColor));
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
				AmbientColor2 = s.SerializeObject<Jade_Color>(AmbientColor2, name: nameof(AmbientColor2));
				if (Version >= 5 && s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRR)) {
					UInt_9C_Version5 = s.Serialize<uint>(UInt_9C_Version5, name: nameof(UInt_9C_Version5));
				}
			} else {
				if (!Loader.IsBinaryData) {
					if (Version >= 4)
						Editor_UInt_Montreal_AfterBackgroundColor_V4 = s.Serialize<uint>(Editor_UInt_Montreal_AfterBackgroundColor_V4, name: nameof(Editor_UInt_Montreal_AfterBackgroundColor_V4));
					Editor_UInt_Montreal_AfterBackgroundColor = s.Serialize<uint>(Editor_UInt_Montreal_AfterBackgroundColor, name: nameof(Editor_UInt_Montreal_AfterBackgroundColor));
				}
			}
			LODCut = s.Serialize<uint>(LODCut, name: nameof(LODCut));
			if (s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon)) {
				Xenon = s.SerializeObject<XenonStruct>(Xenon, onPreSerialize: x => x.Version = Version, name: nameof(Xenon));
			} else {
				if (!Loader.IsBinaryData) Bytes_A4 = s.SerializeArray<byte>(Bytes_A4, 44, name: nameof(Bytes_A4));
			}
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
				Grid0 = s.SerializeObject<Jade_Reference<GRID_WorldGrid>>(Grid0, name: nameof(Grid0))?.Resolve();
				Grid1 = s.SerializeObject<Jade_Reference<GRID_WorldGrid>>(Grid1, name: nameof(Grid1))?.Resolve();
			} else {
				AllGrids0 = s.SerializeObject<Jade_Reference<GRID_GridGroup>>(AllGrids0, name: nameof(AllGrids0));
				AllGrids1 = s.SerializeObject<Jade_Reference<GRID_GridGroup>>(AllGrids1, name: nameof(AllGrids1));

				var grids = !AllGrids1.IsNull ? AllGrids1 : AllGrids0;
				grids?.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount);
			}
			GameObjects = s.SerializeObject<Jade_Reference<WOR_GameObjectGroup>>(GameObjects, name: nameof(GameObjects))
				?.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.DontCache);
			AllNetworks = s.SerializeObject<Jade_Reference<WAY_AllNetworks>>(AllNetworks, name: nameof(AllNetworks))?.Resolve();
			Text = s.SerializeObject<Jade_TextReference>(Text, name: nameof(Text))?.Resolve();

			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
				// Montpellier branches only
				if (Version >= 4) {
					AllSectos = s.SerializeObjectArray<Secto>(AllSectos, 64, name: nameof(AllSectos));
				}
				if (Version > 4 && s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon) && s.GetR1Settings().EngineVersion < EngineVersion.Jade_RRR) {
					LightRejection = s.SerializeObject<Jade_Reference<LIGHT_Rejection>>(LightRejection, name: nameof(LightRejection))?.Resolve();
				}
			} else if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
				if (Version >= 5) MagmaGroup = s.SerializeObject<Jade_Reference<WOR_MagmaGroup>>(MagmaGroup, name: nameof(MagmaGroup))?.Resolve();
			}
		}

		public class Secto : BinarySerializable {
			public uint Flags { get; set; }
			public byte[] RefVis { get; set; }
			public byte[] RefAct { get; set; }
			public byte[] Bytes_24 { get; set; }
			public Portal[] Portals { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
				Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
				RefVis = s.SerializeArray<byte>(RefVis, 0x10, name: nameof(RefVis));
				RefAct = s.SerializeArray<byte>(RefAct, 0x10, name: nameof(RefAct));
				if (!Loader.IsBinaryData) Bytes_24 = s.SerializeArray<byte>(Bytes_24, 0x40, name: nameof(Bytes_24));
				Portals = s.SerializeObjectArray<Portal>(Portals, 16, name: nameof(Portals));
			}

			public class Portal : BinarySerializable {
				public short Flags { get; set; }
				public byte ShareSect { get; set; }
				public byte SharePortal { get; set; }
				public Jade_Vector vA { get; set; }
				public Jade_Vector vB { get; set; }
				public Jade_Vector vC { get; set; }
				public Jade_Vector vD { get; set; }
				public byte[] Bytes_34 { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
					Flags = s.Serialize<short>(Flags, name: nameof(Flags));
					ShareSect = s.Serialize<byte>(ShareSect, name: nameof(ShareSect));
					SharePortal = s.Serialize<byte>(SharePortal, name: nameof(SharePortal));
					vA = s.SerializeObject<Jade_Vector>(vA, name: nameof(vA));
					vB = s.SerializeObject<Jade_Vector>(vB, name: nameof(vB));
					vC = s.SerializeObject<Jade_Vector>(vC, name: nameof(vC));
					vD = s.SerializeObject<Jade_Vector>(vD, name: nameof(vD));
					if (!Loader.IsBinaryData) Bytes_34 = s.SerializeArray<byte>(Bytes_34, 0x40, name: nameof(Bytes_34));
				}
			}
		}

		public class XenonStruct : BinarySerializable {
			public uint Version { get; set; } // Set in onPreSerialize

			public float Float_00 { get; set; }
			public float Float_04 { get; set; }
			public float Float_08 { get; set; }
			public int Int_0C { get; set; }
			public float Float_10 { get; set; }

			public float Float_14 { get; set; }
			public float Float_18 { get; set; }
			public float Float_1C { get; set; }
			public float Float_20 { get; set; }
			public float Float_24 { get; set; }

			public float Type6_Float_00 { get; set; }
			public float Type6_Float_04 { get; set; }
			public float Type6_Float_08 { get; set; }
			public float Type6_Float_0C { get; set; }

			public float Type7_Float_00 { get; set; }
			public float Type7_Float_04 { get; set; }

			public uint Type8_UInt_00 { get; set; } // A color? RGBA (0xFF808080)
			public uint Type8_UInt_04 { get; set; } // same

			public float Type9_Float_00 { get; set; }
			public int Type9_Int_04 { get; set; }

			public float Type10_Float_00 { get; set; }
			public float Type10_Float_04 { get; set; }

			public uint Type11_UInt_00 { get; set; }
			public float Type11_Float_04 { get; set; }
			public float Type11_Float_08 { get; set; }
			public uint Type11_UInt_0C { get; set; }
			public float Type11_Float_10 { get; set; }
			public float Type11_Float_14 { get; set; }

			public float Type12_Float_00 { get; set; }
			public float Type12_Float_04 { get; set; }
			public float Type12_Float_08 { get; set; }

			public float Type13_Float_00 { get; set; }
			public float Type13_Float_04 { get; set; }
			public float Type13_Float_08 { get; set; }
			public float Type13_Float_0C { get; set; }
			public float Type13_Float_10 { get; set; }

			public Jade_Reference<OBJ_GameObject> GameObject { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Float_00 = s.Serialize<float>(Float_00, name: nameof(Float_00));
				Float_04 = s.Serialize<float>(Float_04, name: nameof(Float_04));
				Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
				Int_0C = s.Serialize<int>(Int_0C, name: nameof(Int_0C));
				Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
				Float_14 = s.Serialize<float>(Float_14, name: nameof(Float_14));
				Float_18 = s.Serialize<float>(Float_18, name: nameof(Float_18));
				Float_1C = s.Serialize<float>(Float_1C, name: nameof(Float_1C));
				Float_20 = s.Serialize<float>(Float_20, name: nameof(Float_20));
				Float_24 = s.Serialize<float>(Float_24, name: nameof(Float_24));
				if (Version >= 6) {
					Type6_Float_00 = s.Serialize<float>(Type6_Float_00, name: nameof(Type6_Float_00));
					Type6_Float_04 = s.Serialize<float>(Type6_Float_04, name: nameof(Type6_Float_04));
					Type6_Float_08 = s.Serialize<float>(Type6_Float_08, name: nameof(Type6_Float_08));
					Type6_Float_0C = s.Serialize<float>(Type6_Float_0C, name: nameof(Type6_Float_0C));
				}
				if (Version >= 7) {
					Type7_Float_00 = s.Serialize<float>(Type7_Float_00, name: nameof(Type7_Float_00));
					Type7_Float_04 = s.Serialize<float>(Type7_Float_04, name: nameof(Type7_Float_04));
				}
				if (Version >= 8) {
					Type8_UInt_00 = s.Serialize<uint>(Type8_UInt_00, name: nameof(Type8_UInt_00));
					Type8_UInt_04 = s.Serialize<uint>(Type8_UInt_04, name: nameof(Type8_UInt_04));
				}
				if (Version >= 9) {
					Type9_Float_00 = s.Serialize<float>(Type9_Float_00, name: nameof(Type9_Float_00));
					Type9_Int_04 = s.Serialize<int>(Type9_Int_04, name: nameof(Type9_Int_04));
				}
				if (Version >= 10) {
					Type10_Float_00 = s.Serialize<float>(Type10_Float_00, name: nameof(Type10_Float_00));
					Type10_Float_04 = s.Serialize<float>(Type10_Float_04, name: nameof(Type10_Float_04));
				}
				if (Version >= 11) {
					Type11_UInt_00 = s.Serialize<uint>(Type11_UInt_00, name: nameof(Type11_UInt_00));
					Type11_Float_04 = s.Serialize<float>(Type11_Float_04, name: nameof(Type11_Float_04));
					Type11_Float_08 = s.Serialize<float>(Type11_Float_08, name: nameof(Type11_Float_08));
					Type11_UInt_0C = s.Serialize<uint>(Type11_UInt_0C, name: nameof(Type11_UInt_0C));
					Type11_Float_10 = s.Serialize<float>(Type11_Float_10, name: nameof(Type11_Float_10));
					Type11_Float_14 = s.Serialize<float>(Type11_Float_14, name: nameof(Type11_Float_14));
				}
				if (Version == 12) {
					Type12_Float_00 = s.Serialize<float>(Type12_Float_00, name: nameof(Type12_Float_00));
					Type12_Float_04 = s.Serialize<float>(Type12_Float_04, name: nameof(Type12_Float_04));
					Type12_Float_08 = s.Serialize<float>(Type12_Float_08, name: nameof(Type12_Float_08));
				}
				if (Version >= 13) {
					Type13_Float_00 = s.Serialize<float>(Type13_Float_00, name: nameof(Type13_Float_00));
					Type13_Float_04 = s.Serialize<float>(Type13_Float_04, name: nameof(Type13_Float_04));
					Type13_Float_08 = s.Serialize<float>(Type13_Float_08, name: nameof(Type13_Float_08));
					Type13_Float_0C = s.Serialize<float>(Type13_Float_0C, name: nameof(Type13_Float_0C));
					Type13_Float_10 = s.Serialize<float>(Type13_Float_10, name: nameof(Type13_Float_10));
				}
				GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
			}
		}

		public async UniTask JustAfterLoad(SerializerObject s, bool hasLoadedWorld, bool doPrefabsCheck = false) {
			var gaos = GameObjects?.Value?.GameObjects;
			if (gaos != null) {
				foreach (var gao in gaos) {
					// Hack for King Kong X360's Prefabs bin file
					if (doPrefabsCheck) {
						if (gao?.Value?.Name == "PNJ_RadoJack.gao") {
							Jade_Reference<AI_Instance> univers = new Jade_Reference<AI_Instance>(Context, Loader.BigFiles[0].UniverseKey);
							univers.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.DontUseCachedFile);
							await Loader.LoadBinOrNot(s);
						}
					}
					await JustAfterLoadObject(s, gao?.Value);
				}
				if ((!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong) || Loader.IsLoadingFix) && !hasLoadedWorld) {
					foreach (var gao in Loader.AttachedGameObjects) {
						if (gao != null) {
							// WOR_World_CheckFather
							if (gao.Base?.HierarchyData != null && gao.Base.HierarchyData.Father?.Value != null) {
								var parent = gao.Base.HierarchyData.Father;
								//UnityEngine.Debug.Log($"Has parent: {parent.Key}");
								if (!Loader.IsGameObjectAttached(parent.Value)) {
									//UnityEngine.Debug.Log($"Removing parent: {parent.Key}");
									Loader.RemoveCacheReference(parent.Key);
								}
							}
						}
					}
					foreach (var gao in Loader.AttachedGameObjects) {
						if (gao != null && gaos.Any(g => g.Key == gao.Key)) {
							// WOR_World_CheckGroup
							var grp = gao.Extended?.Group?.Value?.GroupObjectList?.Value?.GroupObjects;
							if (grp != null) {
								foreach (var grp_obj in grp) {
									var grp_gao = grp_obj.GameObject;
									if (grp_gao?.Value != null) {
										if (!Loader.IsGameObjectAttached(grp_gao.Value)) {
											Loader.RemoveCacheReference(grp_gao.Key);
										}
									}
								}
							}
						}
					}
				}
				if (!hasLoadedWorld && (Grid0?.Value != null || Grid1?.Value != null)) {
					if (Loader.CurWorldForGrids != null) {
						var cw = Loader.CurWorldForGrids;
						if (cw.Grid0?.Value != null && cw.Grid0.Value != Grid0?.Value) cw.Grid0.Value.Unload();
						if (cw.Grid1?.Value != null && cw.Grid1.Value != Grid1?.Value) cw.Grid1.Value.Unload();
					}
					Loader.CurWorldForGrids = this;
				}
			}
			Loader.WorldToLoadIn = null;
		}

		public async UniTask JustAfterLoad_Montreal(SerializerObject s, bool hasLoadedWorld) {
			var gaos = GameObjects?.Value?.GameObjects;
			if (gaos != null) {
				foreach (var gao in gaos) {
					await JustAfterLoadObject(s, gao?.Value);
				}
			}
			Loader.WorldToLoadIn = null;
		}

		private async UniTask JustAfterLoadObject(SerializerObject s, OBJ_GameObject gao) {
			if (gao == null) return;
			// Attach this
			if (!Loader.IsGameObjectAttached(gao)) Loader.AttachedGameObjects.Add(gao);
			//if (gao?.Value == null || BitHelpers.ExtractBits(gao.Value.FlagsFix, 1, 4) != 0) continue;
			//if (gao?.Value == null || gao.Value.IsInitialized) continue;
			gao.IsInitialized = true;

			// Resolve references in ANI_pst_Load
			var actionData = gao.Base?.ActionData;
			if (actionData != null) {
				string prevState = Controller.DetailedState;
				Controller.DetailedState = $"{prevState}\nLoading GameObject references: {gao.Name}";

				actionData.Shape?.Resolve();
				if (Loader.IsBinaryData && Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRR)) {
					await Loader.LoadBinOrNot(s);
				}

				actionData.SkeletonGroup?.Resolve();
				await Loader.LoadBinOrNot(s);

				// Attach skeleton
				var skelGroup = actionData?.SkeletonGroup?.Value?.GroupObjectList?.Value?.GroupObjects;
				if (skelGroup != null) {
					foreach (var grp_obj in skelGroup) {
						var grp_gao = grp_obj.GameObject?.Value;
						if (grp_gao != null && !Loader.IsGameObjectAttached(grp_gao)) {
							await JustAfterLoadObject(s, grp_gao);
							// Attach each skeleton member's group members
							var extendedGroup = grp_gao.Extended?.Group?.Value?.GroupObjectList?.Value?.GroupObjects;
							if (extendedGroup != null) {
								foreach (var ext_grp_obj in extendedGroup) {
									var ext_grp_gao = ext_grp_obj.GameObject?.Value;
									if (ext_grp_gao != null && !Loader.IsGameObjectAttached(ext_grp_gao)) {
										await JustAfterLoadObject(s, ext_grp_gao);
									}
								}
							}
						}
					}
				}

				if (actionData.ActionKit?.Value == null) {
					actionData.ListTracks?.Resolve();
					await Loader.LoadBinOrNot(s);
				}
				Controller.DetailedState = prevState;
			}
		}
	}
}
