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
	public class Jade_GameActions_ModToolsRRRPrototype : Jade_GameActions {
		public Jade_GameActions_ModToolsRRRPrototype(Jade_BaseManager manager) : base(manager) { }

		protected bool FixGameObjectMorphChannels(Context context, OBJ_GameObject gao) {
			var modifiers = gao?.Extended?.Modifiers;
			if (modifiers == null || modifiers.Length == 0) return false;

			var morphModObj = modifiers.FirstOrDefault(m => m.Type == MDF_ModifierType.GEO_ModifierMorphing);
			if (morphModObj == null) return false;

			var morphMod = (GEO_ModifierMorphing)morphModObj.Modifier;
			bool shouldFix = (morphMod.ChannelsCount % 4 != 0);

			if (!shouldFix && morphMod.MorphChannels.Any(c => c.Name != null)) {
				for (int i = 0; i < morphMod.MorphChannels.Length / 4; i++) {
					if (morphMod.MorphChannels[i].Name.StartsWith("D_")) {
						shouldFix = true;
						context.SystemLogger?.LogWarning($"{morphMod.GetType().Name} @ {gao.Key}: Dummy channel among regular channels - this might cause errors");
						break;
					}
				}
				for (int i = morphMod.MorphChannels.Length / 4; i < morphMod.MorphChannels.Length; i++) {
					var realChannelIndex = (i - (morphMod.MorphChannels.Length / 4)) / 3;
					var dummyIndex = (i - (morphMod.MorphChannels.Length / 4)) % 3;
					if (morphMod.MorphChannels[i].Name != $"D_{morphMod.MorphChannels[realChannelIndex].Name}_{dummyIndex}") {
						shouldFix = true;
						break;
					}

				}
			}

			if (shouldFix) {
				// Step 1: Delete dummy channels
				var realChannels = morphMod.MorphChannels.Where(c => c.Name == null || !c.Name.StartsWith("D_"));

				// Step 2: Create new dummy channels
				var morphChannels = realChannels.Concat(
					realChannels.SelectMany((rc, i) => new GEO_ModifierMorphing.Channel[] {
						new GEO_ModifierMorphing.Channel() {
							Name = $"D_{rc.Name ?? $"C{i}"}_1",
							DataCount = 2,
							DataIndices = new uint[] { 0, 0 }
						},
						new GEO_ModifierMorphing.Channel() {
							Name = $"D_{rc.Name ?? $"C{i}"}_2",
							DataCount = 2,
							DataIndices = new uint[] { 0, 0 }
						},
						new GEO_ModifierMorphing.Channel() {
							Name = $"D_{rc.Name ?? $"C{i}"}_3",
							DataCount = 2,
							DataIndices = new uint[] { 0, 0 }
						},
					}));

				// Step 3: Set new channels in object
				var previousDataSize = morphMod.DataSize;
				var previousChannelsSize = (uint)morphMod.MorphChannels.Sum(md => 12 + 64 + md.DataCount * 4);

				morphMod.MorphChannels = morphChannels.ToArray();
				morphMod.ChannelsCount = (uint)morphMod.MorphChannels.Length;

				var newChannelsSize = (uint)morphMod.MorphChannels.Sum(md => 12 + 64 + md.DataCount * 4);

				morphMod.DataSize = 4 * 4;
				morphMod.DataSize += (uint)morphMod.MorphData.Sum(md => 4 + 64 + md.VectorsCount * (12 + 4));
				morphMod.DataSize += (uint)morphMod.MorphChannels.Sum(md => 12 + 64 + md.DataCount * 4);

				gao.FileSize -= previousChannelsSize;
				gao.FileSize += newChannelsSize;
				//gaoRef.Value.FileSize += (morphMod.DataSize - previousDataSize);
			}

			return shouldFix;
		}

		public async UniTask FixMorphChannels(GameSettings settings, string inputDir, string outputDir) {
			using (var context = new Ray1MapContext(settings)) {
				await LoadFilesAsync(context);
				LOA_Loader loader = await InitJadeAsync(context, initAI: false, initTextures: true, initSound: true);
				var texList = context.GetStoredObject<TEX_GlobalList>(TextureListKey);

				Dictionary<string, KeyValuePair<string, Dictionary<int, TEXT_OneText>>?[]> textGroups
					= new Dictionary<string, KeyValuePair<string, Dictionary<int, TEXT_OneText>>?[]>();

				foreach (var kvp in loader.FileInfos) {
					var fileInfo = kvp.Value;
					if (fileInfo.FileName != null && fileInfo.FileName.EndsWith(".gao")) {
						try {
							Jade_Reference<OBJ_GameObject> gaoRef = new Jade_Reference<OBJ_GameObject>(context, fileInfo.Key);
							gaoRef.Resolve();
							loader.LoadSingle = true;
							await loader.LoadLoop(context.Deserializer);

							bool shouldFix = FixGameObjectMorphChannels(context, gaoRef?.Value);

							if (shouldFix) {
								// Write

								using (var writeContext = new Ray1MapContext(outputDir, settings)) {
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

									Jade_Reference<OBJ_GameObject> writeGao = new Jade_Reference<OBJ_GameObject>(writeContext, gaoRef.Key) {
										Value = gaoRef.Value
									};
									writeGao.Resolve();

									var s = writeContext.Serializer;
									await writeloader.LoadLoop(s);
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
			}

			// Unload textures
			await Controller.WaitIfNecessary();
			await Resources.UnloadUnusedAssets();

			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
			GC.WaitForPendingFinalizers();

			Debug.Log($"Finished export");
		}

		public async UniTask UpscaleGeometricObjects(GameSettings settings, string inputDir, string outputDir) {
			using (var context = new Ray1MapContext(settings)) {
				await LoadFilesAsync(context);
				LOA_Loader loader = await InitJadeAsync(context, initAI: false, initTextures: true, initSound: true);
				var texList = context.GetStoredObject<TEX_GlobalList>(TextureListKey);

				Dictionary<string, KeyValuePair<string, Dictionary<int, TEXT_OneText>>?[]> textGroups
					= new Dictionary<string, KeyValuePair<string, Dictionary<int, TEXT_OneText>>?[]>();

				foreach (var kvp in loader.FileInfos) {
					var fileInfo = kvp.Value;
					if (fileInfo.FileName != null && fileInfo.FileName.EndsWith(".gro")) {
						switch (fileInfo.FileName) {
							case "Unnamed0@6532fac.gro":
							case "BRK_44.gro":
							case "Unnamed@6a47a6c.gro":
								break;
							default:
								continue;
						}
						try {
							Jade_Reference<GEO_Object> textRef = new Jade_Reference<GEO_Object>(context, fileInfo.Key);
							textRef.Resolve();
							await loader.LoadLoop(context.Deserializer);

							if (textRef.Value?.RenderObject?.Value == null) continue;

							GEO_GeometricObject gro = textRef.Value.RenderObject.Value as GEO_GeometricObject;
							foreach (var vert in gro.Vertices) {
								vert.X *= 2;
								vert.Y *= 2;
								vert.Z *= 2;
							}
							if (gro.OK3_Boxes?.Boxes != null) {
								foreach (var vert in gro.OK3_Boxes.Boxes) {
									vert.Min.X *= 2;
									vert.Min.Y *= 2;
									vert.Min.Z *= 2;
									vert.Max.X *= 2;
									vert.Max.Y *= 2;
									vert.Max.Z *= 2;
								}
							}
							using (var writeContext = new Ray1MapContext(outputDir, settings)) {
								// Set up loader
								LOA_Loader writeloader = new LOA_Loader(loader.BigFiles, writeContext) {
									Raw_WriteFilesAlreadyInBF = true
								};
								writeContext.StoreObject<LOA_Loader>(LoaderKey, writeloader);

								Jade_Reference<GEO_Object> wave = new Jade_Reference<GEO_Object>(writeContext, textRef.Key) {
									Value = textRef.Value
								};
								wave.Resolve();

								var s = writeContext.Serializer;
								await writeloader.LoadLoop(s);
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
			}

			// Unload textures
			await Controller.WaitIfNecessary();
			await Resources.UnloadUnusedAssets();

			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
			GC.WaitForPendingFinalizers();

			Debug.Log($"Finished export");
		}


		public async UniTask TempTools(GameSettings settings, string inputDir, string outputDir) {
			using (var context = new Ray1MapContext(settings)) {
				await LoadFilesAsync(context);
				LOA_Loader loader = await InitJadeAsync(context, initAI: false, initTextures: true, initSound: true);
				var texList = context.GetStoredObject<TEX_GlobalList>(TextureListKey);

				Dictionary<string, KeyValuePair<string, Dictionary<int, TEXT_OneText>>?[]> textGroups
					= new Dictionary<string, KeyValuePair<string, Dictionary<int, TEXT_OneText>>?[]>();

				// TODO: See functions above for what to do here
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
