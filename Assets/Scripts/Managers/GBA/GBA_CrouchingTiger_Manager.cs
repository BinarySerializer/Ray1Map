using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace R1Engine
{
    public class GBA_CrouchingTiger_Manager : GBA_Shanghai_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 50) // TODO: Get count
        };

        public override int[] MenuLevels => new int[0];
        public override int DLCLevelCount => 0;
        public override int[] AdditionalSprites4bpp => new int[0];
        public override int[] AdditionalSprites8bpp => new int[0];

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();

        protected override BaseColor[] GetSpritePalette(GBA_BatmanVengeance_Puppet puppet, GBA_Data data) => data.Shanghai_Scene.ObjPal.Palette;
    }
}