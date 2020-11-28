using System;
using System.Collections.Generic;

namespace R1Engine
{
    public class GBC_ChannelEvent : GBC_BaseChannelEvent
    {
        public GBC_ChannelEventInstruction[] Instructions { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            base.SerializeImpl(s);

            // Serialize instructions
            if (Instructions == null)
            {
                var instructions = new List<GBC_ChannelEventInstruction>();

                var p = s.CurrentPointer;
                var endPointer = p + EventDataSize;
                var index = 0;

                try
                {
                    while (endPointer.AbsoluteOffset > s.CurrentPointer.AbsoluteOffset)
                    {
                        instructions.Add(s.SerializeObject<GBC_ChannelEventInstruction>(default, name: $"{nameof(Instructions)}[{index}]"));
                        index++;
                    }

                    if (endPointer.AbsoluteOffset != s.CurrentPointer.AbsoluteOffset)
                        throw new Exception("Instruction overflow!");
                }
                catch (Exception ex)
                {
                    s.Log($"Errors parsing instruction: {ex.Message}");

                    s.Goto(endPointer);
                }

                Instructions = instructions.ToArray();
            }
            else
            {
                Instructions = s.SerializeObjectArray<GBC_ChannelEventInstruction>(Instructions, Instructions.Length, name: nameof(Instructions));
            }
        }
    }
}