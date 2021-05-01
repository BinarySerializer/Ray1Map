using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;
using Cysharp.Threading.Tasks;

namespace R1Engine.Jade {
	public class WOR_WorldList : Jade_File {
		public Jade_GenericReference[] Worlds { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Worlds = s.SerializeObjectArray<Jade_GenericReference>(Worlds, FileSize / 8, name: nameof(Worlds));
		}

		public async UniTask ResolveReferences(SerializerObject s) {
			int worldIndex = 0;
			foreach (var world in Worlds) {
				Controller.DetailedState = $"Loading world {worldIndex + 1}/{Worlds.Length}";
				bool hasLoadedWorld = Loader.LoadedWorlds.Any(w => w.Key == world.Key);
				world.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.DontUseCachedFile);
				await Loader.LoadLoopBINAsync();

				if (world?.Value != null && world.Value is WOR_World w) {
					Controller.DetailedState = $"Loading world {worldIndex+1}/{Worlds.Length}: {w.Name}";
					var gaos = w.GameObjects?.Value?.GameObjects;
					if (gaos != null) {
						foreach (var gao in gaos) {
							await JustAfterLoadObject(gao?.Value);
						}
						if ((s.GetR1Settings().EngineVersion < EngineVersion.Jade_KingKong || Loader.IsLoadingFix) && !hasLoadedWorld) {
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
						if (!hasLoadedWorld && (w.Grid0?.Value != null || w.Grid1?.Value != null)) {
							if (Loader.CurWorldForGrids != null) {
								var cw = Loader.CurWorldForGrids;
								if (cw.Grid0?.Value != null && cw.Grid0.Value != w.Grid0?.Value) cw.Grid0.Value.Unload();
								if (cw.Grid1?.Value != null && cw.Grid1.Value != w.Grid1?.Value) cw.Grid1.Value.Unload();
							}
							Loader.CurWorldForGrids = w;
						}
					}
					Loader.WorldToLoadIn = null;
				}
				worldIndex++;

			}
		}

		private async UniTask JustAfterLoadObject(OBJ_GameObject gao) {
			if(gao == null) return;
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
				if (Context.GetR1Settings().EngineVersion >= EngineVersion.Jade_RRR) {
					await Loader.LoadLoopBINAsync();
				}

				actionData.SkeletonGroup?.Resolve();
				await Loader.LoadLoopBINAsync();

				// Attach skeleton
				var skelGroup = actionData?.SkeletonGroup?.Value?.GroupObjectList?.Value?.GroupObjects;
				if (skelGroup != null) {
					foreach (var grp_obj in skelGroup) {
						var grp_gao = grp_obj.GameObject?.Value;
						if (grp_gao != null && !Loader.IsGameObjectAttached(grp_gao)) {
							await JustAfterLoadObject(grp_gao);
							// Attach each skeleton member's group members
							var extendedGroup = grp_gao.Extended?.Group?.Value?.GroupObjectList?.Value?.GroupObjects;
							if (extendedGroup != null) {
								foreach (var ext_grp_obj in extendedGroup) {
									var ext_grp_gao = ext_grp_obj.GameObject?.Value;
									if (ext_grp_gao != null && !Loader.IsGameObjectAttached(ext_grp_gao)) {
										await JustAfterLoadObject(ext_grp_gao);
									}
								}
							}
						}
					}
				}

				if (actionData.ActionKit?.Value == null) {
					actionData.ListTracks?.Resolve();
					await Loader.LoadLoopBINAsync();
				}
				Controller.DetailedState = prevState;
			}
		}
	}
}
