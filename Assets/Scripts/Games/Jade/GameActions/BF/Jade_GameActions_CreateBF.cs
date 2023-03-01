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
	public class Jade_GameActions_CreateBF : Jade_GameActions {
		public Jade_GameActions_CreateBF(Jade_BaseManager manager) : base(manager) { }


		public async UniTask CreateBFAsync(GameSettings settings, string inputDir, string outputDir,
			bool keepAllUnbinarizedFiles = true,
			bool optimized = false) {
			BIG_BigFile originalBF = null;
			LOA_Loader originalLoader = null;
			BIG_BigFile.FileInfoForCreate[] FilesToPack = null;
			BIG_BigFile.DirectoryInfoForCreate[] DirectoriesToPack = null;
			using (var readContext = new Ray1MapContext(settings)) {
				await LoadFilesAsync(readContext);
				originalLoader = await InitJadeAsync(readContext, initAI: false);
				originalBF = originalLoader.BigFiles[0];

				Dictionary<string, List<LOA_Loader.FileInfo>> fileInfos = null;
				Dictionary<string, uint> moddedFileInfos = new Dictionary<string, uint>();
				Dictionary<string, Tuple<string, string>> modPathReplace = new Dictionary<string, Tuple<string, string>>();
				List<LOA_Loader.FileInfo> GetJadeFileByPath(string path) {
					if (fileInfos == null) {
						fileInfos = new Dictionary<string, List<LOA_Loader.FileInfo>>();
						foreach (var fi in originalLoader.FileInfos) {
							var fp = fi.Value.FilePathValidCharacters;
							if (!fileInfos.ContainsKey(fp)) fileInfos[fp] = new List<LOA_Loader.FileInfo>();
							fileInfos[fp].Add(fi.Value);
						}
					}
					if (fileInfos.TryGetValue(path, out List<LOA_Loader.FileInfo> val)) {
						return val;
					} else
						return null;
				}
				// Read file keys (unbinarized)
				string keyListPath = Path.Combine(inputDir, "original/filekeys.txt");
				Dictionary<uint, string> fileKeysUnbinarized = new Dictionary<uint, string>();

				if (File.Exists(keyListPath)) {
					string[] lines = File.ReadAllLines(keyListPath);
					foreach (var l in lines) {
						var lineSplit = l.Split(',');
						if (lineSplit.Length != 2) continue;
						uint k;
						if (uint.TryParse(lineSplit[0], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out k)) {
							var path = lineSplit[1].Replace('\\', '/');
							var absolutePath = Path.Combine(inputDir, $"original/files/{path}");
							if (File.Exists(absolutePath)) {
								fileKeysUnbinarized[k] = path;
							}
						}
					}
				}

				// Get file keys (existing in BF)
				Dictionary<uint, string> fileKeysExisting = new Dictionary<uint, string>();
				if (keepAllUnbinarizedFiles) {
					foreach (var fi in originalLoader.FileInfos) {
						if (fi.Key.Type == Jade_Key.KeyType.Unknown && !fi.Key.IsNull) {
							fileKeysExisting.Add(fi.Key.Key, fi.Value.FilePath);
							if (fileKeysUnbinarized.ContainsKey(fi.Key.Key)) fileKeysUnbinarized.Remove(fi.Key.Key);
						}
					}
				}

				// Read file keys (modded)
				List<KeyValuePair<string, Dictionary<uint, string>>> mods = new List<KeyValuePair<string, Dictionary<uint, string>>>();
				var modDirectory = Path.Combine(inputDir, "mod");
				foreach (var modDir in Directory.GetDirectories(modDirectory).OrderBy(dirName => dirName)) {
					readContext?.SystemLogger?.LogInfo($"Loading mod: {new DirectoryInfo(modDir).Name}");
					string configPath = Path.Combine(modDir, "config.txt");
					bool overwrite = true;
					bool useKeysFromBFFilenameOnly = false;
					bool ignoreNonexistentFiles = false;
					if (File.Exists(configPath)) {
						string[] lines = File.ReadAllLines(configPath);
						foreach (var l in lines) {
							var trimmed = l.Trim();
							if (trimmed.Contains("=")) {
								var configElement = trimmed.Split("=");
								var variable = configElement[0];
								var value = configElement[1];
								switch (variable) {
									case "overwrite":
										if (value == "false") overwrite = false;
										break;
									case "use_keys_from_bf_filename_only":
										if (value == "true") useKeysFromBFFilenameOnly = true;
										break;
									case "ignore_nonexistent_files":
										if (value == "true") ignoreNonexistentFiles = true;
										break;
									case "replacepath":
										modPathReplace[modDir] = new Tuple<string, string>(configElement[1], configElement[2]);
										break;
								}
							}
						}
					}
					string modKeyListPath = Path.Combine(modDir, "filekeys.txt");
					if (File.Exists(modKeyListPath)) {
						var mod = new KeyValuePair<string, Dictionary<uint, string>>(modDir, new Dictionary<uint, string>());
						mods.Add(mod);
						string[] lines = File.ReadAllLines(modKeyListPath);
						foreach (var l in lines) {
							var lineSplit = l.Split(',');
							if (lineSplit.Length != 2) continue;
							uint k;
							if (uint.TryParse(lineSplit[0], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out k)) {
								var path = lineSplit[1].Replace('\\', '/');
								var absolutePath = Path.Combine(modDir, $"files/{path}");
								if (File.Exists(absolutePath)) {
									bool isNewModOnlyFile = true;
									bool addModFile = true;
									if (fileKeysUnbinarized.ContainsKey(k)) {
										isNewModOnlyFile = false;
										if (overwrite) fileKeysUnbinarized.Remove(k);
										else addModFile = false;
									}
									if (fileKeysExisting.ContainsKey(k)) {
										isNewModOnlyFile = false;
										if (overwrite) fileKeysExisting.Remove(k);
										else addModFile = false;
									}
									foreach (var otherMod in mods) {
										if (otherMod.Key != mod.Key && otherMod.Value.ContainsKey(k)) {
											isNewModOnlyFile = false;
											if (overwrite) {
												readContext?.SystemLogger?.LogInfo($"Overwriting previously loaded mod file: {path}");
												otherMod.Value.Remove(k);
											} else addModFile = false;
										}
									}
									if (addModFile) mod.Value[k] = path;
									if (isNewModOnlyFile)
										moddedFileInfos[path] = k;
								} else if (!ignoreNonexistentFiles) {
									readContext?.SystemLogger?.LogInfo($"File does not exist in mod directory: {path}");
								}
							}
						}
					} else if (useKeysFromBFFilenameOnly) {
						var mod = new KeyValuePair<string, Dictionary<uint, string>>(modDir, new Dictionary<uint, string>());
						mods.Add(mod);
						var modFilesDir = Path.Combine(modDir, $"files");
						foreach (string file in Directory.EnumerateFiles(modFilesDir, "*", SearchOption.AllDirectories)) {
							string relPath = file.Substring(modFilesDir.Length + 1).Replace('\\', '/');
							var fil = GetJadeFileByPath(relPath);
							if (fil != null) {
								foreach (var fi in fil) {
									uint k = fi.Key;
									bool addModFile = true;
									if (fileKeysUnbinarized.ContainsKey(k)) {
										if (overwrite) fileKeysUnbinarized.Remove(k);
										else addModFile = false;
									}
									if (fileKeysExisting.ContainsKey(k)) {
										if (overwrite) fileKeysExisting.Remove(k);
										else addModFile = false;
									} else {
										foreach (var otherMod in mods) {
											if (otherMod.Key != mod.Key && otherMod.Value.ContainsKey(k)) {
												if (overwrite) {
													readContext?.SystemLogger?.LogInfo($"Overwriting previously loaded mod file: {relPath}");
													otherMod.Value.Remove(k);
												} else addModFile = false;
											}
										}
									}
									if (addModFile) mod.Value[k] = relPath;
								}
							} else if (moddedFileInfos.ContainsKey(relPath)) {
								var k = moddedFileInfos[relPath];
								bool addModFile = true;
								foreach (var otherMod in mods) {
									if (otherMod.Key != mod.Key && otherMod.Value.ContainsKey(k)) {
										if (overwrite) {
											readContext?.SystemLogger?.LogInfo($"Overwriting previously loaded mod file: {relPath}");
											otherMod.Value.Remove(k);
										} else addModFile = false;
									}
								}
								if (addModFile) mod.Value[k] = relPath;
							} else if (!ignoreNonexistentFiles) {
								readContext?.SystemLogger?.LogInfo($"Could not find matching file in BF: {relPath}");
							}
						}
					}
				}
				readContext?.SystemLogger?.LogInfo($"Finished loading mods");

				// Read all files
				FilesToPack =
					fileKeysUnbinarized.Select(fk => new BIG_BigFile.FileInfoForCreate() {
						FullPath = fk.Value ?? $"{fk.Key:X8}",
						Key = new Jade_Key(readContext, fk.Key),
						Source = BIG_BigFile.FileInfoForCreate.FileSource.Unbinarized,
						DirectoryIndex = -1,
					}).Concat(
					fileKeysExisting.Select(fk => new BIG_BigFile.FileInfoForCreate() {
						FullPath = fk.Value ?? $"{fk.Key:X8}",
						Key = new Jade_Key(readContext, fk.Key),
						Source = BIG_BigFile.FileInfoForCreate.FileSource.Existing,
						DirectoryIndex = -1,
					})).ToArray();

				foreach (var mod in mods) {
					FilesToPack = FilesToPack.Concat(mod.Value.Select(fk => new BIG_BigFile.FileInfoForCreate() {
						FullPath = modPathReplace.ContainsKey(mod.Key)
							? (fk.Value ?? $"{fk.Key:X8}")
								.Replace(modPathReplace[mod.Key].Item1, modPathReplace[mod.Key].Item2) // Replace path here
							: (fk.Value ?? $"{fk.Key:X8}"),
						FullPathBeforeReplace = fk.Value ?? $"{fk.Key:X8}",
						Key = new Jade_Key(readContext, fk.Key),
						Source = BIG_BigFile.FileInfoForCreate.FileSource.Mod,
						ModDirectory = mod.Key,
						DirectoryIndex = -1,
					})
					).ToArray();
				}
				FilesToPack = FilesToPack.OrderBy(f => f.Key.Key).ToArray();

				// Create directories
				List<BIG_BigFile.DirectoryInfoForCreate> directories = new List<BIG_BigFile.DirectoryInfoForCreate>();
				int curDirectoriesCount = 0;
				int curFileToPackIndex = 0;
				foreach (var fileToPack in FilesToPack) {
					var filePath = fileToPack.FullPath;

					int AddOrFindDirectoryRecursive(string dirPath) {
						if (string.IsNullOrEmpty(dirPath)) return -1;
						var indexInList = directories.FindItemIndex(d => d.FullDirectoryString == dirPath);

						if (indexInList == -1) {
							BIG_BigFile.DirectoryInfoForCreate createdDirectory = null;
							if (dirPath.Contains('/')) {
								var lastDirIndex = dirPath.LastIndexOf('/');
								var parentPath = dirPath.Substring(0, lastDirIndex);
								var curDirName = dirPath.Substring(lastDirIndex + 1);
								int parentIndex = AddOrFindDirectoryRecursive(parentPath);

								createdDirectory = new BIG_BigFile.DirectoryInfoForCreate() {
									DirectoryName = curDirName,
									FullDirectoryString = dirPath,
									ParentIndex = parentIndex,
									DirectoryIndex = curDirectoriesCount
								};
								if (parentIndex != -1) {
									var parent = directories[parentIndex];
									parent.SubDirectories.Add(createdDirectory);
								}
							} else {
								createdDirectory = new BIG_BigFile.DirectoryInfoForCreate() {
									DirectoryName = dirPath,
									FullDirectoryString = dirPath,
									ParentIndex = -1,
									DirectoryIndex = curDirectoriesCount
								};
							}
							directories.Add(createdDirectory);
							indexInList = createdDirectory.DirectoryIndex;
							curDirectoriesCount++;
						}
						return indexInList;
					}
					fileToPack.DirectoryIndex = AddOrFindDirectoryRecursive(filePath.Substring(0, filePath.LastIndexOf('/')));

					if (!string.IsNullOrEmpty(filePath) && filePath.Contains('/')) {
						var lastDirIndex = filePath.LastIndexOf('/');
						var directoryString = filePath.Substring(0, lastDirIndex);
						var filenameString = filePath.Substring(lastDirIndex + 1);
						var dirIndex = AddOrFindDirectoryRecursive(directoryString);
						fileToPack.DirectoryIndex = dirIndex;
						fileToPack.Filename = filenameString;
					} else {
						fileToPack.Filename = filePath;
					}
					if (fileToPack.DirectoryIndex != -1) {
						var dir = directories[fileToPack.DirectoryIndex];
						dir.Files.Add(fileToPack);
					}
					fileToPack.FileIndex = curFileToPackIndex;
					curFileToPackIndex++;
				}
				DirectoriesToPack = directories.ToArray();

				// Create next/previous lists for files & directories
				foreach (var dir in DirectoriesToPack) {
					for (int i = 0; i < dir.SubDirectories.Count; i++) {
						if (i > 0) dir.SubDirectories[i].PreviousDirectoryID = dir.SubDirectories[i - 1].DirectoryIndex;
						if (i < dir.SubDirectories.Count - 1) dir.SubDirectories[i].NextDirectoryID = dir.SubDirectories[i + 1].DirectoryIndex;
					}
					if (dir.SubDirectories.Any())
						dir.FirstDirectoryID = dir.SubDirectories[0].DirectoryIndex;

					for (int i = 0; i < dir.Files.Count; i++) {
						if (i > 0) dir.Files[i].PreviousFileInDirectoryIndex = dir.Files[i - 1].FileIndex;
						if (i < dir.Files.Count - 1) dir.Files[i].NextFileInDirectoryIndex = dir.Files[i + 1].FileIndex;
					}
					if (dir.Files.Any())
						dir.FirstFileID = dir.Files[0].FileIndex;
				}

				foreach (var file in FilesToPack) {
					switch (file.Source) {
						case BIG_BigFile.FileInfoForCreate.FileSource.Unbinarized:
							file.Bytes = File.ReadAllBytes(Path.Combine(inputDir, $"original/files/{file.FullPath}"));
							file.DateLastModified = File.GetLastWriteTime(Path.Combine(inputDir, $"original/files/{file.FullPath}"));
							file.P4Revision = 1;
							break;
						case BIG_BigFile.FileInfoForCreate.FileSource.Mod:
							file.Bytes = File.ReadAllBytes(Path.Combine(file.ModDirectory, $"files/{file.FullPathBeforeReplace}"));
							file.DateLastModified = DateTime.Now;
							file.P4Revision = 1;
							if (originalLoader.FileInfos.ContainsKey(file.Key)) {
								file.P4Revision = originalLoader.FileInfos[file.Key].FatFileInfo.P4RevisionClient + 1;
							}
							break;
						case BIG_BigFile.FileInfoForCreate.FileSource.Existing:
							var s = readContext.Deserializer;
							file.DateLastModified = originalLoader.FileInfos[file.Key].FatFileInfo.DateLastModified;
							file.P4Revision = originalLoader.FileInfos[file.Key].FatFileInfo.P4RevisionClient;
							var reference = new Jade_Reference<Jade_ByteArrayFile>(readContext, file.Key);
							reference.Resolve(flags: LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.DontUseCachedFile);
							await originalLoader.LoadLoop(s);
							file.Bytes = reference.Value?.Bytes ?? new byte[0];

							/*if (file.Bytes.Length >= 0x3966C0 - 2
								&& file.Bytes[0] == 0x52 && file.Bytes[1] == 0x49 && file.Bytes[2] == 0x46 && file.Bytes[3] == 0x46 // RIFF
								&& file.Bytes[8] == 0x57 && file.Bytes[9] == 0x41 && file.Bytes[10] == 0x56 && file.Bytes[11] == 0x45) { // WAVE

								// TODO: Keep the music files intact, but just replace them now
								if (loader.FileInfos[file.Key].FileName != null) {
									var extension = Path.GetExtension(loader.FileInfos[file.Key].FileName);
									var smaller = loader.FileInfos.FirstOrDefault(smollerfile => smollerfile.Value.FileName != null
									&& Path.GetExtension(smollerfile.Value.FileName) == extension
									&& smollerfile.Value.FatFileInfo.FileSize > 8
									&& smollerfile.Value.FatFileInfo.FileSize < 0x3966C0 - 0x1000);
									reference = new Jade_Reference<Jade_ByteArrayFile>(readContext, smaller.Key);
									reference.Resolve();
									await loader.LoadLoop(s);
									file.Bytes = reference.Value.Bytes;
								}
							}*/

							/*if (file.Bytes.Length >= 12
								&& file.Bytes[0] == 0x52 && file.Bytes[1] == 0x49 && file.Bytes[2] == 0x46 && file.Bytes[3] == 0x46 // RIFF
								&& file.Bytes[8] == 0x57 && file.Bytes[9] == 0x41 && file.Bytes[10] == 0x56 && file.Bytes[11] == 0x45) { // WAVE
								var actualSize = file.Bytes[4] | file.Bytes[5] << 8 | file.Bytes[6] << 16 | file.Bytes[7] << 24;
								var bytes = file.Bytes;
								Array.Resize<byte>(ref bytes, actualSize + 8);
								file.Bytes = bytes;
							}*/

							/*if (loader.FileInfos.ContainsKey(file.Key)) {
								if (loader.FileInfos[file.Key].FatFileInfo != null && loader.FileInfos[file.Key].FatFileInfo.Name != null) {
									var fileSize = loader.FileInfos[file.Key].FatFileInfo.FileSize;
									if (fileSize != 0 && file.Bytes.Length > fileSize) {
										var bytes = file.Bytes;
										Array.Resize<byte>(ref bytes, (int)fileSize);
										file.Bytes = bytes;
									}

								}
							}*/
							break;
					}
					file.FileSize = (uint)file.Bytes.Length;
				}
			}

			using (Context writeContext = new Ray1MapContext(outputDir, settings)) {
				writeContext?.SystemLogger?.LogInfo($"Creating bigfile...");
				var targetFilePath = "out.bf";
				var bfFile = new LinearFile(writeContext, targetFilePath, Endian.Little);
				writeContext.AddFile(bfFile);

				BIG_BigFile newBF = BIG_BigFile.Create(originalBF, bfFile.StartPointer, FilesToPack, DirectoriesToPack, paddingBetweenFiles: (optimized ? (uint)0 : 0x100), writeFilenameInPadding: !optimized, increaseSizeOfFat: !optimized);
				var s = writeContext.Serializer;
				s.Goto(bfFile.StartPointer);
				newBF = s.SerializeObject<BIG_BigFile>(newBF, name: nameof(newBF));
				newBF.SerializeFatFiles(s);

				foreach (var file in FilesToPack) {
					s.Goto(file.Offset);
					s.Serialize<uint>(file.FileSize, name: nameof(file.FileSize));
					s.SerializeArray<byte>(file.Bytes, file.Bytes.Length, name: nameof(file.Bytes));
					if (!optimized) {
						s.Goto(file.NameOffset);
						s.SerializeString($"#{file.Filename}#", encoding: Jade_BaseManager.Encoding, name: nameof(file.Filename));
					}
				}
				writeContext?.SystemLogger?.LogInfo($"Finished creating bigfile");
			}
		}

	}
}
