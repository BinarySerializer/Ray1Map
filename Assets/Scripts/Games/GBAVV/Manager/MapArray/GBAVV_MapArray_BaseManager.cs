using System.Linq;
using BinarySerializer;
using Cysharp.Threading.Tasks;


namespace Ray1Map.GBAVV
{
    public abstract class GBAVV_MapArray_BaseManager : GBAVV_BaseManager
    {
        // Metadata
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, LevelsCount).ToArray()),
        });
        public abstract int LevelsCount { get; }
        public virtual bool HasAssignedObjTypeGraphics => false;

        // Exports
        public override GBAVV_BaseROM LoadROMForExport(Context context) => FileFactory.Read<GBAVV_ROM_MapArray>(GetROMFilePath, context);
        public override UniTask ExportCutscenesAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();

        // Load
        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            if (GraphicsDataPointers.Length == 0)
                FindDataInROM(context.Deserializer);

            if (ObjTypeInitInfos == null && ObjTypesCount > 0)
                LogObjTypeInit(context.Deserializer);

            Controller.DetailedState = "Loading data";
            await Controller.WaitIfNecessary();

            var rom = FileFactory.Read<GBAVV_ROM_MapArray>(GetROMFilePath, context);

            return await LoadMap2DAsync(context, rom, rom.CurrentMap, HasAssignedObjTypeGraphics);
        }
    }
}