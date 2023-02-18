using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using Cysharp.Threading.Tasks;

namespace Ray1Map.Jade {
	public class WOR_World : Jade_File {
		public override bool HasHeaderBFFile => true;
		public override string Export_Extension => "wow"; // "WOnderful World"
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
		public byte[] XenonDummyBytes { get; set; }
		public XenonStruct Xenon { get; set; }
		public Jade_Reference<GRID_WorldGrid> Grid0 { get; set; }
		public Jade_Reference<GRID_WorldGrid> Grid1 { get; set; }
		public Jade_Reference<GRID_GridGroup> AllGrids0 { get; set; }
		public Jade_Reference<GRID_GridGroup> AllGrids1 { get; set; }
		public Jade_Reference<WOR_GameObjectGroup> GameObjects { get; set; }
		public Jade_Reference<WAY_AllNetworks> AllNetworks { get; set; }
		public Jade_TextReference Text { get; set; }
		public WOR_Secto[] AllSectos { get; set; }
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
				if (Version >= 5 && s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRR)
					&& s.GetR1Settings().GameModeSelection != GameModeSelection.RaymanRavingRabbidsPCPrototype) {
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
				if (!Loader.IsBinaryData) XenonDummyBytes = s.SerializeArray<byte>(XenonDummyBytes, 44, name: nameof(XenonDummyBytes));
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
					AllSectos = s.SerializeObjectArray<WOR_Secto>(AllSectos, 64, name: nameof(AllSectos));
				}
				if (Version > 4 && s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon) && s.GetR1Settings().EngineVersion < EngineVersion.Jade_RRR) {
					LightRejection = s.SerializeObject<Jade_Reference<LIGHT_Rejection>>(LightRejection, name: nameof(LightRejection))?.Resolve();
				}
			} else if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
				if (Version >= 5) MagmaGroup = s.SerializeObject<Jade_Reference<WOR_MagmaGroup>>(MagmaGroup, name: nameof(MagmaGroup))?.Resolve();
			}
		}

		public class XenonStruct : BinarySerializable {
			public uint Version { get; set; } // Set in onPreSerialize

			public float GlowLuminosityMin { get; set; }
			public float GlowZNear { get; set; }
			public float GlowZFar { get; set; }
			public int GlowColor { get; set; }
			public float GlowLuminosityMax { get; set; }

			public float GlowIntensity { get; set; }
			public float GaussianStrength { get; set; }
			public float RLIScale { get; set; }
			public float RLIOffset { get; set; }
			public float MipMapLODBias { get; set; }

			public float DrySpecularBoost { get; set; }
			public float WetSpecularBoost { get; set; }
			public float RainEffectDelay { get; set; }
			public float RainEffectDryDelay { get; set; }

			public float DryDiffuseFactor { get; set; }
			public float WetDiffuseFactor { get; set; }

			public uint DiffuseColor { get; set; } // A color? RGBA (0xFF808080)
			public uint SpecularColor { get; set; } // same

			public float GodRayIntensity { get; set; }
			public int GodRayIntensityColor { get; set; }

			public float SpecularShiny { get; set; }
			public float SpecularStrength { get; set; }

			public uint MaterialLODEnable { get; set; }
			public float MaterialLODNear { get; set; }
			public float MaterialLODFar { get; set; }
			public uint MaterialLODDetailEnable { get; set; }
			public float MaterialLODDetailNear { get; set; }
			public float MaterialLODDetailFar { get; set; }

			public float Saturation { get; set; }
			public float Brightness { get; set; }
			public float ContrastDividedBy5 { get; set; }

			public float BrightnessX { get; set; }
			public float BrightnessY { get; set; }
			public float BrightnessZ { get; set; }
			public float Contrast { get; set; }

			public Jade_Reference<OBJ_GameObject> SPG2LightGameObject { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				GlowLuminosityMin = s.Serialize<float>(GlowLuminosityMin, name: nameof(GlowLuminosityMin));
				GlowZNear = s.Serialize<float>(GlowZNear, name: nameof(GlowZNear));
				GlowZFar = s.Serialize<float>(GlowZFar, name: nameof(GlowZFar));
				GlowColor = s.Serialize<int>(GlowColor, name: nameof(GlowColor));
				GlowLuminosityMax = s.Serialize<float>(GlowLuminosityMax, name: nameof(GlowLuminosityMax));
				GlowIntensity = s.Serialize<float>(GlowIntensity, name: nameof(GlowIntensity));
				GaussianStrength = s.Serialize<float>(GaussianStrength, name: nameof(GaussianStrength));
				RLIScale = s.Serialize<float>(RLIScale, name: nameof(RLIScale));
				RLIOffset = s.Serialize<float>(RLIOffset, name: nameof(RLIOffset));
				MipMapLODBias = s.Serialize<float>(MipMapLODBias, name: nameof(MipMapLODBias));
				if (Version >= 6) {
					DrySpecularBoost = s.Serialize<float>(DrySpecularBoost, name: nameof(DrySpecularBoost));
					WetSpecularBoost = s.Serialize<float>(WetSpecularBoost, name: nameof(WetSpecularBoost));
					RainEffectDelay = s.Serialize<float>(RainEffectDelay, name: nameof(RainEffectDelay));
					RainEffectDryDelay = s.Serialize<float>(RainEffectDryDelay, name: nameof(RainEffectDryDelay));
				}
				if (Version >= 7) {
					DryDiffuseFactor = s.Serialize<float>(DryDiffuseFactor, name: nameof(DryDiffuseFactor));
					WetDiffuseFactor = s.Serialize<float>(WetDiffuseFactor, name: nameof(WetDiffuseFactor));
				}
				if (Version >= 8) {
					DiffuseColor = s.Serialize<uint>(DiffuseColor, name: nameof(DiffuseColor));
					SpecularColor = s.Serialize<uint>(SpecularColor, name: nameof(SpecularColor));
				}
				if (Version >= 9) {
					GodRayIntensity = s.Serialize<float>(GodRayIntensity, name: nameof(GodRayIntensity));
					GodRayIntensityColor = s.Serialize<int>(GodRayIntensityColor, name: nameof(GodRayIntensityColor));
				}
				if (Version >= 10) {
					SpecularShiny = s.Serialize<float>(SpecularShiny, name: nameof(SpecularShiny));
					SpecularStrength = s.Serialize<float>(SpecularStrength, name: nameof(SpecularStrength));
				}
				if (Version >= 11) {
					MaterialLODEnable = s.Serialize<uint>(MaterialLODEnable, name: nameof(MaterialLODEnable));
					MaterialLODNear = s.Serialize<float>(MaterialLODNear, name: nameof(MaterialLODNear));
					MaterialLODFar = s.Serialize<float>(MaterialLODFar, name: nameof(MaterialLODFar));
					MaterialLODDetailEnable = s.Serialize<uint>(MaterialLODDetailEnable, name: nameof(MaterialLODDetailEnable));
					MaterialLODDetailNear = s.Serialize<float>(MaterialLODDetailNear, name: nameof(MaterialLODDetailNear));
					MaterialLODDetailFar = s.Serialize<float>(MaterialLODDetailFar, name: nameof(MaterialLODDetailFar));
				}
				if (Version == 12) {
					Saturation = s.Serialize<float>(Saturation, name: nameof(Saturation));
					Brightness = s.Serialize<float>(Brightness, name: nameof(Brightness));
					ContrastDividedBy5 = s.Serialize<float>(ContrastDividedBy5, name: nameof(ContrastDividedBy5));
				}
				if (Version >= 13) {
					Saturation = s.Serialize<float>(Saturation, name: nameof(Saturation));
					BrightnessX = s.Serialize<float>(BrightnessX, name: nameof(BrightnessX));
					BrightnessY = s.Serialize<float>(BrightnessY, name: nameof(BrightnessY));
					BrightnessZ = s.Serialize<float>(BrightnessZ, name: nameof(BrightnessZ));
					Contrast = s.Serialize<float>(Contrast, name: nameof(Contrast));
				}
				SPG2LightGameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(SPG2LightGameObject, name: nameof(SPG2LightGameObject))?.Resolve();
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
			//if (gao?.Value == null || gao.Value.IsObjInitialized) continue;
			gao.IsObjInitialized = true;

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
