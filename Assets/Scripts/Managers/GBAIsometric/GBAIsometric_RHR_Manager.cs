using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace R1Engine
{
    public class GBAIsometric_RHR_Manager : IGameManager
    {
        public const int CellSize = 8;

        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 20).ToArray()), 
        });

        public virtual string GetROMFilePath => $"ROM.gba";

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
                new GameAction("Export Music & Sample Data", false, true, (input, output) => ExportMusicAsync(settings, output))
        };

        public async UniTask ExportMusicAsync(GameSettings settings, string outputPath) {
            using (var context = new Context(settings)) {
                var s = context.Deserializer;

                void ExportSampleSigned(string directory, string filename, sbyte[] data, uint sampleRate, ushort channels) {
                    // Create the directory
                    Directory.CreateDirectory(directory);

                    byte[] unsignedData = data.Select(b => (byte)(b + 128)).ToArray();

                    // Create WAV data
                    var formatChunk = new WAVFormatChunk() {
                        ChunkHeader = "fmt ",
                        FormatType = 1,
                        ChannelCount = channels,
                        SampleRate = sampleRate,
                        BitsPerSample = 8,
                    };

                    var wav = new WAV {
                        Magic = "RIFF",
                        FileTypeHeader = "WAVE",
                        Chunks = new WAVChunk[]
                        {
                            formatChunk,
                            new WAVChunk()
                            {
                                ChunkHeader = "data",
                                Data = unsignedData
                            }
                        }
                    };

                    formatChunk.ByteRate = (formatChunk.SampleRate * formatChunk.BitsPerSample * formatChunk.ChannelCount) / 8;
                    formatChunk.BlockAlign = (ushort)((formatChunk.BitsPerSample * formatChunk.ChannelCount) / 8);

                    // Get the output path
                    var outputFilePath = Path.Combine(directory, filename + ".wav");

                    // Create and open the output file
                    using (var outputStream = File.Create(outputFilePath)) {
                        // Create a context
                        using (var wavContext = new Context(settings)) {
                            // Create a key
                            const string wavKey = "wav";

                            // Add the file to the context
                            wavContext.AddFile(new StreamFile(wavKey, outputStream, wavContext));

                            // Write the data
                            FileFactory.Write<WAV>(wavKey, wav, wavContext);
                        }
                    }
                }

                await LoadFilesAsync(context);

                Pointer ptr = context.FilePointer(GetROMFilePath);
                var pointerTable = PointerTables.GBAIsometric_PointerTable(s.GameSettings.GameModeSelection, ptr.file);
                MusyX_File musyxFile = null;
                s.DoAt(pointerTable[GBAIsometric_Pointer.MusyxFile], () => {
                    musyxFile = s.SerializeObject<MusyX_File>(musyxFile, name: nameof(musyxFile));
                });
                string outPath = outputPath + "/Sounds/";
                for (int i = 0; i < musyxFile.SampleTable.Value.Samples.Length; i++) {
                    var e = musyxFile.SampleTable.Value.Samples[i].Value;
                    //Util.ByteArrayToFile(outPath + $"{i}_{e.Offset.AbsoluteOffset:X8}.bin", e.SampleData);
                    ExportSampleSigned(outPath, $"{i}_{musyxFile.SampleTable.Value.Samples[i].pointer.SerializedOffset:X8}", e.SampleData, e.SampleRate, 1);
                }
                outPath = outputPath + "/SongData/";
                for (int i = 0; i < musyxFile.SongTable.Value.Length; i++) {
                    var songBytes = musyxFile.SongTable.Value.SongBytes[i];
                    Util.ByteArrayToFile(outPath + $"{i}_{musyxFile.SongTable.Value.Songs[i].SerializedOffset:X8}.son", songBytes);
                }
                outPath = outputPath + "/InstrumentData/";
                for (int i = 0; i < musyxFile.InstrumentTable.Value.Instruments.Length; i++) {
                    var instrumentBytes = musyxFile.InstrumentTable.Value.InstrumentBytes[i];
                    Util.ByteArrayToFile(outPath + $"{i}_{musyxFile.InstrumentTable.Value.Instruments[i].SerializedOffset:X8}.bin", instrumentBytes);
                }
            }
        }

        public UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            // Read the rom
            var rom = FileFactory.Read<GBAIsometric_ROM>(GetROMFilePath, context);

            var levelInfo = rom.LevelInfos[context.Settings.Level];
            var levelData = levelInfo.LevelDataPointer.Value;

            var maps = levelData.MapLayers.Select(x =>
            {
                var width = (ushort)(x.DataPointer.Value.Width * (64 / CellSize));
                var height = (ushort)(x.DataPointer.Value.Height * (64 / CellSize));

                return new Unity_Map
                {
                    Width = width,
                    Height = height,
                    TileSetWidth = 1,
                    TileSet = new Unity_MapTileMap[]
                    {
                        new Unity_MapTileMap(CellSize),
                    },
                    MapTiles = Enumerable.Repeat(new Unity_Tile(new MapTile()), width * height).ToArray(),
                };
            }).ToArray();

            var objManager = new Unity_ObjectManager_GBAIsometric(context, rom.ObjectTypes, levelData.ObjectsCount);

            var allObjects = new List<Unity_Object>();

            // Add normal objects
            allObjects.AddRange(levelData.Objects.Select(x => (Unity_Object)new Unity_Object_GBAIsometric(x, objManager)));

            // Add waypoints
            allObjects.AddRange(levelData.Waypoints.Select(x => (Unity_Object)new Unity_Object_GBAIsometricWaypoint(x, objManager)));

            // Add localization
            var loc = rom.Localization.Localization.Select((x, i) => new
            {
                key = rom.Localization.Localization[0][i],
                strings = x
            }).ToDictionary(x => x.key, x => x.strings);

            return UniTask.FromResult(new Unity_Level(
                maps: maps, 
                objManager: objManager,
                eventData: allObjects,
                cellSize: CellSize,
                localization: loc));
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public virtual async UniTask LoadFilesAsync(Context context) => await context.AddGBAMemoryMappedFile(GetROMFilePath, 0x08000000);
    }
}