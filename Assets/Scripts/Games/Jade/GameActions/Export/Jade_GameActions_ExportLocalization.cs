using BinarySerializer;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Ray1Map.Jade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using static Ray1Map.Jade_BaseManager;

namespace Ray1Map {
	public class Jade_GameActions_ExportLocalization : Jade_GameActions {
		public Jade_GameActions_ExportLocalization(Jade_BaseManager manager) : base(manager) { }

		public async UniTask ExportLocalizationAsync(GameSettings settings, string outputDir, bool perWorld) {
			var parsedTexs = new HashSet<uint>();

			var levIndex = 0;
			if (settings.EngineVersionTree.HasParent(EngineVersion.Jade_Montreal))
				throw new NotImplementedException($"Not yet implemented for Montreal version");

			Dictionary<int, Dictionary<string, string>> langTables = null;
			if (!perWorld) langTables = new Dictionary<int, Dictionary<string, string>>();
			HashSet<uint> textKeys = new HashSet<uint>();


			void ExportLangTable(string name) {
				var output = langTables.Select(langTable => new {
					Language = ((TEXT_Language)langTable.Key).ToString(),
					Text = langTable.Value
					/*Text = langTable.Value.Select(ltv => new {
						Key = ltv.Key,
						Value = ltv.Value
					}).ToArray()*/
				});
				string json = JsonConvert.SerializeObject(output, Formatting.Indented);
				Util.ByteArrayToFile(Path.Combine(outputDir, $"{name}.json"), System.Text.Encoding.UTF8.GetBytes(json));
			}

			foreach (var lev in LevelInfos) {
				if (lev?.Type.HasValue ?? false) {
					// If there are WOL files, there are also raw WOW files. It's better to process those one by one.
					if (lev.Type.Value.HasFlag(LevelInfo.FileType.WOL) || lev.Type.Value.HasFlag(LevelInfo.FileType.Unbinarized)) {
						levIndex++;
						continue;
					}
				}

				Debug.Log($"Exporting for level {levIndex++ + 1}/{LevelInfos.Length}: {lev.MapName}");

				try {
					using (var context = new Ray1MapContext(settings)) {
						await LoadFilesAsync(context);
						await LoadJadeAsync(context, new Jade_Key(context, lev.Key), LoadFlags.Maps | LoadFlags.TextNoSound);

						Debug.Log($"Loaded level. Exporting text...");
						LOA_Loader loader = context.GetStoredObject<LOA_Loader>(LoaderKey);
						textKeys.UnionWith(loader.TextKeys);

						foreach (var w in loader.LoadedWorlds) {
							if (perWorld) langTables = new Dictionary<int, Dictionary<string, string>>();
							var text = w?.Text;
							await ExportTextList(text, w?.Name);
						}

						async UniTask ExportTextList(Jade_TextReference text, string worldName) {
							await UniTask.CompletedTask;
							if (text.IsNull) return;
							foreach (var kv in text.Text) {
								var langID = kv.Key;
								var allText = kv.Value;
								if (!langTables.ContainsKey(langID)) langTables[langID] = new Dictionary<string, string>();
								if (allText?.Text == null) continue;
								foreach (var g in allText.Text) {
									if (g.IsNull || g.Value == null) continue;
									var group = (TEXT_TextGroup)g.Value;
									var usedRef = group?.GetUsedReference(langID);
									if (usedRef == null || usedRef.IsNull || usedRef.TextList == null) continue;
									var txl = usedRef.TextList;
									foreach (var t in txl.Text) {
										var id = t.IDString ?? $"{txl.Key}-{t.IdKey}";
										var content = t.Text;
										if (langTables[langID].ContainsKey(id) && langTables[langID][id] != content) {
											Debug.LogWarning($"Different content for same IdKey {id}: {langTables[langID][id]} - {content}");
										}
										langTables[langID][id] = content;
									}
								}
							}
							if (perWorld) ExportLangTable(worldName);
						}
					}
				} catch (Exception ex) {
					Debug.LogError($"Failed to export for level {lev.MapName}: {ex.ToString()}");
				}


				// Unload textures
				await Controller.WaitIfNecessary();
				await Resources.UnloadUnusedAssets();

				GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
				GC.WaitForPendingFinalizers();
				/*if (settings.EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
					await UniTask.Delay(500);
				}*/
			}
			if (!perWorld) ExportLangTable("localization");


			StringBuilder b = new StringBuilder();
			foreach (var tk in textKeys) {
				b.AppendLine($"{tk:X8}");
			}
			File.WriteAllText(Path.Combine(outputDir, "textkeys.txt"), b.ToString());

			Debug.Log($"Finished export");
		}
		public async UniTask ExportLocalizationUnbinarizedAsync(GameSettings settings, string outputDir) {
			HashSet<uint> textKeys = new HashSet<uint>();
			using (var context = new Ray1MapContext(settings)) {
				await LoadFilesAsync(context);
				LOA_Loader loader = await InitJadeAsync(context, initAI: false, initTextures: true, initSound: true);
				loader.TextKeys = textKeys;

				var texList = context.GetStoredObject<TEX_GlobalList>(TextureListKey);

				Dictionary<string, KeyValuePair<string, Dictionary<int, TEXT_OneText>>?[]> textGroups
					= new Dictionary<string, KeyValuePair<string, Dictionary<int, TEXT_OneText>>?[]>();

				foreach (var kvp in loader.FileInfos) {
					var fileInfo = kvp.Value;
					if (fileInfo.FileName != null && fileInfo.FileName.EndsWith(".txg")) {
						try {
							Jade_Reference<TEXT_TextGroup> textRef = new Jade_Reference<TEXT_TextGroup>(context, fileInfo.Key);
							textRef.Resolve();
							await loader.LoadLoop(context.Deserializer);

							if (textRef.Value?.Text == null || textRef.Value.Text.Length == 0) continue;

							KeyValuePair<string, Dictionary<int, TEXT_OneText>>?[] curTextGroup = null;

							for (int curLang = 0; curLang < textRef.Value.Text.Length; curLang++) {
								var text = textRef.Value.Text[curLang];
								if (text.IsNull || text.TextList.Text == null) continue;
								var txl = text.TextList;
								if (curTextGroup == null) {
									curTextGroup = new KeyValuePair<string, Dictionary<int, TEXT_OneText>>?[txl.Text.Length];
									textGroups[fileInfo.FilePath] = curTextGroup;
								}
								for (int j = 0; j < txl.Text.Length; j++) {
									if (curTextGroup[j] == null) {
										curTextGroup[j] = new KeyValuePair<string, Dictionary<int, TEXT_OneText>>(
											txl.Text[j].IDString,
											new Dictionary<int, TEXT_OneText>());
									}
									curTextGroup[j].Value.Value.Add(curLang, txl.Text[j]);
								}
							}
						} catch (Exception ex) {
							UnityEngine.Debug.LogError(ex);
						} finally {
							texList.Textures?.Clear();
							texList.Palettes?.Clear();
						}
						await Controller.WaitIfNecessary();
					}
				}


				var output = textGroups.Select(langTable => new {
					Group = langTable.Key, //((TEXT_Language)langTable.Key).ToString(),
					Text = langTable.Value.Where(v => v.HasValue).Select(v => new {
						ID = v.Value.Key,
						Comments = v.Value.Value.FirstOrDefault().Value.Comments,
						/*Text = v.Value.Value.Where(t => t.Value.Text != "[need translation]").Select(t => new {
							Language = ((TEXT_Language)t.Key).ToString(),
							Text = t.Value.Text
						}),*/
						Text = new Dictionary<string, string>(v.Value.Value
						.Where(t => t.Value.Text != "[need translation]")
						.Select(t => new KeyValuePair<string, string>(
							((TEXT_Language)t.Key).ToString(),
							t.Value.Text)
						))
					})
				});
				string json = JsonConvert.SerializeObject(output, Formatting.Indented);
				Util.ByteArrayToFile(Path.Combine(outputDir, $"localization_{settings.GameModeSelection}.json"), System.Text.Encoding.UTF8.GetBytes(json));
			}

			// Export text keys
			StringBuilder b = new StringBuilder();
			foreach (var tk in textKeys) {
				b.AppendLine($"{tk:X8}");
			}
			File.WriteAllText(Path.Combine(outputDir, "textkeys.txt"), b.ToString());

			// Unload textures
			await Controller.WaitIfNecessary();
			await Resources.UnloadUnusedAssets();

			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
			GC.WaitForPendingFinalizers();

			Debug.Log($"Finished export");
		}
	}
}
