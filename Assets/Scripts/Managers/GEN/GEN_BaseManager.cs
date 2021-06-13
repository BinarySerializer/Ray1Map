using BinarySerializer;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                    Concat(Directory.EnumerateFiles(context.BasePath, "*.DIB", SearchOption.AllDirectories)).
                    Concat(Directory.EnumerateFiles(context.BasePath, "*.bmp", SearchOption.AllDirectories)).
                    Concat(Directory.EnumerateFiles(context.BasePath, "*.BMP", SearchOption.AllDirectories))) {
                    dibPalette = await LoadPalFromDIB(s, dibPath);
                    break;
                }
            }
            var mainPal = ProcessPalette(paletteFile?.Palette ?? dibPalette ?? pal);

            HashSet<string> exportedPaths = new HashSet<string>();
            Dictionary<string, string> backgroundsForSprite = new Dictionary<string, string>();

            async UniTask CheckScriptUnbinarized(string scriptPath) {
                await UniTask.CompletedTask;
                var lines = File.ReadAllLines(scriptPath, Encoding);
                List<string> rlxs = new List<string>();
                List<string> ubis = new List<string>();
                List<string> backgrounds = new List<string>();
                Match m = default;
                string SpritePattern = @"""(?<filepath>[^""]*\.(?<type>(RLX|UBI)))""";
                string BackgroundPattern = @"SETBACKGROUND[^""]*""(?<filepath>[^""]*)""";
                foreach (var line in lines) {
                    if ((m = Regex.Match(line, BackgroundPattern, RegexOptions.IgnoreCase)).Success) {
                        string backgroundPath = m.Groups["filepath"].Value;
                        backgrounds.Add(backgroundPath);
                        continue;
                    }
                    if ((m = Regex.Match(line, SpritePattern, RegexOptions.IgnoreCase)).Success) {
                        string spritePath = m.Groups["filepath"].Value;
                        string type = m.Groups["type"].Value;
                        if (type.ToLower() == "rlx") {
                            rlxs.Add(spritePath);
                        } else {
                            ubis.Add(spritePath);
                        }
                        continue;
                    }
                }
                backgrounds = backgrounds.Distinct().ToList();
                rlxs = rlxs.Distinct().ToList();
                ubis = ubis.Distinct().ToList();

                if (backgrounds.Count != 1) return;
                var back = backgrounds.First();
                foreach (var rlx in rlxs.Concat(ubis)) {
                    var normalized = rlx.Replace("\\", "/").ToLower();
                    backgroundsForSprite[normalized] = back.Replace("\\", "/");
                }
                /*if (backgrounds.Any()) {
                    if (backgrounds.Count > 1) {
                        Debug.LogWarning($"{Path.GetFileName(scriptPath)}: {backgrounds.Count} - {backgrounds.First()}");
                    }
                }*/

            }

            // Unbinarized scripts
            foreach (var scriptPath in Directory.EnumerateFiles(context.BasePath, "*.mms", SearchOption.AllDirectories).
                Concat(Directory.EnumerateFiles(context.BasePath, "*.MMS", SearchOption.AllDirectories)).
                Concat(Directory.EnumerateFiles(context.BasePath, "*.MMs", SearchOption.AllDirectories))) {
                await CheckScriptUnbinarized(scriptPath);
            }

            bool mmbIsBinary = false;
            bool isDictees = false;
            var mmiFiles = Directory.EnumerateFiles(context.BasePath, "*.mmi", SearchOption.AllDirectories).
                Concat(Directory.EnumerateFiles(context.BasePath, "*.MMI", SearchOption.AllDirectories)).
                Concat(Directory.EnumerateFiles(context.BasePath, "*.MMi", SearchOption.AllDirectories));
            if (mmiFiles.Any()) mmbIsBinary = true;
            if (!mmbIsBinary) {
                foreach (var scriptPath in Directory.EnumerateFiles(context.BasePath, "*.mmb", SearchOption.AllDirectories).
                    Concat(Directory.EnumerateFiles(context.BasePath, "*.MMB", SearchOption.AllDirectories)).
                    Concat(Directory.EnumerateFiles(context.BasePath, "*.MMb", SearchOption.AllDirectories))) {
                    await CheckScriptUnbinarized(scriptPath);
                    isDictees = true;
                }
            }

            foreach (var filePath in Directory.EnumerateFiles(context.BasePath, "*.ubi", SearchOption.AllDirectories).
                Concat(Directory.EnumerateFiles(context.BasePath, "*.UBI", SearchOption.AllDirectories))) {
                string lookup = filePath.Substring(context.BasePath.Length).Replace("\\", "/").ToLower();
                BaseColor[] curDibPalette = null;
                if (backgroundsForSprite.ContainsKey(lookup)) {
                    curDibPalette = await LoadPalFromDIB(s, s.Context.GetAbsoluteFilePath(backgroundsForSprite[lookup]));
                }
                if(curDibPalette != null) {
                    await ConvertUBI(s, filePath, ProcessPalette(curDibPalette), false, exportGif, outputDir, isDictees);
                } else {
                    await ConvertUBI(s, filePath, mainPal, true, exportGif, outputDir, isDictees);
                }
            }
            foreach (var filePath in Directory.EnumerateFiles(context.BasePath, "*.rlx", SearchOption.AllDirectories).
                Concat(Directory.EnumerateFiles(context.BasePath, "*.RLX", SearchOption.AllDirectories))) {
                string lookup = filePath.Substring(context.BasePath.Length).Replace("\\", "/").ToLower();
                BaseColor[] curDibPalette = null;
                if (backgroundsForSprite.ContainsKey(lookup)) {
                    curDibPalette = await LoadPalFromDIB(s, s.Context.GetAbsoluteFilePath(backgroundsForSprite[lookup]));
                }
                if (curDibPalette != null) {
                    await ConvertRLX(s, filePath, ProcessPalette(curDibPalette), false, outputDir);
                } else {
                    await ConvertRLX(s, filePath, mainPal, true, outputDir);
                }
            }
        }



        public static readonly Encoding Encoding = Encoding.GetEncoding(1252);

        async UniTask<BaseColor[]> LoadPalFromDIB(SerializerObject s, string filePath) {
            string fileName = filePath.Substring(s.Context.BasePath.Length).Replace("\\", "/");
            BinaryFile file = null;
            if (!s.Context.FileExists(fileName)) {
                file = await s.Context.AddLinearSerializedFileAsync(fileName, Endian.Little);
            } else {
                file = s.Context.GetFile(fileName);
            }
            BGRA8888Color[] Palette = null;
            return s.DoAt(file.StartPointer + 0x36, () => s.SerializeObjectArray<BGRA8888Color>(Palette, 256, name: nameof(Palette)));
        }
        Color[] ProcessPalette(BaseColor[] palette) {
            return palette.Select(p => {
                Color c = p.GetColor();
                c.a = 1f;
                return c;
            }).ToArray();
        }

        public async UniTask ConvertUBI(SerializerObject s, string filePath, Color[] mainPal, bool searchForPalette, bool exportGif, string outputDir, bool isDictees) {
            var ubiPal = mainPal;
            var context = s.Context;
            string fileName = filePath.Substring(context.BasePath.Length).Replace("\\", "/");
            await context.AddLinearSerializedFileAsync(fileName, Endian.Little);
            GEN_UBI ubi = null;
            try {
                ubi = FileFactory.Read<GEN_UBI>(fileName, context);
            } catch (Exception) {
                return;
            }
            if (!ubi.Frames.Any(f => f.SpriteData?.Sections.Any(sec => (sec.RLX?.Data?.Data?.Length ?? 0) > 0) ?? false)) return;

            if (searchForPalette)
            {
                string dir = Path.GetDirectoryName(filePath);
                BaseColor[] curDibPalette = null;
                foreach (var dibPath in Directory.EnumerateFiles(dir, "*.dib", SearchOption.TopDirectoryOnly).
                    Concat(Directory.EnumerateFiles(dir, "*.DIB", SearchOption.TopDirectoryOnly))) {
                    curDibPalette = await LoadPalFromDIB(s, dibPath);
                    break;
                }
                if (curDibPalette != null) ubiPal = ProcessPalette(curDibPalette);
            }

            GEN_RLX rlxFile = null;
            GEN_RLXData firstFrame = null;
            if (isDictees) {
                string rlxPath = filePath.Substring(0, filePath.Length-3) + "rlx";
                if (File.Exists(rlxPath)) {
                    string rlxfileName = rlxPath.Substring(context.BasePath.Length).Replace("\\", "/");
                    if (!context.FileExists(rlxfileName)) {
                        await context.AddLinearSerializedFileAsync(rlxfileName, Endian.Little);
                    }
                    try {
                        rlxFile = FileFactory.Read<GEN_RLX>(rlxfileName, context);
                        firstFrame = rlxFile.Data;
                    } catch (Exception) {
                    }
                }
            }

            int width = ubi.Frames.Max(f => f.SpriteData?.Sections.Max(sec => (sec.RLX?.Data?.X ?? 0) + (sec.RLX?.Data?.Width ?? 0)) ?? 0);
            int height = ubi.Frames.Max(f => f.SpriteData?.Sections.Max(sec => (sec.RLX?.Data?.Y ?? 0) + (sec.RLX?.Data?.Height ?? 0)) ?? 0);
            if (firstFrame != null) {
                width = Math.Max(width, firstFrame.X + firstFrame.Width);
                height = Math.Max(height, firstFrame.Y + firstFrame.Height);
            }
            Texture2D workingTexture = TextureHelpers.CreateTexture2D(width, height, clear: true, applyClear: true);
            if(firstFrame != null) {
                workingTexture = firstFrame.ToTexture2D(ubiPal, texture: workingTexture);
            }
            List<Texture2D> frames = new List<Texture2D>();
            for (int i = 0; i < ubi.Frames.Length; i++) {
                var f = ubi.Frames[i];
                int rlxType = 0;
                for (int j = 0; j < f.SpriteData?.Sections.Length; j++) {
                    var section = f.SpriteData.Sections[j];
                    if (section.Palette != null) ubiPal = ProcessPalette(section.Palette);
                    if(section.SectionType != 3) continue;
                    if (section.RLX != null) {
                        var rlx = section.RLX.Data;
                        rlxType = rlx.RLXType;
                        workingTexture = rlx.ToTexture2D(ubiPal, texture: workingTexture);
                        //workingTexture = rlx.ToTexture2D(ubiPal, raw: true);
                    }
                }
                if (exportGif) {
                    Texture2D frameTexture = TextureHelpers.CreateTexture2D(workingTexture.width, workingTexture.height);
                    Graphics.CopyTexture(workingTexture, frameTexture);
                    frameTexture.Apply();
                    frames.Add(frameTexture);
                } else {
                    var path = Path.Combine(outputDir, $"{fileName}/{i}_{rlxType}.png");
                    Util.ByteArrayToFile(path, workingTexture.EncodeToPNG());
                }
            }
            if (exportGif && frames.Any()) {
                Util.ExportAnimAsGif(frames, 1, false, false, Path.Combine(outputDir, $"{fileName}.gif"));
            }
            await Controller.WaitFrame();
        }

        public async UniTask ConvertRLX(SerializerObject s, string filePath, Color[] mainPal, bool searchForPalette, string outputDir) {
            var context = s.Context;
            var ubiPal = mainPal;
            string fileName = filePath.Substring(context.BasePath.Length).Replace("\\", "/");
            await context.AddLinearSerializedFileAsync(fileName, Endian.Little);
            GEN_RLX rlxFile = null;
            try {
                rlxFile = FileFactory.Read<GEN_RLX>(fileName, context);
            } catch (Exception) {
                return;
            }


            if (searchForPalette) {
                string dir = Path.GetDirectoryName(filePath);
                BaseColor[] curDibPalette = null;
                foreach (var dibPath in Directory.EnumerateFiles(dir, "*.dib", SearchOption.TopDirectoryOnly).
                    Concat(Directory.EnumerateFiles(dir, "*.DIB", SearchOption.TopDirectoryOnly))) {
                    curDibPalette = await LoadPalFromDIB(s, dibPath);
                    break;
                }
                if (curDibPalette != null) ubiPal = ProcessPalette(curDibPalette);
            }

            {
                var rlx = rlxFile.Data;
                if (rlx == null) return;
                var tex = rlx.ToTexture2D(ubiPal);
                var path = Path.Combine(outputDir, $"{fileName}.png");
                Util.ByteArrayToFile(path, tex.EncodeToPNG());
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