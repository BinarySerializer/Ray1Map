using Cysharp.Threading.Tasks;
using Ray1Map.Jade;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Ray1Map.Jade_BaseManager;

namespace Ray1Map {
	public class Jade_GameActions_ExportSounds : Jade_GameActions {
		public Jade_GameActions_ExportSounds(Jade_BaseManager manager) : base(manager) { }

		public async UniTask ExportSoundsAsync(GameSettings settings, string outputDir, bool useComplexNames) {
			var parsedSounds = new HashSet<uint>();

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
						await LoadJadeAsync(context, new Jade_Key(context, lev.Key), LoadFlags.Maps | LoadFlags.TextNoSound | LoadFlags.TextSound | LoadFlags.Sounds);

						SND_GlobalList sndList = context.GetStoredObject<SND_GlobalList>(SoundListKey);

						//string worldName = null;
						if (context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
							throw new NotImplementedException("Not yet implemented for Montreal");
						} else {
							await ExportSndList(sndList);
						}

						async UniTask ExportSndList(SND_GlobalList sndList) {
							if (sndList.Waves != null && sndList.Waves.Any()) {
								Debug.Log($"Loaded level. Exporting {sndList?.Waves?.Count} sounds...");
								await Controller.WaitIfNecessary();

								LOA_Loader actualLoader = context.GetStoredObject<LOA_Loader>(LoaderKey);

								using (var writeContext = new Ray1MapContext(outputDir, settings)) {
									// Set up loader
									LOA_Loader loader = new LOA_Loader(actualLoader.BigFiles, writeContext);
									writeContext.StoreObject<LOA_Loader>(LoaderKey, loader);
									var sndListWrite = new SND_GlobalList();
									writeContext.StoreObject<SND_GlobalList>(SoundListKey, sndListWrite);

									foreach (var t in sndList.Waves) {
										if (parsedSounds.Contains(t.Key.Key))
											continue;

										parsedSounds.Add(t.Key.Key);

										Jade_Reference<SND_Wave> wave = new Jade_Reference<SND_Wave>(writeContext, t.Key) {
											Value = t
										};
										wave.Resolve();
									}
									var s = writeContext.Serializer;
									await loader.LoadLoop(s);
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

	}
}
