using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R1Engine.Jade;
using System.IO;
using BinarySerializer;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
			new GameAction("Export textures", false, true, (input, output) => ExportTexturesAsync(settings, output)),
		};
        public async UniTask ExtractFilesAsync(GameSettings settings, string outputDir, bool decompressBIN = false) {
            using (var context = new R1Context(settings)) {
				var s = context.Deserializer;
                await LoadFilesAsync(context);
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
									while (curDir.ParentDirectory != -1) {
										curDir = fat.DirectoryInfos[curDir.ParentDirectory];
										dirName = Path.Combine(curDir.Name, dirName);
									}
									directories[i] = Path.Combine(outputDir, dirName);
									Directory.CreateDirectory(directories[i]);
								}
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
										s.DoEncoded(new Jade_Lzo1xEncoder(fileSize, xbox360Version: settings.Jade_Version == Jade_Version.Xenon), () => {
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
									if (fi.ParentDirectory >= 0) {
										Util.ByteArrayToFile(Path.Combine(directories[fi.ParentDirectory], fileName), fileBytes);
									}
								} else {
									fileName = $"no_name_{fat.Files[i].Key:X8}.dat";
									if (fileIsCompressed) {
										fileName += ".dec";
									}
									Util.ByteArrayToFile(Path.Combine(outputDir, fileName), fileBytes);
								}
							}
						}
						// Extract hidden files
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
        public async UniTask ExportTexturesAsync(GameSettings settings, string outputDir)
        {
			var parsedTexs = new HashSet<uint>();

			var levIndex = 0;
			uint currentKey = 0;

            foreach (var lev in LevelInfos)
            {
				Debug.Log($"Exporting for level {levIndex++ + 1}/{LevelInfos.Length}: {lev.MapName}");

                try
                {
                    using (var context = new R1Context(settings)) {
						currentKey = 0;
						await LoadFilesAsync(context);
                        await LoadJadeAsync(context, (Jade_Key)lev.Key);

                        TEX_GlobalList texList = context.GetStoredObject<TEX_GlobalList>(TextureListKey);

						if (texList.Textures != null && texList.Textures.Any()) {
							Debug.Log($"Loaded level. Exporting {texList?.Textures?.Count} textures...");
							await Controller.WaitIfNecessary();

							foreach (var t in texList.Textures) {
								if (parsedTexs.Contains(t.Key.Key))
									continue;

								parsedTexs.Add(t.Key.Key);

								Texture2D tex = null;
								currentKey = t.Key;
								tex = (t.Content ?? t.Info).ToTexture2D();
								
								if (tex == null)
									continue;

								string name = $"{t.Key.Key:X8}";
								/*if ((t.Content ?? t.Info)?.Content_Xenon != null) {
									name += "_" + (t.Content ?? t.Info).Content_Xenon.Format.ToString();
								}*/
								Util.ByteArrayToFile(Path.Combine(outputDir, $"{name}.png"), tex.EncodeToPNG());
							}
						}
						if (texList.CubeMaps != null && texList.CubeMaps.Any()) {
							foreach (var t in texList.CubeMaps) {
								if (parsedTexs.Contains(t.Key.Key))
									continue;
								parsedTexs.Add(t.Key.Key);
								var dds = t.Value.DDS;

                                for (int i = 0; i < dds.Textures.Length; i++)
                                {
								    Util.ByteArrayToFile(Path.Combine(outputDir, "Cubemaps", $"{t.Key.Key:X8}_{i}.png"), dds.Textures[i].Items[0].ToTexture2D().EncodeToPNG());
                                }
							}
						}
                    }
                }
				catch (Exception ex)
                {
					if (currentKey == 0) {
						Debug.LogError($"Failed to export for level {lev.MapName}: {ex.ToString()}");
					} else {
						Debug.LogError($"Failed to export for level {lev.MapName}: [{currentKey:X8}] {ex.ToString()}");
					}
                }


				// Unload textures
				await Controller.WaitIfNecessary();
				await Resources.UnloadUnusedAssets();
			}

            Debug.Log($"Finished export");
		}

        // Version properties
		public abstract string[] BFFiles { get; }
		public virtual string[] FixWorlds { get; }
		public virtual string JadeSpePath { get; }

		// Helpers
        public virtual void CreateLevelList(LOA_Loader l) {
			var groups = l.FileInfos.GroupBy(l => Jade_Key.WorldKey(l.Key)).OrderBy(l => l.Key);
			List<KeyValuePair<uint, LOA_Loader.FileInfo>> levels = new List<KeyValuePair<uint, LOA_Loader.FileInfo>>();
			foreach (var g in groups) {
				if(!g.Any(f => f.Key.Type == Jade_Key.KeyType.Map)) continue;
				var kvpair = g.FirstOrDefault(f => f.Value.FileName != null && f.Value.FileName.EndsWith(".wol"));
				if (kvpair.Value == null) {
					kvpair = g.FirstOrDefault(f => f.Value.FileName != null && f.Key.Type == Jade_Key.KeyType.Map);
				}
				//if (kvpair.Value != null) {
				//	Debug.Log($"{g.Key:X8} - {kvpair.Value.FilePath }");
				//}
				levels.Add(new KeyValuePair<uint, LOA_Loader.FileInfo>(g.Key, kvpair.Value));
			}

			var str = new StringBuilder();

			foreach (var kv in levels.OrderBy(l => l.Value?.DirectoryName).ThenBy(l => l.Value?.FileName)) 
            {
				str.AppendLine($"new LevelInfo(0x{kv.Key:X8}, \"{kv.Value?.DirectoryName}\", \"{kv.Value?.FileName}\"),");
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

		public void LoadJadeSPE(Context context) {
			if(JadeSpePath == null) return;
			var s = context.Deserializer;
			LOA_Loader loader = context.GetStoredObject<LOA_Loader>(LoaderKey);
			loader.SpecialArray = FileFactory.Read<LOA_SpecialArray>(JadeSpePath, context);
		}
		public async UniTask<Jade_Reference<WOR_WorldList>> LoadWorldList(Context context, Jade_Key worldKey, bool isFix = false) {
			LOA_Loader loader = context.GetStoredObject<LOA_Loader>(LoaderKey);
			loader.IsLoadingFix = isFix;

			Jade_Reference<WOR_WorldList> worldList = new Jade_Reference<WOR_WorldList>(context, worldKey);
			worldList.Resolve(queue: LOA_Loader.QueueType.Maps); 

			// Set up texture list
			TEX_GlobalList texList = new TEX_GlobalList();
			context.StoreObject<TEX_GlobalList>(TextureListKey, texList);
			loader.Caches[LOA_Loader.CacheType.TextureInfo].Clear();
			loader.Caches[LOA_Loader.CacheType.TextureContent].Clear();

			loader.BeginSpeedMode(worldKey.GetBinary(Jade_Key.KeyType.Map), serializeAction: async s => {
				await loader.LoadLoopBINAsync();
				if (worldList?.Value != null) {
					await worldList.Value.ResolveReferences(s);
				}
			});
			await loader.LoadLoop(context.Deserializer);
			if (texList.Textures != null && texList.Textures.Any()) {
				Controller.DetailedState = $"Loading textures";
				loader.BeginSpeedMode((Jade_Key)Jade_Key.KeyTypeTextures, serializeAction: async s => {
					Controller.DetailedState = $"Loading textures: Info";
					for (int i = 0; i < texList.Textures.Count; i++) {
						texList.Textures[i].LoadInfo();
						await loader.LoadLoopBINAsync();
					}
					Controller.DetailedState = $"Loading textures: Palettes";
					if (texList.Palettes != null) {
						for (int i = 0; i < (texList.Palettes?.Count ?? 0); i++) {
							texList.Palettes[i].Load();
						}
						await loader.LoadLoopBINAsync();
					}
					Controller.DetailedState = $"Loading textures: Content";
					for (int i = 0; i < texList.Textures.Count; i++) {
						texList.Textures[i].LoadContent();
						await loader.LoadLoopBINAsync();
						if (texList.Textures[i].Content != null && texList.Textures[i].Info.FileFormat != TEX_File.TexFileFormat.RawPal) {
							if (texList.Textures[i].Content.Width != texList.Textures[i].Info.Width ||
								texList.Textures[i].Content.Height != texList.Textures[i].Info.Height ||
								texList.Textures[i].Content.Uint_0C != texList.Textures[i].Info.Uint_0C) {
								throw new Exception($"Info & Content width/height mismatch for texture with key {texList.Textures[i].Key}");
							}
						}
					}
					Controller.DetailedState = $"Loading textures: CubeMaps";
					for (int i = 0; i < (texList.CubeMaps?.Count ?? 0); i++) {
						texList.CubeMaps[i].Load();
						await loader.LoadLoopBINAsync();
					}
					Controller.DetailedState = $"Loading textures: End";
					texList.FillInReferences();
				});
				await loader.LoadLoop(context.Deserializer);
			}
			loader.EndSpeedMode();
			loader.IsLoadingFix = false;

			return worldList;
		}

		// Load
		public async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures) 
        {
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			var loader = await LoadJadeAsync(context, (Jade_Key)(uint)context.GetR1Settings().Level);

			stopWatch.Stop();

			Debug.Log($"Loaded BINs in {stopWatch.ElapsedMilliseconds}ms");

			throw new NotImplementedException("BINs serialized. Time to do something with this data :)");
		}
		public async UniTask<LOA_Loader> LoadJadeAsync(Context context, Jade_Key worldKey) 
        {
            List<BIG_BigFile> bfs = new List<BIG_BigFile>();
			foreach (var bfPath in BFFiles) {
				var bf = await LoadBF(context, bfPath);
				bfs.Add(bf);
			}
			// Set up loader
			LOA_Loader loader = new LOA_Loader(bfs.ToArray());
			context.StoreObject<LOA_Loader>(LoaderKey, loader);

			// Load jade.spe
			LoadJadeSPE(context);

			// Create level list if null
			if (LevelInfos == null) CreateLevelList(loader);

			// Set up AI types
			AI_Links aiLinks = AI_Links.GetAILinks(context.GetR1Settings());
			context.StoreObject<AI_Links>(AIKey, aiLinks);

			// Load univers
			Controller.DetailedState = $"Loading universe";
			await Controller.WaitIfNecessary();

			Jade_Reference<AI_Instance> univers = new Jade_Reference<AI_Instance>(context, bfs[0].UniversKey);
			univers.Resolve();
			await loader.LoadLoop(context.Deserializer); // First resolve universe

			// Load world
			Controller.DetailedState = $"Loading worlds";
			await Controller.WaitIfNecessary();

			if (FixWorlds != null && LevelInfos != null) {
				var levelInfos = LevelInfos;
				foreach (var world in FixWorlds) {
					var fixInfo = levelInfos.FindItem(li => li.MapName.Contains(world));
					if (fixInfo != null) {
						Jade_Key fixKey = (Jade_Key)fixInfo.Key;
						if (fixKey == worldKey) {
							UnityEngine.Debug.LogWarning($"Loading fix world with name {world} as a regular map world");
							break;
						}
						var fixWorldList = await LoadWorldList(context, fixKey, isFix: true);
					} else {
						UnityEngine.Debug.LogWarning($"Fix world with name {world} could not be found");
					}
				}
			}
			var worldList = await LoadWorldList(context, worldKey);

			return loader;
        }

        public async UniTask LoadFilesAsync(Context context) {
			foreach (var bfPath in BFFiles) {
				await context.AddLinearSerializedFileAsync(bfPath, bigFileCacheLength: 8);
			}
			if (JadeSpePath != null) {
				await context.AddLinearSerializedFileAsync(JadeSpePath);
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
