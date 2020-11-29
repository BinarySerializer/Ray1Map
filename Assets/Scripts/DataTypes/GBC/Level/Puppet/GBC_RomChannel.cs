using System;
using System.Collections.Generic;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Contains all the commands to update an image.
    /// The first RomChannel will have a lot of data, while other RomChannels will only contain the differences.
    /// Better name: Keyframe
    /// </summary>
    public class GBC_RomChannel : R1Serializable {
        public byte Time { get; set; } // Amount of frames to show this image (60FPS)
        public byte DataSize { get; set; }
        public GBC_ChannelData ChannelData { get; set; } // Set before serializing
        public GBC_RomChannelCommand[] Commands { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Time = s.Serialize<byte>(Time, name: nameof(Time));
            DataSize = s.Serialize<byte>(DataSize, name: nameof(DataSize));

            // Serialize commands
            if (Commands == null)
            {
                var instructions = new List<GBC_RomChannelCommand>();

                var p = s.CurrentPointer;
                var endPointer = p + DataSize;
                var index = 0;

                try
                {
                    while (endPointer.AbsoluteOffset > s.CurrentPointer.AbsoluteOffset)
                    {
                        instructions.Add(s.SerializeObject<GBC_RomChannelCommand>(default, onPreSerialize: x => x.ChannelData = ChannelData, name: $"{nameof(Commands)}[{index}]"));
                        index++;
                    }

                    if (endPointer.AbsoluteOffset != s.CurrentPointer.AbsoluteOffset)
                        throw new Exception("Instruction overflow!");
                }
                catch (Exception ex)
                {
                    s.Log($"Error parsing instruction: {ex.Message}");
                    Debug.LogWarning($"Error parsing instruction at {Offset}: {ex.Message}");

                    s.Goto(endPointer);
                }

                Commands = instructions.ToArray();
            }
            else
            {
                Commands = s.SerializeObjectArray<GBC_RomChannelCommand>(Commands, Commands.Length, name: nameof(Commands));
            }
        }
    }
}