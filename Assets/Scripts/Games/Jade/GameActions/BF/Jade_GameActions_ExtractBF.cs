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
	public class Jade_GameActions_ExtractBF : Jade_GameActions {
		public Jade_GameActions_ExtractBF(Jade_BaseManager manager) : base(manager) { }


		public async UniTask ExtractFilesAsync(GameSettings settings, string outputDir,
			bool decompressBIN = false,
			bool exportKeyList = true,
			bool exportTimeline = true,
			bool extractHidden = false,

			// Modify below for faster extraction of recent files
			bool createAllDirectories = true,
			bool onlyRecent = false,
			string compareEquality = null) {
			using (var context = new Ray1MapContext(settings)) {
				var s = context.Deserializer;
				await LoadFilesAsync(context);
				Dictionary<uint, string> fileKeys = new Dictionary<uint, string>();
				List<KeyValuePair<DateTime, string>> timelineList = new List<KeyValuePair<DateTime, string>>();
				foreach (var bfPath in BFFiles) {
					var bf = await LoadBF(context, bfPath);
					List<KeyValuePair<long, long>> fileSizes = new List<KeyValuePair<long, long>>();
					try {
						string[] directories = new string[0];
						for (int fatIndex = 0; fatIndex < bf.FatFiles.Length; fatIndex++) {
							var fat = bf.FatFiles[fatIndex];
							if (fat.DirectoryInfos?.Length > 0) {
								directories = new string[fat.DirectoryInfos.Length];
								for (int i = 0; i < directories.Length; i++) {
									var dir = fat.DirectoryInfos[i];
									var dirName = dir.Name;
									var curDir = dir;
									while (curDir.Parent != -1) {
										curDir = fat.DirectoryInfos[curDir.Parent];
										dirName = Path.Combine(curDir.Name, dirName);
									}
									directories[i] = dirName;
									if (createAllDirectories) Directory.CreateDirectory(Path.Combine(outputDir, dirName));
								}
							}
							for (int i = 0; i < fat.Files.Length; i++) {
								var f = fat.Files[i];
								var fi = fat.FileInfos[i];
								if (onlyRecent && (fi.DateLastModified < DateTime.Now.AddDays(-28)/*DateTime.Now.AddHours(-1)*/)) continue;
								bool fileIsCompressed = decompressBIN && f.IsCompressed;
								if (fileIsCompressed && fi.Name != null && !fi.Name.EndsWith(".bin")) {
									// Hack. Really whether it's compressed or not also depends on whether speed mode is enabled when loading this specific key
									fileIsCompressed = false;
								}
								if (!fileIsCompressed && fi.Name != null && fi.Name.EndsWith(".bin") && fi.Name.Contains("_wo")
									&& context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CPP)) {
									fileIsCompressed = true;
								}
								//UnityEngine.Debug.Log($"{bf.Offset.file.AbsolutePath} - {i} - {f.Key} - {(fi.Name != null ? Path.Combine(directories[fi.ParentDirectory], fi.Name) : fi.Name)}");
								byte[] fileBytes = null;
								await bf.SerializeFile(s, fatIndex, i, (fileSize, isBranch) => {
									fileSizes.Add(new KeyValuePair<long, long>(f.FileOffset.AbsoluteOffset, fileSize + 4));
									if (fileIsCompressed) {
										try {
											s.DoEncoded(new Jade_Lzo1xEncoder(fileSize, xbox360Version: settings.EngineFlags.HasFlag(EngineFlags.Jade_Xenon)), () => {
												fileBytes = s.SerializeArray<byte>(fileBytes, s.CurrentLength, name: "FileBytes");
											});
										} catch (Exception) {
											UnityEngine.Debug.LogWarning($"File with key {f.Key} is not compressed, trying uncompressed version");
											fileIsCompressed = false;
											s.Goto(f.FileOffset + 4);
											fileBytes = s.SerializeArray<byte>(fileBytes, fileSize, name: "FileBytes");
										}
									} else {
										fileBytes = s.SerializeArray<byte>(fileBytes, fileSize, name: "FileBytes");
									}
								});

								string fileName = null;
								if (!string.IsNullOrEmpty(fi.Name)) {
									fileName = fi.Name;
									fileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
									if (fileIsCompressed) {
										fileName += ".dec";
									}
									if (fi.ParentDirectory >= 0) {
										var outPath = Path.Combine(outputDir, directories[fi.ParentDirectory], fileName);

										if (compareEquality != null) {
											// Compare code
											//if (fileName.EndsWith(".grm") || fileName.EndsWith(".act")) continue;
											var comparePath = Path.Combine(compareEquality, directories[fi.ParentDirectory], fileName);
											if (File.Exists(comparePath)) {
												//continue;
												var compareBytes = File.ReadAllBytes(comparePath);
												if (compareBytes.Length == fileBytes.Length) {
													bool equal = true;
													for (int j = 0; j < compareBytes.Length; j++) {
														if (fileBytes[j] != compareBytes[j]) {
															equal = false;
															break;
														}
													}
													if (equal) continue;
												}
											} //else continue;
										}

										Util.ByteArrayToFile(outPath, fileBytes);
										File.SetLastWriteTime(outPath, fi.DateLastModified);
										if (exportKeyList) fileKeys[f.Key.Key] = Path.Combine(directories[fi.ParentDirectory], fileName);
										if (exportTimeline) timelineList.Add(new KeyValuePair<DateTime, string>(fi.DateLastModified, Path.Combine(directories[fi.ParentDirectory], fileName)));
									}
								} else {
									fileName = $"no_name_{i}_{fat.Files[i].Key:X8}.dat";
									if (fileIsCompressed) {
										fileName += ".dec";
									}
									Util.ByteArrayToFile(Path.Combine(outputDir, fileName), fileBytes);
									if (exportKeyList) fileKeys[f.Key.Key] = fileName;
								}
							}
						}
						// Extract hidden files
						if (extractHidden) {
							s.Goto(bf.Offset);
							var sortedFileSizes = fileSizes.OrderBy(f => f.Key).ToArray();
							for (int i = 0; i < sortedFileSizes.Length; i++) {
								var nextOffset = i == sortedFileSizes.Length - 1 ? s.CurrentLength : sortedFileSizes[i + 1].Key;
								var curOffset = sortedFileSizes[i].Key;
								var curSize = sortedFileSizes[i].Value;
								while (curOffset + curSize < nextOffset) {
									curOffset = curOffset + curSize;
									Pointer curPtr = bf.Offset + curOffset;
									byte[] fileBytes = null;
									Debug.Log($"Reading hidden file @ {curPtr}");
									await bf.SerializeAt(s, curPtr, (fileSize) => {
										fileBytes = s.SerializeArray<byte>(fileBytes, fileSize, name: "FileBytes");
									});
									if (fileBytes.Length == 0) {
										s.DoAt(curPtr, () => {
											fileBytes = s.SerializeArray<byte>(fileBytes, nextOffset - curOffset, name: "FileBytes");
										});
										curSize = fileBytes.Length;
									} else {
										curSize = fileBytes.Length + 4;
									}
									string fileName = $"hidden_file_{curPtr.StringFileOffset}.dat";
									Util.ByteArrayToFile(Path.Combine(outputDir, fileName), fileBytes);
								}
								if (curOffset + curSize > nextOffset) {
									UnityEngine.Debug.Log($"error @ {(bf.Offset + curOffset)}");
								}
							}
							/*{
								s.Goto(bf.Offset);
								byte[] fileBytes = File.ReadAllBytes(Path.Combine(context.BasePath, BFFile));
								foreach (var c in fileSizes) {
									for (int i = 0; i < c.Value; i++) {
										fileBytes[c.Key + i] = 0;
									}
								}
								Util.ByteArrayToFile(Path.Combine(outputDir, "unread.bf"), fileBytes);
							}*/
						}
					} catch (Exception ex) {
						UnityEngine.Debug.LogError(ex);
					}
				}
				if (exportKeyList) {
					StringBuilder b = new StringBuilder();
					foreach (var kv in fileKeys) {
						b.AppendLine($"{kv.Key:X8},{kv.Value}");
					}
					File.WriteAllText(Path.Combine(outputDir, "keylist.txt"), b.ToString());
				}
				if (exportTimeline) {
					StringBuilder b = new StringBuilder();
					timelineList.Sort((x, y) => x.Key.CompareTo(y.Key));
					foreach (var kv in timelineList) {
						b.AppendLine($"{kv.Key:ddd, dd/MM/yyyy - HH:mm:ss}\t{kv.Value}");
					}
					File.WriteAllText(Path.Combine(outputDir, "timeline.txt"), b.ToString());
				}
			}
		}

	}
}
