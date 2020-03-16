using System;
using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// Event CMD data for the Mapper
    /// </summary>
    public class Mapper_EventCMD : IBinarySerializable
    {
        /// <summary>
        /// The event items
        /// </summary>
        public PC_Mapper_EventCMDItem[] Items { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            // Temporary list for adding new items
            var items = new List<PC_Mapper_EventCMDItem>();

            // Use a command parser
            using (var parser = new MapperEngineCommandParser(deserializer.BaseStream))
            {
                string firstValue;

                while ((firstValue = parser.NextValue()) != null)
                {
                    // Create the manifest item
                    var eventManifestItem = new PC_Mapper_EventCMDItem();

                    // Get the values
                    eventManifestItem.ETAFile = firstValue;
                    eventManifestItem.Name = parser.NextValue();

                    // Get the commands
                    var cmds = new List<int>();

                    // Keep track of if the end of the command (33, 255) has been reached
                    bool is33 = false;

                    // Loop until we reach the end of the command
                    while (true)
                    {
                        // Get the value
                        var v = parser.NextValue();

                        cmds.Add(Int32.Parse(v));

                        // If previous value was 33 and this is 255 we exit
                        if (is33 && v == "255")
                            break;

                        // Indicate if this value is 33
                        is33 = v == "33";
                    }

                    // Read the commands
                    eventManifestItem.EventCommands = cmds.ToArray();

                    // Read values
                    eventManifestItem.XPosition = Int32.Parse(parser.NextValue());
                    eventManifestItem.YPosition = Int32.Parse(parser.NextValue());

                    if (eventManifestItem.Name != "always")
                    {
                        eventManifestItem.Unk1 = Int32.Parse(parser.NextValue());
                        eventManifestItem.LinkID = Int32.Parse(parser.NextValue());
                    }

                    eventManifestItem.Etat = UInt32.Parse(parser.NextValue());
                    eventManifestItem.SubEtat = parser.NextValue();

                    eventManifestItem.Offset_BX = UInt32.Parse(parser.NextValue());
                    eventManifestItem.Offset_BY = UInt32.Parse(parser.NextValue());
                    eventManifestItem.Offset_HY = UInt32.Parse(parser.NextValue());

                    eventManifestItem.Follow_enabled = UInt32.Parse(parser.NextValue());
                    eventManifestItem.Follow_sprite = UInt32.Parse(parser.NextValue());
                    eventManifestItem.Hitpoints = UInt32.Parse(parser.NextValue());

                    eventManifestItem.Obj_type = parser.NextValue();
                    eventManifestItem.Hit_sprite = UInt32.Parse(parser.NextValue());
                    eventManifestItem.DesignerGroup = Int32.Parse(parser.NextValue());

                    // Set the if commands if any were read
                    if (parser.PreviousIfCommands.Any())
                    {
                        // Set the commands
                        eventManifestItem.IfCommand = parser.PreviousIfCommands.ToArray();

                        // Clear the buffer
                        parser.PreviousIfCommands.Clear();
                    }

                    // Add the item
                    items.Add(eventManifestItem);
                }
            }

            // Set the read items
            Items = items.ToArray();
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}