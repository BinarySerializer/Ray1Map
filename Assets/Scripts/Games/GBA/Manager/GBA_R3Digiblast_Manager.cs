using BinarySerializer;
using Cysharp.Threading.Tasks;
using System.IO;
using System.Linq;

namespace Ray1Map.GBA
{
    public class GBA_R3Digiblast_Manager : GBA_R3_Manager {
        public override string GetROMFilePath(Context context) => $"Rayman";

        public override int DLCLevelCount => 0;
        public override bool HasR3SinglePakLevel => false;

        public override GBA_Data LoadDataBlock(Context context) => FileFactory.Read<GBA_Data>(GetROMFilePath(context), context);
        public override GBA_Localization LoadLocalizationTable(Context context) => FileFactory.Read<GBA_Data>(GetROMFilePath(context), context).Localization;

        public override async UniTask LoadFilesAsync(Context context) => await context.AddMemoryMappedFile(GetROMFilePath(context), 0x8000);

        // Game actions
        public override GameAction[] GetGameActions(GameSettings settings) {
            return base.GetGameActions(settings).Concat(new GameAction[]
            {
                new GameAction("Export Music", false, true, (input, output) => ExportMusicAsync(settings, output)),
            }).ToArray();
        }

        public async UniTask ExportMusicAsync(GameSettings settings, string outputPath) {
            using (var context = new Ray1MapContext(settings)) {
                string soundDirectory = "Rayman2/Data/Sound/mp2000/";
                string midiDirectory = soundDirectory + "midi/";
                string sampleDirectory = midiDirectory + "instruments/";

                foreach (var xmPath in Directory.EnumerateFiles(context.BasePath + midiDirectory, "*.xm", SearchOption.TopDirectoryOnly)) {
                    //if(!xmPath.Contains("win") && !xmPath.Contains("sadslide")) continue;
                    //if (!xmPath.Contains("bigplatform")) continue;
                    await LoadXM(xmPath);
                }
                async UniTask<short[]> ReadSample(string sampleName) {
                    string fileName = sampleDirectory + sampleName;
                    BinaryFile file = null;
                    if (!context.FileExists(fileName)) {
                        file = await context.AddLinearFileAsync(fileName, Endian.Little);
                    } else {
                        file = context.GetFile(fileName);
                    }
                    Array<short> sample = FileFactory.Read<Array<short>>(fileName, context, onPreSerialize: (s, f) => f.Length = s.CurrentLength / 2);
                    return sample.Value;
                }

                async UniTask LoadXM(string filePath) {
                    string fileName = filePath.Substring(context.BasePath.Length).Replace("\\", "/");
                    BinaryFile file = null;
                    if (!context.FileExists(fileName)) {
                        file = await context.AddLinearFileAsync(fileName, Endian.Little);
                    } else {
                        file = context.GetFile(fileName);
                    }
                    XM xm = FileFactory.Read<XM>(fileName, context);
                    foreach (var inst in xm.Instruments) {
                        if (inst.NumSamples == 0) {
                            inst.InstrumentSize = 29;
                            continue;
                        } else {
                            inst.InstrumentSize = 243;
                            foreach (var sam in inst.Samples) {
                                if (BitHelpers.ExtractBits(sam.Type, 1, 4) == 1) {
                                    if (sam.SampleLength > 0) continue;
                                    sam.SampleData16 = await ReadSample(sam.SampleName);
                                    sam.SampleLength = (uint)sam.SampleData16.Length * 2;
                                } else {
                                    throw new System.Exception("Byte samples are not supported");
                                }
                            }
                        }
                    }
                    // Get the output path
                    var name = Path.GetFileName(fileName);
                    var outputFilePath = Path.Combine(outputPath, name);

                    using (var outputStream = File.Create(outputFilePath)) {
                        using (var xmContext = new Ray1MapContext(outputPath, settings)) {
                            xmContext.AddFile(new StreamFile(context, name, outputStream));
                            FileFactory.Write<XM>(name, xm, xmContext);
                        }
                    }
                }
            }
        }
    }
}