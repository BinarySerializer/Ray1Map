using Cysharp.Threading.Tasks;
using Ray1Map.Jade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Ray1Map.Jade_BaseManager;

namespace Ray1Map {
	public class Jade_GameActions_ExportTextures : Jade_GameActions {
		public Jade_GameActions_ExportTextures(Jade_BaseManager manager) : base(manager) { }

		public async UniTask ExportTexturesAsync(GameSettings settings, string outputDir, bool useComplexNames) {
			var parsedTexs = new HashSet<uint>();

			var levIndex = 0;
			uint currentKey = 0;

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

						TEX_GlobalList texList = context.GetStoredObject<TEX_GlobalList>(TextureListKey);

						string worldName = null;
						if (context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
							LOA_Loader loader = context.GetStoredObject<LOA_Loader>(LoaderKey);
							foreach (var w in loader.LoadedWorlds) {
								texList = w?.TextureList_Montreal;
								worldName = w?.Name;
								await ExportTexList(texList);
							}
						} else {
							await ExportTexList(texList);
						}

						async UniTask ExportTexList(TEX_GlobalList texList) {
							if (texList.Textures != null && texList.Textures.Any()) {
								Debug.Log($"Loaded level. Exporting {texList?.Textures?.Count} textures...");
								await Controller.WaitIfNecessary();

								foreach (var t in texList.Textures) {
									if (parsedTexs.Contains(t.Key.Key))
										continue;

									if (context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)
										&& t.Content == null && t.Info == null) continue;

									parsedTexs.Add(t.Key.Key);

									Texture2D tex = null;
									currentKey = t.Key;
									tex = (t.Content ?? t.Info).ToTexture2D();

									if (tex == null)
										continue;

									string name = $"{t.Key.Key:X8}";
									if (useComplexNames) {
										if (worldName != null) name = $"{worldName}/{name}";
										var file = (t.Content ?? t.Info);
										if (file != null) {
											name += $"_{file.Type}";
											if (file.Content_Xenon != null) name += $"_{file.Content_Xenon.Format}";
											if (file.Content_JTX != null) name += $"_{file.Content_JTX.Format}";
											if (file.Content_TGA != null) name += $"_{file.Format}";
										}
									}

									Util.ByteArrayToFile(Path.Combine(outputDir, $"{name}.png"), tex.EncodeToPNG());
								}
							}
							if (texList.CubeMaps != null && texList.CubeMaps.Any()) {
								foreach (var t in texList.CubeMaps) {
									if (parsedTexs.Contains(t.Key.Key))
										continue;
									parsedTexs.Add(t.Key.Key);
									var dds = t.Value.DDS;

									for (int i = 0; i < dds.Faces.Length; i++) {
										Util.ByteArrayToFile(Path.Combine(outputDir, "Cubemaps", $"{t.Key.Key:X8}_{i}.png"), dds.Faces[i].Surfaces[0].ToTexture2D().EncodeToPNG());
									}
								}
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

		public async UniTask ExportTexturesUnbinarized(GameSettings settings, string outputDir) {
			using (var context = new Ray1MapContext(settings)) {
				await LoadFilesAsync(context);

				LOA_Loader loader = await InitJadeAsync(context, initAI: false, initTextures: true, initSound: false);
				var texList = context.GetStoredObject<TEX_GlobalList>(TextureListKey);

				foreach (var kvp in loader.FileInfos) {
					var fileInfo = kvp.Value;
					if (fileInfo.FileName != null && (fileInfo.FileName.EndsWith(".tex") || fileInfo.FileName.EndsWith(".jtx"))) {
						try {
							Jade_TextureReference texRef = new Jade_TextureReference(context, fileInfo.Key);
							texRef.Resolve();
							await loader.LoadLoop(context.Deserializer);

							var t = texList.Textures[0];

							Texture2D tex = null;
							uint currentKey = t.Key;
							tex = (t.Content ?? t.Info).ToTexture2D();

							if (tex == null)
								continue;

							string name = fileInfo.FilePath;
							/*if ((t.Content ?? t.Info)?.Content_Xenon != null) {
								name += "_" + (t.Content ?? t.Info).Content_Xenon.Format.ToString();
							}*/
							Util.ByteArrayToFile(Path.Combine(outputDir, $"{name}.png"), tex.EncodeToPNG());
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
		}

	}
}
