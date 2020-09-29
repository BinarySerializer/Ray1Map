using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// Event CMD data for the Mapper
    /// </summary>
    public class R1_Mapper_EventCMD : R1TextSerializable
    {
        public R1_Mapper_EventCMDItem[] Events { get; set; }
        public R1_Mapper_AlwaysEventCMDItem[] AlwaysEvents { get; set; }

        public override void Read(R1TextParser parser)
        {
            var ed = new List<R1_Mapper_EventCMDItem>();
            var aed = new List<R1_Mapper_AlwaysEventCMDItem>();

            while (true)
            {
                // Get the next value
                var etaFile = parser.ReadValue();

                // Stop reading once we reached the end
                if (etaFile == null)
                    break;

                // Create a definition
                R1_Mapper_EventDefinition item;

                // Get the name
                var name = parser.ReadValue();

                // Create the definition
                if (name == "always")
                {
                    var alwaysItem = new R1_Mapper_AlwaysEventCMDItem();
                    aed.Add(alwaysItem);
                    item = alwaysItem;
                }
                else
                {
                    var normalItem = new R1_Mapper_EventCMDItem();
                    ed.Add(normalItem);
                    item = normalItem;
                }

                // Set the ETA file name
                item.ETAFile = etaFile;

                // Set the name
                item.Name = name;

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

                if (item is R1_Mapper_EventCMDItem med)
                {
                    med.DisplayPrio = parser.ReadByteValue();
                    med.LinkID = parser.ReadShortValue();
                }

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
                item.DesignerGroup = parser.ReadByteValue();
            }

            // Set the parsed values
            Events = ed.ToArray();
            AlwaysEvents = aed.ToArray();
        }
    }
}