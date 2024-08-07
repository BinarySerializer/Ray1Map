﻿using Cysharp.Threading.Tasks;
using Ray1Map.Jade;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Ray1Map.Jade_BaseManager;

namespace Ray1Map {
	public class Jade_GameActions_ExportModels: Jade_GameActions {
		public Jade_GameActions_ExportModels(Jade_BaseManager manager) : base(manager) { }

		public async UniTask ExportModelsAsync(GameSettings settings, string outputDir) {
			if (PakFiles != null && PakFiles.Any()) {
				await ExportModelsUnbinarizedPAKAsync(settings, outputDir);
				return;
			}

			var parsedTexs = new HashSet<uint>();

			var levIndex = 0;
			uint currentKey = 0;
			HashSet<uint> exportedObjects = new HashSet<uint>();
			HashSet<string> exportedObjectIDs = new HashSet<string>();
			HashSet<uint> exportedWOWs = new HashSet<uint>();

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
							if(exportedWOWs.Contains(w.Key)) continue;
							exportedWOWs.Add(w.Key);
							worldName = w?.Name;
							var gaos = w.GameObjects?.Value?.GameObjects;
							if (gaos == null) continue;
							foreach (var o in gaos) {
								if (o.IsNull || exportedObjects.Contains(o.Key.Key)) continue;
								o.Value.UnoptimizeGeometry();
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
							await FBXExporter.ExportFBXAsync(w, outputDir);
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
			if (PakFiles != null && PakFiles.Any()) {
				await ExportModelsUnbinarizedPAKAsync(settings, outputDir);
				return;
			}

			var parsedTexs = new HashSet<uint>();
			HashSet<uint> exportedObjects = new HashSet<uint>();
			HashSet<uint> exportedWOWs = new HashSet<uint>();
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
					} else if (fileInfo.FileName != null && (fileInfo.FileName.EndsWith(".wow"))) {

						try {
							Jade_Reference<WOR_World> wowRef = new Jade_Reference<WOR_World>(context, fileInfo.Key);
							wowRef.Resolve();
							await loader.LoadLoop(context.Deserializer);

							var o = wowRef;

							if (o.IsNull || exportedWOWs.Contains(o.Key.Key)) continue;
							exportedWOWs.Add(o.Key.Key);
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

		public async UniTask ExportModelsUnbinarizedPAKAsync(GameSettings settings, string outputDir) {

			var parsedTexs = new HashSet<uint>();
			HashSet<uint> exportedObjects = new HashSet<uint>();
			HashSet<uint> exportedWOWs = new HashSet<uint>();
			HashSet<string> exportedObjectIDs = new HashSet<string>();

			using (var context = new Ray1MapContext(settings)) {
				await LoadFilesAsync(context);
				LOA_Loader l = await InitJadeAsync(context, initAI: true, initTextures: true, initSound: true);

				var s = l.Context.Deserializer;
				HashSet<Jade_Key> worlds = new HashSet<Jade_Key>();
				HashSet<Jade_Key> gaos = new HashSet<Jade_Key>();
				foreach (var f in l.FileInfos) {
					var key = f.Key;
					var file = f.Value;
					var pak = file.PakFile;
					if (pak == null) continue;
					var pakFileInfo = pak.FileTable[file.FileIndex];
					if (pakFileInfo.Info.UncompressedSize < 100) continue;
					Jade_FileType FileType = null;
					string Name = null;
					await pak.SerializeFile(s, file.FileIndex, (fSize, _) => {
						var start = s.CurrentPointer;
						FileType = s.SerializeObject<Jade_FileType>(FileType, name: nameof(FileType));
						if (FileType.Type == Jade_FileType.FileType.WOR_World) {
							worlds.Add(key);
						} else if (FileType.Type == Jade_FileType.FileType.OBJ_GameObject) {
							gaos.Add(key);
						}
						s.Goto(start + fSize);
					});
				}

				var texList = context.GetStoredObject<TEX_GlobalList>(TextureListKey);

				foreach (var gaoKey in gaos) {
					try {
						Jade_Reference<OBJ_GameObject> gaoRef = new Jade_Reference<OBJ_GameObject>(context, gaoKey);
						gaoRef.Resolve();
						await l.LoadLoop(context.Deserializer);

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
				foreach (var wowKey in worlds) {
					try {
						Jade_Reference<WOR_World> wowRef = new Jade_Reference<WOR_World>(context, wowKey);
						wowRef.Resolve();
						await l.LoadLoop(context.Deserializer);

						var o = wowRef;

						if (o.IsNull || exportedWOWs.Contains(o.Key.Key)) continue;
						exportedWOWs.Add(o.Key.Key);
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
