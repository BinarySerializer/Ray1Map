using System;
using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// Compiles and decompiles event commands
    /// </summary>
    public static class EventCommandCompiler
    {
        /// <summary>
        /// Compiles the commands
        /// </summary>
        /// <param name="commands">The commands</param>
        /// <param name="commandBytes">The command bytes</param>
        /// <returns>The compiled the command data</returns>
        public static CompiledEventCommandData Compile(Common_EventCommandCollection commands, byte[] commandBytes)
        {
            // Create the lists to compile to
            var compiledCommands = new List<Common_EventCommand>();
            var labelOffsets = new List<ushort>();
            
            // Keep track of the command byte index
            var commandByteIndex = 0;
            var commandIndex = 0;

            foreach (var command in commands.Commands)
            {
                // Increment the index for the command declaration
                commandByteIndex++;

                EventCommand GetCompiledVersion()
                {
                    switch (command.Command)
                    {
                        case EventCommand.GO_GOSUB:
                            return EventCommand.RESERVED_GO_GOSUB;

                        case EventCommand.GO_GOTO:
                            return EventCommand.RESERVED_GO_GOTO;

                        case EventCommand.GO_BRANCHTRUE:
                            return EventCommand.RESERVED_GO_GOTOT;

                        case EventCommand.GO_BRANCHFALSE:
                            return EventCommand.RESERVED_GO_GOTOF;

                        case EventCommand.GO_SKIP:
                            return EventCommand.RESERVED_GO_SKIP;

                        case EventCommand.GO_SKIPTRUE:
                            return EventCommand.RESERVED_GO_SKIPT;

                        case EventCommand.GO_SKIPFALSE:
                            return EventCommand.RESERVED_GO_SKIPF;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(command.Command), command.Command, null);
                    }
                }

                switch (command.Command)
                {
                    // Handle commands with offsets
                    case EventCommand.GO_GOSUB:
                    case EventCommand.GO_GOTO:
                    case EventCommand.GO_BRANCHTRUE:
                    case EventCommand.GO_BRANCHFALSE:

                        // Get the label
                        var label = command.Arguments[0];

                        // Find the matching commands
                        var labelCmdIndex = commands.Commands.FindItemIndex(x => x.Command == EventCommand.GO_LABEL && x.Arguments.FirstOrDefault() == label);

                        // Get the offset 
                        var offset = commands.Commands.Take(labelCmdIndex).Sum(x => x.Length) + 1;

                        // Add the offset to the label offsets
                        labelOffsets.Add((ushort)offset);

                        // Add the command
                        compiledCommands.Add(new Common_EventCommand()
                        {
                            Command = GetCompiledVersion(),
                            Arguments = new byte[]
                            {
                                (byte)(labelOffsets.Count - 1)
                            }
                        });

                        break;

                    // Handle commands with offsets based on current position
                    case EventCommand.GO_SKIP:
                    case EventCommand.GO_SKIPTRUE:
                    case EventCommand.GO_SKIPFALSE:

                        // Get the number of cmds to skip
                        var toSkip = command.Arguments[0];

                        // Get the offset to skip to, starting at the next command index
                        var skipToOffset = commandByteIndex + 1;

                        int curCmd = commandIndex + 1;
                        for (int i = 0; i < toSkip; i++)
                        {
                            var cmd = commands.Commands[curCmd % commands.Commands.Length];
                            skipToOffset += cmd.Length;
                            if (cmd.Command == EventCommand.INVALID_CMD)
                            {
                                curCmd = 0;
                                skipToOffset = commands.Commands[curCmd].Length;
                            }
                            curCmd++;
                        }

                        if (commands.Commands[curCmd % commands.Commands.Length].Command == EventCommand.GO_LABEL)
                        {
                            skipToOffset += 2;
                            curCmd++;
                        }

                        skipToOffset -= 1;

                        // Overflow
                        skipToOffset %= commands.Commands.Sum(x => x.Length);

                        // Add the offset to the label offsets
                        labelOffsets.Add((byte)skipToOffset);

                        // Add the command
                        compiledCommands.Add(new Common_EventCommand()
                        {
                            Command = GetCompiledVersion(),
                            Arguments = new byte[]
                            {
                                (byte)(labelOffsets.Count - 1)
                            }
                        });

                        break;

                    // Handle others
                    default:

                        // Copy the command
                        compiledCommands.Add(new Common_EventCommand()
                        {
                            Command = command.Command,
                            Arguments = command.Arguments.ToArray()
                        });

                    break;
                }

                // Increment the index for every argument
                commandByteIndex += command.Arguments.Length;
                commandIndex++;
            }

            // Return the compiled commands
            return new CompiledEventCommandData(new Common_EventCommandCollection()
            {
                Commands = compiledCommands.ToArray()
            }, labelOffsets.ToArray());
        }

        /// <summary>
        /// Decompiles the commands
        /// </summary>
        /// <param name="commands">The commands</param>
        /// <param name="commandBytes">The command bytes</param>
        /// <returns>The decompiled commands</returns>
        public static Common_EventCommandCollection Decompile(CompiledEventCommandData commands, byte[] commandBytes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Data for compiled event commands
        /// </summary>
        public class CompiledEventCommandData
        {
            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="commands">The event commands</param>
            /// <param name="labelOffsets">The label offsets</param>
            public CompiledEventCommandData(Common_EventCommandCollection commands, ushort[] labelOffsets)
            {
                Commands = commands;
                LabelOffsets = labelOffsets;
            }

            /// <summary>
            /// The event commands
            /// </summary>
            public Common_EventCommandCollection Commands { get; }

            /// <summary>
            /// The label offsets
            /// </summary>
            public ushort[] LabelOffsets { get; }
        }
    }
}