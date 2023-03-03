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

		public async UniTask ExportAIUnbinarized(GameSettings settings, string outputDir) {
			using (var context = new Ray1MapContext(settings)) {
				await LoadFilesAsync(context);

				LOA_Loader loader = await InitJadeAsync(context, initAI: true, initTextures: false, initSound: false);

				foreach (var kvp in loader.FileInfos) {
					var fileInfo = kvp.Value;
					if (fileInfo.FileName != null) {
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
									var bytes = System.Text.Encoding.UTF8.GetBytes(modelVars);
									Util.ByteArrayToFile(Path.Combine(outputDir, "AI", "Models", $"{key:X8}_{modelName}.txt"), bytes);
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
									var bytes = System.Text.Encoding.UTF8.GetBytes(procExport);
									Util.ByteArrayToFile(Path.Combine(outputDir, "AI", "Procs", $"{fceRef.Key.Key:X8}_{procsName}.txt"), bytes);
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
