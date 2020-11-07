using System;
using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class GBAIsometric_Spyro_DialogData : R1Serializable
    {
        public Entry[] Entries { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (Entries == null)
            {
                var entries = new List<Entry>();

                while (entries.LastOrDefault()?.InstructionFlags.HasFlag(Entry.Flags.LastEntry) != true)
                    entries.Add(s.SerializeObject<Entry>(default, name: $"{nameof(Entries)}[{entries.Count}]"));

                Entries = entries.ToArray();
            }
            else
            {
                s.SerializeObjectArray<Entry>(Entries, Entries.Length, name: nameof(Entries));
            }
        }

        public class Entry : R1Serializable
        {
            // Instruction data
            public Instruction InstructionByte { get; set; }
            public Flags InstructionFlags { get; set; }

            // Parsed data
            public ushort PortraitIndex { get; set; }
            public GBAIsometric_LocIndex[] LocIndices { get; set; }
            public GBAIsometric_LocIndex[] MultiChoiceLocIndices { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                // NOTE: The game loops until it gets a valid instruction byte value (0-3), but we probably don't need to assuming it's always formatted correctly
                InstructionByte = s.Serialize<Instruction>(InstructionByte, name: nameof(InstructionByte));
                InstructionFlags = s.Serialize<Flags>(InstructionFlags, name: nameof(InstructionFlags));

                switch (InstructionByte)
                {
                    case Instruction.DrawPortrait:
                        PortraitIndex = s.Serialize<ushort>(PortraitIndex, name: nameof(PortraitIndex));
                        break;

                    case Instruction.DrawText:
                    case Instruction.DrawMultiChoiceText:
                        if (LocIndices == null)
                        {
                            var blocks = new List<GBAIsometric_LocIndex>();

                            var index = 0;

                            while (true)
                            {
                                var v = s.SerializeObject<GBAIsometric_LocIndex>(default, x => x.ParseIndexFunc = i => i & 0x7FFF, name: $"{nameof(LocIndices)}[{index++}]");

                                blocks.Add(v);

                                if ((v.LocIndex & 0x8000) != 0)
                                    break;
                            }

                            LocIndices = blocks.ToArray();
                        }
                        else
                        {
                            s.SerializeObjectArray<GBAIsometric_LocIndex>(LocIndices, LocIndices.Length, name: nameof(LocIndices));
                        }
                        break;

                    case Instruction.MoveCamera:
                        // No data gets parsed here, instead it jumps to r3
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(InstructionByte), InstructionByte, null);
                }

                if (InstructionFlags.HasFlag(Flags.HasMultiChoiceIndices))
                    MultiChoiceLocIndices = s.SerializeObjectArray<GBAIsometric_LocIndex>(MultiChoiceLocIndices, 4, x => x.ParseIndexFunc = i => i == 0 ? -1 : i, name: nameof(MultiChoiceLocIndices));
            }

            public enum Instruction : byte
            {
                DrawPortrait = 0,
                DrawText = 1,
                DrawMultiChoiceText = 2,
                MoveCamera = 3
            }

            [Flags]
            public enum Flags : byte
            {
                None = 0,
                Flag_0 = 1 << 0,
                Flag_1 = 1 << 1,
                HasMultiChoiceIndices = 1 << 2, // Seems to indicate some additional data after the block
                LastEntry = 1 << 3,
                Flag_4 = 1 << 4,
                Flag_5 = 1 << 5,
                Flag_6 = 1 << 6, // Always set?
                Flag_7 = 1 << 7,
            }
        }
    }
}