using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace R1Engine
{
    public class GBAVV_Script : R1Serializable
    {
        public GBAVV_ScriptCommand[] Commands { get; set; }

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
                } while (cmds.Last().Type != GBAVV_ScriptCommand.CommandType.Terminator && index < 200);

                if (index == 200)
                    Debug.Log($"Invalid script at {Offset}");

                Commands = cmds.ToArray();
            }
            else
            {
                Commands = s.SerializeObjectArray<GBAVV_ScriptCommand>(Commands, Commands.Length, name: nameof(Commands));
            }
        }

        public void TranslatedString(StringBuilder str)
        {
            var depth = 0;

            void logCMD(GBAVV_ScriptCommand cmd, string parsedText) => log($"CMD: {cmd.PrimaryCommandType:00}-{cmd.SecondaryCommandType:00} -> {parsedText}");
            string getLogPrefix() => new string(' ', depth * 2);
            void log(string logStr) => str.AppendLine($"{getLogPrefix()}{logStr}");
            void logSubScript(GBAVV_Script scr)
            {
                log($"{scr.Name}();");

                /*
                depth++;
                logScript(cmd.ReferencedScript);
                depth--;
                */
            }

            // Log every command
            foreach (var cmd in Commands)
            {
                switch (cmd.Type)
                {
                    case GBAVV_ScriptCommand.CommandType.Name:
                        log($"{cmd.Name}() [0x{Offset.AbsoluteOffset:X8}]");
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
            log("}");
        }
    }
}