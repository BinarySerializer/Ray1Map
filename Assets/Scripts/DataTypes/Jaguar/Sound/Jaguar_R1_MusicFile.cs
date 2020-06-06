using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// Music file for Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_MusicFile : R1Serializable
    {
        /// <summary>
        /// The commands
        /// </summary>
        public Jaguar_R1_MusicFileEntry[] Entries { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            if (Entries == null) {
                // Create a temporary list
                var entr = new List<Jaguar_R1_MusicFileEntry>();

                int index = 0;

                // Loop until we reach the end command
                while (entr.LastOrDefault()?.Time != int.MaxValue) {
                    entr.Add(s.SerializeObject<Jaguar_R1_MusicFileEntry>(null, name: $"Entries[{index}]"));
                    index++;
                }

                // Set the commands
                Entries = entr.ToArray();
            } else {
                // Serialize the commands
                s.SerializeObjectArray(Entries, Entries.Length, name: nameof(Entries));
            }
        }
    }
}