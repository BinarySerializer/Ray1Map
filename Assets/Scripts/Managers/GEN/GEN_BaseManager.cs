using BinarySerializer;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GEN_BaseManager : BaseGameManager
	{
		// Levels
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => new GameInfo_Volume[0];

		// Game actions
		public override GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
		{
			new GameAction("Extract UBI & RLX files", false, true, (input, output) => ConvertSprites(settings, output, true)),
		};
        public async UniTask ConvertSprites(GameSettings settings, string outputDir, bool exportGif)
        {
            Color[] ProcessPalette(BaseColor[] palette) {
                return palette.Select(p => {
                    Color c = p.GetColor();
                    c.a = 1f;
                    return c;
                }).ToArray();
            }

            using var context = new R1Context(settings);
            var s = context.Deserializer;
            var pal = Util.CreateDummyPalette(256, false);


            async UniTask<BaseColor[]> LoadPalFromDIB(string filePath) {
                string fileName = filePath.Substring(context.BasePath.Length).Replace("\\", "/");
                var file = await context.AddLinearSerializedFileAsync(fileName, Endian.Little);
                BGRA8888Color[] Palette = null;
                return s.DoAt(file.StartPointer + 0x36, () => s.SerializeObjectArray<BGRA8888Color>(Palette, 256, name: nameof(Palette)));
            }

            bool hasMainPal = false;
            string palettePath = "Root/COMMUN/PAL.PAL";
            GEN_Palette paletteFile = null;
            if (File.Exists(context.BasePath + palettePath)) {
                await context.AddLinearSerializedFileAsync(palettePath, Endian.Little);
                paletteFile = FileFactory.Read<GEN_Palette>(palettePath, context);
                hasMainPal = true;
            }
            BaseColor[] dibPalette = null;
            if (!hasMainPal) {
                foreach (var dibPath in Directory.EnumerateFiles(context.BasePath, "*.dib", SearchOption.AllDirectories).
                    Concat(Directory.EnumerateFiles(context.BasePath, "*.DIB", SearchOption.AllDirectories))) {
                    dibPalette = await LoadPalFromDIB(dibPath);
                    break;
                }
            }

            var mainPal = ProcessPalette(paletteFile?.Palette ?? dibPalette ?? pal);

            foreach (var filePath in Directory.EnumerateFiles(context.BasePath, "*.ubi", SearchOption.AllDirectories).
                Concat(Directory.EnumerateFiles(context.BasePath, "*.UBI", SearchOption.AllDirectories))) 
            {
                var ubiPal = mainPal;
                string fileName = filePath.Substring(context.BasePath.Length).Replace("\\", "/");
                await context.AddLinearSerializedFileAsync(fileName, Endian.Little);
                GEN_UBI ubi = null;
                try {
                    ubi = FileFactory.Read<GEN_UBI>(fileName, context);
                } catch (Exception) {
                    continue;
                }
                if(!ubi.Frames.Any(f => f.SpriteData?.Sections.Any(sec => (sec.RLX?.Data?.Data?.Length ?? 0) > 0) ?? false)) continue;

                /*if (!hasMainPal)*/ {
                    string dir = Path.GetDirectoryName(filePath);
                    BaseColor[] curDibPalette = null;
                    foreach (var dibPath in Directory.EnumerateFiles(dir, "*.dib", SearchOption.TopDirectoryOnly).
                        Concat(Directory.EnumerateFiles(dir, "*.DIB", SearchOption.TopDirectoryOnly))) {
                        curDibPalette = await LoadPalFromDIB(dibPath);
                        break;
                    }
                    if(curDibPalette != null) ubiPal = ProcessPalette(curDibPalette);
                }

                int width = ubi.Frames.Max(f => f.SpriteData?.Sections.Max(sec => (sec.RLX?.Data?.X ?? 0) + (sec.RLX?.Data?.Width ?? 0)) ?? 0);
                int height = ubi.Frames.Max(f => f.SpriteData?.Sections.Max(sec => (sec.RLX?.Data?.Y ?? 0) + (sec.RLX?.Data?.Height ?? 0)) ?? 0);
                Texture2D workingTexture = TextureHelpers.CreateTexture2D(width, height, clear: true, applyClear: true);
                List<Texture2D> frames = new List<Texture2D>();
                for (int i = 0; i < ubi.Frames.Length; i++) {
                    var f = ubi.Frames[i];
                    for (int j = 0; j < f.SpriteData?.Sections.Length; j++) {
                        var section = f.SpriteData.Sections[j];
                        if (section.Palette != null) ubiPal = ProcessPalette(section.Palette);

                        if (section.RLX != null) {
                            var rlx = section.RLX.Data;
                            workingTexture = rlx.ToTexture2D(ubiPal, texture: workingTexture);
                        }
                    }
                    if (exportGif) {
                        Texture2D frameTexture = TextureHelpers.CreateTexture2D(workingTexture.width, workingTexture.height);
                        Graphics.CopyTexture(workingTexture, frameTexture);
                        frameTexture.Apply();
                        frames.Add(frameTexture);
                    } else {
                        var path = Path.Combine(outputDir, $"{fileName}_{i}.png");
                        Util.ByteArrayToFile(path, workingTexture.EncodeToPNG());
                    }
                }
                if (exportGif && frames.Any()) {
                    Util.ExportAnimAsGif(frames, 1, false, false, Path.Combine(outputDir, $"{fileName}.gif"));
                }
                await Controller.WaitFrame();
            }
            foreach (var filePath in Directory.EnumerateFiles(context.BasePath, "*.rlx", SearchOption.AllDirectories).
                Concat(Directory.EnumerateFiles(context.BasePath, "*.RLX", SearchOption.AllDirectories))) {
                var ubiPal = mainPal;
                string fileName = filePath.Substring(context.BasePath.Length).Replace("\\", "/");
                await context.AddLinearSerializedFileAsync(fileName, Endian.Little);
                GEN_RLX rlxFile = null;
                try {
                    rlxFile = FileFactory.Read<GEN_RLX>(fileName, context);
                } catch (Exception) {
                    continue;
                }
                {
                    var rlx = rlxFile.Data;
                    if(rlx == null) continue;
                    var tex = rlx.ToTexture2D(ubiPal);
                    var path = Path.Combine(outputDir, $"{fileName}.png");
                    Util.ByteArrayToFile(path, tex.EncodeToPNG());
                }
            }
        }

		// Load
		public override async UniTask<Unity_Level> LoadAsync(Context context) 
        {
			throw new NotImplementedException();
		}

        public override async UniTask LoadFilesAsync(Context context) {
			throw new NotImplementedException();
		}
    }
}