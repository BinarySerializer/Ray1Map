using Cysharp.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R1Engine.Jade;
using System.IO;
using BinarySerializer;
using UnityEngine;

namespace R1Engine 
{
	public abstract class Jade_BaseManager : IGameManager 
    {
		// Levels
        public virtual GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(LevelInfos?.GroupBy(x => x.WorldName).Select((x, i) =>
        {
            return new GameInfo_World(
                index: i, 
                worldName: x.Key.ReplaceFirst(CommonLevelBasePath, String.Empty),
                maps: x.Select(m => (int) m.Key).ToArray(),
                mapNames: x.Select(m => m.MapName).ToArray());
        }).ToArray() ?? new GameInfo_World[0]);
		public abstract LevelInfo[] LevelInfos { get; }
		public virtual string CommonLevelBasePath => @"ROOT\EngineDatas\06 Levels\";

		// Game actions
		public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
		{
			new GameAction("Extract BF file(s)", false, true, (input, output) => ExtractFilesAsync(settings, output, false)),
			new GameAction("Extract BF file(s) - BIN decompression", false, true, (input, output) => ExtractFilesAsync(settings, output, true)),
		};
        public async UniTask ExtractFilesAsync(GameSettings settings, string outputDir, bool decompressBIN = false) {
            using (var context = new R1Context(settings)) {
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

		// Version properties
        public abstract string[] BFFiles { get; }

		// Helpers
        public virtual void CreateLevelList(LOA_Loader l) {
			var groups = l.FileInfos.GroupBy(l => Jade_Key.WorldKey(l.Key)).OrderBy(l => l.Key);
			List<KeyValuePair<uint, LOA_Loader.FileInfo>> levels = new List<KeyValuePair<uint, LOA_Loader.FileInfo>>();
			foreach (var g in groups) {
				if(!g.Any(f => f.Key.Type == Jade_Key.KeyType.Map)) continue;
				var kvpair = g.FirstOrDefault(f => f.Value.FileName != null && f.Value.FileName.EndsWith(".wol"));
				//if (kvpair.Value != null) {
				//	Debug.Log($"{g.Key:X8} - {kvpair.Value.FilePath }");
				//}
				levels.Add(new KeyValuePair<uint, LOA_Loader.FileInfo>(g.Key, kvpair.Value));
			}

			var str = new StringBuilder();

			foreach (var kv in levels.OrderBy(l => l.Value.DirectoryName).ThenBy(l => l.Value.FileName)) 
            {
				str.AppendLine($"new LevelInfo(0x{kv.Key:X8}, \"{kv.Value.DirectoryName}\", \"{kv.Value.FileName}\"),");
				//Debug.Log($"{kv.Key:X8} - {kv.Value }");
			}

			str.ToString().CopyToClipboard();
		}
        public async UniTask<BIG_BigFile> LoadBF(Context context, string bfPath) {
			var s = context.Deserializer;
			s.Goto(context.GetFile(bfPath).StartPointer);
			await s.FillCacheForReadAsync((int)BIG_BigFile.HeaderLength);
			var bfFile = FileFactory.Read<BIG_BigFile>(bfPath, context);
			await s.FillCacheForReadAsync((int)bfFile.TotalFatFilesLength);
			bfFile.SerializeFatFiles(s);
			return bfFile;
		}

		// Load
		public async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures) {
			List<BIG_BigFile> bfs = new List<BIG_BigFile>();
			foreach (var bfPath in BFFiles) {
				var bf = await LoadBF(context, bfPath);
				bfs.Add(bf);
			}
			// Set up loader
			LOA_Loader loader = new LOA_Loader(bfs.ToArray());
			context.StoreObject<LOA_Loader>(LoaderKey, loader);

			// Create level list if null
			if (LevelInfos == null) CreateLevelList(loader);

			// Set up AI types
			AI_Links aiLinks = AI_Links.GetAILinks(context.GetR1Settings());
			context.StoreObject<AI_Links>(AIKey, aiLinks);

			// Set up texture list
			TEX_GlobalList texList = new TEX_GlobalList();
			context.StoreObject<TEX_GlobalList>(TextureListKey, texList);

			// Load univers
			Controller.DetailedState = $"Loading universe";
			await Controller.WaitIfNecessary();

			Jade_Reference<AI_Instance> Univers = new Jade_Reference<AI_Instance>(context, bfs[0].UniversKey);
			Univers.Resolve();
			await loader.LoadLoop(context.Deserializer); // First resolve universe

			// Load world
			Controller.DetailedState = $"Loading world";
			await Controller.WaitIfNecessary();

			var worldKey = (Jade_Key)(uint)context.GetR1Settings().Level;

			Jade_Reference<WOR_WorldList> WorldList = new Jade_Reference<WOR_WorldList>(context, worldKey);
			WorldList.Resolve(queue: LOA_Loader.QueueType.Maps);

			loader.BeginSpeedMode(worldKey.GetBinary(Jade_Key.KeyType.Map));
			await loader.LoadLoop(context.Deserializer);
			if (texList.Textures.Any()) {
				loader.BeginSpeedMode((Jade_Key)Jade_Key.KeyTypeTextures, serializeAction: s => {
					for (int i = 0; i < texList.Textures.Count; i++) {
						texList.Textures[i].LoadInfo();
						loader.LoadLoopBIN();
					}
					if (texList.Palettes != null) {
						for (int i = 0; i < texList.Palettes.Count; i++) {
							texList.Palettes[i].Load();
						}
						loader.LoadLoopBIN();
					}
					for (int i = 0; i < texList.Textures.Count; i++) {
						texList.Textures[i].LoadContent();
					}
					loader.LoadLoopBIN();
					texList.FillInReferences();
				});
				await loader.LoadLoop(context.Deserializer);
			}
			loader.EndSpeedMode();
			throw new NotImplementedException("BINs serialized. Time to do something with this data :)");
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

		// Constants
		public static readonly Encoding Encoding = Encoding.GetEncoding(1252);
		public const string LoaderKey = "loader";
		public const string TextureListKey = "textureList";
		public const string AIKey = "ai";

		public class LevelInfo
        {
            public LevelInfo(uint key, string directoryPath, string filePath)
            {
                Key = key;
                DirectoryPath = directoryPath;
                FilePath = filePath;

				WorldName = Path.GetDirectoryName(DirectoryPath);
				MapName = Path.GetFileNameWithoutExtension(FilePath);
            }

            public uint Key { get; }
			public string DirectoryPath { get; }
			public string FilePath { get; }

			public string WorldName { get; }
			public string MapName { get; }
		}
    }
}
