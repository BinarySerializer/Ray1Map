using BinarySerializer;
using Cysharp.Threading.Tasks;


namespace R1Engine
{
    public class GBA_R3Digiblast_Manager : GBA_R3_Manager
    {
        public override string GetROMFilePath(Context context) => $"Rayman";

        public override int DLCLevelCount => 0;
        public override bool HasR3SinglePakLevel => false;

        public override GBA_Data LoadDataBlock(Context context) => FileFactory.Read<GBA_Data>(GetROMFilePath(context), context);
        public override GBA_LocLanguageTable LoadLocalizationTable(Context context) => null;

        public override async UniTask LoadFilesAsync(Context context) => await context.AddLinearSerializedFileAsync(GetROMFilePath(context));
    }
}