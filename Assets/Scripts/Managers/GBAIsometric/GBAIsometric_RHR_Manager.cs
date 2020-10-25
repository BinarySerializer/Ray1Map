using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;

namespace R1Engine
{
    public class GBAIsometric_RHR_Manager : IGameManager
    {
        public const int CellSize = 8;

        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {

        });

        public virtual string GetROMFilePath => $"ROM.gba";

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {

        };

        public UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            throw new NotImplementedException();
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public virtual async UniTask LoadFilesAsync(Context context) => await context.AddGBAMemoryMappedFile(GetROMFilePath, 0x08000000);
    }
}