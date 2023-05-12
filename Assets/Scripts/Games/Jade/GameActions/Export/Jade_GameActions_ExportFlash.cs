using Cysharp.Threading.Tasks;
using Ray1Map.Jade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Ray1Map.Jade_BaseManager;

namespace Ray1Map {
	public class Jade_GameActions_ExportFlash : Jade_GameActions {
		public Jade_GameActions_ExportFlash(Jade_BaseManager manager) : base(manager) { }

		public async UniTask ExportFlashPackages(GameSettings settings, string outputDir) {
			using (var context = new Ray1MapContext(settings)) {
				await LoadFilesAsync(context);
				LOA_Loader loader = await InitJadeAsync(context, initAI: false, initTextures: false, initSound: false);
				loader.LoadSingle = true;
				var flashPackages = await Jade_Montreal_BaseManager.LoadFlashPackages(context.Deserializer);
				if (flashPackages?.Files != null) {
					foreach (var flash in flashPackages.Files) {
						Util.ByteArrayToFile(Path.Combine(outputDir, $"{flash.Name}.swf"), flash.Data);
					}
				}
			}
			Debug.Log($"Finished export");
		}

		public async UniTask ExportFlashBinarized(GameSettings settings, string outputDir) {
			var parsedFla = new HashSet<uint>();

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
						var loader = await LoadJadeAsync(context, new Jade_Key(context, lev.Key), LoadFlags.Maps | LoadFlags.Textures);

						foreach (var file in loader.TotalCache) {
							if(file.Value == null) continue;
							if (file.Value is FLA_FlashMovie fla) {
								if(parsedFla.Contains(file.Key)) continue;
								parsedFla.Add(file.Key);
								Util.ByteArrayToFile(Path.Combine(outputDir, $"{file.Key.Key:X8}.swf"), fla.Data);
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

	}
}
