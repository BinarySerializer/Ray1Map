using System.Linq;
using BinarySerializer;
using Cysharp.Threading.Tasks;


namespace R1Engine
{
    public class GBAVV_SpyMuppetsLicenseToCroak_Manager : GBAVV_BaseManager
    {
        // Metadata
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 0).ToArray()),
        });

        // Exports
        public override GBAVV_BaseROM LoadROMForExport(Context context) => FileFactory.Read<GBAVV_ROM_Default>(GetROMFilePath, context);
        public override UniTask ExportCutscenesAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();

        // Graphics
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x08024718,
            0x08025D7C,
            0x0802CF24,
            0x08031D78,
            0x08038814,
            0x08047144,
            0x0804BD28,
            0x080521BC,
            0x080574E0,
            0x0805C8E8,
            0x08063A94,
            0x08067018,
            0x080697B0,
            0x0807AAD4,
            0x0807BCC4,
            0x0807C80C,
        };

        // Load
        public override UniTask<Unity_Level> LoadAsync(Context context)
        {
            // All maps are hard-coded
            throw new System.NotImplementedException();
        }
    }
}