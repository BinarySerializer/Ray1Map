using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R1Engine
{
    public abstract class GBAVV_Fusion_Manager : GBAVV_BaseManager
    {
        public override UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            //FindDataInROM(context.Deserializer, context.FilePointer(GetROMFilePath));
            //LogParsedScripts(context.Deserializer, ScriptPointers.Select(x => new Pointer(x, context.GetFile(GetROMFilePath))));
            //LogLevelInfos(FileFactory.Read<GBAVV_ROM>(GetROMFilePath, context, (s, r) => r.CurrentLevInfo = LevInfos[context.Settings.Level]));
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
                    foundScripts.Add(new Tuple<long, string>(getPointer(i), s.DoAt(new Pointer(values[i + 2], offset.file), () => s.SerializeString(default, name: "ScriptName"))));
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
            var depth = 0;

            // Keep track of logged scripts to avoid loop
            var loggedScripts = new HashSet<GBAVV_Script>();

            void logScript(GBAVV_Script script)
            {
                void logCMD(GBAVV_ScriptCommand cmd, string parsedText) => log($"CMD: {cmd.PrimaryCommandType:00}-{cmd.SecondaryCommandType:00} -> {parsedText}");
                string getLogPrefix() => new string(' ', depth * 2);
                void log(string logStr) => str.AppendLine($"{getLogPrefix()}{logStr}");
                void logSubScript(GBAVV_Script scr)
                {
                    log($"{scr.Commands.First().Name}();");

                    /*
                    depth++;
                    logScript(cmd.ReferencedScript);
                    depth--;
                    */
                }

                if (loggedScripts.Contains(script))
                    return;

                loggedScripts.Add(script);

                // Log every command
                foreach (var cmd in script.Commands)
                {
                    switch (cmd.Type)
                    {
                        case GBAVV_ScriptCommand.CommandType.Name:
                            log($"{cmd.Name}() [0x{script.Offset.AbsoluteOffset:X8}]");
                            log("{");
                            depth++;
                            break;

                        case GBAVV_ScriptCommand.CommandType.Script:
                            logSubScript(cmd.ReferencedScript);
                            break;

                        case GBAVV_ScriptCommand.CommandType.Terminator:
                            log($"return;");
                            break;

                        case GBAVV_ScriptCommand.CommandType.Wait:
                            log($"wait({cmd.Param});");
                            break;

                        case GBAVV_ScriptCommand.CommandType.Dialog:
                            log($"show(\"{cmd.Dialog.Item.Text}\");");
                            break;

                        case GBAVV_ScriptCommand.CommandType.IsFlipped:
                            log($"Flipped = {(cmd.Param == 1).ToString().ToLower()};");
                            break;

                        case GBAVV_ScriptCommand.CommandType.Animation:
                            log($"Animation = 0x{cmd.ParamPointer.AbsoluteOffset:X8};");
                            break;

                        case GBAVV_ScriptCommand.CommandType.IsEnabled:
                            log($"Enabled = {(cmd.Param == 1).ToString().ToLower()};");
                            break;

                        case GBAVV_ScriptCommand.CommandType.ConditionalScript:
                            log($"if (condition_{cmd.ConditionalScriptReference.Condition:00})");
                            //log("{");
                            depth++;

                            logSubScript(cmd.ConditionalScriptReference.Script);

                            depth--;
                            //log("}");
                            break;

                        case GBAVV_ScriptCommand.CommandType.Movement_X:
                        case GBAVV_ScriptCommand.CommandType.Movement_Y:
                            log($"move{(cmd.Type == GBAVV_ScriptCommand.CommandType.Movement_X ? "X" : "Y")}(speed: {cmd.Movement.Speed}, param_2: {cmd.Movement.Param_1}, param_3: {cmd.Movement.Param_2});");
                            break;

                        case GBAVV_ScriptCommand.CommandType.DialogPortrait:
                            log($"Portrait = 0x{cmd.ParamPointer.AbsoluteOffset:X8};");
                            break;

                        default:
                            if (cmd.ParamPointer != null)
                                logCMD(cmd, $"0x{cmd.ParamPointer.AbsoluteOffset:X8}");
                            else if (cmd.Param >= GBA_ROMBase.Address_WRAM && cmd.Param < GBA_ROMBase.Address_WRAM + 0x40000)
                                logCMD(cmd, $"0x{cmd.Param:X8}");
                            else
                                logCMD(cmd, $"{cmd.Param}");
                            break;
                    }
                }

                depth--;
                str.AppendLine("}");
            }

            // Start by parsing every script
            var scripts = scriptPointers.Select(x => s.DoAt(x, () => s.SerializeObject<GBAVV_Script>(default))).ToArray();

            // Enumerate every script
            foreach (var script in scripts)
            {
                // Ignore the script if it's referenced from another script as it's a sub-script then
                //if (scripts.SelectMany(x => x.Commands).Any(x => x.ReferencedScript == script))
                //continue;

                // Ignore the script if it's CS script
                if (script.Commands.First().Name.Contains("_CS"))
                    continue;

                // Ignore movie scripts
                if (script.Commands.First().Name.Contains("Movie") || script.Commands.First().Name.Contains("movie"))
                    continue;

                loggedScripts.Clear();
                logScript(script);

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

        public abstract int ObjTypesCount { get; }
        public abstract uint ObjTypesPointer { get; }
        public abstract uint[] AnimSetPointers { get; }
        //public abstract uint[] ScriptPointers { get; }
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
    }
}