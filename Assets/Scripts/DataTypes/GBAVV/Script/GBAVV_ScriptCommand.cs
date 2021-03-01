using System.Collections.Generic;

namespace R1Engine
{
    public class GBAVV_ScriptCommand : R1Serializable
    {
        public int PrimaryCommandType { get; set; }
        public int SecondaryCommandType { get; set; }
        public uint Param { get; set; }
        public Pointer ParamPointer { get; set; }

        // Params
        public string Name { get; set; }
        public GBAVV_Script ReferencedScript { get; set; }
        public GBAVV_Input Input { get; set; }
        public GBAVV_LocalizedString Dialog { get; set; }
        public GBAVV_Map2D_Animation Animation { get; set; }
        public GBAVV_ConditionalScriptReference ConditionalScriptReference { get; set; }
        public GBAVV_Movement Movement { get; set; }
        public GBAVV_ObjSpawn ObjSpawn { get; set; }
        public GBAVV_Sound Sound { get; set; }

        // Helper properties
        public CommandType Type { get; set; }
        public string NormalizedName => $"{Name.Replace("\"", "")}";
        public string DisplayName => $"{NormalizedName}{(NameCounts[NormalizedName] > 1 ? $"_{NameIndex}" : "")}";
        public int NameIndex { get; set; } // Some scripts share the same name
        private static Dictionary<string, int> NameCounts { get; } = new Dictionary<string, int>();

        public override void SerializeImpl(SerializerObject s)
        {
            PrimaryCommandType = s.Serialize<int>(PrimaryCommandType, name: nameof(PrimaryCommandType));
            SecondaryCommandType = s.Serialize<int>(SecondaryCommandType, name: nameof(SecondaryCommandType));
            Param = s.Serialize<uint>(Param, name: nameof(Param));

            Type = ((GBAVV_Fusion_Manager)s.GameSettings.GetGameManager).ScriptCommands.TryGetItem(PrimaryCommandType * 100 + SecondaryCommandType, CommandType.Unknown);

            // If the param is a valid pointer to the ROM we parse the pointer
            if (Param >= GBA_ROMBase.Address_ROM && Param < GBA_ROMBase.Address_ROM + s.CurrentLength)
            {
                ParamPointer = new Pointer(Param, Offset.file);
                s.Log($"Param: {ParamPointer}");
            }

            // Parse the parameter
            switch (Type)
            {
                case CommandType.Name:
                    Name = s.DoAt(ParamPointer, () => s.SerializeString(Name, name: nameof(Name)));

                    if (!NameCounts.ContainsKey(NormalizedName))
                        NameCounts[NormalizedName] = 0;

                    NameIndex = NameCounts[NormalizedName];
                    NameCounts[NormalizedName]++;
                    break;

                case CommandType.Script:
                    ReferencedScript = s.DoAt(ParamPointer, () => s.SerializeObject<GBAVV_Script>(ReferencedScript, name: nameof(ReferencedScript)));
                    break;

                case CommandType.SkipNextIfInputCheck:
                case CommandType.WaitWhileInputCheck:
                case CommandType.SetUnknownInputData:
                    Input = s.DoAt(ParamPointer, () => s.SerializeObject<GBAVV_Input>(Input, name: nameof(Input)));
                    break;

                case CommandType.Dialog:
                    Dialog = s.DoAt(ParamPointer, () => s.SerializeObject<GBAVV_LocalizedString>(Dialog, name: nameof(Dialog)));
                    break;

                case CommandType.Animation:
                case CommandType.SecondaryAnimation:
                    Animation = s.DoAt(ParamPointer, () => s.SerializeObject<GBAVV_Map2D_Animation>(Animation, name: nameof(Animation)));
                    break;

                case CommandType.ConditionalScript:
                    ConditionalScriptReference = s.DoAt(ParamPointer, () => s.SerializeObject<GBAVV_ConditionalScriptReference>(ConditionalScriptReference, name: nameof(ConditionalScriptReference)));
                    break;

                case CommandType.Movement_X:
                case CommandType.Movement_Y:
                    Movement = s.DoAt(ParamPointer, () => s.SerializeObject<GBAVV_Movement>(Movement, name: nameof(Movement)));
                    break;

                case CommandType.SpawnObject:
                    ObjSpawn = s.DoAt(ParamPointer, () => s.SerializeObject<GBAVV_ObjSpawn>(ObjSpawn, name: nameof(ObjSpawn)));
                    break;

                case CommandType.PlaySound:
                    Sound = s.DoAt(ParamPointer, () => s.SerializeObject<GBAVV_Sound>(Sound, name: nameof(Sound)));
                    break;

                case CommandType.DialogPortrait:
                    break;

                // Do nothing
                case CommandType.SkipNextIfField08:
                case CommandType.Reset:
                case CommandType.Wait:
                case CommandType.IsFlipped:
                case CommandType.IsEnabled:
                case CommandType.Return:
                case CommandType.Unknown:
                default:
                    break;
            }
        }

        public enum CommandType
        {
            Unknown,

            Name,
            Script,
            SkipNextIfInputCheck,
            SkipNextIfField08,
            Reset,
            Return,
            SetUnknownInputData,
            Wait,
            WaitWhileInputCheck,

            Dialog,

            IsFlipped,
            Animation,
            IsEnabled,
            ConditionalScript,
            Movement_X,
            Movement_Y,
            SpawnObject,
            SecondaryAnimation,
            PlaySound,

            DialogPortrait
        }

        public class GBAVV_ConditionalScriptReference : R1Serializable
        {
            public int Condition { get; set; } // 1 = die, 3/6 = jumpedOn, 4/5 = spunInto, 7 = touch?, 57 = moveRight, 58 = moveLeft, 59 = moveUp, 60 = moveDown
            public Pointer ScriptPointer { get; set; }

            public GBAVV_Script Script { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Condition = s.Serialize<int>(Condition, name: nameof(Condition));
                ScriptPointer = s.SerializePointer(ScriptPointer, name: nameof(ScriptPointer));

                Script = s.DoAt(ScriptPointer, () => s.SerializeObject<GBAVV_Script>(Script, name: nameof(Script)));
            }
        }

        public class GBAVV_Movement : R1Serializable
        {
            public int Speed { get; set; }
            public int Param_1 { get; set; }
            public int Param_2 { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Speed = s.Serialize<int>(Speed, name: nameof(Speed));
                Param_1 = s.Serialize<int>(Param_1, name: nameof(Param_1));
                Param_2 = s.Serialize<int>(Param_2, name: nameof(Param_2));
            }
        }

        public class GBAVV_Input : R1Serializable
        {
            public int Param_0 { get; set; }
            public int Param_1 { get; set; }
            public int Param_2 { get; set; }
            public int Param_3 { get; set; }
            public int Param_4 { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Param_0 = s.Serialize<int>(Param_0, name: nameof(Param_0));
                Param_1 = s.Serialize<int>(Param_1, name: nameof(Param_1));
                Param_2 = s.Serialize<int>(Param_2, name: nameof(Param_2));
                Param_3 = s.Serialize<int>(Param_3, name: nameof(Param_3));
                Param_4 = s.Serialize<int>(Param_4, name: nameof(Param_4));
            }

            public string[] GetArgs() => new string[] { $"{Param_0}", $"{Param_1}", $"{Param_2}", $"{Param_3}", $"{Param_4}" };
        }

        public class GBAVV_Sound : R1Serializable
        {
            public int Param_0 { get; set; }
            public int Param_1 { get; set; }
            public int Param_2 { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Param_0 = s.Serialize<int>(Param_0, name: nameof(Param_0));
                Param_1 = s.Serialize<int>(Param_1, name: nameof(Param_1));
                Param_2 = s.Serialize<int>(Param_2, name: nameof(Param_2));
            }

            public string[] GetArgs() => new string[] { $"{Param_0}", $"{Param_1}", $"{Param_2}" };
        }

        public class GBAVV_ObjSpawn : R1Serializable
        {
            public int Param_0 { get; set; }
            public int Param_1 { get; set; }
            public int Param_2 { get; set; }
            public int Param_3 { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Param_0 = s.Serialize<int>(Param_0, name: nameof(Param_0));
                Param_1 = s.Serialize<int>(Param_1, name: nameof(Param_1));
                Param_2 = s.Serialize<int>(Param_2, name: nameof(Param_2));
                Param_3 = s.Serialize<int>(Param_3, name: nameof(Param_3));
            }

            public string[] GetArgs() => new string[] { $"{Param_0}", $"{Param_1}", $"{Param_2}", $"{Param_3}" };
        }
    }
}