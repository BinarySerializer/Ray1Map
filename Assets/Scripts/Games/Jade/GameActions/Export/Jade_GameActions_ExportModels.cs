using Cysharp.Threading.Tasks;
using Ray1Map.Jade;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Ray1Map.Jade_BaseManager;

namespace Ray1Map {
	public class Jade_GameActions_ExportModels: Jade_GameActions {
		public Jade_GameActions_ExportModels(Jade_BaseManager manager) : base(manager) { }

		public async UniTask ExportModelsAsync(GameSettings settings, string outputDir) {
			var parsedTexs = new HashSet<uint>();

			var levIndex = 0;
			uint currentKey = 0;
			HashSet<uint> exportedObjects = new HashSet<uint>();
			HashSet<string> exportedObjectIDs = new HashSet<string>();

			foreach (var lev in LevelInfos) {
				// If there are WOL files, there are also raw WOW files. It's better to process those one by one.
				if (lev?.Type.HasValue ?? false) {
					if (lev.Type.Value.HasFlag(LevelInfo.FileType.WOL) || lev.Type.Value.HasFlag(LevelInfo.FileType.Unbinarized)) {
						levIndex++;
						continue;
					}
				}

				Debug.Log($"Exporting for level {levIndex++ + 1}/{LevelInfos.Length}: {lev.MapName}");

				try {
					using (var context = new Ray1MapContext(settings)) {
						currentKey = 0;
						await LoadFilesAsync(context);
						await LoadJadeAsync(context, new Jade_Key(context, lev.Key), LoadFlags.Maps | LoadFlags.Textures);

						string worldName = null;

						LOA_Loader loader = context.GetStoredObject<LOA_Loader>(LoaderKey);
						foreach (var w in loader.LoadedWorlds) {
							worldName = w?.Name;
							var gaos = w.GameObjects?.Value?.GameObjects;
							if (gaos == null) continue;
							foreach (var o in gaos) {
								if (o.IsNull || exportedObjects.Contains(o.Key.Key)) continue;
								exportedObjects.Add(o.Key.Key);
								string objectID = "";
								if (o?.Value?.Base?.Visual != null) {
									var visu = o?.Value?.Base?.Visual;
									objectID += $"G{visu.GeometricObject.Key.Key:X8}-M{visu.Material.Key.Key:X8}";
								}
								if (o?.Value?.Base?.ActionData?.SkeletonGroup != null) {
									objectID += $"-S{o.Value.Base.ActionData.SkeletonGroup.Key.Key:X8}";
								}
								if (o?.Value?.Extended?.Group != null) {
									objectID += $"-Gr{o?.Value.Extended?.Group.Key.Key:X8}";
								}
								if (exportedObjectIDs.Contains(objectID)) continue;
								exportedObjectIDs.Add(objectID);
								if (o.Value != null) await FBXExporter.ExportFBXAsync(o.Value, outputDir);
							}
						}
					}
				} catch (Exception ex) {
					if (currentKey == 0) {
						Debug.LogError($"Failed to export for level {lev.MapName}: {ex.ToString()}");
					} else {
						Debug.LogError($"Failed to export for level {lev.MapName}: [{currentKey:X8}] {ex.ToString()}");
					}
				}


				// Unload textures
				await Controller.WaitIfNecessary();
				await Resources.UnloadUnusedAssets();

				GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
				GC.WaitForPendingFinalizers();
				if (settings.EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
					await UniTask.Delay(2000);
				}
			}

			Debug.Log($"Finished export");
		}
		public async UniTask ExportModelsUnbinarizedAsync(GameSettings settings, string outputDir) {

			var parsedTexs = new HashSet<uint>();
			HashSet<uint> exportedObjects = new HashSet<uint>();
			HashSet<string> exportedObjectIDs = new HashSet<string>();

			using (var context = new Ray1MapContext(settings)) {
				await LoadFilesAsync(context);
				LOA_Loader loader = await InitJadeAsync(context, initAI: true, initTextures: true, initSound: true);
				var texList = context.GetStoredObject<TEX_GlobalList>(TextureListKey);

				foreach (var kvp in loader.FileInfos) {
					var fileInfo = kvp.Value;
					if (fileInfo.FileName != null && (fileInfo.FileName.EndsWith(".gao"))) {
						try {
							Jade_Reference<OBJ_GameObject> gaoRef = new Jade_Reference<OBJ_GameObject>(context, fileInfo.Key);
							gaoRef.Resolve();
							await loader.LoadLoop(context.Deserializer);

							var o = gaoRef;

							if (o.IsNull || exportedObjects.Contains(o.Key.Key)) continue;
							exportedObjects.Add(o.Key.Key);
							string objectID = "";
							if (o?.Value?.Base?.Visual != null) {
								var visu = o?.Value?.Base?.Visual;
								objectID += $"G{visu.GeometricObject.Key.Key:X8}-M{visu.Material.Key.Key:X8}";
							}
							if (o?.Value?.Base?.ActionData?.SkeletonGroup != null) {
								objectID += $"-S{o.Value.Base.ActionData.SkeletonGroup.Key.Key:X8}";
							}
							if (o?.Value?.Extended?.Group != null) {
								objectID += $"-Gr{o?.Value.Extended?.Group.Key.Key:X8}";
							}
							if (exportedObjectIDs.Contains(objectID)) continue;
							exportedObjectIDs.Add(objectID);
							if (o.Value != null) await FBXExporter.ExportFBXAsync(o.Value, outputDir);

						} catch (Exception ex) {
							UnityEngine.Debug.LogError(ex);
						} finally {
							texList.Textures?.Clear();
							texList.Palettes?.Clear();
						}
						await Controller.WaitIfNecessary();
					}
				}
			}


			// Unload textures
			await Controller.WaitIfNecessary();
			await Resources.UnloadUnusedAssets();

			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
			GC.WaitForPendingFinalizers();
			if (settings.EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
				await UniTask.Delay(2000);
			}
			Debug.Log($"Finished export");
		}

	}
}
