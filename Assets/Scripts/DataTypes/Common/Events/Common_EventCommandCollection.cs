using System.Collections.Generic;
using System.Linq;

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