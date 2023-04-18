using BinarySerializer;
using Cysharp.Threading.Tasks;
using Ray1Map.Jade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using static Ray1Map.Jade_BaseManager;

namespace Ray1Map {
	public class Jade_GameActions_ConvertHorsez : Jade_GameActions {
		public Jade_GameActions_ConvertHorsez(Jade_BaseManager manager) : base(manager) { }

		public async UniTask FixHorsezAI(GameSettings settings, string inputDir, string outputDir) {

			Dictionary<uint, string> fileKeys = new Dictionary<uint, string>();
			using (var context = new Ray1MapContext(settings)) {
				await LoadFilesAsync(context);
				LOA_Loader loader = await InitJadeAsync(context, initAI: true, initTextures: true, initSound: true);

				loader.LoadSingle = true;

				List<Jade_Key> assignedKeys = new List<Jade_Key>();
				List<uint> movedDirectories = new List<uint>();

				string FilenameWithoutExtension(string str) {
					if (str.Contains("."))
						return str.Substring(0, str.LastIndexOf("."));
					return str;
				}
				string Extension(string str) {
					if (str.Contains("."))
						return str.Substring(str.LastIndexOf("."));
					return "";
				}


				// First: extract all AI files with the correct keys
				foreach (var bf in loader.BigFiles) {

					var s = context.Deserializer;
					try {
						BIG_FatFile.DirectoryInfo[] directoryInfos = new BIG_FatFile.DirectoryInfo[0];
						BIG_FatFile.FileReference[] fileRefs = new BIG_FatFile.FileReference[0];
						BIG_FatFile.FileInfo[] fileInfos = new BIG_FatFile.FileInfo[0];
						int aiModelsDirIndex = 0;
						int globalLibraryDirIndex = 0;

						// Create global lists combining all fats
						// Files
						fileRefs = bf.FatFiles.SelectMany(f => f.Files).ToArray();
						fileInfos = bf.FatFiles.SelectMany(f => f.FileInfos).ToArray();

						// Directories
						// Assume there are no directory infos aside from the first fat
						directoryInfos = bf.FatFiles[0].DirectoryInfos; //.SelectMany(f => f.DirectoryInfos ?? new BIG_FatFile.DirectoryInfo[0]).ToArray();
						{
							var fat = bf.FatFiles[0];
							if (fat.DirectoryInfos?.Length > 0) {
								aiModelsDirIndex = fat.DirectoryInfos.FindItemIndex(d => d.Name == "AI Models");
								globalLibraryDirIndex = fat.DirectoryInfos.FindItemIndex(d => d.Name == "Global Library");
								for (uint i = 0; i < fat.DirectoryInfos.Length; i++) {
									var dir = fat.DirectoryInfos[i];
									if (dir.Parent == -1 && dir.Name != "ROOT") {
										switch (dir.Name) {
											case "Lib":
											case "Triggers":
											case "common":
												dir.Parent = globalLibraryDirIndex;
												break;
											default:
												dir.Parent = aiModelsDirIndex;
												break;
										}
										movedDirectories.Add(i);
									}
								}
							}
						}

						// First, create mdl.omd mapping
						Dictionary<string, Tuple<int, Jade_GenericReference[]>> mdls = new Dictionary<string, Tuple<int, Jade_GenericReference[]>>();
						Dictionary<string, Tuple<int, Jade_GenericReference[]>> omds = new Dictionary<string, Tuple<int, Jade_GenericReference[]>>();
						Dictionary<string, uint> mdlVarKey = new Dictionary<string, uint>();

						for (int i = 0; i < fileRefs.Length; i++) {
							var f = fileRefs[i];
							var fi = fileInfos[i];
							string fileName = null;
							if (!string.IsNullOrEmpty(fi.Name)) {
								fileName = fi.Name;
								var baseName = FilenameWithoutExtension(fileName);
								var extension = Extension(fileName);
								switch (extension) {
									case ".mdl":
									case ".omd":
										if (extension == ".mdl") {
											// Find directory and correctly parent it
											var subdir = movedDirectories.FindItemIndex(d => directoryInfos[d].Name == baseName);
											if (subdir != -1) {
												directoryInfos[movedDirectories[subdir]].Parent = fi.ParentDirectory;
											}
										}
										// Now read this file
										Jade_GenericReference[] jaderefs = null;
										int fat = bf.FatFiles.FindItemIndex(bfat => i >= bfat.FirstIndex && i <= bfat.LastIndex);
										var indexInFat = i - bf.FatFiles[fat].FirstIndex;

										await bf.SerializeFile(s, fat, (int)indexInFat, (fileSize, isBranch) => {
											jaderefs = s.SerializeObjectArray<Jade_GenericReference>(jaderefs, fileSize / 8, name: "JadeRefs");
										});
										if (extension == ".mdl") {
											mdls[baseName] = new Tuple<int, Jade_GenericReference[]>(i, jaderefs);
										} else {
											omds[baseName] = new Tuple<int, Jade_GenericReference[]>(i, jaderefs);
										}
										break;
								}
							}
						}
						/*foreach (var mdlPair in mdls) {
							var mdlName = mdlPair.Key;
							var omd = omds[mdlName];
							var mdl = mdlPair.Value;
							var mdlRefs = mdl.Item2;
							var omdRefs = omd.Item2;
							var omdParent = fileInfos[omd.Item1].ParentDirectory;
							int mdli = 0;
							int firstVar = -1;
							List<string> extraVars = new List<string>();
							for (int i = 0; i < omdRefs.Length; i++, mdli++) {
								var omdKey = omdRefs[i].Key;
								var mdlKey = mdlRefs[mdli].Key;
								var omdExtension = omdRefs[i].FileType.Extension;
								var mdlExtension = mdlRefs[mdli].FileType.Extension;

								// Find filename associated to omd key
								var omdFilename = loader.FileInfos[omdKey].FileName;
								string wantedMdlExtension = omdExtension switch {
									".ova" => ".var",
									".ofc" => ".fct",
									".fce" => ".fcl",
									_ => throw new Exception($"Unhandled extension ${omdExtension}"),
								};
								while (wantedMdlExtension != mdlExtension) {
									if (mdlExtension == ".var" && firstVar != -1) {
										int extraVarIndex = fileInfos.FindItemIndex(fi => fi.Name.EndsWith(".var") && fi.ParentDirectory == omdParent && !extraVars.Contains(fi.Name));
										if (extraVarIndex != -1) {
											extraVars.Add(fileInfos[extraVarIndex].Name);
											fileRefs[extraVarIndex].Key = mdlKey;
											assignedKeys.Add(mdlKey);
										} else {
											context.SystemLogger?.LogWarning($"Could not find extra var file for {omdFilename}: {mdlKey}");
										}
									}
									mdli++;
									mdlExtension = mdlRefs[mdli].FileType.Extension;
									mdlKey = mdlRefs[mdli].Key;
								}

								var filename = $"{FilenameWithoutExtension(omdFilename)}{mdlExtension}";
								int mdlFileIndex = -1;
								if (mdlExtension == ".var") {
									if (firstVar == -1) {
										firstVar = mdli;
										mdlFileIndex = fileInfos.FindItemIndex(fi => fi.Name == filename);

										if (mdlFileIndex == -1) {
											filename = $"{FilenameWithoutExtension(omdFilename)}_vars{mdlExtension}";
											mdlFileIndex = fileInfos.FindItemIndex(fi => fi.Name == filename);
										}
										if (mdlFileIndex == -1) {
											filename = $"_{FilenameWithoutExtension(omdFilename)}{mdlExtension}";
											mdlFileIndex = fileInfos.FindItemIndex(fi => fi.Name == filename);
										}
										if (mdlFileIndex == -1 && omdFilename.StartsWith("GST_")) {
											filename = $"{FilenameWithoutExtension(omdFilename).Substring(4)}{mdlExtension}";
											mdlFileIndex = fileInfos.FindItemIndex(fi => fi.Name == filename);
										}

										if (mdlFileIndex != -1) {
											extraVars.Add(fileInfos[mdlFileIndex].Name);
											mdlVarKey[mdlName] = fileRefs[mdlFileIndex].Key;
										} else {
											// Revert to it being an extra var
											context.SystemLogger?.LogWarning($"Could not find filename: {filename}");
										}
									} else {
										context.SystemLogger?.LogWarning($"Unexpected VAR in {omdFilename}: {mdlKey}");
									}
								} else {
									mdlFileIndex = fileInfos.FindItemIndex(fi => fi.Name == filename);
								}
								if (mdlFileIndex != -1) {
									fileRefs[mdlFileIndex].Key = mdlKey;
									assignedKeys.Add(mdlKey);
								} else {
									context.SystemLogger?.LogWarning($"Could not find filename: {filename}");
								}
							}
						}*/
						loader.ReinitFileDictionaries();

						// After this, assign keys to models too (check key before var and go back)
						/*foreach (var mdlPair in mdls) {
							var mdl = mdlPair.Value;
							var mdlName = mdlPair.Key;
							if (mdlVarKey.ContainsKey(mdlName)) {
								var k = mdlVarKey[mdlName];
								for (int i = 0; i < 5; i++) {
									var key = new Jade_Key(context, (uint)(k - 1 - i));
									if (!loader.FileInfos.ContainsKey(key)) {
										fileRefs[mdl.Item1].Key = key;
										assignedKeys.Add(key);
										break;
									}
								}
							}
						}
						loader.ReinitFileDictionaries();*/

						// After that, assign keys to all files that don't have them yet except the .pp files, don't need those
						const uint invalidKey = 0xFFFFFFFF;
						/*var custom = 0xA1000000;
						foreach (var fat in bf.FatFiles) {
							foreach (var fi in fat.Files) {
								if (fi.Key.Key == invalidKey) {
									var newk = new Jade_Key(context, custom);
									while (loader.FileInfos.ContainsKey(newk)) {
										custom++;
										newk = new Jade_Key(context, custom);
									}
									fi.Key = newk;
									assignedKeys.Add(newk);
									custom++;
								}
							}
						}*/
						foreach (var fat in bf.FatFiles) {
							for (int i = 0; i < fat.Files.Length; i++) {
								var fi = fat.Files[i];
								if (fi.Key.Key == invalidKey) {
									s.DoAt(fat.FileInfos[i].Offset + 5 * 4 + 0x3A, () => {
										var newk = s.SerializeObject<Jade_Key>(default, name: "newk");
										if (newk.Key == invalidKey || newk.Key == 0) {
											Debug.LogWarning("invalid key");
										} else {
											fi.Key = newk;
											assignedKeys.Add(newk);
										}
									});
								}
							}
						}
						loader.ReinitFileDictionaries();

						foreach (var k in assignedKeys) {
							var fi = loader.FileInfos[k];
							var fileOffset = fi.FileOffset;
							byte[] fileBytes = null;
							await bf.SerializeAt(s, fileOffset, (fileSize) => {
								fileBytes = s.SerializeArray<byte>(fileBytes, fileSize, name: "FileBytes");
							});
							string fileName = null;
							if (!string.IsNullOrEmpty(fi.FileName)) {
								fileName = fi.FilePathValidCharacters;
								var outPath = Path.Combine(outputDir, "files/", fileName);
								Util.ByteArrayToFile(outPath, fileBytes);
								File.SetLastWriteTime(outPath, fi.FatFileInfo.DateLastModified);
								fileKeys[fi.Key] = fileName;
							}
						}
						// Finally, all files in moved directories
						foreach (var fiPair in loader.FileInfos) {
							if (assignedKeys.Contains(fiPair.Key)) continue;
							var fi = fiPair.Value;
							if (fi.DirectoryName.Contains("/AI Models")) {

								var fileOffset = fi.FileOffset;
								byte[] fileBytes = null;
								await bf.SerializeAt(s, fileOffset, (fileSize) => {
									fileBytes = s.SerializeArray<byte>(fileBytes, fileSize, name: "FileBytes");
								});
								string fileName = null;
								if (!string.IsNullOrEmpty(fi.FileName)) {
									fileName = fi.FilePathValidCharacters;
									var outPath = Path.Combine(outputDir, "files/", fileName);
									Util.ByteArrayToFile(outPath, fileBytes);
									File.SetLastWriteTime(outPath, fi.FatFileInfo.DateLastModified);
									fileKeys[fi.Key] = fileName;
								}
							}
						}
					} catch (Exception ex) {
						UnityEngine.Debug.LogError(ex);
					}
				}
			}

			StringBuilder b = new StringBuilder();
			foreach (var kv in fileKeys) {
				b.AppendLine($"{kv.Key:X8},{kv.Value}");
			}
			File.WriteAllText(Path.Combine(outputDir, "filekeys.txt"), b.ToString());

			// Unload textures
			await Controller.WaitIfNecessary();
			await Resources.UnloadUnusedAssets();

			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
			GC.WaitForPendingFinalizers();

			Debug.Log($"Finished export");
		}

		public async UniTask ConvertPhoenixToRRRProto(GameSettings settings, string inputDir, string outputDir) {
			using (var context = new Ray1MapContext(settings)) {
				await LoadFilesAsync(context);
				LOA_Loader loader = await InitJadeAsync(context, initAI: true, initTextures: true, initSound: true);

				loader.LoadSingle = true;


				var newSettings = new GameSettings(GameModeSelection.RaymanRavingRabbidsPCPrototype, settings.GameDirectory, settings.World, settings.Level);

				using (var writeContext = new Ray1MapContext(outputDir, newSettings)) {
					// Set up loader
					LOA_Loader writeloader = new LOA_Loader(loader.BigFiles, writeContext) {
						Raw_WriteFilesAlreadyInBF = true,
						Raw_UseOriginalFileNames = true,
						LoadSingle = true
					};
					writeContext.StoreObject<LOA_Loader>(LoaderKey, writeloader);

					// Set up texture list
					TEX_GlobalList texList2 = new TEX_GlobalList();
					writeContext.StoreObject<TEX_GlobalList>(TextureListKey, texList2);

					// Set up sound list
					SND_GlobalList sndList2 = new SND_GlobalList();
					writeContext.StoreObject<SND_GlobalList>(SoundListKey, sndList2);

					// Set up AI types
					AI_Links aiLinks2 = AI_Links.GetAILinks(writeContext.GetR1Settings());
					writeContext.StoreObject<AI_Links>(AIKey, aiLinks2);

					foreach (var kvp in loader.FileInfos) {
						var fileInfo = kvp.Value;
						if (fileInfo.FileName != null) {
							bool readFile = false;

							async UniTask<T> ReadWriteFile<T>() where T : Jade_File, new() {
								var fileRef = new Jade_Reference<T>(context, fileInfo.Key);
								fileRef.Resolve();
								await loader.LoadLoop(context.Deserializer);

								var writeRef = new Jade_Reference<T>(writeContext, fileRef.Key) {
									Value = fileRef.Value
								};
								writeRef.Resolve();

								readFile = true;

								return fileRef.Value;
							}

							if (fileInfo.FileName.EndsWith("gao")) {
								var gao = await ReadWriteFile<OBJ_GameObject>();

								if (gao?.Extended?.Modifiers != null) {
									gao.Extended.Modifiers = gao.Extended.Modifiers
										.Where(m =>
											m.Type != MDF_ModifierType.Modifier56 && m.Type != MDF_ModifierType.Modifier57 && m.Type != MDF_ModifierType.GAO_ModifierCharacterFX)
										.ToArray();
									if (gao.Extended.Modifiers.Length == 0
										|| (gao.Extended.Modifiers.Length == 1 && gao.Extended.Modifiers[0].Type == MDF_ModifierType.None)) gao.Extended.HasModifiers = 0;
								}
							} else if (fileInfo.FileName.EndsWith("wow")) {
								await ReadWriteFile<WOR_World>();
							} else if (fileInfo.FileName.EndsWith("trl")) {
								await ReadWriteFile<EVE_ListTracks>();
							} else if (fileInfo.FileName.EndsWith("gro")) {
								await ReadWriteFile<GEO_Object>();
							}
							if (readFile) {
								var s = writeContext.Serializer;
								await writeloader.LoadLoop(s);
							}
						}
					}
				}
			}

			// Unload textures
			await Controller.WaitIfNecessary();
			await Resources.UnloadUnusedAssets();

			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
			GC.WaitForPendingFinalizers();

			Debug.Log($"Finished export");
		}
	}
}
