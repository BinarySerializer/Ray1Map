using System;
using System.Collections.Generic;
using BinarySerializer;

namespace Ray1Map.GBC
{
    /// <summary>
    /// Contains all the commands to update an image.
    /// The first keyframe will have a lot of data, while other keyframes will only contain the differences.
    /// </summary>
    public class GBC_Keyframe : BinarySerializable {
        public byte Time { get; set; } // Amount of frames to show this image (60FPS)
        public byte DataSize { get; set; }
        public GBC_RomChannel ChannelData { get; set; } // Set before serializing
        public GBC_Keyframe_Command[] Commands { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Time = s.Serialize<byte>(Time, name: nameof(Time));
            DataSize = s.Serialize<byte>(DataSize, name: nameof(DataSize));

            // Serialize commands
            if (Commands == null)
            {
                var instructions = new List<GBC_Keyframe_Command>();

                var p = s.CurrentPointer;
                var endPointer = p + DataSize;
                var index = 0;

                try
                {
                    while (endPointer.AbsoluteOffset > s.CurrentPointer.AbsoluteOffset)
                    {
                        instructions.Add(s.SerializeObject<GBC_Keyframe_Command>(default, onPreSerialize: x => x.ChannelData = ChannelData, name: $"{nameof(Commands)}[{index}]"));
                        index++;
                    }

                    if (endPointer.AbsoluteOffset != s.CurrentPointer.AbsoluteOffset)
                        throw new Exception($"Instruction overflow for level {s.GetR1Settings().Level}!");
                }
                catch (Exception ex)
                {
                    s.Log("Error parsing instruction: {0}", ex.Message);
                    s.LogWarning("Error parsing instruction at {0}: {1}", Offset, ex.Message);

                    s.Goto(endPointer);
                }

                Commands = instructions.ToArray();
            }
            else
            {
                Commands = s.SerializeObjectArray<GBC_Keyframe_Command>(Commands, Commands.Length, name: nameof(Commands));
            }
        }
    }
}