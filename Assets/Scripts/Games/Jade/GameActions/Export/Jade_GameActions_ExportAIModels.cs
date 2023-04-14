using Cysharp.Threading.Tasks;
using Ray1Map.Jade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Ray1Map.Jade_BaseManager;

namespace Ray1Map {
	public class Jade_GameActions_ExportAIModels : Jade_GameActions {
		public Jade_GameActions_ExportAIModels(Jade_BaseManager manager) : base(manager) { }

		public async UniTask ExportAI(GameSettings settings, string outputDir, bool exportUniverse = true, bool exportModels = true, bool exportInstances = false) {

			void StringToFile(string path, string str) {
				if(str == null) return;
				var bytes = System.Text.Encoding.UTF8.GetBytes(str);
				Util.ByteArrayToFile(Path.Combine(outputDir, path), bytes);
			}
			var exportedKeys = new HashSet<uint>();
			void ExportInstance(AI_Instance instance, string gaoname, string worldname) {
				if(instance == null) return;
				if(exportedKeys.Contains(instance?.Key)) return;
				exportedKeys.Add(instance.Key);
				var modelname = instance?.Model?.Value?.Export_FileBasename ?? $"{instance.Model?.Key:X8}";
				if (exportInstances) {
					StringToFile(
						Path.Combine("AI Overview", "Instances", worldname, modelname, $"{instance.Key:X8}_{gaoname}.txt"),
						instance?.Vars?.Value?.ExportVarsOverview());
				}
				if (exportModels) {
					var model = instance.Model.Value;
					if (!exportedKeys.Contains(model.Key)) {
						exportedKeys.Add(model.Key);
						StringToFile(
							Path.Combine("AI Overview", "Models", $"{model.Key:X8}_{modelname}.txt"),
							model.Vars?.ExportVarsOverview());
						StringToFile(
							Path.Combine("AI Structs", $"{model.Key:X8}_{modelname}.txt"),
							model.Vars?.ExportStruct(modelname)); // Turn SAVE on for universe
						StringToFile(
							Path.Combine("AI Var Files", $"{model.Key:X8}_{modelname}.var"),
							model.Vars?.ExportVarFile());
						StringToFile(
							Path.Combine("AI", "Models", $"{model.Key:X8}_{modelname}.txt"),
							model.Vars?.ExportForUnbinarizeImport());

						foreach (var modelRef in model.References) {
							if(modelRef.Type == Jade_FileType.FileType.AI_ProcList) {
								var fce = modelRef.Value as AI_ProcList;
								if(fce == null) continue;
								if(exportedKeys.Contains(fce.Key)) continue;
								exportedKeys.Add(fce.Key);

								string procExport = fce?.ExportForUnbinarizeImport();
								if (procExport != null) {
									StringToFile(Path.Combine("AI", "Procs", $"{fce.Key:X8}.txt"), procExport);
								}
							}
						}
					}
				}
			}

			var levIndex = 0;
			uint currentKey = 0;

			if (exportUniverse) {
				try {
					using (var context = new Ray1MapContext(settings)) {
						currentKey = 0;
						await LoadFilesAsync(context);
						var loader = await LoadJadeAsync(context, null, LoadFlags.Universe);

						ExportInstance(loader.Universe?.Value, "univers", "Univers");
					}
				} catch (Exception ex) {
					Debug.LogError($"Failed to export universe: {ex.ToString()}");
				}
			}

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
						var loader = await LoadJadeAsync(context, new Jade_Key(context, lev.Key), LoadFlags.Maps);

						foreach (var w in loader.LoadedWorlds) {
							var gaos = w.SerializedGameObjects;
							foreach (var gao in gaos) {
								if (gao?.Extended?.AI?.Value != null) {
									var instance = gao?.Extended?.AI?.Value;
									ExportInstance(instance, gao.Export_FileBasename, w.Name);
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
			}
		}

		public async UniTask ExportAIUnbinarized(GameSettings settings, string outputDir, bool validOnly = true) {
			void StringToFile(string path, string str) {
				var bytes = System.Text.Encoding.UTF8.GetBytes(str);
				Util.ByteArrayToFile(Path.Combine(outputDir, path), bytes);
			}
			using (var context = new Ray1MapContext(settings)) {
				await LoadFilesAsync(context);

				LOA_Loader loader = await InitJadeAsync(context, initAI: true, initTextures: false, initSound: false);

				foreach (var kvp in loader.FileInfos) {
					var fileInfo = kvp.Value;
					if (fileInfo.FileName != null) {
						if(validOnly && !fileInfo.FilePath.Contains("AI Models")) continue;
						if (fileInfo.FileName.EndsWith("omd")) {
							try {
								Jade_Reference<AI_Model> omdRef = new Jade_Reference<AI_Model>(context, fileInfo.Key);
								omdRef.Resolve();
								await loader.LoadLoop(context.Deserializer);

								string modelName = Path.GetFileNameWithoutExtension(fileInfo.FileNameValidCharacters);
								uint key = omdRef.Key.Key;
								var model = omdRef.Value;

								string modelVars = model.Vars?.ExportForUnbinarizeImport();
								if (modelVars != null) {
									StringToFile(Path.Combine("AI", "Models", $"{key:X8}_{modelName}.txt"), modelVars);
								}
							} catch (Exception ex) {
								UnityEngine.Debug.LogError(ex);
							}
							await Controller.WaitIfNecessary();
						} else if (fileInfo.FileName.EndsWith("fce")) {
							try {
								Jade_Reference<AI_ProcList> fceRef = new Jade_Reference<AI_ProcList>(context, fileInfo.Key);
								fceRef.Resolve();
								await loader.LoadLoop(context.Deserializer);

								string procsName = Path.GetFileNameWithoutExtension(fileInfo.FileNameValidCharacters);
								uint key = fceRef.Key.Key;
								var procs = fceRef.Value;

								string procExport = procs?.ExportForUnbinarizeImport();
								if (procExport != null) {
									StringToFile(Path.Combine("AI", "Procs", $"{fceRef.Key.Key:X8}_{procsName}.txt"), procExport);
								}
							} catch (Exception ex) {
								UnityEngine.Debug.LogError(ex);
							}
							await Controller.WaitIfNecessary();
						}
					}
				}
			}
		}
	}
}
