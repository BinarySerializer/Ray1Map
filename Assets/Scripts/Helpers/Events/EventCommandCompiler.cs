using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Ray1;

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
        public static CompiledEventCommandData Compile(CommandCollection commands, byte[] commandBytes)
        {
            // Create the lists to compile to
            var compiledCommands = new List<Command>();
            var labelOffsets = new List<ushort>();
            
            // Keep track of the command byte index
            var commandByteIndex = 0;
            var commandIndex = 0;

            foreach (var command in commands.Commands)
            {
                // Increment the index for the command declaration
                commandByteIndex++;

                CommandType GetCompiledVersion()
                {
                    switch (command.CommandType)
                    {
                        case CommandType.GO_GOSUB:
                            return CommandType.RESERVED_GO_GOSUB;

                        case CommandType.GO_GOTO:
                            return CommandType.RESERVED_GO_GOTO;

                        case CommandType.GO_BRANCHTRUE:
                            return CommandType.RESERVED_GO_GOTOT;

                        case CommandType.GO_BRANCHFALSE:
                            return CommandType.RESERVED_GO_GOTOF;

                        case CommandType.GO_SKIP:
                            return CommandType.RESERVED_GO_SKIP;

                        case CommandType.GO_SKIPTRUE:
                            return CommandType.RESERVED_GO_SKIPT;

                        case CommandType.GO_SKIPFALSE:
                            return CommandType.RESERVED_GO_SKIPF;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(command.CommandType), command.CommandType, null);
                    }
                }

                switch (command.CommandType)
                {
                    // Handle commands with offsets
                    case CommandType.GO_GOSUB:
                    case CommandType.GO_GOTO:
                    case CommandType.GO_BRANCHTRUE:
                    case CommandType.GO_BRANCHFALSE:

                        // Get the label
                        var label = command.Arguments[0];

                        // Find the matching commands
                        var labelCmdIndex = commands.Commands.FindItemIndex(x => x.CommandType == CommandType.GO_LABEL && x.Arguments.FirstOrDefault() == label);

                        // Get the offset 
                        var offset = commands.Commands.Take(labelCmdIndex).Sum(x => x.Length) + 1;

                        // Add the offset to the label offsets
                        labelOffsets.Add((ushort)offset);

                        // Add the command
                        compiledCommands.Add(new Command()
                        {
                            CommandType = GetCompiledVersion(),
                            Arguments = new byte[]
                            {
                                (byte)(labelOffsets.Count - 1)
                            }
                        });

                        break;

                    // Handle commands with offsets based on current position
                    case CommandType.GO_SKIP:
                    case CommandType.GO_SKIPTRUE:
                    case CommandType.GO_SKIPFALSE:

                        // Get the number of cmds to skip
                        var toSkip = command.Arguments[0];

                        // Get the offset to skip to, starting at the next command index
                        var skipToOffset = commandByteIndex + 1;

                        int curCmd = commandIndex + 1;
                        for (int i = 0; i < toSkip; i++)
                        {
                            var cmd = commands.Commands[curCmd % commands.Commands.Length];
                            skipToOffset += cmd.Length;
                            if (cmd.CommandType == CommandType.INVALID_CMD)
                            {
                                curCmd = 0;
                                skipToOffset = commands.Commands[curCmd].Length;
                            }
                            curCmd++;
                        }

                        if (commands.Commands[curCmd % commands.Commands.Length].CommandType == CommandType.GO_LABEL)
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
                        compiledCommands.Add(new Command()
                        {
                            CommandType = GetCompiledVersion(),
                            Arguments = new byte[]
                            {
                                (byte)(labelOffsets.Count - 1)
                            }
                        });

                        break;

                    // Handle others
                    default:

                        // Copy the command
                        compiledCommands.Add(new Command()
                        {
                            CommandType = command.CommandType,
                            Arguments = command.Arguments.ToArray()
                        });

                    break;
                }

                // Increment the index for every argument
                commandByteIndex += command.Arguments.Length;
                commandIndex++;
            }

            // Return the compiled commands
            return new CompiledEventCommandData(new CommandCollection()
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
        public static CommandCollection Decompile(CompiledEventCommandData commands, byte[] commandBytes)
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
            public CompiledEventCommandData(CommandCollection commands, ushort[] labelOffsets)
            {
                Commands = commands;
                LabelOffsets = labelOffsets;
            }

            /// <summary>
            /// The event commands
            /// </summary>
            public CommandCollection Commands { get; }

            /// <summary>
            /// The label offsets
            /// </summary>
            public ushort[] LabelOffsets { get; }
        }
    }
}