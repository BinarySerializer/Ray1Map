using Cysharp.Threading.Tasks;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using BinarySerializer.GBA;
using UnityEngine;

namespace R1Engine
{
    public class GBA_R3NGage_Manager : GBA_R3_Manager
    {
        public override string GetROMFilePath(Context context) => $"rayman3.dat";
        public string ExeFilePath => "rayman3.app";

        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 8), // World 1
            Enumerable.Range(8, 9), // World 2
            Enumerable.Range(17, 13), // World 3
            Enumerable.Range(30, 10), // World 4
            Enumerable.Range(40, 8), // Bonus
            Enumerable.Range(48, 6), // Ly
            Enumerable.Range(54, 5), // World
            Enumerable.Range(59, 10), // Multiplayer
        };

        public override int[] MenuLevels => new int[]
        {
            103,
            131
        };

        public override int DLCLevelCount => 0;
        public override bool HasR3SinglePakLevel => false;

        public override int[] AdditionalSprites4bpp => Enumerable.Range(79, 103 - 79).Concat(Enumerable.Range(104, 131 - 104)).Concat(Enumerable.Range(133, 140 - 133)).ToArray();

        public override int[] AdditionalSprites8bpp => new int[]
        {
            132
        };

        public override async UniTask ExtractVignetteAsync(GameSettings settings, string outputDir)
        {
            // Create a context
            using (var context = new R1Context(settings))
            {
                // Load the ROM
                await LoadFilesAsync(context);

                // Load the data
                var dataBlock = LoadDataBlock(context);

                // Get the deserialize
                var s = context.Deserializer;

                const int vignetteCount = 22;
                const int vignetteStart = 143;
                const int palStart = 165;

                for (int i = 0; i < vignetteCount; i++)
                {
                    // Decode image data
                    byte[] data = null;
                    s.DoAt(dataBlock.UiOffsetTable.GetPointer(vignetteStart + i), () => s.DoEncoded(new GBA_LZSSEncoder(), () => data = s.SerializeArray<byte>(default, s.CurrentLength)));

                    // Read palette
                    var palette = s.DoAt(dataBlock.UiOffsetTable.GetPointer(palStart + i), () => s.SerializeObjectArray<RGBA5551Color>(default, 256));

                    // Create a texture
                    var tex = TextureHelpers.CreateTexture2D(176, 208);

                    // Set pixels
                    for (int y = 0; y < tex.height; y++)
                    {
                        for (int x = 0; x < tex.width; x++)
                        {
                            var c = palette[data[y * tex.width + x]].GetColor();

                            // Remove transparency
                            c.a = 1;

                            // Set pixel and reverse height
                            tex.SetPixel(x, tex.height - y - 1, c);
                        }
                    }

                    tex.Apply();

                    // Export
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"Vig_{i}.png"), tex.EncodeToPNG());
                }

                ExtractVignetteCreditsIcons(context, dataBlock, 141, outputDir);
            }
        }

        public override GBA_Data LoadDataBlock(Context context) => FileFactory.Read<GBA_Data>(GetROMFilePath(context), context);
        public override GBA_LocLanguageTable LoadLocalizationTable(Context context) => FileFactory.Read<GBA_R3Ngage_ExeFile>(ExeFilePath, context).Localization;

        public override async UniTask LoadFilesAsync(Context context)
        {
            await context.AddLinearSerializedFileAsync(GetROMFilePath(context));
            await context.AddMemoryMappedFile(ExeFilePath, 0x0fffff84);
        }

        // NOTE: In order to use this the FileSystem class needs to be updated to allow file stream sharing for read/write streams. Also note that this method will overwrite the GBA rom file, so keep a backup! All N-Gage data is appended rather than replaced, so the data can be swapped out by changing the offsets. Only the level offsets are automatically changed to use the N-Gage versions, thus keeping the menu etc.
        public async UniTask AddNGageToGBAAsync(GameModeSelection gbaGameModeSelection)
        {
            // Create a GBA manager
            var gbaManager = new GBA_R3_Manager();

            // Create a GBA context
            using (var gbaContext = new R1Context(new GameSettings(gbaGameModeSelection, Settings.GameDirectories[gbaGameModeSelection], 0, 0)))
            {
                // Load GBA files
                await gbaManager.LoadFilesAsync(gbaContext);

                // Create an N-Gage context
                using (var ngageContext = new R1Context(new GameSettings(GameModeSelection.Rayman3NGage, Settings.GameDirectories[GameModeSelection.Rayman3NGage], 0, 0)))
                {
                    // Load N-Gage files
                    await LoadFilesAsync(ngageContext);

                    // Read the GBA data
                    var gbaData = gbaManager.LoadDataBlock(gbaContext);

                    // Read N-Gage data
                    var ngageData = LoadDataBlock(ngageContext);

                    // Get offsets
                    var gbaBase = gbaData.UiOffsetTable.Offset;
                    var ngageNewBase = ((MemoryMappedFile)gbaContext.GetFile(gbaManager.GetROMFilePath(gbaContext))).StartPointer + ((MemoryMappedFile)gbaContext.GetFile(gbaManager.GetROMFilePath(gbaContext))).Length;
                    var ngageOffset = (ngageNewBase - gbaBase) / 4;

                    // Read all N-Gage blocks
                    var s = ngageContext.Deserializer;
                    var ngageBlocks = Enumerable.Range(0, ngageData.UiOffsetTable.OffsetsCount).Select(x => ngageData.UiOffsetTable.GetPointer(x)).Select(x => s.DoAt(x, () => s.SerializeObject<GBA_DummyBlock>(default))).ToArray();

                    // Keep track of relocated blocks
                    var relocated = new HashSet<GBA_DummyBlock>();

                    // Recursively relocate all offsets
                    foreach (var block in ngageBlocks)
                        RelocateBlock(block);

                    void RelocateBlock(GBA_DummyBlock block)
                    {
                        // Don't relocate blocks which have been relocated
                        if (relocated.Contains(block))
                            return;
                        relocated.Add(block);

                        // Relocate all sub-blocks
                        foreach (var subBlock in block.SubBlocks)
                            RelocateBlock(subBlock);

                        // Relocate offsets
                        for (int i = 0; i < block.OffsetTable.Offsets.Length; i++)
                            block.OffsetTable.Offsets[i] = (int)(block.OffsetTable.Offsets[i] + ngageOffset);

                        // Update the offsets for the R1Serializable
                        block.Init(ngageNewBase + block.Offset.FileOffset);
                        block.OffsetTable.Init(ngageNewBase + block.Offset.FileOffset + block.BlockSize);
                    }

                    // Write the blocks
                    var ss = ngageContext.Serializer;
                    foreach (var block in ngageBlocks)
                        ss.DoAt(block.Offset, () => ss.SerializeObject(block));

                    // Update level offsets
                    for (int i = 0; i < 65; i++)
                        ss.DoAt(gbaBase + 4 + (i * 4), () => ss.Serialize<int>((int)((ngageBlocks[i].Offset.FileOffset - gbaBase.FileOffset) / 4)));
                }
            }
        }

        public override string[] Languages => new string[]
        {
            "English",
            "EnglishUS",
            "French",
            "Spanish",
            "Italian",
            "German",
        };
    }
}