using BinarySerializer;
using Cysharp.Threading.Tasks;
using System;
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
			new GameAction("Extract UBI files", false, true, (input, output) => ExtractFilesAsync(settings, output)),
		};
        public async UniTask ExtractFilesAsync(GameSettings settings, string outputDir)
        {
            using var context = new R1Context(settings);
            var s = context.Deserializer;
            var pal = Util.CreateDummyPalette(256, false);

            string palettePath = "Root/COMMUN/PAL.PAL";
            await context.AddLinearSerializedFileAsync(palettePath, Endian.Little);
            GEN_Palette paletteFile = FileFactory.Read<GEN_Palette>(palettePath, context);
            //GEN_Palette paletteFile = null;

            foreach (var filePath in Directory.EnumerateFiles(context.BasePath, "*.ubi", SearchOption.AllDirectories).
                Concat(Directory.EnumerateFiles(context.BasePath, "*.UBI", SearchOption.AllDirectories))) 
            {
                var ubiPal = paletteFile?.Palette ?? pal;
                string fileName = filePath.Substring(context.BasePath.Length).Replace("\\", "/");
                await context.AddLinearSerializedFileAsync(fileName, Endian.Little);
                GEN_UBI ubi = FileFactory.Read<GEN_UBI>(fileName, context);
                for (int i = 0; i < ubi.Frames.Length; i++) {
                    var f = ubi.Frames[i];
                    for (int j = 0; j < f.SpriteData?.Sections.Length; j++) {
                        var section = f.SpriteData.Sections[j];
                        if (section.Palette != null) ubiPal = section.Palette;
                        if (section.RLX != null) {
                            var rlx = section.RLX.Data;
                            var tex = rlx.ToTexture2D(ubiPal);
                            var path = Path.Combine(outputDir, $"{fileName}_{i}.png");
                            Util.ByteArrayToFile(path, tex.EncodeToPNG());
                        }
                    }
                }
                await Controller.WaitFrame();
            }
            foreach (var filePath in Directory.EnumerateFiles(context.BasePath, "*.rlx", SearchOption.AllDirectories).
                Concat(Directory.EnumerateFiles(context.BasePath, "*.RLX", SearchOption.AllDirectories))) {
                var ubiPal = paletteFile.Palette;
                string fileName = filePath.Substring(context.BasePath.Length).Replace("\\", "/");
                await context.AddLinearSerializedFileAsync(fileName, Endian.Little);
                GEN_RLX ubi = FileFactory.Read<GEN_RLX>(fileName, context);
                {
                    var rlx = ubi.Data;
                    var tex = rlx.ToTexture2D(ubiPal);
                    var path = Path.Combine(outputDir, $"{fileName}.png");
                    Util.ByteArrayToFile(path, tex.EncodeToPNG());
                }
                await Controller.WaitFrame();
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