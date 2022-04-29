﻿using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Nintendo.GBA;
using UnityEngine;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Script : BinarySerializable
    {
        public bool IsValid { get; set; } = true;
        public BinaryFile BaseFile { get; set; } // Set before serializing
        public bool SerializeFLC { get; set; } // Set before serializing

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
                    cmds.Add(s.SerializeObject<GBAVV_ScriptCommand>(default, x =>
                    {
                        x.SerializeFLC = SerializeFLC;
                        x.BaseFile = BaseFile;
                    }, name: $"{nameof(Commands)}[{index++}]"));
                } while (!(cmds[cmds.Count - 1].Type == GBAVV_ScriptCommand.CommandType.Return && cmds.ElementAtOrDefault(cmds.Count - 2)?.Type != GBAVV_ScriptCommand.CommandType.SkipNextIfInputCheck && cmds.ElementAtOrDefault(cmds.Count - 2)?.Type != GBAVV_ScriptCommand.CommandType.SkipNextIfField08) && index < 500 && cmds[cmds.Count - 1].PrimaryCommandType < 100 && cmds[cmds.Count - 1].SecondaryCommandType < 100);
                
                if (index == 500 || cmds.Any(x => x.PrimaryCommandType >= 100 || x.SecondaryCommandType >= 100))
                {
                    Debug.Log($"Invalid script at {Offset}");
                    IsValid = false;
                }

                Commands = cmds.ToArray();
            }
            else
            {
                Commands = s.SerializeObjectArray<GBAVV_ScriptCommand>(Commands, Commands.Length, x =>
                {
                    x.SerializeFLC = SerializeFLC;
                    x.BaseFile = BaseFile;
                }, name: nameof(Commands));
            }
        }

        protected List<string> CachedTranslatedAllStrings { get; set; }

        public List<string> TranslatedStringAll(GBAVV_AnimSet[] animSets, Dictionary<Pointer, int> locTable, List<string> list = null)
        {
            if (CachedTranslatedAllStrings != null)
                return CachedTranslatedAllStrings;

            var output = list ?? new List<string>();
            var foundScripts = new HashSet<GBAVV_Script>();

            translateScript(this);

            void translateScript(GBAVV_Script s)
            {
                if (s == null || foundScripts.Contains(s))
                    return;

                foundScripts.Add(s);

                s.TranslatedString(animSets, locTable, output);

                output.Add(String.Empty);

                foreach (var ss in s.Commands.Select(x => x.ReferencedScript ?? x.ConditionalScriptReference?.Script))
                    translateScript(ss);
            }

            CachedTranslatedAllStrings = output;

            return output;
        }

        public List<string> TranslatedString(GBAVV_AnimSet[] animSets, Dictionary<Pointer, int> locTable, List<string> list = null)
        {
            var output = list ?? new List<string>();

            var depth = 0;
            var scriptIndex = 0;

            void log(string logStr) => output.Add($"{getLogPrefix()}{logStr}");
            void logCommand(string cmdName, params string[] values) => log($"{cmdName}{(values.Any() ? " " : "")}{getParams(values)};");
            void logUnknownCommand(GBAVV_ScriptCommand cmd, string parsedText) => logCommand($"CMD_{cmd.PrimaryCommandType:00}_{cmd.SecondaryCommandType:00}", parsedText);
            string getLogPrefix() => new string('\t', depth);
            string getParams(string[] values) => String.Join(", ", values);
            string getAnimString(Pointer p)
            {
                if (p == null)
                    return $"NULL";

                var animSetIndex = animSets?.FindItemIndex(x => x.Animations.Any(a => a.Offset == p)) ?? -1;
                var animIndex = animSets?.ElementAtOrDefault(animSetIndex)?.Animations.FindItemIndex(x => x.Offset == p) ?? -1;

                return animIndex == -1 ? $"0x{p.StringAbsoluteOffset}" : $"Animations[{animSetIndex}][{animIndex}]";
            }

            void logScriptCMD()
            {
                var cmd = Commands[scriptIndex];
                scriptIndex++;

                switch (cmd.Type)
                {
                    case GBAVV_ScriptCommand.CommandType.Name:
                        log($"{cmd.DisplayName}:");
                        depth++;
                        break;

                    case GBAVV_ScriptCommand.CommandType.Script:
                        logCommand($"CALL", $"{cmd.ReferencedScript.DisplayName}");
                        break;

                    case GBAVV_ScriptCommand.CommandType.SkipNextIfInputCheck:
                        log($"IF (!function({getParams(cmd.Input.GetArgs())}))");
                        depth++;
                        logScriptCMD();
                        depth--;
                        break;

                    case GBAVV_ScriptCommand.CommandType.SkipNextIfField08:
                        log($"IF (!field_0x8)");
                        depth++;
                        logScriptCMD();
                        depth--;
                        break;

                    case GBAVV_ScriptCommand.CommandType.Reset:
                        logCommand($"GOTO", DisplayName);
                        break;

                    case GBAVV_ScriptCommand.CommandType.Return:
                        logCommand($"RETURN");
                        break;

                    case GBAVV_ScriptCommand.CommandType.SetUnknownInputData:
                        logCommand($"SET_UNKNOWN_DATA", cmd.Input.GetArgs());
                        break;

                    case GBAVV_ScriptCommand.CommandType.Wait:
                        logCommand($"WAIT", $"{cmd.Param}");
                        break;

                    case GBAVV_ScriptCommand.CommandType.WaitWhileInputCheck:
                        logCommand($"WAIT_WHILE (function({getParams(cmd.Input.GetArgs())}))");
                        break;

                    case GBAVV_ScriptCommand.CommandType.Dialog:
                    case GBAVV_ScriptCommand.CommandType.Credits:
                        if (FileSystem.mode == FileSystem.Mode.Web)
                        {
                            var locIndex = locTable.TryGetItem(cmd.Dialog.Offset);
                            logCommand($"DISPLAY", $"<locId={locIndex}>Text[{locIndex}]</locId>");
                        }
                        else
                        {
                            logCommand($"DISPLAY", $"\"{cmd.Dialog?.DefaultString?.Replace("\n", @"\n")}\"");
                        }

                        break;

                    case GBAVV_ScriptCommand.CommandType.IsFlipped:
                        logCommand($"FLIP", (cmd.Param == 1).ToString().ToUpper());
                        break;

                    case GBAVV_ScriptCommand.CommandType.Animation:
                        logCommand($"SET_ANIMATION", getAnimString(cmd.ParamPointer));
                        break;

                    case GBAVV_ScriptCommand.CommandType.IsEnabled:
                        logCommand($"ENABLED", (cmd.Param == 1).ToString().ToUpper());
                        break;

                    case GBAVV_ScriptCommand.CommandType.ConditionalScript:
                        log($"IF (condition_{cmd.ConditionalScriptReference.Condition:00})");
                        depth++;

                        logCommand($"CALL", $"{cmd.ConditionalScriptReference.Script.DisplayName}");

                        depth--;
                        break;

                    case GBAVV_ScriptCommand.CommandType.Movement_X:
                    case GBAVV_ScriptCommand.CommandType.Movement_Y:
                        logCommand($"MOVE_{(cmd.Type == GBAVV_ScriptCommand.CommandType.Movement_X ? "X" : "Y")}", $"{cmd.Movement.Speed}", $"{cmd.Movement.Param_1}", $"{cmd.Movement.Param_2}");
                        break;

                    case GBAVV_ScriptCommand.CommandType.SecondaryAnimation:
                        logCommand($"SET_SECONDARY_ANIMATION", getAnimString(cmd.ParamPointer));
                        break;

                    case GBAVV_ScriptCommand.CommandType.SpawnObject:
                        logCommand($"SPAWN_OBJECT", cmd.ObjSpawn.GetArgs());
                        break;

                    case GBAVV_ScriptCommand.CommandType.PlaySound:
                        logCommand($"PLAY_SFX", cmd.Sound.GetArgs());
                        break;

                    case GBAVV_ScriptCommand.CommandType.DialogPortrait:
                        logCommand($"SET_PORTRAIT_ANIMATION", getAnimString(cmd.ParamPointer));
                        break;

                    case GBAVV_ScriptCommand.CommandType.FLC:
                        if (Context.GetR1Settings().EngineVersion == EngineVersion.GBAVV_CrashNitroKart_NGage)
                            logCommand($"PLAY_FLC", $"\"{cmd.NGage_FilePath?.GetFullPath}\"");
                        else
                            logCommand($"PLAY_FLC", $"0x{cmd.Param:X8}");
                        break;

                    case GBAVV_ScriptCommand.CommandType.Unknown:
                    default:
                        // ROM pointer
                        if (cmd.ParamPointer != null)
                            logUnknownCommand(cmd, getAnimString(cmd.ParamPointer));
                        // RAM pointer
                        else if (cmd.Param >= Constants.Address_WRAM && cmd.Param < Constants.Address_WRAM + 0x40000)
                            logUnknownCommand(cmd, $"0x{cmd.Param:X8}");
                        // Value
                        else
                            logUnknownCommand(cmd, $"{(int)cmd.Param}");
                        break;
                }
            }

            // Log every command
            while (scriptIndex < Commands.Length)
                logScriptCMD();

            return output;
        }
    }
}