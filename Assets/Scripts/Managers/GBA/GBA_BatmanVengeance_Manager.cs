using Cysharp.Threading.Tasks;

namespace R1Engine
{
    public class GBA_BatmanVengeance_Manager : GBA_Manager
    {
        // TODO: Get count
        public override int LevelCount => 1;
        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();
    }
}