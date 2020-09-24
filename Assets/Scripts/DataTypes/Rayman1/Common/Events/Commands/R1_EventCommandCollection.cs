using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// A common event command collection
    /// </summary>
    public class R1_EventCommandCollection : R1Serializable
    {
        /// <summary>
        /// The commands
        /// </summary>
        public R1_EventCommand[] Commands { get; set; }

        /// <summary>
        /// Gets a command from the raw bytes
        /// </summary>
        /// <param name="bytes">The command bytes</param>
        /// <param name="settings">The game settings</param>
        /// <returns>The command</returns>
        public static R1_EventCommandCollection FromBytes(byte[] bytes, GameSettings settings)
        {
            // Make sure there are bytes
            if (!bytes.Any())
                return new R1_EventCommandCollection()
                {
                    Commands = new R1_EventCommand[0]
                };

            // Create a new context
            using (var context = new Context(settings)) {
                // Create a memory stream
                using (var memStream = new MemoryStream(bytes)) {
                    // Stream key
                    const string key = "PC_EventCommand";

                    // Add the stream
                    context.AddStreamFile(key, memStream);

                    // Deserialize the bytes
                    return FileFactory.Read<R1_EventCommandCollection>(key, context);
                }
            }
        }

        /// <summary>
        /// Gets the byte representation of the command
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The command bytes</returns>
        public byte[] ToBytes(GameSettings settings)
        {
            using (var context = new Context(settings)) {
                // Create a memory stream
                using (var memStream = new MemoryStream()) {
                    // Stream key
                    const string key = "PC_EventCommand";

                    // Add the stream
                    context.AddStreamFile(key, memStream);

                    // TODO: Pass in this instance
                    // Serialize the command
                    FileFactory.Write<R1_EventCommandCollection>(key, this, context);

                    // Return the bytes
                    return memStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            if (Commands == null)
            {
                // Create a temporary list
                var cmd = new List<R1_EventCommand>();

                int index = 0;

                // Loop until we reach the invalid command
                while (cmd.LastOrDefault()?.Command != R1_EventCommandType.INVALID_CMD && cmd.LastOrDefault()?.Command != R1_EventCommandType.INVALID_CMD_DEMO)
                {
                    cmd.Add(s.SerializeObject((R1_EventCommand)null, name: $"Commands [{index}]"));
                    index++;
                }

                // Set the commands
                Commands = cmd.ToArray();
            }
            else
            {
                // Serialize the commands
                s.SerializeObjectArray(Commands, Commands.Length, name: nameof(Commands));
            }
        }

        public string[] ToTranslatedStrings(ushort[] labelOffsets) {
            int[] lineNumbers;
            if (Commands == null || Commands.Length == 0) return null;
            if (labelOffsets != null && labelOffsets.Length > 0) {
                int[] commandOffsets = new int[Commands.Length + 1];
                int curOff = 0;
                for (int i = 0; i < commandOffsets.Length; i++) {
                    commandOffsets[i] = curOff;
                    if(i < Commands.Length) curOff += Commands[i].Length;
                }
                lineNumbers = labelOffsets.Select(l => Array.IndexOf(commandOffsets, (int)l-1)).ToArray();
            } else {
                lineNumbers = new int[0];
            }
            return Commands.Select(c => c.ToTranslatedString(lineNumbers)).ToArray();
        }
    }
}