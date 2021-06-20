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
            string palettePath = "COMMUN/PAL.PAL";
            GEN_Palette paletteFile = null;
            if (File.Exists(context.BasePath + palettePath)) {
                await context.AddLinearSerializedFileAsync(palettePath, Endian.Little);
                paletteFile = FileFactory.Read<GEN_Palette>(palettePath, context);
                hasMainPal = true;
            }
            BaseColor[] dibPalette = null;
            if (!hasMainPal) {
                foreach (var dibPath in Directory.EnumerateFiles(context.BasePath, "*.dib", SearchOption.AllDirectories).
                    Concat(Directory.EnumerateFiles(context.BasePath, "*.bmp", SearchOption.AllDirectories))) {
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
            foreach (var scriptPath in Directory.EnumerateFiles(context.BasePath, "*.mms", SearchOption.AllDirectories)) {
                await CheckScriptUnbinarized(scriptPath);
            }

            bool mmbIsBinary = false;
            bool isDictees = false;
            var mmiFiles = Directory.EnumerateFiles(context.BasePath, "*.mmi", SearchOption.AllDirectories);
            if (mmiFiles.Any()) mmbIsBinary = true;
            if (!mmbIsBinary) {
                foreach (var scriptPath in Directory.EnumerateFiles(context.BasePath, "*.mmb", SearchOption.AllDirectories)) {
                    await CheckScriptUnbinarized(scriptPath);
                    isDictees = true;
                }
            }

            foreach (var filePath in Directory.EnumerateFiles(context.BasePath, "*.ubi", SearchOption.AllDirectories)) {
                try {
                    string lookup = filePath.Substring(context.BasePath.Length).Replace("\\", "/").ToLower();
                    BaseColor[] curDibPalette = null;
                    if (backgroundsForSprite.ContainsKey(lookup)) {
                        curDibPalette = await LoadPalFromDIB(s, s.Context.GetAbsoluteFilePath(backgroundsForSprite[lookup]));
                    }
                    if (curDibPalette != null) {
                        await ConvertUBI(s, filePath, ProcessPalette(curDibPalette), false, exportGif, outputDir, isDictees);
                    } else {
                        await ConvertUBI(s, filePath, mainPal, true, exportGif, outputDir, isDictees);
                    }
                } catch (Exception) { }
            }
            foreach (var filePath in Directory.EnumerateFiles(context.BasePath, "*.rlx", SearchOption.AllDirectories)) {
                try {
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
                } catch (Exception) { }
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
        async UniTask<BaseColor[]> FindDIBForPalette(SerializerObject s, string filePath) {
            string dir = Path.GetDirectoryName(filePath);
            BaseColor[] curDibPalette = null;
            // Step 1: Try from specific file
            if (curDibPalette == null) {
                string baseName = Path.GetFileNameWithoutExtension(filePath);
                string RayEduNamePattern = @"(?<firstPart>...)[SA]_(?<lastPart>.*)";
                var m = Regex.Match(baseName, RayEduNamePattern, RegexOptions.IgnoreCase);
                if (m.Success) {
                    string dibPath = Path.Combine(Path.GetDirectoryName(filePath), $"{m.Groups["firstPart"]}S_fnd.dib");
                    if (File.Exists(dibPath)) {
                        curDibPalette = await LoadPalFromDIB(s, dibPath);
                    }
                }
            }

            // Step 2: Try from (less) specific file(s)
            if (curDibPalette == null) {
                string baseName = Path.GetFileNameWithoutExtension(filePath);
                string RayEduNamePattern = @".(?<firstPart>..)[SA]_(?<lastPart>.*)";
                string RayEduBackgroundPattern = @".(?<firstPart>..)S_(?<lastPart>.*)";
                var m = Regex.Match(baseName, RayEduNamePattern, RegexOptions.IgnoreCase);
                if (m.Success) {
                    var firstPart = m.Groups["firstPart"].ToString().ToLower();
                    // TODO
                    foreach (var dibPath in Directory.EnumerateFiles(dir, "*.dib", SearchOption.TopDirectoryOnly)) {
                        var dibBaseName = Path.GetFileNameWithoutExtension(dibPath);
                        m = Regex.Match(dibBaseName, RayEduBackgroundPattern, RegexOptions.IgnoreCase);
                        if (m.Success && m.Groups["firstPart"].ToString().ToLower() == firstPart) {
                            curDibPalette = await LoadPalFromDIB(s, dibPath);
                            break;
                        }
                    }
                }
            }
            // Step 3: Any dib file in the directory really
            if (curDibPalette == null) {
                foreach (var dibPath in Directory.EnumerateFiles(dir, "*.dib", SearchOption.TopDirectoryOnly)) {
                    curDibPalette = await LoadPalFromDIB(s, dibPath);
                    break;
                }
            }
            return curDibPalette;
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
            if (!context.FileExists(fileName)) {
                await context.AddLinearSerializedFileAsync(fileName, Endian.Little);
            }
            GEN_UBI ubi = null;
            try {
                ubi = FileFactory.Read<GEN_UBI>(fileName, context);
            } catch (Exception) {
                return;
            }
            if (!ubi.Frames.Any(f => f.SpriteData?.Sections.Any(sec => (sec.RLX?.Data?.Data?.Length ?? 0) > 0) ?? false)) return;

            if (searchForPalette) {
                BaseColor[] curDibPalette = await FindDIBForPalette(s, filePath);
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
            if (firstFrame == null) {
                string baseName = Path.GetFileNameWithoutExtension(filePath);
                string RayEduNamePattern = @"(?<firstPart>...)A_(?<lastPart>.*)";
                var m = Regex.Match(baseName, RayEduNamePattern, RegexOptions.IgnoreCase);
                if (m.Success) {
                    string rlxPath = Path.Combine(Path.GetDirectoryName(filePath), $"{m.Groups["firstPart"]}S_{m.Groups["lastPart"]}.rlx");
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
            }

            if(firstFrame != null) ConvertUBIFrames();
            firstFrame = null;
            ConvertUBIFrames();

            void ConvertUBIFrames() {
                string newFileName = fileName;
                if(firstFrame != null)  newFileName += "_f0";



                int width = ubi.Frames.Max(f => f.SpriteData?.Sections.Max(sec => (sec.RLX?.Data?.X ?? 0) + (sec.RLX?.Data?.Width ?? 0)) ?? 0);
                int height = ubi.Frames.Max(f => f.SpriteData?.Sections.Max(sec => (sec.RLX?.Data?.Y ?? 0) + (sec.RLX?.Data?.Height ?? 0)) ?? 0);
                if (firstFrame != null) {
                    width = Math.Max(width, firstFrame.X + firstFrame.Width);
                    height = Math.Max(height, firstFrame.Y + firstFrame.Height);
                }

                Texture2D workingTexture = TextureHelpers.CreateTexture2D(width, height, clear: true, applyClear: true);
                if (firstFrame != null) {
                    workingTexture = firstFrame.ToTexture2D(ubiPal, texture: workingTexture);
                }
                List<Texture2D> frames = new List<Texture2D>();
                for (int i = 0; i < ubi.Frames.Length; i++) {
                    var f = ubi.Frames[i];
                    int rlxType = 0;
                    for (int j = 0; j < f.SpriteData?.Sections.Length; j++) {
                        var section = f.SpriteData.Sections[j];
                        if (section.Palette != null
                            && (section.SectionType == GEN_UBI.UBI_SpriteData.Section.Type.Palette_15
                            || section.SectionType == GEN_UBI.UBI_SpriteData.Section.Type.Palette_20))
                            ubiPal = ProcessPalette(section.Palette);
                        if (section.SectionType != GEN_UBI.UBI_SpriteData.Section.Type.RLX_Sprite) continue;
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
                        var path = Path.Combine(outputDir, $"{newFileName}/{i}_{rlxType}.png");
                        Util.ByteArrayToFile(path, workingTexture.EncodeToPNG());
                    }
                }
                if (exportGif && frames.Any()) {
                    Util.ExportAnimAsGif(frames, 1, false, false, Path.Combine(outputDir, $"{newFileName}.gif"));
                }
            }
            await Controller.WaitFrame();
        }

        public async UniTask ConvertRLX(SerializerObject s, string filePath, Color[] mainPal, bool searchForPalette, string outputDir) {
            var context = s.Context;
            var ubiPal = mainPal;
            string fileName = filePath.Substring(context.BasePath.Length).Replace("\\", "/");
            if (!context.FileExists(fileName)) {
                await context.AddLinearSerializedFileAsync(fileName, Endian.Little);
            }
            GEN_RLX rlxFile = null;
            try {
                rlxFile = FileFactory.Read<GEN_RLX>(fileName, context);
            } catch (Exception) {
                return;
            }


            if (searchForPalette) {
                BaseColor[] curDibPalette = await FindDIBForPalette(s, filePath);
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
            await UniTask.CompletedTask;
			throw new NotImplementedException();
		}

        public override async UniTask LoadFilesAsync(Context context) {
            await UniTask.CompletedTask;
            throw new NotImplementedException();
		}
    }
}