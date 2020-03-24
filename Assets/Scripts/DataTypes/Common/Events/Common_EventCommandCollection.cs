using System.Collections.Generic;
using System.IO;
using System.Linq;
using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// A common event command collection
    /// </summary>
    public class Common_EventCommandCollection : R1Serializable
    {
        /// <summary>
        /// The commands
        /// </summary>
        public Common_EventCommand[] Commands { get; set; }

        /// <summary>
        /// Gets a command from the raw bytes
        /// </summary>
        /// <param name="bytes">The command bytes</param>
        /// <returns>The command</returns>
        public static Common_EventCommandCollection FromBytes(byte[] bytes)
        {
            // Make sure there are bytes
            if (!bytes.Any())
                return new Common_EventCommandCollection()
                {
                    Commands = new Common_EventCommand[0]
                };

            // Create a new context
            using (var context = new Context(Settings.GetGameSettings)) {
                // Create a memory stream
                using (var memStream = new MemoryStream(bytes)) {
                    // Stream key
                    const string key = "PC_EventCommand";

                    // Add the stream
                    context.AddFile(new StreamFile(key, memStream, context));

                    // Deserialize the bytes
                    return FileFactory.Read<Common_EventCommandCollection>(key, context);
                }
            }
        }

        /// <summary>
        /// Gets the byte representation of the command
        /// </summary>
        /// <returns>The command bytes</returns>
        public byte[] ToBytes()
        {
            using (var context = new Context(Settings.GetGameSettings)) {
                // Create a memory stream
                using (var memStream = new MemoryStream()) {
                    // Stream key
                    const string key = "PC_EventCommand";

                    // Add the stream
                    context.AddFile(new StreamFile(key, memStream, context));

                    // TODO: Pass in this instance
                    // Serialize the command
                    FileFactory.Write<Common_EventCommandCollection>(key, this, context);

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
                var cmd = new List<Common_EventCommand>();

                int index = 0;

                // Loop until we reach the invalid command
                while (cmd.LastOrDefault()?.Command != EventCommand.INVALID_CMD)
                {
                    cmd.Add(s.SerializeObject((Common_EventCommand)null, name: $"Commands [{index}]"));
                    index++;
                }

                // Set the commands
                Commands = cmd.ToArray();
            }
            else
            {
                // Serialize the commands
                s.SerializeObjectArray(Commands, Commands.Length, name: "Commands");
            }
        }
    }
}