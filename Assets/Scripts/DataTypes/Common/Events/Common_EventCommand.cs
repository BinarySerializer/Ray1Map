using System;
using System.Collections.Generic;

namespace R1Engine
{
    /// <summary>
    /// A common event command
    /// </summary>
    public class Common_EventCommand : R1Serializable
    {
        /// <summary>
        /// The command
        /// </summary>
        public EventCommand Command { get; set; }

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
            Command = s.Serialize(Command, name: "Command");

            if (Arguments == null)
            {
                switch (Command)
                {
                    case EventCommand.GO_LEFT:
                    case EventCommand.GO_RIGHT:
                    case EventCommand.GO_WAIT:
                    case EventCommand.GO_UP:
                    case EventCommand.GO_DOWN:
                    case EventCommand.GO_SUBSTATE:
                    case EventCommand.GO_SKIP:
                    case EventCommand.GO_ADD:
                    case EventCommand.GO_STATE:
                    case EventCommand.GO_PREPARELOOP:
                    case EventCommand.GO_LABEL:
                    case EventCommand.GO_GOTO:
                    case EventCommand.GO_GOSUB:
                    case EventCommand.GO_BRANCHTRUE:
                    case EventCommand.GO_BRANCHFALSE:
                    case EventCommand.GO_SETTEST:
                    case EventCommand.GO_WAITSTATE:
                    case EventCommand.RESERVED_GO_SKIP:
                    case EventCommand.RESERVED_GO_GOTO:
                    case EventCommand.RESERVED_GO_GOSUB:
                    case EventCommand.RESERVED_GO_GOTOT:
                    case EventCommand.RESERVED_GO_GOTOF:
                    case EventCommand.RESERVED_GO_SKIPT:
                    case EventCommand.RESERVED_GO_SKIPF:
                    case EventCommand.GO_NOP:
                    case EventCommand.GO_SKIPTRUE:
                    case EventCommand.GO_SKIPFALSE:
                    case EventCommand.INVALID_CMD:
                        Arguments = s.SerializeArray(Arguments, 1, name: "Arguments");
                        break;

                    case EventCommand.GO_DOLOOP:
                    case EventCommand.GO_RETURN:
                        Arguments = s.SerializeArray(Arguments, 0, name: "Arguments");
                        break;

                    case EventCommand.GO_TEST:
                        var tempList = new List<byte>();

                        tempList.Add(s.Serialize((byte)0, name: "Arguments [0]"));

                        if (tempList[0] <= 4)
                            tempList.Add(s.Serialize((byte)0, name: "Arguments [1]"));

                        Arguments = tempList.ToArray();

                        break;

                    case EventCommand.GO_SPEED:
                        Arguments = s.SerializeArray(Arguments, 3, name: "Arguments");
                        break;

                    case EventCommand.GO_X:
                    case EventCommand.GO_Y:
                        Arguments = s.SerializeArray(Arguments, 2, name: "Arguments");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(Command), Command, null);
                }
            }
            else
            {
                s.SerializeArray(Arguments, Arguments.Length, name: "Arguments");
            }
        }
    }
}