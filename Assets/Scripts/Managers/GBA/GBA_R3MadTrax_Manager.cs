using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using R1Engine.Serialize;

namespace R1Engine
{
    public class GBA_R3MadTrax_Manager : GBA_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 2),
            Enumerable.Range(0, 2),
            Enumerable.Range(0, 2),
            Enumerable.Range(0, 2),
            Enumerable.Range(0, 2),
            Enumerable.Range(0, 2),
            Enumerable.Range(0, 2),
            Enumerable.Range(0, 2),
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
    }
}