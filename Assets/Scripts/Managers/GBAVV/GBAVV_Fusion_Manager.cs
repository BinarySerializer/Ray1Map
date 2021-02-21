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

                    // If the command count is too high we ignore it
                    if (script.Commands.Length >= 199)
                    {
                        Debug.Log($"Skipping script {script.Name}");
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
            var str = new StringBuilder();

            // Start by parsing every script
            var scripts = scriptPointers.Select(x => s.DoAt(x, () => s.SerializeObject<GBAVV_Script>(default))).ToArray();

            // Enumerate every script
            foreach (var script in scripts)
            {
                script.TranslatedString(str);

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
            new LevInfo(5, "Sheep Stampede"),
            new LevInfo(6, "Sheep Stampede"),
            new LevInfo(7, "Tanks for the Memories"),
            new LevInfo(8, "Tanks for the Memories"),
            new LevInfo(9, "Chopper Stopper"),
            new LevInfo(10, "Chopper Stopper"),
            new LevInfo(11, "Bonus: Crate Smash"),
            new LevInfo(12, "Bonus: Crate Smash"),
            new LevInfo(13, "Bonus: Crate Smash"),
            new LevInfo(14, "Bonus: Freefallin'"),
            new LevInfo(15, "Bonus: Freefallin'"),
            new LevInfo(16, "Bonus: Crunch Time"),
            new LevInfo(17, "Crashin' down the River"),
            new LevInfo(18, "Crashin' Down the River"),
            new LevInfo(19, "Crashin' Down the River"),
            new LevInfo(20, "Buy Card 1"),
            new LevInfo(21, "Trading Card Shop"),
            new LevInfo(22, "Bridge Fight"),
            new LevInfo(23, "Spyro Battle"),
            new LevInfo(24, "Bridge Fight"),
            new LevInfo(25, "Bonus: Pumpin' Iron"),
            new LevInfo(26, "Bonus: Crate Step"),
            new LevInfo(27, "Bonus: Crate Step"),
            new LevInfo(28, "Bonus: Crate Step"),
            new LevInfo(29, "Crash and Burn"),
            new LevInfo(30, "Crash and Burn"),
            new LevInfo(31, "World 2a"),
            new LevInfo(32, "World 2b"),
            new LevInfo(33, "Tiny Takeover"),
            new LevInfo(34, "Bonus: Crate Smash "),
            new LevInfo(35, "Bonus: Crate Smash"),
            new LevInfo(36, "Bonus: Crate Smash"),
            new LevInfo(37, "Polar Express"),
            new LevInfo(38, "Polar Express"),
            new LevInfo(39, "Polar Express"),
            new LevInfo(40, "Frigid Waters"),
            new LevInfo(41, "Frigid Waters"),
            new LevInfo(42, "Frigid Waters"),
            new LevInfo(43, "Sheep Patrol"),
            new LevInfo(44, "Sheep Patrol"),
            new LevInfo(45, "Blizzard Ball"),
            new LevInfo(46, "Blizzard Ball"),
            new LevInfo(47, "Blizzard Ball"),
            new LevInfo(48, "Shell Game 2"),
            new LevInfo(49, "Mystery Game 2"),
            new LevInfo(50, "Spinning Wheel Game 2"),
            new LevInfo(51, "Crate Shuffle Shop"),
            new LevInfo(52, "Spinning Wheel Shop"),
            new LevInfo(53, "Mystery Shop"),
            new LevInfo(54, "Bridge Fight"),
            new LevInfo(55, "Tankin' over the world"),
            new LevInfo(56, "Tankin' over the world"),
            new LevInfo(57, "Chop 'til you Drop"),
            new LevInfo(58, "Chop 'til you Drop"),
            new LevInfo(59, "Rocket Power"),
            new LevInfo(60, "Rocket Power"),
            new LevInfo(61, "Bonus: Crate Step"),
            new LevInfo(62, "Bonus: Crate Step"),
            new LevInfo(63, "Bonus: Crate Step"),
            new LevInfo(64, "World 3a"),
            new LevInfo(65, "World 3b"),
            new LevInfo(66, "Bat Attack"),
            new LevInfo(67, "Bat Attack"),
            new LevInfo(68, "Bat Attack"),
            new LevInfo(69, "In Hot Water"),
            new LevInfo(70, "In Hot Water"),
            new LevInfo(71, "In Hot Water"),
            new LevInfo(72, "Nina"),
            new LevInfo(73, "Bonus: Weightlift"),
            new LevInfo(74, "Spinning Wheel Game 3"),
            new LevInfo(75, "Spinning Wheel Shop"),
            new LevInfo(76, "Buy Card 3"),
            new LevInfo(77, "Trading Card Shop"),
            new LevInfo(78, "Bridge Fight"),
            new LevInfo(79, "Bonus: Crate Smash"),
            new LevInfo(80, "Bonus: Crate Smash"),
            new LevInfo(81, "Bonus: Crate Smash"),
            new LevInfo(82, "World 4a"),
            new LevInfo(83, "World 4b"),
            new LevInfo(84, "Castle Chaos"),
            new LevInfo(85, "Castle Chaos"),
            new LevInfo(86, "Castle Chaos"),
            new LevInfo(87, "Bats in the Belfry"),
            new LevInfo(88, "Bats in the Belfry"),
            new LevInfo(89, "Bats in the Belfry"),
            new LevInfo(90, "Sheep Shuttle"),
            new LevInfo(91, "Sheep Shuttle"),
            new LevInfo(92, "Up, up, and away"),
            new LevInfo(93, "Up, up, and away"),
            new LevInfo(94, "Bonus: Freefallin'"),
            new LevInfo(95, "Bonus: Freefallin'"),
            new LevInfo(96, "Bonus: Weightlift"),
            new LevInfo(97, "Tanks 'R Us"),
            new LevInfo(98, "Tanks 'R Us"),
            new LevInfo(99, "Buy Card 4"),
            new LevInfo(100, "Trading Card Shop"),
            new LevInfo(101, "Mystery Game 2"),
            new LevInfo(102, "Mystery Shop"),
            new LevInfo(103, "Shell Game 4"),
            new LevInfo(104, "Crate Shuffle Shop"),
            new LevInfo(105, "Ripto's Magical Mystery Tour"),
            new LevInfo(106, "Bonus: Weightlift"),
            new LevInfo(107, "World 5a"),
            new LevInfo(108, "Tech Deflect"),
            new LevInfo(109, "Tech Deflect"),
            new LevInfo(110, "Tech Deflect"),
            new LevInfo(111, "Crash at the Controls"),
            new LevInfo(112, "Crash at the Controls"),
            new LevInfo(113, "Bonus: Crate smash"),
            new LevInfo(114, "Bonus: Crate smash"),
            new LevInfo(115, "Bonus: Crate smash"),
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
            new LevInfo(126, "Spinning Wheel Game 5"),
            new LevInfo(127, "Spinning Wheel Shop"),
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
            new ObjTypeInit(8, 11, null), // 19
            new ObjTypeInit(8, 11, null), // 20
            new ObjTypeInit(8, 11, null), // 21
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
            new ObjTypeInit(9, 57, null), // 36
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
            0x080764F8, // World1_BossBeat_CS
            0x08076A80, // World4_BossBeat_CS
            0x08076B7C, // World5_CS
            0x08076C0C, // World5_BossBeat_CS
            0x08076C9C, // allMovie
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
}