using System.Collections.Generic;
using BinarySerializer;
using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Image;


namespace Ray1Map.GBAVV
{
    public class GBAVV_ScriptCommand : BinarySerializable
    {
        public BinaryFile BaseFile { get; set; } // Set before serializing
        public bool SerializeFLC { get; set; } // Set before serializing

        public int PrimaryCommandType { get; set; }
        public int SecondaryCommandType { get; set; }
        public uint Param { get; set; }
        public uint SecondaryParam { get; set; }
        public Pointer ParamPointer { get; set; }
        public GBAVV_NitroKart_NGage_FilePath NGage_FilePath { get; set; }

        // Params
        public string Name { get; set; }
        public GBAVV_Script ReferencedScript { get; set; }
        public GBAVV_Input Input { get; set; }
        public GBAVV_LocalizedString Dialog { get; set; }
        public GBAVV_Animation Animation { get; set; }
        public GBAVV_ConditionalScriptReference ConditionalScriptReference { get; set; }
        public GBAVV_Movement Movement { get; set; }
        public GBAVV_ObjSpawn ObjSpawn { get; set; }
        public GBAVV_Sound Sound { get; set; }
        public FLIC FLC { get; set; }

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

            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_KidsNextDoorOperationSODA)
                SecondaryParam = s.Serialize<uint>(SecondaryParam, name: nameof(SecondaryParam));

            Type = ((GBAVV_BaseManager)s.GetR1Settings().GetGameManager).ScriptCommands.TryGetItem(PrimaryCommandType * 100 + SecondaryCommandType, CommandType.Unknown);

            // If the param is a valid pointer to the ROM we parse the pointer
            if (Param >= BaseFile.BaseAddress && Param < BaseFile.BaseAddress + 0x1000000)
            {
                ParamPointer = new Pointer(Param, BaseFile);
                s.Log("Param: {0}", ParamPointer);
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
                    ReferencedScript = s.DoAt(ParamPointer, () => s.SerializeObject<GBAVV_Script>(ReferencedScript, x =>
                    {
                        x.SerializeFLC = SerializeFLC;
                        x.BaseFile = BaseFile;
                    }, name: nameof(ReferencedScript)));
                    break;

                case CommandType.SkipNextIfInputCheck:
                case CommandType.WaitWhileInputCheck:
                case CommandType.SetUnknownInputData:
                    Input = s.DoAt(ParamPointer, () => s.SerializeObject<GBAVV_Input>(Input, name: nameof(Input)));
                    break;

                case CommandType.Dialog:
                case CommandType.Credits:
                    Dialog = s.DoAt(ParamPointer, () => s.SerializeObject<GBAVV_LocalizedString>(Dialog, name: nameof(Dialog)));
                    break;

                case CommandType.Animation:
                case CommandType.SecondaryAnimation:
                case CommandType.DialogPortrait:
                    Animation = s.DoAt(ParamPointer, () => s.SerializeObject<GBAVV_Animation>(Animation, name: nameof(Animation)));
                    break;

                case CommandType.ConditionalScript:
                    ConditionalScriptReference = s.DoAt(ParamPointer, () => s.SerializeObject<GBAVV_ConditionalScriptReference>(ConditionalScriptReference, x =>
                    {
                        x.SerializeFLC = SerializeFLC;
                        x.BaseFile = BaseFile;
                    }, name: nameof(ConditionalScriptReference)));
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

                case CommandType.FLC:
                    if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_CrashNitroKart_NGage)
                    {
                        NGage_FilePath = s.DoAt(ParamPointer, () => s.SerializeObject<GBAVV_NitroKart_NGage_FilePath>(NGage_FilePath, name: nameof(NGage_FilePath)));

                        if (SerializeFLC)
                            FLC = NGage_FilePath.DoAtFile(() => s.SerializeObject<FLIC>(FLC, name: nameof(FLC)));
                    }
                    else if (SerializeFLC)
                    {
                        s.DoAt(ParamPointer, () => s.DoEncoded(new BinarySerializer.Nintendo.GBA.LZSSEncoder(), () => FLC = s.SerializeObject<FLIC>(FLC, name: nameof(FLC)), allowLocalPointers: true));
                    }

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

            DialogPortrait,

            FLC,

            Credits
        }

        public class GBAVV_ConditionalScriptReference : BinarySerializable
        {
            public BinaryFile BaseFile { get; set; } // Set before serializing
            public bool SerializeFLC { get; set; } // Set before serializing

            public int Condition { get; set; } // 1 = die, 3/6 = jumpedOn, 4/5 = spunInto, 7 = touch?, 57 = moveRight, 58 = moveLeft, 59 = moveUp, 60 = moveDown
            public Pointer ScriptPointer { get; set; }

            public GBAVV_Script Script { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Condition = s.Serialize<int>(Condition, name: nameof(Condition));
                ScriptPointer = s.SerializePointer(ScriptPointer, name: nameof(ScriptPointer));

                Script = s.DoAt(ScriptPointer, () => s.SerializeObject<GBAVV_Script>(Script, x =>
                {
                    x.SerializeFLC = SerializeFLC;
                    x.BaseFile = BaseFile;
                }, name: nameof(Script)));
            }
        }

        public class GBAVV_Movement : BinarySerializable
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

        public class GBAVV_Input : BinarySerializable
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

        public class GBAVV_Sound : BinarySerializable
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

        public class GBAVV_ObjSpawn : BinarySerializable
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