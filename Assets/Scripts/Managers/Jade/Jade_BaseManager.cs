using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R1Engine.Jade;
using System.IO;
using UnityEngine;

namespace R1Engine {
	public class Jade_BaseManager : IGameManager {
		public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
		{
			new GameAction("Extract BF file(s)", false, true, (input, output) => ExtractFilesAsync(settings, output, false)),
			new GameAction("Extract BF file(s) - BIN decompression", false, true, (input, output) => ExtractFilesAsync(settings, output, true)),
		};


        public async UniTask ExtractFilesAsync(GameSettings settings, string outputDir, bool decompressBIN = false) {
            using (var context = new Context(settings)) {
				var s = context.Deserializer;
                await LoadFilesAsync(context);
				foreach (var bfPath in BFFiles) {
					var bf = await LoadBF(context, bfPath);
					List<KeyValuePair<long, long>> fileSizes = new List<KeyValuePair<long, long>>();
					try {
						for (int fatIndex = 0; fatIndex < bf.FatFiles.Length; fatIndex++) {
							var fat = bf.FatFiles[fatIndex];
							string[] directories = new string[fat.DirectoryInfos.Length];
							for (int i = 0; i < directories.Length; i++) {
								var dir = fat.DirectoryInfos[i];
								var dirName = dir.Name;
								var curDir = dir;
								while (curDir.ParentDirectory != -1) {
									curDir = fat.DirectoryInfos[curDir.ParentDirectory];
									dirName = Path.Combine(curDir.Name, dirName);
								}
								directories[i] = Path.Combine(outputDir, dirName);
								Directory.CreateDirectory(directories[i]);
							}
							for (int i = 0; i < fat.Files.Length; i++) {
								var f = fat.Files[i];
								var fi = fat.FileInfos[i];
								bool fileIsCompressed = decompressBIN && f.IsCompressed;
								if (fileIsCompressed && fi.Name != null && !fi.Name.EndsWith(".bin")) {
									// Hack. Really whether it's compressed or not also depends on whether speed mode is enabled when loading this specific key
									fileIsCompressed = false;
								}
								//UnityEngine.Debug.Log($"{bf.Offset.file.AbsolutePath} - {i} - {f.Key} - {(fi.Name != null ? Path.Combine(directories[fi.ParentDirectory], fi.Name) : fi.Name)}");
								byte[] fileBytes = null;
								await bf.SerializeFile(s, fatIndex, i, (fileSize) => {
									fileSizes.Add(new KeyValuePair<long, long>(f.FileOffset.AbsoluteOffset, fileSize + 4));
									if (fileIsCompressed) {
										s.DoEncoded(new Jade_Lzo1xEncoder(fileSize, xbox360Version: settings.EngineVersion == EngineVersion.Jade_RRR_Xbox360), () => {
											fileBytes = s.SerializeArray<byte>(fileBytes, s.CurrentLength, name: "FileBytes");
										});
									} else {
										fileBytes = s.SerializeArray<byte>(fileBytes, fileSize, name: "FileBytes");
									}
								});
								string fileName = null;
								if (fi.Name != null) {
									fileName = fi.Name;
									if (fileIsCompressed) {
										fileName += ".dec";
									}
									Util.ByteArrayToFile(Path.Combine(directories[fi.ParentDirectory], fileName), fileBytes);
								} else {
									fileName = $"no_name_{fat.Files[i].Key:X8}.dat";
									if (fileIsCompressed) {
										fileName += ".dec";
									}
									Util.ByteArrayToFile(Path.Combine(outputDir, fileName), fileBytes);
								}
							}
						}
						{
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
									UnityEngine.Debug.Log($"error @ {(bf.Offset+curOffset)}");
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
            }
        }

        public virtual GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
		{
			new GameInfo_World(0, Enumerable.Range(0, 1).ToArray()),
		});

		public virtual string[] BFFiles => new string[] {
			"Rayman4.bf"
		};

		public async UniTask<BIG_BigFile> LoadBF(Context context, string bfPath) {
			var s = context.Deserializer;
			s.Goto(context.GetFile(bfPath).StartPointer);
			await s.FillCacheForRead((int)BIG_BigFile.HeaderLength);
			var bfFile = FileFactory.Read<BIG_BigFile>(bfPath, context);
			await s.FillCacheForRead((int)bfFile.TotalFatFilesLength);
			bfFile.SerializeFatFiles(s);
			return bfFile;
		}

		public async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures) {
			List<BIG_BigFile> bfs = new List<BIG_BigFile>();
			foreach (var bfPath in BFFiles) {
				var bf = await LoadBF(context, bfPath);
				bfs.Add(bf);
			}
			// Set up loader
			LOA_Loader loader = new LOA_Loader(bfs.ToArray());
			context.StoreObject<LOA_Loader>("loader", loader);
			// Set up AI types
			AI_Links aiLinks = AI_Links.GetAILinks(context.Settings);
			context.StoreObject<AI_Links>("ai", aiLinks);

			// Load univers
			Controller.DetailedState = $"Loading universe";
			await Controller.WaitIfNecessary();

			Jade_Reference<AI_Instance> Univers = new Jade_Reference<AI_Instance>(context, bfs[0].UniversKey);
			Univers.Resolve();
			await loader.LoadLoop(context.Deserializer); // First resolve universe

			// Load world
			Controller.DetailedState = $"Loading world";
			await Controller.WaitIfNecessary();

			var worldKey = (Jade_Key)0x01261c; // Just a test. Allow selection later

			Jade_Reference<WOR_WorldList> WorldList = new Jade_Reference<WOR_WorldList>(context, worldKey);
			WorldList.Resolve(queue: LOA_Loader.QueueType.Maps);

			loader.BeginSpeedMode(worldKey.GetBinary(Jade_Key.KeyType.Map));
			await loader.LoadLoop(context.Deserializer);
			loader.EndSpeedMode();
			throw new NotImplementedException();
		}

		public async UniTask LoadFilesAsync(Context context) {
			foreach (var bfPath in BFFiles) {
				await context.AddLinearSerializedFileAsync(bfPath, bigFileCacheLength: 8);
			}
		}

		public async UniTask SaveLevelAsync(Context context, Unity_Level level) {
			await Task.CompletedTask;
			throw new NotImplementedException();
		}

		public static readonly Encoding Encoding = Encoding.GetEncoding(1252);
	}
}
