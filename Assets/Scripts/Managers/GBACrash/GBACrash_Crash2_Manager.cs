using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Linq;

namespace R1Engine
{
    public class GBACrash_Crash2_Manager : IGameManager
    {
        public const string LocTableID = "LocTable";

        public const int CellSize = 8;
        public string GetROMFilePath => "ROM.gba";

        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 1).ToArray()),
        });

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[0];

        public UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            var rom = FileFactory.Read<GBACrash_ROM>(GetROMFilePath, context);

            throw new NotImplementedException();
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public async UniTask LoadFilesAsync(Context context) => await context.AddGBAMemoryMappedFile(GetROMFilePath, GBA_ROMBase.Address_ROM);
    }
}