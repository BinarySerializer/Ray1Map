using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// Level load commands for Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_LevelLoadCommandCollection : R1Serializable
    {
        Jaguar_R1_LevelLoadCommand[] Commands { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            if (Commands == null) {
                // Create a temporary list
                var cmd = new List<Jaguar_R1_LevelLoadCommand>();

                int index = 0;

                // Loop until we reach the invalid command
                while (cmd.LastOrDefault()?.Type != 0) {
                    cmd.Add(s.SerializeObject((Jaguar_R1_LevelLoadCommand)null, name: $"Commands[{index}]"));
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