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
        public override UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            //FindDataInROM(context.Deserializer, context.FilePointer(GetROMFilePath));
            //LogParsedScripts(context.Deserializer, ScriptPointers.Select(x => new Pointer(x, context.GetFile(GetROMFilePath))));
            //LogLevelInfos(FileFactory.Read<GBAVV_ROM>(GetROMFilePath, context, (s, r) => r.CurrentLevInfo = LevInfos[context.Settings.Level]));
            //LogObjTypeInit(context.Deserializer);
            //LogObjTypeInit(context.Deserializer, new ObjTypeInitCreation[0]);
            return base.LoadAsync(context, loadTextures);
        }

        public void FindDataInROM(SerializerObject s, Pointer offset)
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

        public void LogParsedScripts(SerializerObject s, IEnumerable<Pointer> scriptPointers)
        {
            // Load the animations
            var graphics = new GBAVV_Map2D_Graphics();
            graphics.Init(s.Context.FilePointer(GetROMFilePath));
            graphics.SerializeImpl(s);
            var animSets = graphics.AnimSets;

            var str = new StringBuilder();

            // Start by parsing every script
            var scripts = scriptPointers.Select(x => s.DoAt(x, () => s.SerializeObject<GBAVV_Script>(default))).ToArray();

            // Enumerate every script
            foreach (var script in scripts)
            {
                foreach (var line in script.TranslatedString(animSets))
                    str.AppendLine(line);

                str.AppendLine();
            }

            str.ToString().CopyToClipboard();
        }

        public void LogLevelInfos(GBAVV_ROM rom)
        {
            var str = new StringBuilder();

            for (int i = 0; i < rom.LevelInfos.Length; i++)
                str.AppendLine($"new LevInfo({i}, \"{rom.LevelInfos[i].Fusion_LevelName.Item.Text}\"),");

            str.ToString().CopyToClipboard();
        }

        public void LogObjTypeInit(SerializerObject s)
        {
            // Load the animations
            var graphics = new GBAVV_Map2D_Graphics();
            graphics.Init(s.Context.FilePointer(GetROMFilePath));
            graphics.SerializeImpl(s);
            var animSets = graphics.AnimSets;

            var str = new StringBuilder();

            var initFunctionPointers = s.DoAt(new Pointer(ObjTypesPointer, s.Context.GetFile(GetROMFilePath)), () => s.SerializePointerArray(default, ObjTypesCount));

            // Enumerate every obj init function
            for (int i = 0; i < initFunctionPointers.Length; i++)
            {
                s.DoAt(initFunctionPointers[i], () =>
                {
                    var foundPointer = false;

                    // Try and read every int as a pointer until we get a valid one 20 times
                    for (int j = 0; j < 20; j++)
                    {
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
            var graphics = new GBAVV_Map2D_Graphics();
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

        public abstract int ObjTypesCount { get; }
        public abstract uint ObjTypesPointer { get; }
        public abstract ObjTypeInit[] ObjTypeInitInfos { get; }
        public abstract uint[] AnimSetPointers { get; }
        public abstract uint[] ScriptPointers { get; }
        public abstract Dictionary<int, GBAVV_ScriptCommand.CommandType> ScriptCommands { get; }

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
            public ObjTypeInit(int animSetIndex, int animIndex, string scriptName)
            {
                AnimSetIndex = animSetIndex;
                AnimIndex = animIndex;
                ScriptName = scriptName;
            }

            public int AnimSetIndex { get; }
            public int AnimIndex { get; }
            public string ScriptName { get; }
        }
    }
    public abstract class GBAVV_CrashFusion_Manager : GBAVV_Fusion_Manager
    {
        public override LevInfo[] LevInfos => Levels;

        public static LevInfo[] Levels = new LevInfo[]
        {
            new LevInfo(0, "World 1"),
            new LevInfo(1, "World 1b"),
            new LevInfo(2, "Grin and Bear it"),
            new LevInfo(3, "Grin and Bear it"),
            new LevInfo(4, "Grin and Bear it"),
            new LevInfo(5, "Sheep Stampede", LevInfo.FusionType.LevInt),
            new LevInfo(6, "Sheep Stampede", LevInfo.FusionType.LevInt),
            new LevInfo(7, "Tanks for the Memories"),
            new LevInfo(8, "Tanks for the Memories"),
            new LevInfo(9, "Chopper Stopper", LevInfo.FusionType.LevIntInt),
            new LevInfo(10, "Chopper Stopper", LevInfo.FusionType.LevIntInt),
            new LevInfo(11, "Bonus: Crate Smash", LevInfo.FusionType.LevTime),
            new LevInfo(12, "Bonus: Crate Smash", LevInfo.FusionType.LevTime),
            new LevInfo(13, "Bonus: Crate Smash", LevInfo.FusionType.LevTime),
            new LevInfo(14, "Bonus: Freefallin'"),
            new LevInfo(15, "Bonus: Freefallin'"),
            new LevInfo(16, "Bonus: Crunch Time", LevInfo.FusionType.IntLevel),
            new LevInfo(17, "Crashin' down the River"),
            new LevInfo(18, "Crashin' Down the River"),
            new LevInfo(19, "Crashin' Down the River"),
            new LevInfo(20, "Buy Card 1", LevInfo.FusionType.Unknown), // Pointer to 212 bytes
            new LevInfo(21, "Trading Card Shop", LevInfo.FusionType.Unknown), // Pointer to comp data, 5 ints
            new LevInfo(22, "Bridge Fight"),
            new LevInfo(23, "Spyro Battle"),
            new LevInfo(24, "Bridge Fight"),
            new LevInfo(25, "Bonus: Pumpin' Iron", LevInfo.FusionType.IntLevel),
            new LevInfo(26, "Bonus: Crate Step", LevInfo.FusionType.LevTime),
            new LevInfo(27, "Bonus: Crate Step", LevInfo.FusionType.LevTime),
            new LevInfo(28, "Bonus: Crate Step", LevInfo.FusionType.LevTime),
            new LevInfo(29, "Crash and Burn"),
            new LevInfo(30, "Crash and Burn"),
            new LevInfo(31, "World 2a"),
            new LevInfo(32, "World 2b"),
            new LevInfo(33, "Tiny Takeover"),
            new LevInfo(34, "Bonus: Crate Smash", LevInfo.FusionType.LevTime),
            new LevInfo(35, "Bonus: Crate Smash", LevInfo.FusionType.LevTime),
            new LevInfo(36, "Bonus: Crate Smash", LevInfo.FusionType.LevTime),
            new LevInfo(37, "Polar Express"),
            new LevInfo(38, "Polar Express"),
            new LevInfo(39, "Polar Express"),
            new LevInfo(40, "Frigid Waters"),
            new LevInfo(41, "Frigid Waters"),
            new LevInfo(42, "Frigid Waters"),
            new LevInfo(43, "Sheep Patrol", LevInfo.FusionType.LevInt),
            new LevInfo(44, "Sheep Patrol", LevInfo.FusionType.LevInt),
            new LevInfo(45, "Blizzard Ball"),
            new LevInfo(46, "Blizzard Ball"),
            new LevInfo(47, "Blizzard Ball"),
            new LevInfo(48, "Shell Game 2", LevInfo.FusionType.Unknown), // 5 ints, pointer to 132 bytes (same format as 'Buy Card 1')
            new LevInfo(49, "Mystery Game 2", LevInfo.FusionType.Unknown), // 3 ints, pointer to 72 bytes (same format as 'Buy Card 1')
            new LevInfo(50, "Spinning Wheel Game 2", LevInfo.FusionType.Unknown), // 4 ints, pointer to 580 bytes (same format as 'Buy Card 1')
            new LevInfo(51, "Crate Shuffle Shop", LevInfo.FusionType.Unknown), // Pointer to comp data, 5 ints
            new LevInfo(52, "Spinning Wheel Shop", LevInfo.FusionType.Unknown), // Pointer to comp data, 5 ints
            new LevInfo(53, "Mystery Shop", LevInfo.FusionType.Unknown), // Pointer to comp data, 5 ints
            new LevInfo(54, "Bridge Fight"),
            new LevInfo(55, "Tankin' over the world"),
            new LevInfo(56, "Tankin' over the world"),
            new LevInfo(57, "Chop 'til you Drop", LevInfo.FusionType.LevIntInt),
            new LevInfo(58, "Chop 'til you Drop", LevInfo.FusionType.LevIntInt),
            new LevInfo(59, "Rocket Power"),
            new LevInfo(60, "Rocket Power"),
            new LevInfo(61, "Bonus: Crate Step", LevInfo.FusionType.LevTime),
            new LevInfo(62, "Bonus: Crate Step", LevInfo.FusionType.LevTime),
            new LevInfo(63, "Bonus: Crate Step", LevInfo.FusionType.LevTime),
            new LevInfo(64, "World 3a"),
            new LevInfo(65, "World 3b"),
            new LevInfo(66, "Bat Attack"),
            new LevInfo(67, "Bat Attack"),
            new LevInfo(68, "Bat Attack"),
            new LevInfo(69, "In Hot Water"),
            new LevInfo(70, "In Hot Water"),
            new LevInfo(71, "In Hot Water"),
            new LevInfo(72, "Nina"),
            new LevInfo(73, "Bonus: Weightlift", LevInfo.FusionType.IntLevel),
            new LevInfo(74, "Spinning Wheel Game 3", LevInfo.FusionType.Unknown), // 4 ints, pointer to 580 bytes (same format as 'Buy Card 1')
            new LevInfo(75, "Spinning Wheel Shop", LevInfo.FusionType.Unknown), // Pointer to comp data, 5 ints
            new LevInfo(76, "Buy Card 3", LevInfo.FusionType.Unknown), // Pointer to 48 bytes
            new LevInfo(77, "Trading Card Shop", LevInfo.FusionType.Unknown), // Pointer to comp data, 5 ints
            new LevInfo(78, "Bridge Fight"),
            new LevInfo(79, "Bonus: Crate Smash", LevInfo.FusionType.LevTime),
            new LevInfo(80, "Bonus: Crate Smash", LevInfo.FusionType.LevTime),
            new LevInfo(81, "Bonus: Crate Smash", LevInfo.FusionType.LevTime),
            new LevInfo(82, "World 4a"),
            new LevInfo(83, "World 4b"),
            new LevInfo(84, "Castle Chaos"),
            new LevInfo(85, "Castle Chaos"),
            new LevInfo(86, "Castle Chaos"),
            new LevInfo(87, "Bats in the Belfry"),
            new LevInfo(88, "Bats in the Belfry"),
            new LevInfo(89, "Bats in the Belfry"),
            new LevInfo(90, "Sheep Shuttle", LevInfo.FusionType.LevInt),
            new LevInfo(91, "Sheep Shuttle", LevInfo.FusionType.LevInt),
            new LevInfo(92, "Up, up, and away"),
            new LevInfo(93, "Up, up, and away"),
            new LevInfo(94, "Bonus: Freefallin'"),
            new LevInfo(95, "Bonus: Freefallin'"),
            new LevInfo(96, "Bonus: Weightlift", LevInfo.FusionType.IntLevel),
            new LevInfo(97, "Tanks 'R Us"),
            new LevInfo(98, "Tanks 'R Us"),
            new LevInfo(99, "Buy Card 4", LevInfo.FusionType.Unknown), // Pointer to data
            new LevInfo(100, "Trading Card Shop", LevInfo.FusionType.Unknown), // Pointer to comp data, 5 ints
            new LevInfo(101, "Mystery Game 2", LevInfo.FusionType.Unknown), // 3 ints, pointer to bytes (same format as 'Buy Card 1')
            new LevInfo(102, "Mystery Shop", LevInfo.FusionType.Unknown), // Pointer to comp data, 5 ints
            new LevInfo(103, "Shell Game 4", LevInfo.FusionType.Unknown), // 5 ints, pointer to bytes (same format as 'Buy Card 1')
            new LevInfo(104, "Crate Shuffle Shop", LevInfo.FusionType.Unknown), // Pointer to comp data, 5 ints
            new LevInfo(105, "Ripto's Magical Mystery Tour"),
            new LevInfo(106, "Bonus: Weightlift", LevInfo.FusionType.IntLevel),
            new LevInfo(107, "World 5a"),
            new LevInfo(108, "Tech Deflect"),
            new LevInfo(109, "Tech Deflect"),
            new LevInfo(110, "Tech Deflect"),
            new LevInfo(111, "Crash at the Controls", LevInfo.FusionType.LevIntInt),
            new LevInfo(112, "Crash at the Controls", LevInfo.FusionType.LevIntInt),
            new LevInfo(113, "Bonus: Crate smash", LevInfo.FusionType.LevTime),
            new LevInfo(114, "Bonus: Crate smash", LevInfo.FusionType.LevTime),
            new LevInfo(115, "Bonus: Crate smash", LevInfo.FusionType.LevTime),
            new LevInfo(116, "Bear with Me"),
            new LevInfo(117, "Bear with Me"),
            new LevInfo(118, "Bear with Me"),
            new LevInfo(119, "Bat to the Future"),
            new LevInfo(120, "Bat to the Future"),
            new LevInfo(121, "Bat to the Future"),
            new LevInfo(122, "Tank You Come Again"),
            new LevInfo(123, "Tank You Come Again"),
            new LevInfo(124, "Bonus: Freefallin'"),
            new LevInfo(125, "Bonus: Freefallin'"),
            new LevInfo(126, "Spinning Wheel Game 5", LevInfo.FusionType.Unknown), // 4 ints, pointer to bytes (same format as 'Buy Card 1')
            new LevInfo(127, "Spinning Wheel Shop", LevInfo.FusionType.Unknown), // Pointer to comp data, 5 ints
            new LevInfo(128, "Bridge Fight"),
            new LevInfo(129, "Space Chase"),
            new LevInfo(130, "Crash 2 Test Level : Tiki Torture"),
            new LevInfo(131, "Jump, Crash, Jump!"),
            new LevInfo(132, "Cold Front"),
            new LevInfo(133, "Hot feet"),
            new LevInfo(134, "Moat Monster"),
            new LevInfo(135, "Wumpa Jump"),
        };

        public override ObjTypeInit[] ObjTypeInitInfos { get; } = new ObjTypeInit[]
        {
            new ObjTypeInit(-1, -1, null), // 0
            new ObjTypeInit(6, 0, null), // 1
            new ObjTypeInit(6, 52, null), // 2
            new ObjTypeInit(6, 69, null), // 3
            new ObjTypeInit(6, 72, null), // 4
            new ObjTypeInit(6, 88, null), // 5
            new ObjTypeInit(4, 14, null), // 6
            new ObjTypeInit(6, 98, null), // 7
            new ObjTypeInit(6, 106, null), // 8
            new ObjTypeInit(6, 118, null), // 9
            new ObjTypeInit(13, 5, null), // 10
            new ObjTypeInit(8, 23, null), // 11
            new ObjTypeInit(8, 9, null), // 12
            new ObjTypeInit(8, 17, null), // 13
            new ObjTypeInit(8, 3, null), // 14
            new ObjTypeInit(8, 4, null), // 15
            new ObjTypeInit(8, 35, null), // 16
            new ObjTypeInit(8, 26, null), // 17
            new ObjTypeInit(8, 24, null), // 18
            new ObjTypeInit(8, 11, null), // 19 // TODO: Fix
            new ObjTypeInit(8, 11, null), // 20 // TODO: Fix
            new ObjTypeInit(8, 13, null), // 21
            new ObjTypeInit(8, 25, null), // 22
            new ObjTypeInit(8, 7, null), // 23
            new ObjTypeInit(8, 31, null), // 24
            new ObjTypeInit(8, 38, null), // 25
            new ObjTypeInit(8, 23, null), // 26
            new ObjTypeInit(8, 9, null), // 27
            new ObjTypeInit(8, 31, null), // 28
            new ObjTypeInit(8, 7, null), // 29
            new ObjTypeInit(8, 24, null), // 30
            new ObjTypeInit(-1, -1, null), // 31
            new ObjTypeInit(-1, -1, null), // 32
            new ObjTypeInit(-1, -1, null), // 33
            new ObjTypeInit(9, 0, null), // 34
            new ObjTypeInit(9, 3, null), // 35
            new ObjTypeInit(9, 56, null), // 36
            new ObjTypeInit(0, 126, "fruitSpawnerScript"), // 37
            new ObjTypeInit(9, 0, null), // 38
            new ObjTypeInit(9, 0, null), // 39
            new ObjTypeInit(18, 11, null), // 40
            new ObjTypeInit(0, 0, "geckoPatrolScript"), // 41
            new ObjTypeInit(0, 157, "labAssPatrolScript"), // 42
            new ObjTypeInit(0, 10, "rhynocJunglePatrolScript"), // 43
            new ObjTypeInit(0, 49, "rhynocJungleFlyerScript"), // 44
            new ObjTypeInit(0, 3, "venusWhackerScript"), // 45
            new ObjTypeInit(0, 7, "sealPatrolScript"), // 46
            new ObjTypeInit(0, 27, "penguinAttackScript"), // 47
            new ObjTypeInit(0, 34, "polarbearPatrolScript"), // 48
            new ObjTypeInit(0, 134, "rhynocIceFlyerScript"), // 49
            new ObjTypeInit(0, 121, "rhynocIcePatrolScript"), // 50
            new ObjTypeInit(0, 83, "sharkeyPatrolScript"), // 51
            new ObjTypeInit(0, 103, "rhynocFireFlyerScript"), // 52
            new ObjTypeInit(0, 53, "infernoLabAssAttackScript"), // 53
            new ObjTypeInit(0, 54, "infernoLabAssWhackerScript"), // 54
            new ObjTypeInit(0, 62, "infernoLabAssShooterScript"), // 55
            new ObjTypeInit(0, 75, "batFlyerScript"), // 56
            new ObjTypeInit(0, 87, "rhynocCastlePatrolScript"), // 57
            new ObjTypeInit(0, 76, "wormWhackerScript"), // 58
            new ObjTypeInit(0, 97, "rhynocThrowScript"), // 59
            new ObjTypeInit(0, 93, "rhynocRockScript"), // 60
            new ObjTypeInit(0, 99, "goatPatrolScript"), // 61
            new ObjTypeInit(0, 109, "tankGuy1StaticScript"), // 62
            new ObjTypeInit(0, 107, "tankGuy1BStaticScript"), // 63
            new ObjTypeInit(0, 126, "sheepSpawnerScript"), // 64
            new ObjTypeInit(0, 138, "chuteGuyScript"), // 65
            new ObjTypeInit(0, 225, "chuteGuyFasterScript"), // 66
            new ObjTypeInit(0, 161, "chuteGuyShooterScript"), // 67
            new ObjTypeInit(0, 229, "chuteGuyShooterFasterScript"), // 68
            new ObjTypeInit(0, 161, "chuteGuyShieldScript"), // 69
            new ObjTypeInit(0, 229, "chuteGuyFasterShieldScript"), // 70
            new ObjTypeInit(0, 161, "chuteGuyShooterShieldScript"), // 71
            new ObjTypeInit(0, 229, "chuteGuyShooterFasterShieldScript"), // 72
            new ObjTypeInit(0, 124, "sheepTopdownScript"), // 73
            new ObjTypeInit(0, 149, "ninaRun"), // 74
            new ObjTypeInit(0, 128, "wumpaShootGuy1Script"), // 75
            new ObjTypeInit(0, 109, "tankGuyFlamerRightScript"), // 76
            new ObjTypeInit(0, 131, "wumpaShootGuy1FasterScript"), // 77
            new ObjTypeInit(0, 75, "batGhostFlyerScript"), // 78
            new ObjTypeInit(0, 75, "batGhost2FlyerScript"), // 79
            new ObjTypeInit(0, 173, "riptoBossScript"), // 80
            new ObjTypeInit(0, 187, "tinyTankBossScript"), // 81
            new ObjTypeInit(0, 201, null), // 82
            new ObjTypeInit(0, 75, "riptoBatScript"), // 83
            new ObjTypeInit(0, 215, "dropBombPlane"), // 84
            new ObjTypeInit(0, 220, "gulpBossScript"), // 85
            new ObjTypeInit(0, 220, "gulpJumpStart"), // 86
            new ObjTypeInit(11, 16, "spyroNina"), // 87
            new ObjTypeInit(0, 75, "riptoBat2Script"), // 88
            new ObjTypeInit(9, 14, "platformBouncyMoveScript"), // 89
            new ObjTypeInit(-1, -1, null), // 90
            new ObjTypeInit(9, 7, "platformMoveSlowScript"), // 91
            new ObjTypeInit(9, 6, "platformMoveVertDownScript"), // 92
            new ObjTypeInit(9, 6, "platformMoveScript"), // 93
            new ObjTypeInit(9, 8, "platformMoveScript"), // 94
            new ObjTypeInit(9, 9, "platformMoveScript"), // 95
            new ObjTypeInit(9, 10, "platformMoveScript"), // 96
            new ObjTypeInit(9, 8, "platformMoveVertDownScript"), // 97
            new ObjTypeInit(9, 11, "platformMoveScript"), // 98
            new ObjTypeInit(9, 11, "platformMoveVertDownScript"), // 99
            new ObjTypeInit(9, 12, "platformMoveScript"), // 100
            new ObjTypeInit(9, 12, "platformScript"), // 101
            new ObjTypeInit(9, 12, "platformMoveVertDownScript"), // 102
            new ObjTypeInit(9, 20, null), // 103
            new ObjTypeInit(9, 25, null), // 104
            new ObjTypeInit(9, 21, null), // 105
            new ObjTypeInit(9, 18, null), // 106
            new ObjTypeInit(9, 22, null), // 107
            new ObjTypeInit(9, 15, null), // 108
            new ObjTypeInit(9, 34, null), // 109
            new ObjTypeInit(9, 18, null), // 110
            new ObjTypeInit(11, 4, "genericNPC"), // 111
            new ObjTypeInit(11, 2, "genericNPC"), // 112
            new ObjTypeInit(11, 1, "genericNPC"), // 113
            new ObjTypeInit(11, 3, "genericNPC"), // 114
            new ObjTypeInit(11, 14, "genericNPC"), // 115
            new ObjTypeInit(11, 0, "genericNPC"), // 116
            new ObjTypeInit(11, 6, "genericNPC"), // 117
            new ObjTypeInit(11, 7, "genericNPC"), // 118
            new ObjTypeInit(11, 9, "genericNPC"), // 119
            new ObjTypeInit(11, 8, "genericNPC"), // 120
            new ObjTypeInit(11, 10, "genericNPC"), // 121
            new ObjTypeInit(11, 15, "SpyroScript"), // 122
            new ObjTypeInit(11, 20, "genericNPC"), // 123
            new ObjTypeInit(11, 21, "genericNPC"), // 124
            new ObjTypeInit(11, 11, "genericNPC"), // 125
            new ObjTypeInit(0, 46, "icicleScript"), // 126
            new ObjTypeInit(0, 43, "icicleFallingScript"), // 127
            new ObjTypeInit(0, 40, "flameHeadScript"), // 128
            new ObjTypeInit(0, 38, "floorSpearsWhackerScript"), // 129
            new ObjTypeInit(0, 47, "lavaFountainScript"), // 130
            new ObjTypeInit(0, 66, "arcticTotemScript"), // 131
            new ObjTypeInit(0, 59, "swingingAxeScript"), // 132
            new ObjTypeInit(-1, -1, null), // 133
            new ObjTypeInit(0, 81, "wallSpikesWhackerScript"), // 134
            new ObjTypeInit(0, 72, "lavaSpurtSpawnerScript"), // 135
            new ObjTypeInit(0, 74, "lavaSpurtScript"), // 136
            new ObjTypeInit(0, 85, "electricWallScript"), // 137
            new ObjTypeInit(8, 36, "nitroScript"), // 138
            new ObjTypeInit(0, 165, "mineStartScript"), // 139
            new ObjTypeInit(0, 79, "floorSpikesWhackerScript"), // 140
            new ObjTypeInit(0, 210, "wallPiece2Script"), // 141
            new ObjTypeInit(0, 207, "torpedoScript"), // 142
            new ObjTypeInit(0, 206, "torpedoSpawnStartScript"), // 143
            new ObjTypeInit(0, 147, "ninaCage"), // 144
            new ObjTypeInit(0, 198, "tinyDropBombScript"), // 145
            new ObjTypeInit(0, 176, "energyBall"), // 146
            new ObjTypeInit(0, 217, "spotlightScript"), // 147
            new ObjTypeInit(0, 217, "spotlightLeftRightScript"), // 148
            new ObjTypeInit(0, 213, "dropBombScript"), // 149
            new ObjTypeInit(0, 233, "groundFenceVertScript"), // 150
            new ObjTypeInit(0, 235, "groundFenceHorScript"), // 151
            new ObjTypeInit(0, 237, "electricFloorScript"), // 152
            new ObjTypeInit(0, 167, null), // 153
            new ObjTypeInit(0, 169, "wallPieceScript"), // 154
            new ObjTypeInit(9, 40, null), // 155
            new ObjTypeInit(9, 42, null), // 156
            new ObjTypeInit(9, 40, null), // 157
            new ObjTypeInit(9, 40, null), // 158
            new ObjTypeInit(0, 16, "triggerGenericScript"), // 159
            new ObjTypeInit(0, 17, "triggerRightScript"), // 160
            new ObjTypeInit(0, 18, "triggerLeftScript"), // 161
            new ObjTypeInit(0, 19, "triggerUpScript"), // 162
            new ObjTypeInit(-1, -1, null), // 163
            new ObjTypeInit(0, 24, "trigger1"), // 164
            new ObjTypeInit(0, 23, "trigger2"), // 165
            new ObjTypeInit(0, 22, "trigger3"), // 166
            new ObjTypeInit(0, 20, "triggerDownScript"), // 167
            new ObjTypeInit(0, 26, "trigger5"), // 168
            new ObjTypeInit(0, 16, "triggerPolarScript"), // 169
            new ObjTypeInit(0, 25, "trigger4"), // 170
            new ObjTypeInit(3, 4, null), // 171
            new ObjTypeInit(3, 5, null), // 172
            new ObjTypeInit(3, 7, null), // 173
            new ObjTypeInit(3, 8, null), // 174
            new ObjTypeInit(3, 9, null), // 175
            new ObjTypeInit(3, 10, null), // 176
            new ObjTypeInit(3, 14, null), // 177
            new ObjTypeInit(3, 6, null), // 178
            new ObjTypeInit(3, 19, null), // 179
            new ObjTypeInit(3, 18, null), // 180
            new ObjTypeInit(3, 21, null), // 181
            new ObjTypeInit(3, 24, null), // 182
            new ObjTypeInit(3, 25, null), // 183
            new ObjTypeInit(3, 27, null), // 184
            new ObjTypeInit(3, 11, null), // 185
            new ObjTypeInit(3, 22, null), // 186
            new ObjTypeInit(3, 12, null), // 187
            new ObjTypeInit(3, 23, null), // 188
            new ObjTypeInit(-1, -1, null), // 189
            new ObjTypeInit(4, 29, "breakoutRhynocScript"), // 190
            new ObjTypeInit(4, 37, null), // 191
            new ObjTypeInit(4, 26, "breakoutLabAssShooterScript"), // 192
            new ObjTypeInit(4, 40, "breakoutRhynocShieldScript"), // 193
            new ObjTypeInit(4, 35, "breakoutLabAssProjectileScript"), // 194
            new ObjTypeInit(4, 29, "breakoutLabAssScript"), // 195
            new ObjTypeInit(4, 45, "breakoutWallOnScript"), // 196
            new ObjTypeInit(4, 43, "breakoutRhynocBallScript"), // 197
            new ObjTypeInit(12, 4, null), // 198
            new ObjTypeInit(7, 7, null), // 199
            new ObjTypeInit(16, 6, null), // 200
            new ObjTypeInit(12, 4, null), // 201
            new ObjTypeInit(12, 5, null), // 202
            new ObjTypeInit(12, 6, null), // 203
            new ObjTypeInit(12, 7, null), // 204
            new ObjTypeInit(12, 8, null), // 205
            new ObjTypeInit(12, 9, null), // 206
            new ObjTypeInit(12, 10, null), // 207
            new ObjTypeInit(12, 11, null), // 208
            new ObjTypeInit(12, 12, null), // 209
            new ObjTypeInit(12, 13, null), // 210
            new ObjTypeInit(12, 14, null), // 211
            new ObjTypeInit(12, 15, null), // 212
            new ObjTypeInit(12, 16, null), // 213
            new ObjTypeInit(12, 17, null), // 214
            new ObjTypeInit(12, 18, null), // 215
            new ObjTypeInit(12, 19, null), // 216
            new ObjTypeInit(12, 28, null), // 217
            new ObjTypeInit(12, 22, null), // 218
            new ObjTypeInit(12, 23, null), // 219
            new ObjTypeInit(12, 24, null), // 220
            new ObjTypeInit(12, 25, null), // 221
            new ObjTypeInit(12, 26, null), // 222
            new ObjTypeInit(12, 27, null), // 223
            new ObjTypeInit(12, 21, null), // 224
            new ObjTypeInit(12, 30, null), // 225
            new ObjTypeInit(12, 31, null), // 226
            new ObjTypeInit(12, 32, null), // 227
            new ObjTypeInit(12, 33, null), // 228
            new ObjTypeInit(12, 34, null), // 229
            new ObjTypeInit(12, 35, null), // 230
            new ObjTypeInit(12, 36, null), // 231
            new ObjTypeInit(12, 29, null), // 232
            new ObjTypeInit(12, 44, null), // 233
            new ObjTypeInit(12, 38, null), // 234
            new ObjTypeInit(12, 39, null), // 235
            new ObjTypeInit(12, 40, null), // 236
            new ObjTypeInit(12, 41, null), // 237
            new ObjTypeInit(12, 42, null), // 238
            new ObjTypeInit(12, 43, null), // 239
            new ObjTypeInit(12, 37, null), // 240
            new ObjTypeInit(6, 121, null), // 241
            new ObjTypeInit(4, 29, "globalController"), // 242
            new ObjTypeInit(-1, -1, null), // 243
        };

        public override Dictionary<int, GBAVV_ScriptCommand.CommandType> ScriptCommands { get; } = new Dictionary<int, GBAVV_ScriptCommand.CommandType>()
        {
            [0501] = GBAVV_ScriptCommand.CommandType.Name,
            [0502] = GBAVV_ScriptCommand.CommandType.Script,
            [0503] = GBAVV_ScriptCommand.CommandType.SkipNextIfInputCheck,
            [0504] = GBAVV_ScriptCommand.CommandType.SkipNextIfField08,
            [0505] = GBAVV_ScriptCommand.CommandType.Reset,
            [0506] = GBAVV_ScriptCommand.CommandType.Return,
            [0507] = GBAVV_ScriptCommand.CommandType.SetUnknownInputData,
            [0508] = GBAVV_ScriptCommand.CommandType.Wait,
            // 0509 is a duplicate of 0510, but never used
            [0510] = GBAVV_ScriptCommand.CommandType.WaitWhileInputCheck,

            [0702] = GBAVV_ScriptCommand.CommandType.Dialog,

            [0800] = GBAVV_ScriptCommand.CommandType.IsFlipped,
            [0807] = GBAVV_ScriptCommand.CommandType.Animation,
            [0815] = GBAVV_ScriptCommand.CommandType.IsEnabled,
            [0818] = GBAVV_ScriptCommand.CommandType.ConditionalScript,
            [0829] = GBAVV_ScriptCommand.CommandType.Movement_X,
            [0830] = GBAVV_ScriptCommand.CommandType.Movement_Y,
            [0859] = GBAVV_ScriptCommand.CommandType.SpawnObject,
            [0863] = GBAVV_ScriptCommand.CommandType.SecondaryAnimation,
            [0871] = GBAVV_ScriptCommand.CommandType.PlaySound,

            [1000] = GBAVV_ScriptCommand.CommandType.DialogPortrait

        };
    }
    public class GBAVV_CrashFusionUS_Manager : GBAVV_CrashFusion_Manager
    {
        public override int ObjTypesCount => 244;
        public override uint ObjTypesPointer => 0x08011144;

        public override uint[] AnimSetPointers => new uint[]
        {
            0x08279D94,
            0x082AE7CC,
            0x082B9060,
            0x082BB230,
            0x082BE78C,
            0x082C3000,
            0x082CBDE8,
            0x082F7078,
            0x082F8E8C,
            0x082FCEC0,
            0x08307450,
            0x08307D40,
            0x08310BC4,
            0x0831AD58,
            0x0831C498,
            0x0831CDE8,
            0x0831D880,
            0x0831FB34,
            0x0831FEFC,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x080698F4, // genericNPC
            0x08069948, // talkToVictim
            0x08069C38, // script_waitForInputOrTime
            0x0806D724, // notFound
            0x0806D91C, // waitForPagedText
            0x0806D984, // missing
            0x0806D9CC, // crunch01
            0x0806DA2C, // moneybags02
            0x0806DA8C, // blinky03
            0x0806DAEC, // blinky25
            0x0806DB48, // coco04
            0x0806DBA4, // coco05
            0x0806DC04, // professor06
            0x0806DC64, // moneybags07
            0x0806DCC4, // moneybags08
            0x0806DD24, // moneybags09
            0x0806DD84, // moneybags10
            0x0806DDE4, // crunch11
            0x0806DE44, // crunch12
            0x0806DEA4, // crunch13
            0x0806DF04, // crunch14
            0x0806DF64, // agent9_15
            0x0806DFC4, // sgtbyrd16
            0x0806E024, // hunter17
            0x0806E084, // moneybags18
            0x0806E0E4, // moneybags19
            0x0806E144, // moneybags20
            0x0806E1A4, // moneybags21
            0x0806E204, // moneybags22
            0x0806E264, // bianca23
            0x0806E2C4, // fakeCrash24
            0x0806E354, // warpHelp50
            0x0806E3C0, // crystalBarrierHelp51
            0x0806E420, // bonusHelp52
            0x0806E484, // crystalHelp53
            0x0806E4E4, // HUDHelp54
            0x0806E548, // barrierHelp55
            0x0806E5A8, // wumpaHelp56
            0x0806E608, // jumpHelp57
            0x0806E66C, // returnHelp58
            0x0806E6D0, // findingHelp59
            0x0806E73C, // instructionsPolar100
            0x0806E808, // instructionsChopper101
            0x0806E930, // instructionsTank102
            0x0806EA60, // instructionsWumpaShoot103
            0x0806EB30, // instructionsWeightlift104
            0x0806EBCC, // instructionsSmash105
            0x0806EC3C, // instructionsDeflection106
            0x0806ED68, // instructionsJetpack107
            0x0806EE38, // instructionsBatAttack108
            0x0806EF60, // instructionsTube109
            0x0806F028, // instructionsJump110
            0x0806F098, // instructionsBridgeFight111
            0x0807619C, // movie_credits
            0x08076CF0, // waitForPagedText
            0x0807875C, // platformMoveVertUpScript
            0x08078810, // platformBouncyMoveLeftScript
            0x08078890, // platformMoveLeftScript
            0x08078920, // platformMoveLeftSlowScript
            0x08078A44, // "resettime"
            0x08078B34, // "doit"
            0x08078C24, // torpedoSpawnerScript
            0x08078C8C, // mineGoRight
            0x08078D04, // "check"
            0x08078DF4, // mineCheck
            0x08078E8C, // "n"
            0x08078F7C, // "outside"
            0x0807906C, // mineMoveLeftSlowScript
            0x080790F8, // spotlightFire
            0x080791F8, // spotlightMoveRight
            0x08079250, // spotlightMoveLeft
            0x080792A4, // spotlightMoveUp
            0x080792FC, // spotlightMoveDown
            0x08079390, // patrol
            0x0807948C, // breakoutBasicPatrol
            0x08079510, // breakoutBasicHitWall
            0x0807977C, // platformScript
            0x080797B4, // platformScriptIron
            0x08079814, // platformMoveVertDownScript
            0x080798BC, // platformBouncyMoveScript
            0x08079944, // platformMoveScript
            0x080799DC, // platformMoveSlowScript
            0x08079A5C, // wumpaBarrierScript
            0x08079A90, // triggerPolarScript
            0x08079AD4, // triggerGenericScript
            0x08079B10, // triggerUpScript
            0x08079B50, // triggerDownScript
            0x08079B90, // triggerLeftScript
            0x08079BD0, // triggerRightScript
            0x08079C38, // lavaSpurtScript
            0x08079D50, // lavaSpurtSpawnerScript
            0x08079DBC, // floorSpearsWhackerScript
            0x08079E68, // floorSpikesWhackerScript
            0x08079F04, // wallSpikesWhackerScript
            0x08079FAC, // groundFenceHorScript
            0x0807A030, // groundFenceVertScript
            0x0807A0B0, // arcticTotemScript
            0x0807A0E4, // flameHeadScript
            0x0807A170, // torpedoSpawnFlip
            0x0807A1D0, // torpedoSpawnStartScript
            0x0807A250, // "move2"
            0x0807A340, // "move"
            0x0807A430, // torpedoScript
            0x0807A504, // mineGoLeft
            0x0807A550, // mineStartScript
            0x0807A5E4, // "b"
            0x0807A6D4, // "go"
            0x0807A7C4, // mineMoveSlowScript
            0x0807A854, // fruitSpawnerScript
            0x0807A8BC, // electricWallScript
            0x0807A948, // icicleScript
            0x0807A9B8, // "fall"
            0x0807AAA8, // icicleFallingScript
            0x0807AB64, // "playsfx"
            0x0807AC54, // lavaFountainScript
            0x0807ACAC, // "sfx"
            0x0807AD9C, // swingingAxeScript
            0x0807AE0C, // "destroy"
            0x0807AEFC, // nitroScript
            0x0807AF54, // wallPieceScript
            0x0807B064, // wallPiece2Script
            0x0807B17C, // dropBombScript
            0x0807B280, // tinyDropBombScript
            0x0807B3BC, // "drop"
            0x0807B4AC, // dropBombPlane
            0x0807B554, // spotlightScript
            0x0807B5C4, // spotlightLeftRightScript
            0x0807B640, // energyBall
            0x0807B738, // electricFloorScript
            0x0807B794, // breakoutLabAssProjectileScript
            0x0807B854, // "InnerLoop"
            0x0807B944, // globalController
            0x0807B9EC, // breakoutLabAssShooterScript
            0x0807BA7C, // breakoutLabAssScript
            0x0807BB10, // breakoutRhynocShieldScript
            0x0807BBB8, // breakoutRhynocBallScript
            0x0807BC48, // breakoutRhynocScript
            0x0807BCE0, // breakoutWallOnScript
            0x0807BD28, // beatBoss
            0x0807BD68, // nitroExplode
            0x0807BDF4, // energyBallDestroyed
            0x0807BE6C, // stepLeft
            0x0807BEB4, // stepRight
            0x0807BEFC, // stepDown
            0x0807BF34, // newWave
            0x0807BF6C, // genericDeathScript
            0x0807C048, // DieAndDropBallScript
            0x0807C090, // genericDrop
            0x0807C0F0, // genericTurnRightAndDrop
            0x0807C138, // genericTurnLeftAndDrop
            0x0807C180, // breakoutWallOffScript
            0x0807C204, // genericTurnLeftScript
            0x0807C258, // genericTurnRightScript
            0x0807C2E0, // "patrol"
            0x0807C3D0, // geckoTurnRightScript
            0x0807C42C, // "patrol"
            0x0807C51C, // labAssTurnRightScript
            0x0807C580, // "patrol"
            0x0807C670, // goatTurnRightScript
            0x0807C6C8, // "patrol"
            0x0807C7B8, // sealTurnRightScript
            0x0807C814, // "patrol"
            0x0807C904, // polarTurnRightScript
            0x0807C964, // "patrol"
            0x0807CA54, // rhynocTurnRightCastleScript
            0x0807CAB0, // "patrol"
            0x0807CBA0, // rhynocTurnRightScript
            0x0807CC00, // "patrol"
            0x0807CCF0, // rhynocIceTurnRightScript
            0x0807CD98, // "WaitingForKill"
            0x0807CE88, // rhynocRockScript
            0x0807CF68, // tankGuyFlamerRightScript
            0x0807D054, // "patrol"
            0x0807D150, // penguinTurnRightScript
            0x0807D20C, // "patrol"
            0x0807D2FC, // infernoTurnRightScript
            0x0807D360, // batUp
            0x0807D400, // rhynocFlyerMoveUpScript
            0x0807D4D8, // rhynocIceFlyerMoveUpScript
            0x0807D5B0, // rhynocFireFlyerMoveUpScript
            0x0807D6A4, // "attack"
            0x0807D794, // "patrol"
            0x0807D884, // sharkeyTurnRightScript
            0x0807D8E8, // sheepGoRight
            0x0807D98C, // gulpRight
            0x0807DA34, // riptoDies
            0x0807DAEC, // riptoShootScript
            0x0807DC1C, // riptoBossBatWaveScript
            0x0807DD98, // riptoBoss2ndWaveScript
            0x0807DEEC, // riptoBoss3rdWaveScript
            0x0807E258, // SpyroScript
            0x0807E304, // "patrol"
            0x0807E3F4, // geckoTurnLeftScript
            0x0807E464, // "patrol"
            0x0807E554, // geckoPatrolScript
            0x0807E5EC, // "patrol"
            0x0807E6DC, // labAssTurnLeftScript
            0x0807E73C, // "patrol"
            0x0807E82C, // labAssPatrolScript
            0x0807E8CC, // "patrol"
            0x0807E9BC, // goatTurnLeftScript
            0x0807EA20, // "patrol"
            0x0807EB10, // goatPatrolScript
            0x0807EB98, // "patrol"
            0x0807EC88, // sealTurnLeftScript
            0x0807ECE8, // "patrol"
            0x0807EDD8, // sealPatrolScript
            0x0807EE6C, // "patrol"
            0x0807EF5C, // polarTurnLeftScript
            0x0807EFC0, // "patrol"
            0x0807F0B0, // polarbearPatrolScript
            0x0807F14C, // "patrol"
            0x0807F23C, // rhynocTurnLeftCastleScript
            0x0807F2A4, // rhynocCastlePatrolScript
            0x0807F348, // "patrol"
            0x0807F438, // rhynocTurnLeftScript
            0x0807F4A0, // rhynocJunglePatrolScript
            0x0807F550, // "patrol"
            0x0807F640, // rhynocIceTurnLeftScript
            0x0807F6A4, // rhynocIcePatrolScript
            0x0807F754, // rhynocThrowScript
            0x0807F860, // tankGuy1StaticScript
            0x0807F904, // tankGuy1BStaticScript
            0x0807F9A4, // tankGuyFlamerExplodeScript
            0x0807FA3C, // tankGuyFlamerDownScript
            0x0807FB38, // "doit"
            0x0807FC28, // wumpaShootGuy1Script
            0x0807FC98, // "doit"
            0x0807FD88, // wumpaShootGuy1FasterScript
            0x0807FDFC, // "patrol"
            0x0807FEEC, // penguinTurnLeftScript
            0x0807FF4C, // penguinAttackScript
            0x08080008, // "patrol"
            0x080800F8, // infernoTurnLeftScript
            0x08080168, // infernoLabAssAttackScript
            0x0808022C, // "patrol"
            0x0808031C, // infernoLabAssWhackerScript
            0x080803F0, // "fire1"
            0x080804E8, // "fire2"
            0x080805D8, // "shoot"
            0x080806C8, // infernoLabAssShooterScript
            0x08080780, // batDown
            0x080807F0, // batFlyerScript
            0x080808C8, // rhynocFlyerMoveDownScript
            0x0808099C, // "fly"
            0x08080A8C, // rhynocJungleFlyerScript
            0x08080B50, // rhynocIceFlyerMoveDownScript
            0x08080C1C, // "fly"
            0x08080D0C, // rhynocIceFlyerScript
            0x08080DC4, // rhynocFireFlyerMoveDownScript
            0x08080E90, // "fly"
            0x08080F80, // rhynocFireFlyerScript
            0x08081030, // "attack"
            0x08081120, // "patrol"
            0x08081210, // sharkeyTurnLeftScript
            0x08081260, // sharkeyPatrolScript
            0x080812E8, // "attackL"
            0x080813E4, // "attackR"
            0x080814D4, // wormWhackerScript
            0x08081578, // "patrol"
            0x08081668, // venusWhackerScript
            0x08081744, // "go"
            0x08081834, // sheepTopdownScript
            0x080818B8, // sheepSpawnerScript
            0x08081938, // chuteGuyScript
            0x08081A1C, // chuteGuyShieldScript
            0x08081B48, // chuteGuyFasterScript
            0x08081C24, // chuteGuyFasterShieldScript
            0x08081D4C, // chuteGuyShooterScript
            0x08081E58, // chuteGuyShooterShieldScript
            0x08081FA8, // chuteGuyShooterFasterScript
            0x080820B0, // chuteGuyShooterFasterShieldScript
            0x0808220C, // "wave one"
            0x08082320, // "wave mini-two"
            0x08082424, // "wave two"
            0x08082524, // "wave three"
            0x08082614, // tinyTankBossScript
            0x080826E8, // riptoBatScript
            0x080827A0, // riptoBat2Script
            0x08082854, // batGhostFlyerScript
            0x08082914, // batGhost2FlyerScript
            0x080829C8, // gulpJumpStart
            0x08082A1C, // gulpLeft
            0x08082AC0, // gulpBossScript
            0x08082B24, // trigger1
            0x08082B68, // trigger2
            0x08082BA0, // trigger3
            0x08082BD8, // trigger4
            0x08082C18, // trigger5
            0x08082C6C, // broadcastTriggerGulp
            0x08082CD4, // broadcastTrigger1
            0x08082D3C, // broadcastTrigger2
            0x08082DA4, // broadcastTrigger3
            0x08082E0C, // broadcastTrigger4
            0x08082E74, // broadcastTrigger5
            0x08082ED8, // "attack!"
            0x08082FE0, // "attack2"
            0x080830DC, // "attack3"
            0x080831CC, // riptoBossScript
            0x08083284, // riptoTakeHit
            0x080832F4, // riptoShoot2Script
            0x08083404, // ninaCage
            0x08083448, // spyroNina
            0x080834D4, // "walkSlower"
            0x080835E0, // "walkFaster"
            0x080836D0, // "walk"
            0x080837C0, // ninaRun
            0x080838A4, // "left"
            0x080839A8, // "right"
            0x08083A98, // spunInto
            0x08083B8C, // jumpedOnGecko
            0x08083C38, // jumpedOnLabAss
            0x08083CE4, // jumpedOnGoat
            0x08083D98, // jumpedOnSeal
            0x08083E50, // jumpedOnPolar
            0x08083F04, // jumpedOnRhynocCastle
            0x08083F8C, // jumpedOnRhynoc
            0x08084024, // jumpedOnIceRhynoc
            0x080840C0, // jumpedOnRhynocThrower
            0x08084158, // tankGuy1Destroyed
            0x080841CC, // tankGuy1BDestroyed
            0x08084258, // tankGuyFlamerLeftScript
            0x08084348, // sheepDie
            0x080843C4, // jumpedOnPenguin
            0x08084488, // jumpedOnInferno
            0x0808451C, // jumpedOnVenus
            0x080845A0, // sheepDead
            0x08084624, // sheepGoLeft
            0x080846A8, // chuteHitPod
            0x08084714, // chuteFall
            0x080847F0, // chuteShooter
            0x08084858, // chuteShooterFall
            0x0808493C, // chuteShooterFasterFall
            0x08084A1C, // chuteShooterFaster
            0x08084A80, // tinyHasShieldUp
            0x08084AC4, // "blowup"
            0x08084BC0, // "takehit"
            0x08084CB0, // tinyTakeHit
            0x08084D60, // tinyTankBombScript
            0x08084E84, // tinyShootGun
            0x08084F80, // riptoBatHit
            0x08084FD4, // riptoBatDie
            0x08085014, // batGhostExplode
            0x0808508C, // batGhostDie
            0x080850F4, // gulpIsInvulnerable
            0x08085148, // gulpShoot1
            0x08085238, // "wave1"
            0x08085348, // "one"
            0x08085450, // "two"
            0x08085540, // "other"
            0x08085630, // trigger1Spawn
            0x08085694, // trigger2Spawn
            0x080856F8, // trigger3Spawn
            0x0808575C, // trigger4Spawn
            0x080857D0, // trigger5Spawn
            0x08085810, // spawnGulp
            0x08085844, // beatRiptoBoss
            0x08085884, // ninaCageFall
            0x08085934, // spyroNinaMoveDown
            0x080859AC, // ninaJump
            0x08085A60, // ninaStop
            0x08085B04, // tankGuyFlamerUpScript
            0x08085BE4, // gulpDie
            0x08085C44, // gulpShoot2
            0x08085CE8, // flipMe
        };
    }

    public abstract class GBAVV_SpyroFusion_Manager : GBAVV_Fusion_Manager
    {
        public override LevInfo[] LevInfos => Levels;

        public static LevInfo[] Levels = new LevInfo[]
        {
            new LevInfo(0, "Fire Mountains"),
            new LevInfo(1, "Gate Crank"),
            new LevInfo(2, "Pull Of Lava"),
            new LevInfo(3, "Crush and Gulp"),
            new LevInfo(4, "Dragon Castles"),
            new LevInfo(5, "Ice Chopper"),
            new LevInfo(6, "Dragon Castles"),
            new LevInfo(7, "Portal Rush"),
            new LevInfo(8, "Crank It Up"),
            new LevInfo(9, "Fire and Ice"),
            new LevInfo(10, "Wall of Fire"),
            new LevInfo(11, "Rumble on the Ramparts"),
            new LevInfo(12, "Arctic Cliffs"),
            new LevInfo(13, "Arctic Cliffs"),
            new LevInfo(14, "Fire Mountains"),
            new LevInfo(15, "Wumpa Jungle"),
            new LevInfo(16, "Wumpa Jungle"),
            new LevInfo(17, "Gem Rush"),
            new LevInfo(18, "Portal Rush"),
            new LevInfo(19, "Gem Rush"),
            new LevInfo(20, "Portal Rush"),
            new LevInfo(21, "Gem Rush"),
            new LevInfo(22, "Portal Rush"),
            new LevInfo(23, "Gem Rush"),
            new LevInfo(24, "Tech Park"),
            new LevInfo(25, "Portal Rush"),
            new LevInfo(26, "Gem Rush"),
            new LevInfo(27, "Nina Cortex"),
            new LevInfo(28, "Arctic Attack"),
            new LevInfo(29, "Space Shoot"),
            new LevInfo(30, "Fire Fight"),
            new LevInfo(31, "Castle Cruisin'"),
            new LevInfo(32, "Riptocs And Rockets"),
            new LevInfo(33, "Snow Steps"),
            new LevInfo(34, "Sky Walker"),
            new LevInfo(35, "Falling To Pieces"),
            new LevInfo(36, "Fall In, Roll Out"),
            new LevInfo(37, "Tread Lightly"),
            new LevInfo(38, "Dragon Assault"),
            new LevInfo(39, "Altitude Adjustment"),
            new LevInfo(40, "Hot Wings"),
            new LevInfo(41, "Treetop Flight"),
            new LevInfo(42, "N/A"),
            new LevInfo(43, "N/A"),
            new LevInfo(44, "N/A"),
            new LevInfo(45, "N/A"),
            new LevInfo(46, "N/A"),
            new LevInfo(47, "N/A"),
            new LevInfo(48, "N/A"),
            new LevInfo(49, "N/A"),
            new LevInfo(50, "N/A"),
            new LevInfo(51, "N/A"),
            new LevInfo(52, "N/A"),
            new LevInfo(53, "N/A"),
            new LevInfo(54, "N/A"),
            new LevInfo(55, "N/A"),
            new LevInfo(56, "N/A"),
            new LevInfo(57, "N/A"),
            new LevInfo(58, "N/A"),
            new LevInfo(59, "N/A"),
            new LevInfo(60, "N/A"),
            new LevInfo(61, "N/A"),
            new LevInfo(62, "N/A"),
            new LevInfo(63, "N/A"),
            new LevInfo(64, "N/A"),
            new LevInfo(65, "N/A"),
            new LevInfo(66, "N/A"),
            new LevInfo(67, "N/A"),
            new LevInfo(68, "Castle Chaos"),
            new LevInfo(69, "Space Chase"),
            new LevInfo(70, "Sheep Shearin'"),
            new LevInfo(71, "Gem Hop"),
            new LevInfo(72, "Neo Cortex"),
            new LevInfo(73, "Death From Behind"),
            new LevInfo(74, "Sheep Shakedown"),
            new LevInfo(75, "Gem Chaser"),
            new LevInfo(76, "Gem Chaser"),
            new LevInfo(77, "Gem Chaser"),
            new LevInfo(78, "Gem Chaser"),
            new LevInfo(79, "Dragon Drop"),
            new LevInfo(80, "Lava Fields"),
            new LevInfo(81, "Icicle Canyon"),
            new LevInfo(82, "Sheep Chase"),
            new LevInfo(83, "Tech Park"),
            new LevInfo(84, "Tech Tug"),
            new LevInfo(85, "Blizzard Balls"),
            new LevInfo(86, "Ring Of Fire"),
            new LevInfo(87, "Riptoc Repellent"),
            new LevInfo(88, "Tech Deflect"),
            new LevInfo(89, "Bridge Fight"),
            new LevInfo(90, "Turn Up the Heat"),
            new LevInfo(91, "Gravity Well"),
            new LevInfo(92, "Crash Bandicoot"),
        };

        public override ObjTypeInit[] ObjTypeInitInfos { get; } = new ObjTypeInit[]
        {
            new ObjTypeInit(-1, -1, null), // 0
            new ObjTypeInit(-1, -1, null), // 1
            new ObjTypeInit(19, 6, null), // 2
            new ObjTypeInit(21, 0, null), // 3
            new ObjTypeInit(19, 6, null), // 4
            new ObjTypeInit(19, 6, null), // 5
            new ObjTypeInit(10, 6, null), // 6
            new ObjTypeInit(24, 7, null), // 7
            new ObjTypeInit(8, 5, null), // 8
            new ObjTypeInit(2, 0, null), // 9
            new ObjTypeInit(10, 6, null), // 10
            new ObjTypeInit(23, 60, null), // 11
            new ObjTypeInit(23, 60, null), // 12
            new ObjTypeInit(23, 67, null), // 13
            new ObjTypeInit(23, 64, null), // 14
            new ObjTypeInit(23, 66, null), // 15
            new ObjTypeInit(23, 69, null), // 16
            new ObjTypeInit(23, 2, null), // 17
            new ObjTypeInit(23, 0, null), // 18
            new ObjTypeInit(23, 4, null), // 19
            new ObjTypeInit(23, 1, null), // 20
            new ObjTypeInit(23, 3, null), // 21
            new ObjTypeInit(23, 3, "gemJoust"), // 22
            new ObjTypeInit(23, 3, null), // 23
            new ObjTypeInit(23, 3, "gem2Joust"), // 24
            new ObjTypeInit(23, 3, "gem3Joust"), // 25
            new ObjTypeInit(23, 3, "gem4Joust"), // 26
            new ObjTypeInit(23, 74, "triggerScript"), // 27
            new ObjTypeInit(23, 79, "trigger1Script"), // 28
            new ObjTypeInit(23, 76, "trigger2Script"), // 29
            new ObjTypeInit(23, 77, "trigger3Script"), // 30
            new ObjTypeInit(23, 78, "trigger4Script"), // 31
            new ObjTypeInit(23, 75, "trigger5Script"), // 32
            new ObjTypeInit(23, 73, "triggerRightScript"), // 33
            new ObjTypeInit(23, 71, "triggerDownScript"), // 34
            new ObjTypeInit(23, 72, "triggerLeftScript"), // 35
            new ObjTypeInit(23, 70, "triggerUpScript"), // 36
            new ObjTypeInit(23, 80, "checkpoint"), // 37
            new ObjTypeInit(23, 81, "triggerJoustPlatform"), // 38
            new ObjTypeInit(23, 82, "sheepPound"), // 39
            new ObjTypeInit(23, 83, "sheepDrop"), // 40
            new ObjTypeInit(23, 84, "sheepHerder"), // 41
            new ObjTypeInit(23, 85, "triggerCandleFlame"), // 42
            new ObjTypeInit(23, 79, "joustCounter"), // 43
            new ObjTypeInit(23, 76, "spawnLabAssRear"), // 44
            new ObjTypeInit(23, 77, "spawnLabAssFront"), // 45
            new ObjTypeInit(23, 86, "ninaBossEndTrigger"), // 46
            new ObjTypeInit(23, 36, null), // 47
            new ObjTypeInit(23, 30, null), // 48
            new ObjTypeInit(23, 43, null), // 49
            new ObjTypeInit(23, 44, null), // 50
            new ObjTypeInit(23, 44, null), // 51
            new ObjTypeInit(23, 55, null), // 52
            new ObjTypeInit(23, 43, null), // 53
            new ObjTypeInit(23, 56, null), // 54
            new ObjTypeInit(23, 57, "hiddenPortalC_C"), // 55
            new ObjTypeInit(23, 57, "hiddenPortalC_J"), // 56
            new ObjTypeInit(23, 57, "hiddenPortalC_S"), // 57
            new ObjTypeInit(23, 57, "hiddenPortalI_C"), // 58
            new ObjTypeInit(23, 57, "hiddenPortalI_J"), // 59
            new ObjTypeInit(23, 57, "hiddenPortalI_S"), // 60
            new ObjTypeInit(23, 57, "hiddenPortalF_C"), // 61
            new ObjTypeInit(23, 57, "hiddenPortalF_J"), // 62
            new ObjTypeInit(23, 57, "hiddenPortalF_S"), // 63
            new ObjTypeInit(23, 57, "hiddenPortalJ_C"), // 64
            new ObjTypeInit(23, 57, "hiddenPortalJ_J"), // 65
            new ObjTypeInit(23, 57, "hiddenPortalJ_S"), // 66
            new ObjTypeInit(1, 131, "poopASpyro"), // 67
            new ObjTypeInit(1, 54, "lavaFountain"), // 68
            new ObjTypeInit(1, 1, "floorSpikesScript"), // 69
            new ObjTypeInit(1, 3, "wallSpikesScript"), // 70
            new ObjTypeInit(1, 63, "icicleScript"), // 71
            new ObjTypeInit(1, 64, "icicleFallingScript"), // 72
            new ObjTypeInit(1, 78, "venusPlant"), // 73
            new ObjTypeInit(1, 100, "swinginAxe"), // 74
            new ObjTypeInit(1, 5, "electrix"), // 75
            new ObjTypeInit(1, 4, "electrix"), // 76
            new ObjTypeInit(1, 102, "plantVines"), // 77
            new ObjTypeInit(1, 125, "lavaSpawner"), // 78
            new ObjTypeInit(1, 127, "lavaSpurt"), // 79
            new ObjTypeInit(1, 128, "moatWorm"), // 80
            new ObjTypeInit(1, 134, "landmine"), // 81
            new ObjTypeInit(1, 38, "rhynocPatrolScript"), // 82
            new ObjTypeInit(1, 6, "infernoLabAssPatrolScript"), // 83
            new ObjTypeInit(1, 30, "rhynocThrowScript"), // 84
            new ObjTypeInit(1, 32, "rhynocRockScript"), // 85
            new ObjTypeInit(1, 44, "sheepPatrol"), // 86
            new ObjTypeInit(1, 56, "rhynocFly"), // 87
            new ObjTypeInit(1, 60, "rhynocJungleFly"), // 88
            new ObjTypeInit(1, 70, "rhynocClubPatrol"), // 89
            new ObjTypeInit(1, 76, "infernoLabAssShoot"), // 90
            new ObjTypeInit(1, 77, null), // 91
            new ObjTypeInit(1, 94, "labAssPatrol"), // 92
            new ObjTypeInit(1, 96, "labAssCamo"), // 93
            new ObjTypeInit(1, 72, "rhynocIcePatrol"), // 94
            new ObjTypeInit(1, 49, "butterflyFlutter"), // 95
            new ObjTypeInit(1, 99, "coconutThrown"), // 96
            new ObjTypeInit(1, 45, "sheepFall"), // 97
            new ObjTypeInit(1, 123, "bunny"), // 98
            new ObjTypeInit(1, 130, "sheepShoot"), // 99
            new ObjTypeInit(1, 53, "sheepRun"), // 100
            new ObjTypeInit(1, 45, "sheepChaseSpawn"), // 101
            new ObjTypeInit(1, 76, "walkerLabAssFloater"), // 102
            new ObjTypeInit(1, 132, null), // 103
            new ObjTypeInit(13, 0, "genericNPC"), // 104
            new ObjTypeInit(13, 1, "genericNPC"), // 105
            new ObjTypeInit(13, 2, "genericNPC"), // 106
            new ObjTypeInit(13, 5, "blinkySnowAppear"), // 107
            new ObjTypeInit(13, 5, "blinkyDirtAppear"), // 108
            new ObjTypeInit(13, 6, "genericNPC"), // 109
            new ObjTypeInit(13, 8, "genericNPC"), // 110
            new ObjTypeInit(13, 7, "genericNPC"), // 111
            new ObjTypeInit(13, 9, "genericNPC"), // 112
            new ObjTypeInit(13, 10, "genericNPC"), // 113
            new ObjTypeInit(13, 11, "genericNPC"), // 114
            new ObjTypeInit(13, 12, "genericNPC"), // 115
            new ObjTypeInit(13, 13, "genericNPC"), // 116
            new ObjTypeInit(13, 14, "genericNPC"), // 117
            new ObjTypeInit(13, 15, "genericNPC"), // 118
            new ObjTypeInit(13, 16, "genericNPC"), // 119
            new ObjTypeInit(1, 14, "crushRunStartScript"), // 120
            new ObjTypeInit(1, 18, "gulpSpawny"), // 121
            new ObjTypeInit(1, 22, "gulpRocketScript"), // 122
            new ObjTypeInit(1, 83, "ninaSpawn"), // 123
            new ObjTypeInit(5, 0, "roofScript"), // 124
            new ObjTypeInit(1, 86, "ninaCrateL"), // 125
            new ObjTypeInit(5, 7, "ninaProfCage"), // 126
            new ObjTypeInit(5, 8, "ninaCocoCage"), // 127
            new ObjTypeInit(1, 86, "ninaCrateR"), // 128
            new ObjTypeInit(1, 106, "cortexFly"), // 129
            new ObjTypeInit(1, 108, "cortexMine"), // 130
            new ObjTypeInit(1, 114, null), // 131
            new ObjTypeInit(5, 1, "boilerScript"), // 132
            new ObjTypeInit(1, 26, "gulpScorch"), // 133
            new ObjTypeInit(23, 74, "crushCounter"), // 134
            new ObjTypeInit(3, 2, null), // 135
            new ObjTypeInit(3, 0, null), // 136
            new ObjTypeInit(3, 1, null), // 137
            new ObjTypeInit(3, 3, null), // 138
            new ObjTypeInit(3, 4, null), // 139
            new ObjTypeInit(3, 5, null), // 140
            new ObjTypeInit(3, 6, null), // 141
            new ObjTypeInit(3, 15, null), // 142
            new ObjTypeInit(3, 14, null), // 143
            new ObjTypeInit(3, 16, null), // 144
            new ObjTypeInit(3, 17, null), // 145
            new ObjTypeInit(3, 18, null), // 146
            new ObjTypeInit(3, 19, null), // 147
            new ObjTypeInit(3, 20, null), // 148
            new ObjTypeInit(3, 21, null), // 149
            new ObjTypeInit(3, 26, null), // 150
            new ObjTypeInit(3, 23, null), // 151
            new ObjTypeInit(3, 24, null), // 152
            new ObjTypeInit(3, 25, null), // 153
            new ObjTypeInit(3, 22, null), // 154
            new ObjTypeInit(3, 28, null), // 155
            new ObjTypeInit(3, 27, null), // 156
            new ObjTypeInit(3, 30, "triggerIceTorch"), // 157
            new ObjTypeInit(3, 29, null), // 158
            new ObjTypeInit(3, 31, null), // 159
            new ObjTypeInit(3, 32, null), // 160
            new ObjTypeInit(3, 33, null), // 161
            new ObjTypeInit(3, 7, null), // 162
            new ObjTypeInit(3, 8, null), // 163
            new ObjTypeInit(3, 9, null), // 164
            new ObjTypeInit(3, 10, null), // 165
            new ObjTypeInit(3, 11, "triggerTorchSmall"), // 166
            new ObjTypeInit(3, 12, "triggerTorchLarge"), // 167
            new ObjTypeInit(13, 16, null), // 168
            new ObjTypeInit(11, 10, null), // 169
            new ObjTypeInit(11, 44, null), // 170
            new ObjTypeInit(11, 45, null), // 171
            new ObjTypeInit(11, 47, null), // 172
            new ObjTypeInit(11, 50, null), // 173
            new ObjTypeInit(11, 57, null), // 174
            new ObjTypeInit(11, 56, null), // 175
            new ObjTypeInit(11, 63, null), // 176
            new ObjTypeInit(11, 38, null), // 177
            new ObjTypeInit(11, 24, null), // 178
            new ObjTypeInit(11, 32, null), // 179
            new ObjTypeInit(11, 21, null), // 180
            new ObjTypeInit(11, 23, null), // 181
            new ObjTypeInit(11, 1, null), // 182
            new ObjTypeInit(11, 2, null), // 183
            new ObjTypeInit(11, 12, null), // 184
            new ObjTypeInit(11, 5, null), // 185
            new ObjTypeInit(11, 20, null), // 186
            new ObjTypeInit(11, 10, null), // 187
            new ObjTypeInit(23, 12, "platformUpScript"), // 188
            new ObjTypeInit(23, 13, "platformRightScript"), // 189
            new ObjTypeInit(23, 14, "platformRightScript"), // 190
            new ObjTypeInit(23, 18, "platformUpScript"), // 191
            new ObjTypeInit(23, 15, "platformRightScript"), // 192
            new ObjTypeInit(23, 19, "platformUpScript"), // 193
            new ObjTypeInit(23, 16, "platformUpScript"), // 194
            new ObjTypeInit(23, 17, "platformRightScript"), // 195
            new ObjTypeInit(23, 20, "platformIdleScript"), // 196
            new ObjTypeInit(23, 16, "joustPlatformL"), // 197
            new ObjTypeInit(23, 23, "bouncyBranch"), // 198
            new ObjTypeInit(23, 24, "platformRightScript"), // 199
            new ObjTypeInit(23, 25, "platformUpScript"), // 200
            new ObjTypeInit(23, 16, "joustPlatformR"), // 201
            new ObjTypeInit(23, 16, "joustPlatform2L"), // 202
            new ObjTypeInit(23, 16, "joustPlatform2R"), // 203
            new ObjTypeInit(23, 17, "joustPlatform3L"), // 204
            new ObjTypeInit(23, 17, "joustPlatform3R"), // 205
            new ObjTypeInit(23, 17, "joustPlatform4L"), // 206
            new ObjTypeInit(23, 17, "joustPlatform4R"), // 207
            new ObjTypeInit(20, 6, null), // 208
            new ObjTypeInit(7, 7, null), // 209
            new ObjTypeInit(-1, -1, null), // 210
            new ObjTypeInit(14, 4, null), // 211
            new ObjTypeInit(14, 5, null), // 212
            new ObjTypeInit(14, 6, null), // 213
            new ObjTypeInit(14, 7, null), // 214
            new ObjTypeInit(14, 8, null), // 215
            new ObjTypeInit(14, 9, null), // 216
            new ObjTypeInit(14, 10, null), // 217
            new ObjTypeInit(14, 11, null), // 218
            new ObjTypeInit(14, 12, null), // 219
            new ObjTypeInit(14, 13, null), // 220
            new ObjTypeInit(14, 14, null), // 221
            new ObjTypeInit(14, 15, null), // 222
            new ObjTypeInit(14, 16, null), // 223
            new ObjTypeInit(14, 17, null), // 224
            new ObjTypeInit(14, 18, null), // 225
            new ObjTypeInit(14, 19, null), // 226
            new ObjTypeInit(14, 28, null), // 227
            new ObjTypeInit(14, 22, null), // 228
            new ObjTypeInit(14, 23, null), // 229
            new ObjTypeInit(14, 24, null), // 230
            new ObjTypeInit(14, 25, null), // 231
            new ObjTypeInit(14, 26, null), // 232
            new ObjTypeInit(14, 27, null), // 233
            new ObjTypeInit(14, 21, null), // 234
            new ObjTypeInit(14, 30, null), // 235
            new ObjTypeInit(14, 31, null), // 236
            new ObjTypeInit(14, 32, null), // 237
            new ObjTypeInit(14, 33, null), // 238
            new ObjTypeInit(14, 34, null), // 239
            new ObjTypeInit(14, 35, null), // 240
            new ObjTypeInit(14, 36, null), // 241
            new ObjTypeInit(14, 29, null), // 242
            new ObjTypeInit(14, 44, null), // 243
            new ObjTypeInit(14, 38, null), // 244
            new ObjTypeInit(14, 39, null), // 245
            new ObjTypeInit(14, 40, null), // 246
            new ObjTypeInit(14, 41, null), // 247
            new ObjTypeInit(14, 42, null), // 248
            new ObjTypeInit(14, 43, null), // 249
            new ObjTypeInit(14, 37, null), // 250
            new ObjTypeInit(21, 16, null), // 251
            new ObjTypeInit(21, 15, null), // 252
            new ObjTypeInit(21, 17, null), // 253
            new ObjTypeInit(21, 14, null), // 254
            new ObjTypeInit(21, 24, null), // 255
            new ObjTypeInit(21, 28, null), // 256
            new ObjTypeInit(21, 31, null), // 257
            new ObjTypeInit(21, 35, null), // 258
            new ObjTypeInit(21, 38, null), // 259
            new ObjTypeInit(21, 41, null), // 260
            new ObjTypeInit(21, 24, null), // 261
            new ObjTypeInit(21, 44, "sheepHerd"), // 262
            new ObjTypeInit(21, 46, "topDownPortal"), // 263
            new ObjTypeInit(21, 47, "topDownPortal"), // 264
            new ObjTypeInit(21, 41, null), // 265
            new ObjTypeInit(21, 52, "chaseWall"), // 266
            new ObjTypeInit(21, 46, "topDownPortalFail"), // 267
            new ObjTypeInit(4, 29, "breakoutRhynocScript"), // 268
            new ObjTypeInit(4, 29, "breakoutRhynocScript"), // 269
            new ObjTypeInit(24, 26, null), // 270
            new ObjTypeInit(4, 26, "breakoutLabAssShooterScript"), // 271
            new ObjTypeInit(4, 40, "breakoutRhynocShieldScript"), // 272
            new ObjTypeInit(4, 35, "breakoutLabAssProjectileScript"), // 273
            new ObjTypeInit(4, 29, "breakoutLabAssScript"), // 274
            new ObjTypeInit(4, 43, "breakoutRhynocBallScript"), // 275
            new ObjTypeInit(1, 14, "crushRunStartScript"), // 276
            new ObjTypeInit(4, 29, "globalController"), // 277
            new ObjTypeInit(1, 137, "bouncySheep"), // 278
            new ObjTypeInit(15, 7, null), // 279
            new ObjTypeInit(1, 137, "bouncySheep"), // 280
            new ObjTypeInit(24, 26, null), // 281
        };

        public override Dictionary<int, GBAVV_ScriptCommand.CommandType> ScriptCommands { get; } = new Dictionary<int, GBAVV_ScriptCommand.CommandType>()
        {
            [0501] = GBAVV_ScriptCommand.CommandType.Name,
            [0502] = GBAVV_ScriptCommand.CommandType.Script,
            [0503] = GBAVV_ScriptCommand.CommandType.SkipNextIfInputCheck,
            [0504] = GBAVV_ScriptCommand.CommandType.SkipNextIfField08,
            [0505] = GBAVV_ScriptCommand.CommandType.Reset,
            [0506] = GBAVV_ScriptCommand.CommandType.Return,
            [0507] = GBAVV_ScriptCommand.CommandType.SetUnknownInputData,
            [0508] = GBAVV_ScriptCommand.CommandType.Wait,
            // 0509 is a duplicate of 0510, but never used
            [0510] = GBAVV_ScriptCommand.CommandType.WaitWhileInputCheck,

            [0702] = GBAVV_ScriptCommand.CommandType.Dialog,
            
            [0800] = GBAVV_ScriptCommand.CommandType.IsFlipped,
            [0807] = GBAVV_ScriptCommand.CommandType.Animation,
            [0812] = GBAVV_ScriptCommand.CommandType.IsEnabled,
            [0813] = GBAVV_ScriptCommand.CommandType.ConditionalScript,
            [0821] = GBAVV_ScriptCommand.CommandType.Movement_X,
            [0822] = GBAVV_ScriptCommand.CommandType.Movement_Y,
            [0844] = GBAVV_ScriptCommand.CommandType.SpawnObject,
            //[0863] = GBAVV_ScriptCommand.CommandType.SecondaryAnimation, // TODO: Find
            //[0871] = GBAVV_ScriptCommand.CommandType.PlaySound, // TODO: Find

            [1000] = GBAVV_ScriptCommand.CommandType.DialogPortrait

        };
    }
    public class GBAVV_SpyroFusionUS_Manager : GBAVV_SpyroFusion_Manager
    {
        public override int ObjTypesCount => 282;
        public override uint ObjTypesPointer => 0x0800e468;

        public override uint[] AnimSetPointers => new uint[]
        {
            0x082627EC,
            0x082677B8,
            0x082860B8,
            0x08287998,
            0x08289DF8,
            0x0828E9DC,
            0x08291C70,
            0x0829AA58,
            0x0829C86C,
            0x0829D9CC,
            0x0829E464,
            0x082A1B94,
            0x082BB054,
            0x082BB944,
            0x082BF844,
            0x082C99D8,
            0x082CAD6C,
            0x082CE5E0,
            0x082D0AAC,
            0x082D1A40,
            0x082DA294,
            0x082DC548,
            0x082E09A4,
            0x082E63A0,
            0x082EFDFC,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x0805E1D4, // genericNPC
            0x0805E23C, // talkToVictim
            0x0805E4D4, // script_waitForInputOrTime
            0x08060418, // notFound
            0x080606C0, // waitForPagedText
            0x08060728, // missing
            0x08060770, // Sparx01H01
            0x080607C4, // Sparx01H02
            0x08060818, // Sparx01H03
            0x0806086C, // Blinky01A01
            0x080608E4, // Bianca01A01
            0x0806095C, // Bianca01A03
            0x080609D4, // Blinky01A03
            0x08060A28, // Hunter01B01
            0x08060AC4, // Blinky01B04
            0x08060B18, // Blinky01B05
            0x08060B90, // Blinky01B07
            0x08060BE8, // Moneybags01A01
            0x08060C64, // Moneybags01B01
            0x08060CB8, // Boss0101
            0x08060D0C, // Sparx02H01
            0x08060D88, // Professor02A01
            0x08060E48, // Coco02A01
            0x08060EE4, // Hunter02A01
            0x08060F38, // Byrd02A01
            0x08060FB0, // Crash02B03
            0x08061008, // Moneybags02A01
            0x08061084, // Moneybags02B01
            0x080610D8, // Boss0201
            0x0806112C, // Sparx03H01
            0x080611A4, // Crunch03A01
            0x080611F8, // Blinky03B04
            0x0806124C, // Crash03B01
            0x080612A4, // Moneybags03A01
            0x080612FC, // Moneybags03B01
            0x08061350, // Boss0301
            0x080613EC, // Boss0304
            0x08061440, // Boss0305
            0x08061494, // Boss0306
            0x080614E8, // Hunter04A01
            0x0806153C, // Crash04A01
            0x08061590, // Agent04B04
            0x080615E8, // Moneybags04A01
            0x08061640, // Moneybags04B01
            0x08061694, // Boss0401
            0x08061734, // Professor05A01
            0x08061788, // Crash05A01
            0x080617E0, // Moneybags05A01
            0x0806185C, // Moneybags05B01
            0x080618B0, // Boss0501
            0x08061904, // Boss0502
            0x0806197C, // Boss0504
            0x080619F4, // Boss0506
            0x08061AA0, // instructionsRaiseTheGate
            0x08061B24, // instructionsIceWallMelt
            0x08061BB0, // instructionsFireWallExtinguish
            0x08061C38, // instructionsRockVinePull
            0x08061CBC, // instructionsTugOfWar
            0x08061D40, // instructionsIceChopper
            0x08061DE8, // instructionsAirAttack
            0x08061E90, // instructionsSpyroGlide
            0x08061F3C, // instructionsDragonAssault
            0x08061FE4, // instructionsDeflection
            0x080620D0, // instructionsJeep
            0x080621BC, // instructionsWalker
            0x08062288, // instructionsBridgeFight
            0x08062374, // instructionsGemRush
            0x080623D4, // instructionsPortalRush
            0x08062430, // instructionsChase
            0x080624D4, // instructionsJoust
            0x08062558, // instructionsPlatforms
            0x080625DC, // instructionsSheepHerd
            0x08062684, // instructionsSheepBounce
            0x0806272C, // instructionsSheepShake
            0x080627D4, // instructionsSheepJeep
            0x080D8A30, // movie_license
            0x080D8AE0, // movie_credits
            0x080D8B54, // movie_secretCredits
            0x080D8C4C, // "titleText"
            0x080D9434, // waitForPagedText
            0x080D9528, // takeCrateHitL
            0x080D95A0, // takeCrateHitR
            0x080D9628, // ninaBossEndTrigger
            0x080D96B0, // boilerScript
            0x080D970C, // roofScript
            0x080D9784, // genericCrateHit
            0x080D9890, // leftish
            0x080D998C, // rightish
            0x080D9A7C, // runningBackAndForth
            0x080D9B9C, // gotoNextRound
            0x080D9C8C, // mainLoop
            0x080D9D7C, // ninaSpawn
            0x080D9DF4, // ninaCrateL
            0x080D9E48, // ninaCrateR
            0x080D9EB4, // ninaProfCage
            0x080D9F84, // ninaCocoCage
            0x080DA058, // boilerOpen
            0x080DA09C, // moveUp
            0x080DA0EC, // ninaRunLeft
            0x080DA1BC, // ninaRunRight
            0x080DA2A4, // genericCrateCode
            0x080DA3B8, // raiseRoof
            0x080DD8A8, // spawnLabAssRear
            0x080DD94C, // spawnLabAssFront
            0x080DD9E8, // takeHornDive
            0x080DDAD0, // takeFlame
            0x080DDC14, // attack
            0x080DDD04, // idle
            0x080DDDF4, // venusPlant
            0x080DDEB0, // turninright
            0x080DDFB8, // turninleft
            0x080DE0A8, // "patrol"
            0x080DE198, // rhynocTurnScript
            0x080DE224, // rhynocPatrolScript
            0x080DE2F8, // rhynocIcePatrol
            0x080DE3B4, // turninright
            0x080DE4A4, // turninleft
            0x080DE594, // rhynocIcePatrolTurn
            0x080DE654, // rhynocClubPatrol
            0x080DE72C, // whamright
            0x080DE828, // whamleft
            0x080DE918, // rhynocClubTurn
            0x080DE970, // rhynocClubWham
            0x080DEA18, // rhynocThrowScript
            0x080DEB64, // "WaitingForKill"
            0x080DEC54, // rhynocRockScript
            0x080DED18, // rhynocFly
            0x080DEDAC, // rhynocHoverUp
            0x080DEE34, // rhynocHoverDown
            0x080DEECC, // rhynocJungleFly
            0x080DEF64, // rhynocJungleHoverUp
            0x080DEFF4, // rhynocJungleHoverDown
            0x080DF094, // turninright
            0x080DF184, // turninleft
            0x080DF274, // infernoLabAssTurnScript
            0x080DF3E0, // infernoLabAssPatrolScript
            0x080DF504, // firin
            0x080DF5F4, // idlin
            0x080DF6E4, // infernoLabAssShoot
            0x080DF770, // labAssPatrol
            0x080DF82C, // walkin
            0x080DF91C, // walkin
            0x080DFA0C, // labAssTurn
            0x080DFAD4, // throwin
            0x080DFBC4, // labAssCamo
            0x080DFC78, // flying
            0x080DFD68, // coconutThrown
            0x080DFE2C, // coconutExplode
            0x080DFEAC, // walkerLabAssFloater
            0x080DFFF8, // gulpSpawny
            0x080E0058, // gulpBarrageScript
            0x080E0134, // gulpFireScript
            0x080E01E0, // gulpDead
            0x080E02DC, // gulpHit
            0x080E03FC, // "WaitingForBoom"
            0x080E04F8, // gulpRocketScript
            0x080E0648, // "WaitingForBoom"
            0x080E0744, // gulpRocketRepel
            0x080E0818, // gulpScorch
            0x080E0858, // gulpScorched
            0x080E08AC, // gulpF1Script
            0x080E0924, // gulpF2Script
            0x080E09A8, // gulpF3Script
            0x080E0A20, // gulpF4Script
            0x080E0A98, // gulpF5Script
            0x080E0B08, // gulpFUpScript
            0x080E0B8C, // crushCounter
            0x080E0BF8, // counting
            0x080E0CE8, // crushCounting
            0x080E0D40, // crushRunStartScript
            0x080E0DC8, // headinleft
            0x080E0ED8, // headinright
            0x080E0FC8, // crushRunScript
            0x080E1058, // crushStop
            0x080E10F8, // crushStun
            0x080E11A8, // weregood
            0x080E1298, // crushGetUp
            0x080E12EC, // headinleft
            0x080E13DC, // headinright
            0x080E14E8, // goincrazy
            0x080E15D8, // crushRocketed
            0x080E16AC, // crushTurn
            0x080E176C, // turninright
            0x080E1874, // turninleft
            0x080E1964, // crushCharge
            0x080E19F0, // crushUpAndAtThem
            0x080E1ABC, // crushFlame
            0x080E1B50, // crushStayPut
            0x080E1BC8, // cortexFly
            0x080E1C70, // cortexFlipUp
            0x080E1D34, // cortexFlipDown
            0x080E1DF0, // cortexFlipDownHurt
            0x080E1EBC, // cortexDropMine
            0x080E1F78, // droppin
            0x080E2068, // cortexMine
            0x080E213C, // cortexUberBlast
            0x080E21B4, // cortexOw
            0x080E227C, // cortexLaughAtPunyMortal
            0x080E2340, // cortexReload
            0x080E23AC, // cortexDefeat
            0x080E2440, // cortexEscape
            0x080E24F4, // weregood
            0x080E25E4, // crushUpCharge
            0x080E263C, // lavaFountain
            0x080E26B8, // floorSpikesScript
            0x080E2744, // wallSpikesScript
            0x080E27DC, // icicleScript
            0x080E2880, // "fall"
            0x080E297C, // icicleFallingScript
            0x080E2A2C, // "sfx"
            0x080E2B1C, // swinginAxe
            0x080E2B84, // electrix
            0x080E2BE4, // plantVines
            0x080E2C6C, // lavaSpawner
            0x080E2CCC, // lavaSpurt
            0x080E2DC0, // "attackL"
            0x080E2EBC, // "attackR"
            0x080E2FAC, // moatWorm
            0x080E302C, // landmine
            0x080E3084, // landmineBoom
            0x080E30C8, // chaseWall
            0x080E311C, // chaseWallHit1
            0x080E3170, // chaseWallHit2
            0x080E31C8, // chaseWallDead
            0x080E32C4, // flipping
            0x080E3408, // platformer
            0x080E34F8, // triggerJoustPlatform
            0x080E360C, // spawnJoustPlatformL
            0x080E3714, // spawnJoustPlatformR
            0x080E3800, // turnin
            0x080E38F0, // turnin
            0x080E39E0, // triggerJoustPlatformTurn
            0x080E3A50, // platformCount
            0x080E3A9C, // joustPlatformL
            0x080E3B1C, // joustPlatformR
            0x080E3BA8, // joustPlatform2L
            0x080E3C34, // joustPlatform2R
            0x080E3CC0, // joustPlatform3L
            0x080E3D4C, // joustPlatform3R
            0x080E3DD8, // joustPlatform4L
            0x080E3E64, // joustPlatform4R
            0x080E3F34, // gemJoust
            0x080E4024, // gem2Joust
            0x080E4114, // gem3Joust
            0x080E4204, // gem4Joust
            0x080E4308, // joustPlatformFall
            0x080E435C, // joustCounter
            0x080E43B8, // joustCount
            0x080E4480, // triggerScript
            0x080E44C0, // triggerUpScript
            0x080E4504, // triggerDownScript
            0x080E4548, // triggerLeftScript
            0x080E458C, // triggerRightScript
            0x080E45CC, // trigger1Script
            0x080E460C, // trigger2Script
            0x080E464C, // trigger3Script
            0x080E468C, // trigger4Script
            0x080E46CC, // trigger5Script
            0x080E471C, // checkpoint
            0x080E4768, // topDownPortal
            0x080E47AC, // topDownPortalFail
            0x080E47EC, // triggerCandleFlame
            0x080E4850, // lightCandle
            0x080E48CC, // triggerTorchSmall
            0x080E4934, // lightTorchSmall
            0x080E49B0, // triggerTorchLarge
            0x080E4A04, // lightTorchLarge
            0x080E4A7C, // triggerIceTorch
            0x080E4AD0, // lightIceTorch
            0x080E4B44, // NPCScript
            0x080E4B78, // NPCTalkScript
            0x080E4BF8, // blinkyDirtAppear
            0x080E4CBC, // blinkySnowAppear
            0x080E4D7C, // spawnPortal
            0x080E4E6C, // hiddenPortalC_C
            0x080E4EF0, // spawnPortal
            0x080E4FE0, // hiddenPortalC_J
            0x080E5064, // spawnPortal
            0x080E5154, // hiddenPortalC_S
            0x080E51D8, // hiddenPortalI_C
            0x080E5248, // hiddenPortalI_J
            0x080E52B8, // hiddenPortalI_S
            0x080E533C, // hiddenPortalF_C
            0x080E53AC, // hiddenPortalF_J
            0x080E541C, // hiddenPortalF_S
            0x080E54A0, // hiddenPortalJ_C
            0x080E5510, // hiddenPortalJ_J
            0x080E5580, // hiddenPortalJ_S
            0x080E560C, // hiddenPortalSpawnC
            0x080E5680, // hiddenPortalSpawnJ
            0x080E56F4, // hiddenPortalSpawnS
            0x080E5748, // poopASpyro
            0x080E591C, // bouncyBranchBounce
            0x080E5980, // idle
            0x080E5A70, // bouncyBranch
            0x080E5AE0, // platformUpScript
            0x080E5B74, // platformDownScript
            0x080E5BE4, // platformLeftScript
            0x080E5C60, // platformRightScript
            0x080E5CD0, // platformIdleScript
            0x080E5D18, // platformCrumbleScript
            0x080E5E50, // checkspawn
            0x080E5F54, // sheepHornDive
            0x080E601C, // sheepPound
            0x080E605C, // sheepPounded
            0x080E60AC, // sheepDrop
            0x080E6120, // droppin
            0x080E6210, // sheepDropped
            0x080E627C, // sheepFall
            0x080E636C, // movin
            0x080E6470, // movin
            0x080E658C, // "eating"
            0x080E667C, // "hungry"
            0x080E676C, // sheepPatrol
            0x080E6850, // turnRight
            0x080E694C, // turnLeft
            0x080E6A5C, // "startle"
            0x080E6B4C, // sheepTurn
            0x080E6C88, // sheepDie
            0x080E6DDC, // butterflyFlutter
            0x080E6E74, // sniff
            0x080E6F7C, // bunny
            0x080E70A8, // bunnyDie
            0x080E71AC, // bouncySheep
            0x080E7270, // herding
            0x080E7360, // sheepHerder
            0x080E7424, // again
            0x080E753C, // sheepHerd
            0x080E75DC, // sheepHerdKill
            0x080E7618, // sheepHerded
            0x080E76C4, // sheepChaseSpawn
            0x080E77D8, // sheepRun
            0x080E7850, // sheepJump
            0x080E78C0, // sheepJumpBig
            0x080E7930, // sheepJumpDown
            0x080E79D8, // bouncySheepBounce
            0x080E7AA8, // lavaFountainScript
            0x080E7B8C, // sheepShoot
            0x080E7C40, // childKilled
            0x080E7D04, // breakoutLabAssProjectileScript
            0x080E7DC4, // "InnerLoop"
            0x080E7EB4, // globalController
            0x080E7F38, // breakoutLabAssShooterScript
            0x080E7FC8, // breakoutLabAssScript
            0x080E805C, // breakoutRhynocShieldScript
            0x080E8104, // breakoutRhynocBallScript
            0x080E8194, // breakoutRhynocScript
            0x080E822C, // breakoutWallOnScript
            0x080E8280, // stepLeft
            0x080E82C8, // stepRight
            0x080E8310, // stepDown
            0x080E8348, // newWave
            0x080E838C, // genericDeathScript
            0x080E845C, // DieAndDropBallScript
            0x080E84A4, // genericDrop
            0x080E8504, // genericTurnRightAndDrop
            0x080E854C, // genericTurnLeftAndDrop
            0x080E8594, // breakoutWallOffScript
            0x080E85F4, // genericTurnLeftScript
            0x080E8648, // genericTurnRightScript
        };
    }
}