using Cysharp.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;
using BinarySerializer.GBA;
using UnityEngine;

namespace R1Engine
{
    public abstract class GBAVV_Fusion_Manager : GBAVV_BaseManager
    {
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, LevInfos.Length).ToArray()),
        });

        public override GBAVV_BaseROM LoadROMForExport(Context context) => FileFactory.Read<GBAVV_ROM_Fusion>(GetROMFilePath, context, (s, r) => r.CurrentLevInfo = LevInfos[context.GetR1Settings().Level]);
        public override async UniTask ExportCutscenesAsync(GameSettings settings, string outputDir)
        {
            using (var context = new R1Context(settings))
            {
                await LoadFilesAsync(context);

                // Read the rom
                var rom = FileFactory.Read<GBAVV_ROM_Fusion>(GetROMFilePath, context, (s, r) =>
                {
                    r.CurrentLevInfo = LevInfos[context.GetR1Settings().Level];
                    r.SerializeFLC = true;
                });

                ExportCutscenesFromScripts(rom.GetAllScripts, outputDir);
            }

            Debug.Log($"Finished export");
        }

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            //FindDataInROM(context.Deserializer, context.FilePointer(GetROMFilePath));
            //LogLevelInfos(FileFactory.Read<GBAVV_ROM>(GetROMFilePath, context, (s, r) => r.CurrentLevInfo = LevInfos[context.GetR1Settings().Level]));
            //LogObjTypeInit(context.Deserializer);
            //LogObjTypeInit(context.Deserializer, new ObjTypeInitCreation[0]);
            //await LogInvalidObjTypesAsync(context.GetR1Settings());

            Controller.DetailedState = "Loading data";
            await Controller.WaitIfNecessary();

            var rom = FileFactory.Read<GBAVV_ROM_Fusion>(GetROMFilePath, context, (s, r) => r.CurrentLevInfo = LevInfos[context.GetR1Settings().Level]);

            return await LoadMap2DAsync(context, rom, rom.CurrentMap);
        }

        public override void FindDataInROM(SerializerObject s, Pointer offset)
        {
            // Read ROM as a uint array
            var values = s.DoAt(offset, () => s.SerializeArray<uint>(default, s.CurrentLength / 4, name: "Values"));

            // Helper for getting a pointer
            long getPointer(int index) => GBAConstants.Address_ROM + index * 4;

            // Keep track of found data
            var foundAnimSets = new List<long>();
            var foundScripts = new List<Tuple<long, string>>();

            // Find animation sets by finding pointers which references itself
            for (int i = 0; i < values.Length; i++)
            {
                var p = getPointer(i);

                if (values[i] == p)
                    // We found a valid animation set!
                    foundAnimSets.Add(p);
            }

            // Find scripts by finding the name command which is always the first one
            for (int i = 0; i < values.Length - 2; i++)
            {
                if (values[i] == 5 && values[i + 1] == 1 && values[i + 2] > GBAConstants.Address_ROM && values[i + 2] < GBAConstants.Address_ROM + s.CurrentLength)
                {
                    // Serialize the script
                    var script = s.DoAt(new Pointer(getPointer(i), offset.File), () => s.SerializeObject<GBAVV_Script>(default));

                    // If the script is invalid we ignore it
                    if (!script.IsValid)
                    {
                        Debug.Log($"Skipping script {script.DisplayName}");
                        continue;
                    }

                    foundScripts.Add(new Tuple<long, string>(getPointer(i), script.Name));
                }
            }

            // Log found data to clipboard
            var str = new StringBuilder();

            str.AppendLine($"AnimSets:");

            foreach (var anim in foundAnimSets)
                str.AppendLine($"0x{anim:X8},");

            str.AppendLine();
            str.AppendLine($"Scripts:");

            foreach (var (p, name) in foundScripts)
                str.AppendLine($"0x{p:X8}, // {name}");

            str.ToString().CopyToClipboard();
        }

        public void LogLevelInfos(GBAVV_ROM_Fusion rom)
        {
            var str = new StringBuilder();

            for (int i = 0; i < rom.Crash_LevelInfos.Length; i++)
                str.AppendLine($"new LevInfo({i}, \"{rom.Crash_LevelInfos[i].LevelName.Items[0].Text}\"),");

            str.ToString().CopyToClipboard();
        }

        private void LogObjTypeInit(SerializerObject s, params ObjTypeInitCreation[] types)
        {
            var str = new StringBuilder();

            // Load the animations
            var graphics = new GBAVV_Graphics();
            graphics.Init(s.Context.FilePointer(GetROMFilePath));
            graphics.SerializeImpl(s);
            var animSets = graphics.AnimSets;

            foreach (var t in types)
            {
                var animSetIndex = animSets.FindItemIndex(x => x.Animations.Any(a => a.Offset.AbsoluteOffset == t.AnimPointer));
                var animIndex = animSets.ElementAtOrDefault(animSetIndex)?.Animations.FindItemIndex(x => x.Offset.AbsoluteOffset == t.AnimPointer) ?? -1;

                str.AppendLine($"new ObjTypeInit({animSetIndex}, {animIndex}, null), // {t.ObjType}");
            }

            str.ToString().CopyToClipboard();
        }

        public async UniTask LogInvalidObjTypesAsync(GameSettings settings)
        {
            var data = Enumerable.Range(0, ObjTypesCount).Select(x => new HashSet<int>()).ToArray();

            for (int i = 0; i < LevInfos.Length; i++)
            {
                settings.Level = i;

                using (var context = new R1Context(settings))
                {
                    await LoadFilesAsync(context);

                    var rom = FileFactory.Read<GBAVV_ROM_Fusion>(GetROMFilePath, context, (s, r) => r.CurrentLevInfo = LevInfos[context.GetR1Settings().Level]);
                    var objects = rom.CurrentMap.ObjData?.GetObjects;

                    if (objects == null)
                        continue;

                    foreach (var obj in objects)
                    {
                        var init = ObjTypeInitInfos[obj.ObjType];

                        if (init.AnimIndex == -1)
                            data[obj.ObjType].Add(i);
                    }
                }
            }

            var str = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Any())
                    str.AppendLine($"{i:000}: {String.Join(", ", data[i].Select(x => LevInfos[x].LevelIndex))} ({String.Join(", ", data[i].Select(x => LevInfos[x].DisplayName))})");
            }

            str.ToString().CopyToClipboard();
        }

        public abstract int DialogScriptsCount { get; }
        public abstract byte[] HardCodedScripts { get; }

        public abstract FusionLevInfo[] LevInfos { get; }

        private class ObjTypeInitCreation
        {
            public ObjTypeInitCreation(int objType, uint animPointer)
            {
                ObjType = objType;
                AnimPointer = animPointer;
            }

            public int ObjType { get; }
            public uint AnimPointer { get; }
        }

        public class FusionLevInfo
        {
            public FusionLevInfo(int levelIndex, string displayName, FusionType fusionType = FusionType.Normal)
            {
                LevelIndex = levelIndex;
                DisplayName = displayName;
                Fusion_Type = fusionType;
            }

            public int LevelIndex { get; }
            public FusionType Fusion_Type { get; }
            public string DisplayName { get; set; }

            public enum FusionType
            {
                Normal,
                LevTime,
                LevInt,
                LevIntInt,
                IntLevel,
                Unknown
            }
        }
    }
}