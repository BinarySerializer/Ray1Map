using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// Level load commands for Rayman 1 (Jaguar)
    /// </summary>
    public class R1Jaguar_LevelLoadCommandCollection : BinarySerializable
    {
        /// <summary>
        /// The commands
        /// </summary>
        public R1Jaguar_LevelLoadCommand[] Commands { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            if (Commands == null) {
                // Create a temporary list
                var cmd = new List<R1Jaguar_LevelLoadCommand>();

                int index = 0;

                // Loop until we reach the end command
                while (cmd.LastOrDefault()?.Type != R1Jaguar_LevelLoadCommand.LevelLoadCommandType.End) {
                    cmd.Add(s.SerializeObject((R1Jaguar_LevelLoadCommand)null, name: $"Commands[{index}]"));
                    index++;
                }

                // Set the commands
                Commands = cmd.ToArray();
            } else {
                // Serialize the commands
                s.SerializeObjectArray(Commands, Commands.Length, name: nameof(Commands));
            }
        }
    }
}