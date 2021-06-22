using Cysharp.Threading.Tasks;
using R1Engine.Jade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Jade_PetzHorseClub_PC_Manager : Jade_Montpellier_BaseManager {

		// Game actions
		public override GameAction[] GetGameActions(GameSettings settings) {
			return base.GetGameActions(settings).Concat(new GameAction[]
			{
				new GameAction("Export textures (unbinarized)", false, true, (input, output) => ExportTexturesUnbinarized(settings, output)), // TODO: make it export later
			}).ToArray();
		}

		// Levels
		public override LevelInfo[] LevelInfos => null;

		public override string[] BFFiles => new string[] {
			"Horsez2_HD.bf"
		};


		public async UniTask ExportTexturesUnbinarized(GameSettings settings, string outputDir) {
			using (var context = new R1Context(settings)) {
				await LoadFilesAsync(context);
				List<BIG_BigFile> bfs = new List<BIG_BigFile>();
				foreach (var bfPath in BFFiles) {
					var bf = await LoadBF(context, bfPath);
					bfs.Add(bf);
				}
				// Set up loader
				LOA_Loader loader = new LOA_Loader(bfs.ToArray(), context);
				context.StoreObject<LOA_Loader>(LoaderKey, loader);

				// Set up texture list
				TEX_GlobalList texList = new TEX_GlobalList();
				context.StoreObject<TEX_GlobalList>(TextureListKey, texList);

				foreach (var kvp in loader.FileInfos) {
					var fileInfo = kvp.Value;
					if (fileInfo.FileName != null && fileInfo.FileName.EndsWith(".tex")) {
						try {
							Jade_TextureReference texRef = new Jade_TextureReference(context, fileInfo.Key);
							texRef.Resolve();

							var t = texList.Textures[0];

							for (int i = 0; i < texList.Textures.Count; i++) {
								texList.Textures[i].LoadInfo();
								await loader.LoadLoop(context.Deserializer);
							}
							if (texList.Palettes != null) {
								for (int i = 0; i < (texList.Palettes?.Count ?? 0); i++) {
									texList.Palettes[i].Load();
								}
								await loader.LoadLoop(context.Deserializer);
							}
							for (int i = 0; i < texList.Textures.Count; i++) {
								texList.Textures[i].LoadContent();
								await loader.LoadLoop(context.Deserializer);
								if (texList.Textures[i].Content != null && texList.Textures[i].Info.Type != TEX_File.TexFileType.RawPal) {
									if (texList.Textures[i].Content.Width != texList.Textures[i].Info.Width ||
										texList.Textures[i].Content.Height != texList.Textures[i].Info.Height ||
										texList.Textures[i].Content.Color != texList.Textures[i].Info.Color) {
										throw new Exception($"Info & Content width/height mismatch for texture with key {texList.Textures[i].Key}");
									}
								}
							}
							texList.FillInReferences();


							Texture2D tex = null;
							uint currentKey = t.Key;
							tex = (t.Content ?? t.Info).ToTexture2D();

							if (tex == null)
								continue;

							string name = fileInfo.FilePath;
							/*if ((t.Content ?? t.Info)?.Content_Xenon != null) {
								name += "_" + (t.Content ?? t.Info).Content_Xenon.Format.ToString();
							}*/
							Util.ByteArrayToFile(Path.Combine(outputDir, $"{name}.png"), tex.EncodeToPNG());
						} catch (Exception ex) {
							UnityEngine.Debug.LogError(ex);
						} finally {
							texList.Textures.Clear();
							texList.Palettes.Clear();
						}
					}
				}
			}
		}
	}
}
