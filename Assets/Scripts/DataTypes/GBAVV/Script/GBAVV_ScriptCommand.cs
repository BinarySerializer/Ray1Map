namespace R1Engine
{
    public class GBAVV_ScriptCommand : R1Serializable
    {
        public int PrimaryCommandType { get; set; }
        public int SecondaryCommandType { get; set; }
        public uint Param { get; set; }
        public Pointer ParamPointer { get; set; }

        public CommandType Type => (CommandType)(PrimaryCommandType * 100 + SecondaryCommandType);

        // Params
        public string Name { get; set; }
        public GBAVV_Script ReferencedScript { get; set; }
        public GBAVV_Fusion_LocalizedString Dialog { get; set; }
        public GBAVV_Map2D_Animation Animation { get; set; }
        public GBAVV_ConditionalScriptReference ConditionalScriptReference { get; set; }
        public GBAVV_Movement Movement { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            PrimaryCommandType = s.Serialize<int>(PrimaryCommandType, name: nameof(PrimaryCommandType));
            SecondaryCommandType = s.Serialize<int>(SecondaryCommandType, name: nameof(SecondaryCommandType));
            Param = s.Serialize<uint>(Param, name: nameof(Param));

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
                    break;

                case CommandType.Script:
                    ReferencedScript = s.DoAt(ParamPointer, () => s.SerializeObject<GBAVV_Script>(ReferencedScript, name: nameof(ReferencedScript)));
                    break;

                case CommandType.Dialog:
                    Dialog = s.DoAt(ParamPointer, () => s.SerializeObject<GBAVV_Fusion_LocalizedString>(Dialog, name: nameof(Dialog)));
                    break;

                case CommandType.Animation:
                    Animation = s.DoAt(ParamPointer, () => s.SerializeObject<GBAVV_Map2D_Animation>(Animation, name: nameof(Animation)));
                    break;

                case CommandType.ConditionalScript:
                    ConditionalScriptReference = s.DoAt(ParamPointer, () => s.SerializeObject<GBAVV_ConditionalScriptReference>(ConditionalScriptReference, name: nameof(ConditionalScriptReference)));
                    break;

                case CommandType.Movement_X:
                case CommandType.Movement_Y:
                    Movement = s.DoAt(ParamPointer, () => s.SerializeObject<GBAVV_Movement>(Movement, name: nameof(Movement)));
                    break;

                case CommandType.DialogPortrait:
                    break;

                // Do nothing
                case CommandType.Wait:
                case CommandType.IsFlipped:
                case CommandType.IsEnabled:
                case CommandType.Terminator:
                default:
                    break;
            }
        }

        public enum CommandType
        {
            Name = 0501,
            Script = 0502,
            Terminator = 0506,
            Wait = 0508,

            Dialog = 0702,

            IsFlipped = 0800,
            Animation = 0807,
            IsEnabled = 0815,
            ConditionalScript = 0818,
            Movement_X = 0829,
            Movement_Y = 0830,

            DialogPortrait = 1000
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
    }
}