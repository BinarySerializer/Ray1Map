using System;
using System.Collections.Generic;

namespace R1Engine
{
    /// <summary>
    /// A common event command
    /// </summary>
    public class R1_EventCommand : R1Serializable
    {
        /// <summary>
        /// The command
        /// </summary>
        public R1_EventCommandType Command { get; set; }

        /// <summary>
        /// The command arguments
        /// </summary>
        public byte[] Arguments { get; set; }

        /// <summary>
        /// The length of the command in bytes
        /// </summary>
        public int Length => Arguments.Length + 1;

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Command = s.Serialize<R1_EventCommandType>(Command, name: nameof(Command));

            if (Arguments == null)
            {
                switch (Command)
                {
                    case R1_EventCommandType.GO_LEFT:
                    case R1_EventCommandType.GO_RIGHT:
                    case R1_EventCommandType.GO_WAIT:
                    case R1_EventCommandType.GO_UP:
                    case R1_EventCommandType.GO_DOWN:
                    case R1_EventCommandType.GO_SUBSTATE:
                    case R1_EventCommandType.GO_SKIP:
                    case R1_EventCommandType.GO_ADD:
                    case R1_EventCommandType.GO_STATE:
                    case R1_EventCommandType.GO_PREPARELOOP:
                    case R1_EventCommandType.GO_LABEL:
                    case R1_EventCommandType.GO_GOTO:
                    case R1_EventCommandType.GO_GOSUB:
                    case R1_EventCommandType.GO_BRANCHTRUE:
                    case R1_EventCommandType.GO_BRANCHFALSE:
                    case R1_EventCommandType.GO_SETTEST:
                    case R1_EventCommandType.GO_WAITSTATE:
                    case R1_EventCommandType.RESERVED_GO_SKIP:
                    case R1_EventCommandType.RESERVED_GO_GOTO:
                    case R1_EventCommandType.RESERVED_GO_GOSUB:
                    case R1_EventCommandType.RESERVED_GO_GOTOT:
                    case R1_EventCommandType.RESERVED_GO_GOTOF:
                    case R1_EventCommandType.RESERVED_GO_SKIPT:
                    case R1_EventCommandType.RESERVED_GO_SKIPF:
                    case R1_EventCommandType.GO_NOP:
                    case R1_EventCommandType.GO_SKIPTRUE:
                    case R1_EventCommandType.GO_SKIPFALSE:
                    case R1_EventCommandType.INVALID_CMD:
                    case R1_EventCommandType.INVALID_CMD_DEMO:
                        Arguments = s.SerializeArray<byte>(Arguments, 1, name: nameof(Arguments));
                        break;

                    case R1_EventCommandType.GO_DOLOOP:
                    case R1_EventCommandType.GO_RETURN:
                        Arguments = s.SerializeArray<byte>(Arguments, s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 ? 1 : 0, name: nameof(Arguments));
                        break;

                    case R1_EventCommandType.GO_TEST:
                        var tempList = new List<byte>();

                        tempList.Add(s.Serialize<byte>((byte)0, name: nameof(Arguments)  + "[0]"));

                        if (tempList[0] <= 4)
                            tempList.Add(s.Serialize<byte>((byte)0, name: nameof(Arguments) + "[1]"));

                        Arguments = tempList.ToArray();

                        break;

                    case R1_EventCommandType.GO_SPEED:
                        Arguments = s.SerializeArray<byte>(Arguments, 3, name: nameof(Arguments));
                        break;

                    case R1_EventCommandType.GO_X:
                    case R1_EventCommandType.GO_Y:
                        Arguments = s.SerializeArray<byte>(Arguments, 2, name: nameof(Arguments));
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(Command), Command, null);
                }
            }
            else
            {
                s.SerializeArray<byte>(Arguments, Arguments.Length, name: nameof(Arguments));
            }
        }

        public string ToTranslatedString(int[] labelOffsetsLineNumbers, R1_EventCommand prevCmd, R1_EventCommand nextCmd) 
        {
            string cmd = $"{Command}";

            var prepend = "\t";

            if (RequiresTestTrue() || RequiresTestFalse())
            {
                if (prevCmd?.Command == R1_EventCommandType.GO_TEST || prevCmd?.Command == R1_EventCommandType.GO_SETTEST)
                    prepend = $"{prepend}\t";
                else
                    prepend += $"IF ({(RequiresTestFalse() ? "!" : "")}TEST) ";
            }

            cmd = cmd.Replace("FALSE", "");
            cmd = cmd.Replace("TRUE", "");

            switch (Command) 
            {
                case R1_EventCommandType.RESERVED_GO_GOTO:
                case R1_EventCommandType.RESERVED_GO_GOTOF:
                case R1_EventCommandType.RESERVED_GO_GOTOT:
                    cmd = "GOTO";
                    return $"{prepend}{cmd} LINE {labelOffsetsLineNumbers[Arguments[0]]};";
                            
                case R1_EventCommandType.RESERVED_GO_GOSUB:
                case R1_EventCommandType.RESERVED_GO_SKIP:
                case R1_EventCommandType.RESERVED_GO_SKIPF:
                case R1_EventCommandType.RESERVED_GO_SKIPT:
                    cmd = cmd.Replace("RESERVED_GO_", "");
                    if(cmd == "SKIPT" || cmd == "SKIPF") cmd = "SKIP";

                    return $"{prepend}{cmd} TO LINE {labelOffsetsLineNumbers[Arguments[0]]};";

                case R1_EventCommandType.GO_LABEL:
                    return "LABEL " + Arguments[0] + ":";

                case R1_EventCommandType.GO_RETURN:
                    if (Arguments.Length == 0) {
                        return $"{prepend}RETURN;";
                    } else {
                        cmd = cmd.Replace("GO_", "");
                        return $"{prepend}{cmd} {String.Join(" ", Arguments)};";
                    }

                case R1_EventCommandType.GO_SPEED:
                    cmd = cmd.Replace("GO_", "");
                    return $"{prepend}{cmd} {Arguments[0]} {(sbyte)Arguments[1]} {(sbyte)Arguments[2]}";

                case R1_EventCommandType.GO_X:
                case R1_EventCommandType.GO_Y:
                    cmd = cmd.Replace("GO_", "");
                    return $"{prepend}{cmd} {Arguments[0] * 100 + Arguments[1]}";

                case R1_EventCommandType.GO_TEST:
                case R1_EventCommandType.GO_SETTEST:
                    var append = String.Empty;

                    if (nextCmd?.RequiresTestFalse() == true || nextCmd?.RequiresTestTrue() == true)
                    {
                        prepend = $"{prepend}IF (";
                        append = $")";
                    }
                    else
                    {
                        prepend = $"{prepend}TEST = ";
                    }

                    if (Command == R1_EventCommandType.GO_SETTEST)
                        return $"{prepend}{(Arguments[0] == 1).ToString().ToUpper()}{append}";
                    else
                        switch (Arguments[0])
                        {
                            case 0:
                                return $"{prepend}{(Arguments[1] == 1 ? "": "!")}ISFLIPPED{append}";
                            case 1:
                                return $"{prepend}RANDOM({Arguments[1]})"; // myRand. RandArray[RandomIndex] % (Argument1 + 1);
                            case 2:
                                return $"{prepend}RAYMAN.X {((Arguments[1] == 1) ? ">" : "<=")} X{append}";
                            case 3:
                                return $"{prepend}STATE == {Arguments[1]}{append}";
                            case 4:
                                return $"{prepend}SUBSTATE == {Arguments[1]}{append}";
                            case 70:
                                return $"{prepend}OBJ_IN_ZONE{append}";
                            case 71:
                                return $"{prepend}HASFLAG(0){append}"; // TODO: What is this flag?
                            case 72:
                                return $"{prepend}!HASFLAG(4){append}"; // TODO: What is this flag?
                            default:
                                return $"{prepend}<UNKNOWN TEST ({Arguments[0]})>{append}";
                        }

                default:
                    cmd = cmd.Replace("GO_", "");
                    return $"{prepend}{cmd}{(Arguments.Length > 0 ? (" " + String.Join(" ", Arguments)) : "")};";
            }
        }

        public bool RequiresTestTrue() => Command == R1_EventCommandType.RESERVED_GO_SKIPT ||
                                          Command == R1_EventCommandType.RESERVED_GO_GOTOT ||
                                          Command == R1_EventCommandType.GO_BRANCHTRUE ||
                                          Command == R1_EventCommandType.GO_SKIPTRUE;
        public bool RequiresTestFalse() => Command == R1_EventCommandType.RESERVED_GO_SKIPF ||
                                           Command == R1_EventCommandType.RESERVED_GO_GOTOF ||
                                           Command == R1_EventCommandType.GO_BRANCHFALSE ||
                                           Command == R1_EventCommandType.GO_SKIPFALSE;

        public bool UsesLabelOffsets {
            get {
                switch (Command) {
                    case R1_EventCommandType.RESERVED_GO_GOTO:
                    case R1_EventCommandType.RESERVED_GO_GOTOF:
                    case R1_EventCommandType.RESERVED_GO_GOTOT:
                    case R1_EventCommandType.RESERVED_GO_GOSUB:
                    case R1_EventCommandType.RESERVED_GO_SKIP:
                    case R1_EventCommandType.RESERVED_GO_SKIPF:
                    case R1_EventCommandType.RESERVED_GO_SKIPT:
                        return true;
                    default:
                        return false;
                }
            }
        }
    }
}