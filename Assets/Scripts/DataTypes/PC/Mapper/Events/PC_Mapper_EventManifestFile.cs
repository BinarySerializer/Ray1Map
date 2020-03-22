using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// Event manifest data for Rayman Mapper (PC)
    /// </summary>
    public class PC_Mapper_EventManifestFile : MapperEngineSerializable
    {
        /// <summary>
        /// The event manifest items
        /// </summary>
        public PC_Mapper_ManifestItem[] Items { get; set; }

        public override void Read(MapperEngineCommandParser parser) {
            // Temporary list for adding new items
            var items = new List<PC_Mapper_ManifestItem>();

            string firstValue;

            while ((firstValue = parser.NextValue()) != null) {
                // Create the manifest item
                var eventManifestItem = new PC_Mapper_ManifestItem();

                // Make sure we're starting an event definition
                if (firstValue != "£def")
                    throw new Exception("Invalid event definition");

                // Get the values
                eventManifestItem.Name = parser.NextValue();
                eventManifestItem.DESFile = parser.NextValue();
                eventManifestItem.UnkGroup = UInt32.Parse(parser.NextValue());
                eventManifestItem.ETAFile = parser.NextValue();

                // Get the commands
                var cmds = new List<int>();

                // Keep track of if the end of the command (33, 255) has been reached
                bool is33 = false;

                // Loop until we reach the end of the command
                while (true) {
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

                // Only read the group if it's not an always object
                if (eventManifestItem.Name != "always")
                    eventManifestItem.DesignerGroup = Int32.Parse(parser.NextValue());
                else
                    eventManifestItem.DesignerGroup = -1;

                // Set the if commands if any were read
                if (parser.PreviousIfCommands.Any()) {
                    // Set the commands
                    eventManifestItem.IfCommand = parser.PreviousIfCommands.ToArray();

                    // Clear the buffer
                    parser.PreviousIfCommands.Clear();
                }

                // Add the item
                items.Add(eventManifestItem);

                // Set the read items
                Items = items.ToArray();
            }
        }
    }
}