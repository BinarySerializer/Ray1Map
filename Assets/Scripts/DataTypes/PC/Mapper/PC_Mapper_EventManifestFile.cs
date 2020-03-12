using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// Event manifest data for Rayman Mapper (PC)
    /// </summary>
    [Description("Rayman Mapper (PC) Event Manifest File")]
    public class PC_Mapper_EventManifestFile : IBinarySerializable
    {
        /// <summary>
        /// The event manifest items
        /// </summary>
        public PC_RD_EventManifestItem[] Items { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            // NOTE: Comments are currently not deserialized

            // Temporary list for adding new items
            var items = new List<PC_RD_EventManifestItem>();

            // Use a stream reader for easier string handling
            using (var reader = new StreamReader(deserializer.BaseStream, Settings.StringEncoding, false, 1024, true))
            {
                // Flag for if we've reached the end of the file
                bool end = false;

                // Keep track if within certain scopes
                bool isInComment = false;
                bool isInIf = false;

                // Buffer for keeping the current if commands
                List<string> ifCommandBuffer = new List<string>();

                // Enumerate each item definition
                while (!reader.EndOfStream && !end)
                {
                    // Helper method for reading a new line
                    string getNewLine()
                    {
                        // Loop until we find a valid line
                        while (!reader.EndOfStream)
                        {
                            // Read the line
                            var line = reader.ReadLine();

                            // Ignore if empty
                            if (line == null || String.IsNullOrWhiteSpace(line))
                                continue;

                            string processedLine = String.Empty;

                            // Process every character in the line
                            foreach (var c in line)
                            {
                                // Check if it's the comment separator
                                if (c == '/')
                                    isInComment ^= true;

                                // If not in a comment, add the character
                                else if (!isInComment)
                                    processedLine += c;
                            }

                            // Return if not empty
                            if (!String.IsNullOrWhiteSpace(processedLine))
                                return processedLine;
                        }

                        // Throw if end of file was reached
                        throw new Exception("The end of the file has been reached");
                    }

                    // Temporary buffer for the current line
                    List<string> lineBuffer = new List<string>();

                    // Helper method for getting the next value from the current line
                    string nextValue()
                    {
                        // The value to return
                        string value;

                        // Indicates if it has to loop again
                        bool redo = false;

                        do
                        {
                            // If this was a redo of the loop, set to false to finish
                            if (redo)
                                redo = false;

                            // If buffer is empty, read a new line
                            if (!lineBuffer.Any())
                                // Split it up and trim
                                lineBuffer = getNewLine().Split(',').Select(x => x.Trim()).Where(x => !String.IsNullOrWhiteSpace(x)).ToList();

                            // Get the value
                            value = lineBuffer.First();

                            // Remove the retrieved value
                            lineBuffer.RemoveAt(0);

                            // If the if command is used we are entering an engine if scope
                            if (value == "£if")
                            {
                                // Clear the buffer
                                ifCommandBuffer.Clear();
                                
                                // Indicate that we are inside of an if command
                                isInIf = true;
                            }
                            // Check if we're exiting an if command
                            else if (value == "£endif")
                            {
                                // Indicate that the if command has ended
                                isInIf = false;

                                // Indicate that we need to loop again to get the actual value
                                redo = true;
                            }
                            // If inside of an if command, add to the buffer
                            else if (isInIf)
                            {
                                ifCommandBuffer.Add(value);
                            }
                        } while (isInIf || redo);

                        // Return the value
                        return value;
                    }

                    // Create the manifest item
                    var eventManifestItem = new PC_RD_EventManifestItem();

                    // Get the next value
                    var def = nextValue();

                    // Check if the end of file has been reached
                    if (def == "*")
                    {
                        end = true;
                        continue;
                    }

                    // Make sure we're starting an event definition
                    if (def != "£def")
                        throw new Exception("Invalid event definition");

                    // Get the values
                    eventManifestItem.Name = nextValue();
                    eventManifestItem.DESFile = nextValue();
                    eventManifestItem.UnkGroup = UInt32.Parse(nextValue());
                    eventManifestItem.ETAFile = nextValue();

                    // Get the commands
                    var cmds = new List<int>();

                    // Keep track of if the end of the command (33, 255) has been reached
                    bool is33 = false;

                    // Loop until we reach the end of the command
                    while (true)
                    {
                        // Get the value
                        var v = nextValue();

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
                    eventManifestItem.XPosition = Int32.Parse(nextValue());
                    eventManifestItem.YPosition = Int32.Parse(nextValue());

                    eventManifestItem.Etat = UInt32.Parse(nextValue());
                    eventManifestItem.SubEtat = nextValue();

                    eventManifestItem.Offset_BX = UInt32.Parse(nextValue());
                    eventManifestItem.Offset_BY = UInt32.Parse(nextValue());
                    eventManifestItem.Offset_HY = UInt32.Parse(nextValue());

                    eventManifestItem.Follow_enabled = UInt32.Parse(nextValue());
                    eventManifestItem.Follow_sprite = UInt32.Parse(nextValue());
                    eventManifestItem.Hitpoints = UInt32.Parse(nextValue());

                    eventManifestItem.Obj_type = nextValue();
                    eventManifestItem.Hit_sprite = UInt32.Parse(nextValue());

                    // Only read the group if it's not an always object
                    if (eventManifestItem.Name != "always")
                        eventManifestItem.DesignerGroup = Int32.Parse(nextValue());
                    else
                        eventManifestItem.DesignerGroup = -1;

                    // Set the if commands if any were read
                    if (ifCommandBuffer.Any())
                    {
                        // Set the commands
                        eventManifestItem.IfCommand = ifCommandBuffer.ToArray();

                        // Clear the buffer
                        ifCommandBuffer.Clear();
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

        /// <summary>
        /// Event manifest item data for Rayman Designer (PC)
        /// </summary>
        public class PC_RD_EventManifestItem
        {
            public string Name { get; set; }

            public string DESFile { get; set; }

            public string[] IfCommand { get; set; }

            public uint UnkGroup { get; set; }

            public string ETAFile { get; set; }

            public int[] EventCommands { get; set; }

            public int XPosition { get; set; }

            public int YPosition { get; set; }

            public uint Etat { get; set; }

            public string SubEtat { get; set; }

            public uint Offset_BX { get; set; }

            public uint Offset_BY { get; set; }

            public uint Offset_HY { get; set; }

            public uint Follow_enabled { get; set; }

            public uint Follow_sprite { get; set; }

            public uint Hitpoints { get; set; }

            public string Obj_type { get; set; }

            public uint Hit_sprite { get; set; }

            public int DesignerGroup { get; set; }
        }
    }
}