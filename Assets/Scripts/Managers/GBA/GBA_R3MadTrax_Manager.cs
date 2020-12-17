using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public class GBA_R3MadTrax_Manager : GBA_Manager
    {
        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return base.GetGameActions(settings).Concat(new GameAction[]
            {
                new GameAction("Export Mad Trax Sprites", false, true, (input, output) => ExportSprites(settings, output)), 
            }).ToArray();
        }

        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
        };

        public override string GetROMFilePath(Context context) => $"{(Files)context.Settings.World}.bin";

        public enum Files
        {
            client_pad_english,
            client_pad_french,
            client_pad_german,
            client_pad_italian,
            client_pad_spanish,

            // EU only
            client_pad145,
            client_pad2,
            client_pad3,
        }

        public override int[] MenuLevels => new int[0];
        public override int DLCLevelCount => 0;

        public override int[] AdditionalSprites4bpp => new int[0];
        public override int[] AdditionalSprites8bpp => new int[0];

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new NotImplementedException();

        public async UniTask ExportSprites(GameSettings settings, string outputDir)
        {
            using (var context = new Context(settings))
            {
                await LoadFilesAsync(context);

                var s = context.Deserializer;

                s.Goto(context.FilePointer(GetROMFilePath(context)) + 0x00E000);

                int index = 0;

                ExportSprites(s, 8, 8, 9, ref index, outputDir); // Gumsi
                ExportSprites(s, 8, 8, 9, ref index, outputDir); // Rayman
                ExportSprites(s, 2, 4, 5, ref index, outputDir); // Big blocks
                ExportSprites(s, 4, 4, 1, ref index, outputDir); // Big blocks
                ExportSprites(s, 2, 4, 1, ref index, outputDir); // Missile
                ExportSprites(s, 4, 4, 21, ref index, outputDir); // Blocks
                ExportSprites(s, 2, 4, 6, ref index, outputDir); // Missiles
                ExportSprites(s, 2, 2, 1, ref index, outputDir); // Missiles

                ExportSprites(s, 2, 1, 1, ref index, outputDir); // Effect
                ExportSprites(s, 2, 4, 1, ref index, outputDir); // Vertical bar
                ExportSprites(s, 2, 1, 1, ref index, outputDir); // Arrow
                ExportSprites(s, 2, 2, 4, ref index, outputDir); // ?

                ExportSprites(s, 4, 4, 7, ref index, outputDir); // Explosion 
                ExportSprites(s, 2, 2, 1, ref index, outputDir); // ?
                ExportSprites(s, 4, 4, 1, ref index, outputDir); // ?
                ExportSprites(s, 1, 1, 3, ref index, outputDir); // ?

                s.Goto(s.CurrentPointer + 0x20 * 4); // ? 

                ExportSprites(s, 4, 128, 1, ref index, outputDir); // Controls screen
            }
        }

        public void ExportSprites(SerializerObject s, int width, int height, int length, ref int index, string outputDir)
        {
            for (int i = 0; i < length; i++)
            {
                var pointer = s.CurrentPointer;

                var pal = s.SerializeObjectArray<RGBA5551Color>(default, 16, name: $"Palette");
                var tileData = s.SerializeArray<byte>(default, width * height * (CellSize * CellSize / 2), name: $"TileData");

                var tex = Util.ToTileSetTexture(tileData, Util.ConvertGBAPalette(pal), Util.TileEncoding.Linear_4bpp, CellSize, true, wrap: width);

                Util.ByteArrayToFile(Path.Combine(outputDir, $"Sprite_{index++}_0x{pointer.AbsoluteOffset:X8}.png"), tex.EncodeToPNG());
            }
        }
    }
}