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
            public ushort[] Ushorts { get; set; }

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

                    case Instruction.HandleUshorts:
                    case Instruction.HandleUshorts2:
                        if (Ushorts == null)
                        {
                            var blocks = new List<ushort>();

                            var index = 0;

                            while (true)
                            {
                                var v = s.Serialize<ushort>(default, name: $"{nameof(Ushorts)}[{index++}]");

                                blocks.Add(v);

                                if ((v & 0x8000) != 0)
                                    break;
                            }

                            Ushorts = blocks.ToArray();
                        }
                        else
                        {
                            s.SerializeArray<ushort>(Ushorts, Ushorts.Length, name: nameof(Ushorts));
                        }
                        break;

                    case Instruction.Unk3:
                        // No data gets parsed here, instead function 0x08060C10 is called (US rom)
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(InstructionByte), InstructionByte, null);
                }
            }

            public enum Instruction : byte
            {
                DrawPortrait = 0,
                HandleUshorts = 1,
                HandleUshorts2 = 2, // Some values are set first if this instruction is used - for multiple choices? Seems to always be used in combination with Flag_2.
                Unk3 = 3
            }

            [Flags]
            public enum Flags : byte
            {
                None = 0,
                Flag_0 = 1 << 0,
                Flag_1 = 1 << 1,
                Flag_2 = 1 << 2, // Seems to indicate some additional data after the block
                LastEntry = 1 << 3,
                Flag_4 = 1 << 4,
                Flag_5 = 1 << 5,
                Flag_6 = 1 << 6, // Always set?
                Flag_7 = 1 << 7,
            }
        }
    }
}