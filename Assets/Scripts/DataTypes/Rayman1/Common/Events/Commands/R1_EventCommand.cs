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

        public string ToTranslatedString() {
            string cmd = Command.ToString();
            switch (Command) {
                case R1_EventCommandType.GO_LABEL:
                    return "LABEL " + Arguments[0] + ":";
                case R1_EventCommandType.GO_RETURN:
                    if (Arguments.Length == 0) {
                        return "RETURN;";
                    } else {
                        cmd = cmd.Replace("GO_", "");
                        return "\t" + cmd + (Arguments.Length > 0 ? (" " + string.Join(" ", Arguments)) : "") + ";";
                    }
                default:
                    cmd = cmd.Replace("GO_", "");
                    return "\t" + cmd + (Arguments.Length > 0 ? (" " + string.Join(" ", Arguments)) : "") + ";";
            }
        }
    }
}