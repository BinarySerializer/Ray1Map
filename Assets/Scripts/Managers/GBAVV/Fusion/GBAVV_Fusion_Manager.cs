using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace R1Engine
{
    public abstract class GBAVV_Fusion_Manager : GBAVV_BaseManager
    {
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, LevInfos.Length).ToArray()),
        });

        public override GBAVV_BaseROM LoadROMForExport(Context context) => FileFactory.Read<GBAVV_ROM_Fusion>(GetROMFilePath, context, (s, r) => r.CurrentLevInfo = LevInfos[context.Settings.Level]);
        public override async UniTask ExportCutscenesAsync(GameSettings settings, string outputDir)
        {
            using (var context = new Context(settings))
            {
                await LoadFilesAsync(context);

                // Read the rom
                var rom = FileFactory.Read<GBAVV_ROM_Fusion>(GetROMFilePath, context, (s, r) =>
                {
                    r.CurrentLevInfo = LevInfos[context.Settings.Level];
                    r.SerializeFLC = true;
                });

                ExportCutscenesFromScripts(rom.GetAllScripts, outputDir);
            }

            Debug.Log($"Finished export");
        }

        public override async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            //FindDataInROM(context.Deserializer, context.FilePointer(GetROMFilePath));
            //LogLevelInfos(FileFactory.Read<GBAVV_ROM>(GetROMFilePath, context, (s, r) => r.CurrentLevInfo = LevInfos[context.Settings.Level]));
            //LogObjTypeInit(context.Deserializer);
            //LogObjTypeInit(context.Deserializer, new ObjTypeInitCreation[0]);
            //await LogInvalidObjTypesAsync(context.Settings);

            Controller.DetailedState = "Loading data";
            await Controller.WaitIfNecessary();

            var rom = FileFactory.Read<GBAVV_ROM_Fusion>(GetROMFilePath, context, (s, r) => r.CurrentLevInfo = LevInfos[context.Settings.Level]);

            return await LoadMap2DAsync(context, rom, rom.CurrentMap);
        }

        public override void FindDataInROM(SerializerObject s, Pointer offset)
        {
            // Read ROM as a uint array
            var values = s.DoAt(offset, () => s.SerializeArray<uint>(default, s.CurrentLength / 4, name: "Values"));

            // Helper for getting a pointer
            long getPointer(int index) => GBA_ROMBase.Address_ROM + index * 4;

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
                if (values[i] == 5 && values[i + 1] == 1 && values[i + 2] > GBA_ROMBase.Address_ROM && values[i + 2] < GBA_ROMBase.Address_ROM + s.CurrentLength)
                {
                    // Serialize the script
                    var script = s.DoAt(new Pointer((uint)getPointer(i), offset.file), () => s.SerializeObject<GBAVV_Script>(default));

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

        public void LogObjTypeInit(SerializerObject s)
        {
            // Load the animations
            var graphics = new GBAVV_Graphics();
            graphics.Init(s.Context.FilePointer(GetROMFilePath));
            graphics.SerializeImpl(s);
            var animSets = graphics.AnimSets;

            var str = new StringBuilder();

            var initFunctionPointers = s.DoAt(new Pointer(ObjTypesPointer, s.Context.GetFile(GetROMFilePath)), () => s.SerializePointerArray(default, ObjTypesCount));
            var orderedPointers = initFunctionPointers.OrderBy(x => x.AbsoluteOffset).ToArray(); ;

            // Enumerate every obj init function
            for (int i = 0; i < initFunctionPointers.Length; i++)
            {
                var nextPointer = orderedPointers.ElementAtOrDefault(orderedPointers.FindItemIndex(x => x == initFunctionPointers[i]) + 1);

                s.DoAt(initFunctionPointers[i], () =>
                {
                    var foundPointer = false;

                    // Try and read every int as a pointer until we get a valid one 20 times
                    for (int j = 0; j < 20; j++)
                    {
                        if (nextPointer != null && s.CurrentPointer.AbsoluteOffset >= nextPointer.AbsoluteOffset)
                            break;

                        var p = s.SerializePointer(default);

                        // First we check if the pointer leads directly to an animation
                        tryParseAnim(p);

                        if (foundPointer)
                            return;

                        // If not we assume it leads to a struct with the animation pointer
                        s.DoAt(p, () =>
                        {
                            // First pointer here should lead to an animation
                            var animPointer = s.SerializePointer(default);

                            tryParseAnim(animPointer);
                        });

                        if (foundPointer)
                            return;

                        // Spyro has structs where the second value is the animation pointer
                        s.DoAt(p, () =>
                        {
                            s.Serialize<int>(default);

                            var animPointer = s.SerializePointer(default);

                            tryParseAnim(animPointer);
                        });

                        void tryParseAnim(Pointer ap)
                        {
                            s.DoAt(ap, () =>
                            {
                                // If it's a valid animation the first pointer will lead to a pointer to itself
                                var animSetPointer = s.SerializePointer(default);

                                s.DoAt(animSetPointer, () =>
                                {
                                    var selfPointer = s.SerializePointer(default);

                                    if (selfPointer == animSetPointer)
                                    {
                                        // Sometimes the pointer after the animation pointer leads to a script, so we check that
                                        var scriptPointer = s.DoAt(p + 4, () => s.SerializePointer(default));

                                        // Attempt to get the script name
                                        var scriptName = s.DoAt(scriptPointer, () =>
                                        {
                                            var primary = s.Serialize<int>(default);
                                            var secondary = s.Serialize<int>(default);
                                            var namePointer = s.SerializePointer(default);

                                            if (primary != 5 || secondary != 1 || namePointer == null)
                                                return null;
                                            else
                                                return s.DoAt(namePointer, () => s.SerializeString(default));
                                        });

                                        var animSetIndex = animSets.FindItemIndex(x => x.Offset == animSetPointer);
                                        var animIndex = animSets[animSetIndex].Animations.FindItemIndex(x => x.Offset == ap);

                                        str.AppendLine($"new ObjTypeInit({animSetIndex}, {animIndex}, {(scriptName == null ? "null" : $"\"{scriptName}\"")}), // {i}");
                                        foundPointer = true;
                                    }
                                });
                            });
                        }

                        if (foundPointer)
                            return;
                    }

                    // No pointer found...
                    str.AppendLine($"new ObjTypeInit(-1, -1, null), // {i}");
                });
            }

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

                using (var context = new Context(settings))
                {
                    await LoadFilesAsync(context);

                    var rom = FileFactory.Read<GBAVV_ROM_Fusion>(GetROMFilePath, context, (s, r) => r.CurrentLevInfo = LevInfos[context.Settings.Level]);
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

        public abstract int ObjTypesCount { get; }
        public abstract uint ObjTypesPointer { get; }
        public abstract ObjTypeInit[] ObjTypeInitInfos { get; }
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

        public class ObjTypeInit
        {
            public ObjTypeInit(int animSetIndex, int animIndex, string scriptName, int? jpAnimIndex = null)
            {
                AnimSetIndex = animSetIndex;
                AnimIndex = animIndex;
                ScriptName = scriptName;
                JPAnimIndex = jpAnimIndex;
            }

            public int AnimSetIndex { get; }
            public int AnimIndex { get; }
            public string ScriptName { get; }
            public int? JPAnimIndex { get; }
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