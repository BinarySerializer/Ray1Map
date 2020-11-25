using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System.Linq;

namespace R1Engine
{
    public class GBC_R1_Manager : GBC_BaseManager
    {
        public string GetROMFilePath => "ROM.gbc";

        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 47).ToArray()), 
        });

        public override async UniTask<GBC_SceneList> GetSceneListAsync(Context context)
        {
            var file = await context.AddLinearSerializedFileAsync(GetROMFilePath);
            var pointerTable = PointerTables.GBC_PointerTable(context.Settings.GameModeSelection, file);
            var s = context.Deserializer;
            return s.DoAt(pointerTable[GBC_R1_Pointer.SceneList], () => s.SerializeObject<GBC_SceneList>(default, name: "SceneList"));
        }
        public override ARGBColor[] GetTilePalette(GBC_Scene scene) => new ARGBColor[0]; // TODO: Implement
    }
}