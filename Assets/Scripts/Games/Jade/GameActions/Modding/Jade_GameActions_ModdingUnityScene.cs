using BinarySerializer;
using Cysharp.Threading.Tasks;
using Ray1Map.Jade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using static Ray1Map.Jade_BaseManager;

namespace Ray1Map {
	public class Jade_GameActions_ModdingUnityScene : Jade_GameActions {
		public Jade_GameActions_ModdingUnityScene(Jade_BaseManager manager) : base(manager) { }


		public async UniTask AddModdedGameObjects(GameSettings settings, string inputDir, string outputDir) {
			// Input: Keys to avoid
			// Output: folder where raw files will be saved
			var modBehaviour = GameObject.FindObjectOfType<JadeModBehaviour>();
			var customModels = GameObject.FindObjectsOfType<JadeModelImportBehaviour>();
			if (modBehaviour != null || customModels.Length > 0) {
				List<ModdedGameObject> ModdedGameObjects = new List<ModdedGameObject>();
				Jade_Reference<WOR_World> ModWorld = null;
				string ModWorldName = null;
				var moddedObjCount = modBehaviour?.gameObject.transform.childCount ?? 0;

				if (moddedObjCount > 0 || customModels.Length > 0) {
					bool saveModWorld = false;
					LOA_Loader readLoader = null;

					using (var readContext = new Ray1MapContext(settings)) {
						await LoadFilesAsync(readContext);
						readLoader = await InitJadeAsync(readContext, initAI: true, initTextures: true, initSound: true);

						for (int moddedObjIndex = 0; moddedObjIndex < moddedObjCount; moddedObjIndex++) {
							var transform = modBehaviour.gameObject.transform.GetChild(moddedObjIndex);
							var transformName = transform.gameObject.name;
							bool createPrefab = false;
							Debug.Log($"Processing GameObject: {transformName}");
							string parsedName = null;

							// Parse name to get key + desired GameObject name
							const string GaoNamePattern = @"^(?<prefab>Prefab - )?\[(?<key>........)\] (?<name>.*)$";
							Match m = Regex.Match(transformName, GaoNamePattern, RegexOptions.IgnoreCase);
							uint key = 0;
							if (m.Success) {
								string keyString = m.Groups["key"].Value;
								parsedName = m.Groups["name"].Value;
								createPrefab = m.Groups["prefab"].Success;
								UInt32.TryParse(keyString,
									System.Globalization.NumberStyles.HexNumber,
									System.Globalization.CultureInfo.CurrentCulture,
									out key);
							} else {
								Debug.Log("GameObject name regex failed!");
							}
							if (key != 0) {
								var file = new Jade_Reference<OBJ_GameObject>(readContext, new Jade_Key(readContext, key));
								file.Resolve();
								// Save transform & name & file in struct for later
								ModdedGameObjects.Add(new ModdedGameObject() {
									ReadReference = file,
									Name = parsedName,
									PrefabKey = key,
									Transform = transform,
									CreatePrefab = createPrefab
								});
								if (!createPrefab) saveModWorld = true;
							}
						}
						if (saveModWorld) {
							const string WorldNamePattern = @"^\[(?<key>........)\] (?<name>.*)$";
							Match m = Regex.Match(modBehaviour.gameObject.name, WorldNamePattern, RegexOptions.IgnoreCase);
							uint key = 0;
							if (m.Success) {
								string keyString = m.Groups["key"].Value;
								ModWorldName = m.Groups["name"].Value;
								UInt32.TryParse(keyString,
									System.Globalization.NumberStyles.HexNumber,
									System.Globalization.CultureInfo.CurrentCulture,
									out key);
							} else {
								Debug.Log("GameObject name regex failed!");
							}
							if (key != 0) {
								ModWorld = new Jade_Reference<WOR_World>(readContext, new Jade_Key(readContext, key));
								ModWorld.Resolve();
							}
						}
						await readLoader.LoadLoop(readContext.Deserializer);
					}

					if (ModdedGameObjects.Count == 0 && customModels.Length == 0) return;

					// Read keys to avoid
					HashSet<uint> keysToAvoid = new HashSet<uint>();
					if (inputDir != null) {
						DirectoryInfo dinfo = new DirectoryInfo(inputDir);
						var keyFiles = dinfo.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
						foreach (var f in keyFiles) {
							string[] lines = File.ReadAllLines(f.FullName);
							foreach (var l in lines) {
								var lineSplit = l.Split(',');
								if (lineSplit.Length < 1) continue;
								uint k;
								if (uint.TryParse(lineSplit[0], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out k)) {
									if (!keysToAvoid.Contains(k)) keysToAvoid.Add(k);
								}
							}
						}
					}
					Dictionary<uint, string> writtenFileKeys = new Dictionary<uint, string>();

					Debug.Log("Opening write context");
					// Open write context with outputDir and configure KeysToAvoid. Make sure it doesn't write files already in the BF.
					using (var writeContext = new Ray1MapContext(outputDir, settings)) {
						// Set up loader
						LOA_Loader loader = new LOA_Loader(readLoader.BigFiles, writeContext);
						loader.Raw_RelocateKeys = false; // Don't relocate keys by default. We'll determine which ones to relocate and which to keep
						loader.Raw_KeysToAvoid = keysToAvoid;
						loader.Raw_WriteFilesAlreadyInBF = false;
						uint currentUnusedKeyInstance = 0x88000000;
						uint currentUnusedKeyPrefab = 0x11000000;
						loader.Raw_CurrentUnusedKey = currentUnusedKeyInstance; // Start key will be this one
						loader.WrittenFileKeys = writtenFileKeys;

						writeContext.StoreObject<LOA_Loader>(LoaderKey, loader);
						var sndListWrite = new SND_GlobalList();
						writeContext.StoreObject<SND_GlobalList>(SoundListKey, sndListWrite);
						var texListWrite = new TEX_GlobalList();
						writeContext.StoreObject<TEX_GlobalList>(TextureListKey, texListWrite);
						var aiLinks = readLoader.Context.GetStoredObject<AI_Links>(AIKey);
						writeContext.StoreObject<AI_Links>(AIKey, aiLinks);
						// Process modded objects
						Jade_Key newKey() => new Jade_Key(writeContext, loader.Raw_GetNewKey());
						foreach (var moddedObject in ModdedGameObjects) {
							if (moddedObject.CreatePrefab) {
								loader.Raw_CurrentUnusedKey = currentUnusedKeyPrefab;
							} else {
								loader.Raw_CurrentUnusedKey = currentUnusedKeyInstance;
							}
							// Apply new transform, set name and key
							var obj = moddedObject.ReadReference.Value;
							var transform = moddedObject.Transform;
							obj.Matrix.SetTRS(
								transform.localPosition,
								transform.localRotation,
								transform.localScale,
								newType: TypeFlags.Translation | TypeFlags.Rotation | TypeFlags.Scale,
								convertAxes: true);
							obj.Name = $"{(moddedObject.CreatePrefab ? "PREFAB_" : "")}{moddedObject.Name}.gao";
							obj.NameLength = (uint)obj.Name.Length + 1;
							obj.Key = newKey();

							// Rewrite AI. Two objects with the same AI instance & same instance vars buffer cannot exist.
							if (obj.Extended?.AI?.Value != null) {
								var ai = obj.Extended.AI;
								ai.Value.Key = newKey();
								ai.Key = ai.Value.Key;
								if (ai.Value.Vars?.Value != null) {
									ai.Value.Vars.Value.Key = newKey();
									ai.Value.Vars.Key = ai.Value.Vars.Value.Key;
								}
							}
							if (!moddedObject.CreatePrefab) {
								// Same for COL Instance & COLMap
								if (obj.COL_Instance?.Value != null) {
									var cin = obj.COL_Instance;
									cin.Value.Key = newKey();
									cin.Key = cin.Value.Key;
								}
								if (obj.COL_ColMap?.Value != null) {
									var colmap = obj.COL_ColMap;
									colmap.Value.Key = newKey();
									colmap.Key = colmap.Value.Key;
								}
							}

							Jade_Reference<OBJ_GameObject> newRef = new Jade_Reference<OBJ_GameObject>(writeContext, obj.Key) {
								Value = obj
							};
							Debug.Log($"Processing Modded Object: {moddedObject.Name} [Assigned Key: {obj.Key}]");

							// Resolve this new reference with NoCache
							newRef.Resolve(flags: LOA_Loader.ReferenceFlags.DontUseCachedFile | LOA_Loader.ReferenceFlags.MustExist);
							moddedObject.WriteReference = newRef;
							await loader.LoadLoop(writeContext.Serializer);
							writeContext.Serializer.ClearWrittenObjects();
							if (moddedObject.CreatePrefab) {
								currentUnusedKeyPrefab = loader.Raw_CurrentUnusedKey;
							} else {
								currentUnusedKeyInstance = loader.Raw_CurrentUnusedKey;
							}
						}
						if (saveModWorld && ModWorld?.Value != null) {
							Debug.Log("Saving mod world");
							var w = ModWorld.Value;
							w.Name = ModWorldName;
							w.Key = newKey();
							ModWorld.Key = ModWorld.Value.Key;
							w.AmbientColor = SerializableColor.Clear;
							w.BackgroundColor = SerializableColor.Clear;
							//w.SerializedGameObjects.Clear();
							var gol = w.GameObjects.Value;
							if (gol != null) {
								w.GameObjects.Key = newKey();
								gol.Key = w.GameObjects.Key;
								var moddedGaos = ModdedGameObjects.Where(g => g.WriteReference != null && !g.CreatePrefab).ToArray();
								gol.GameObjects = new WOR_GameObjectGroup.GameObjectRef[moddedGaos.Length];
								gol.FileSize = (uint)gol.GameObjects.Length * 8;
								for (int i = 0; i < moddedGaos.Length; i++) {
									gol.GameObjects[i] = new WOR_GameObjectGroup.GameObjectRef();
									var gao = gol.GameObjects[i];
									var mod = moddedGaos[i];
									gao.Reference = mod.WriteReference;
									gao.Type = new Jade_FileType() { Extension = ".gao" };
								}
								Jade_Reference<WOR_World> newRef = new Jade_Reference<WOR_World>(writeContext, w.Key) {
									Value = w
								};
								newRef.Resolve();
								await loader.LoadLoop(writeContext.Serializer);
							}
						}

						// Export models
						foreach (var cMod in customModels) {
							var geos = cMod.GetJadeModel(loader, newKey);
							foreach (var geo in geos) {
								Jade_Reference<GEO_Object> newRef = new Jade_Reference<GEO_Object>(writeContext, geo.Key) {
									Value = geo
								};
								newRef.Resolve();
								await loader.LoadLoop(writeContext.Serializer);
							}
						}

					}

					// Save written file keys
					StringBuilder b = new StringBuilder();
					foreach (var fk in writtenFileKeys) {
						b.AppendLine($"{fk.Key:X8},{fk.Value}");
					}
					File.WriteAllText(Path.Combine(outputDir, "filekeys.txt"), b.ToString());
				}
			}
		}
	}
}
