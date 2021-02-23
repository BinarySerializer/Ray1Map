using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace R1Engine
{
    public class GBAVV_Script : R1Serializable
    {
        public bool IsValid { get; set; } = true;

        public GBAVV_ScriptCommand[] Commands { get; set; }

        public string DisplayName => Commands?.FirstOrDefault()?.DisplayName;
        public string Name => Commands?.FirstOrDefault()?.Name;

        public override void SerializeImpl(SerializerObject s)
        {
            if (Commands == null)
            {
                var cmds = new List<GBAVV_ScriptCommand>();
                var index = 0;

                do
                {
                    cmds.Add(s.SerializeObject<GBAVV_ScriptCommand>(default, name: $"{nameof(Commands)}[{index++}]"));
                } while (!(cmds[cmds.Count - 1].Type == GBAVV_ScriptCommand.CommandType.Return && cmds.ElementAtOrDefault(cmds.Count - 2)?.Type != GBAVV_ScriptCommand.CommandType.SkipNextIfInputCheck && cmds.ElementAtOrDefault(cmds.Count - 2)?.Type != GBAVV_ScriptCommand.CommandType.SkipNextIfField08) && index < 100);
                
                if (index == 100 || cmds.Any(x => x.PrimaryCommandType >= 100))
                {
                    Debug.Log($"Invalid script at {Offset}");
                    IsValid = false;
                }

                Commands = cmds.ToArray();
            }
            else
            {
                Commands = s.SerializeObjectArray<GBAVV_ScriptCommand>(Commands, Commands.Length, name: nameof(Commands));
            }
        }

        public List<string> TranslatedString(GBAVV_Map2D_AnimSet[] animSets)
        {
            var output = new List<string>();

            var depth = 0;
            var scriptIndex = 0;

            void logCMD(GBAVV_ScriptCommand cmd, string parsedText)
            {
                log($"unknownCMD_{cmd.PrimaryCommandType:00}_{cmd.SecondaryCommandType:00}({parsedText});");
                // log($"CMD: {cmd.PrimaryCommandType:00}-{cmd.SecondaryCommandType:00} -> {parsedText}");
            }
            string getLogPrefix() => new string(' ', depth * 2);
            void log(string logStr, Pointer p = null) => output.Add($"{getLogPrefix()}{logStr}{(p != null ? $" // 0x{p.AbsoluteOffset:X8}" : "")}");
            void logSubScript(GBAVV_Script scr)
            {
                log($"{scr.DisplayName}();", scr.Offset);

                /*
                depth++;
                logScript(cmd.ReferencedScript);
                depth--;
                */
            }
            string getAnimString(Pointer p)
            {
                if (p == null)
                    return $"null";

                var animSetIndex = animSets.FindItemIndex(x => x.Animations.Any(a => a.Offset == p));
                var animIndex = animSets.ElementAtOrDefault(animSetIndex)?.Animations.FindItemIndex(x => x.Offset == p) ?? -1;

                return $"Animations[{animSetIndex}][{animIndex}]";
            }
            void logScriptCMD()
            {
                var cmd = Commands[scriptIndex];
                scriptIndex++;

                switch (cmd.Type)
                {
                    case GBAVV_ScriptCommand.CommandType.Name:
                        log($"{cmd.DisplayName}()", Offset);
                        log("{");
                        depth++;
                        break;

                    case GBAVV_ScriptCommand.CommandType.Script:
                        logSubScript(cmd.ReferencedScript);
                        break;

                    case GBAVV_ScriptCommand.CommandType.SkipNextIfInputCheck:
                        log($"if (checkInputs({cmd.Input.AsArgs()}) == false)");
                        depth++;
                        logScriptCMD();
                        depth--;
                        break;

                    case GBAVV_ScriptCommand.CommandType.SkipNextIfField08:
                        log($"if (field_0x8 == false)");
                        depth++;
                        logScriptCMD();
                        depth--;
                        break;

                    case GBAVV_ScriptCommand.CommandType.Reset:
                        logSubScript(this);
                        break;

                    case GBAVV_ScriptCommand.CommandType.Return:
                        log($"return;");
                        break;

                    case GBAVV_ScriptCommand.CommandType.SetUnknownInputData:
                        log($"InputData = ({cmd.Input.AsArgs()});");
                        break;

                    case GBAVV_ScriptCommand.CommandType.Wait:
                        log($"wait({cmd.Param})");
                        break;

                    case GBAVV_ScriptCommand.CommandType.WaitWhileInputCheck:
                        log($"waitWhile(checkInputs({cmd.Input.AsArgs()}) == true)");
                        break;

                    case GBAVV_ScriptCommand.CommandType.Dialog:
                        log($"show(\"{cmd.Dialog.Item.Text}\");");
                        break;

                    case GBAVV_ScriptCommand.CommandType.IsFlipped:
                        log($"Flipped = {(cmd.Param == 1).ToString().ToLower()};");
                        break;

                    case GBAVV_ScriptCommand.CommandType.Animation:
                        log($"Animation = {getAnimString(cmd.ParamPointer)};", cmd.ParamPointer);
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
                        log($"move{(cmd.Type == GBAVV_ScriptCommand.CommandType.Movement_X ? "X" : "Y")}({cmd.Movement.Speed}, {cmd.Movement.Param_1}, {cmd.Movement.Param_2});");
                        break;

                    case GBAVV_ScriptCommand.CommandType.SecondaryAnimation:
                        log($"SecondaryAnimation = {getAnimString(cmd.ParamPointer)};", cmd.ParamPointer);
                        break;

                    case GBAVV_ScriptCommand.CommandType.PlaySound:
                        log($"playSound({cmd.Sound.AsArgs()});");
                        break;

                    case GBAVV_ScriptCommand.CommandType.DialogPortrait:
                        log($"Portrait = {getAnimString(cmd.ParamPointer)};", cmd.ParamPointer);
                        break;

                    default:
                        if (cmd.ParamPointer != null) // ROM pointer
                            logCMD(cmd, $"0x{cmd.ParamPointer.AbsoluteOffset:X8}");
                        else if (cmd.Param >= GBA_ROMBase.Address_WRAM && cmd.Param < GBA_ROMBase.Address_WRAM + 0x40000) // RAM pointer
                            logCMD(cmd, $"0x{cmd.Param:X8}");
                        else // Value
                            logCMD(cmd, $"{(int)cmd.Param}");
                        break;
                }
            }

            // Log every command
            while (scriptIndex < Commands.Length)
                logScriptCMD();

            depth--;
            log("}");

            return output;
        }
    }
}