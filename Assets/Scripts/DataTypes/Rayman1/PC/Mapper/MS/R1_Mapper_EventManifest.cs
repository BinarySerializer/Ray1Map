using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class R1_Mapper_EventManifest : R1TextSerializable
    {
        public R1_Mapper_EventDefinition[] EventDefinitions { get; set; }
        public R1_Mapper_AlwaysEventDefinition[] AlwaysEventDefinitions { get; set; }

        public override void Read(R1TextParser parser)
        {
            var ed = new List<R1_Mapper_EventDefinition>();
            var aed = new List<R1_Mapper_AlwaysEventDefinition>();

            while (true)
            {
                // Get the next value
                var value = parser.ReadValue();

                // Stop reading once we reached the end
                if (value == null)
                    break;

                // Only parse event definitions
                if (value != "£def")
                    continue;

                // Create a definition
                R1_Mapper_BaseEventDefinition item;

                // Get the name
                var name = parser.ReadValue();
                
                // Create the definition
                if (name == "always")
                {
                    var alwaysItem = new R1_Mapper_AlwaysEventDefinition();
                    aed.Add(alwaysItem);
                    item = alwaysItem;
                }
                else
                {
                    var normalItem = new R1_Mapper_EventDefinition();
                    ed.Add(normalItem);
                    item = normalItem;
                }

                // Set the name
                item.Name = name;

                var nextValue = parser.ReadValue();

                // Read the if values if it's an always object
                if (item is R1_Mapper_AlwaysEventDefinition ae && nextValue == "£if")
                {
                    var ifBuffer = new List<string>();

                    while (true)
                    {
                        nextValue = parser.ReadValue();

                        if (nextValue == "£endif")
                            break;

                        ifBuffer.Add(nextValue);
                    }

                    ae.IfCommand = ifBuffer.ToArray();

                    nextValue = parser.ReadValue();
                }

                item.DESFile = nextValue;
                item.DisplayPrio = parser.ReadByteValue();
                item.ETAFile = parser.ReadValue();

                var cmdBuffer = new List<short>();

                do
                {
                    // Since command parameters can be both signed and unsigned bytes we read them as shorts
                    cmdBuffer.Add(parser.ReadShortValue());
                } while (cmdBuffer.Last() != 0xFF);

                item.EventCommands = cmdBuffer.Select(x => (byte)x).ToArray();

                // Read values
                item.XPosition = parser.ReadShortValue();
                item.YPosition = parser.ReadShortValue();

                item.Etat = parser.ReadByteValue();
                item.SubEtat = parser.ReadByteValue();

                item.OffsetBX = parser.ReadByteValue();
                item.OffsetBY = parser.ReadByteValue();
                item.OffsetHY = parser.ReadByteValue();

                item.FollowEnabled = parser.ReadByteValue();
                item.FollowSprite = parser.ReadByteValue();
                item.HitPoints = parser.ReadUIntValue();

                item.Type = (R1_EventType)parser.ReadShortValue();
                item.HitSprite = parser.ReadByteValue();

                // Read the group if it's not an always object
                if (item is R1_Mapper_EventDefinition e)
                    e.DesignerGroup = parser.ReadByteValue();
            }

            // Set the parsed values
            EventDefinitions = ed.ToArray();
            AlwaysEventDefinitions = aed.ToArray();
        }
    }
}