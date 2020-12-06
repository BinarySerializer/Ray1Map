﻿using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class GBC_SoundProgram : GBC_BaseBlock {
        public ushort UShort_00 { get; set; }
        public ushort PatternOffsetsOffset { get; set; }
        public byte Byte_04 { get; set; }
        public byte PatternCount { get; set; }
        public uint UInt_06 { get; set; }
        public ushort[] PatternOffsets { get; set; }
        public Pattern[] Patterns { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            UShort_00 = s.Serialize<ushort>(UShort_00, name: nameof(UShort_00));
            PatternOffsetsOffset = s.Serialize<ushort>(PatternOffsetsOffset, name: nameof(PatternOffsetsOffset));
            Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
            PatternCount = s.Serialize<byte>(PatternCount, name: nameof(PatternCount));
            UInt_06 = s.Serialize<uint>(UInt_06, name: nameof(UInt_06));

            s.DoAt(BlockStartPointer + PatternOffsetsOffset, () => {
                PatternOffsets = s.SerializeArray<ushort>(PatternOffsets, PatternCount, name: nameof(PatternOffsets));
            });
            if(Patterns == null) Patterns = new Pattern[PatternCount];
            for (int i = 0; i < PatternCount; i++) {
                Patterns[i] = s.DoAt(BlockStartPointer + PatternOffsets[i], () => {
                    return s.SerializeObject<Pattern>(Patterns[i], name: $"{nameof(Patterns)}[{i}]");
                });
            }
            if(Patterns.Length > 0) s.Goto(Patterns[Patterns.Length - 1].Offset + Patterns[Patterns.Length - 1].Size);
        }

		public class Pattern : R1Serializable {
            public byte PatByte_00 { get; set; }
            public byte PatByte_01 { get; set; }
            public byte PatByte_02 { get; set; }
            public Row[] Rows { get; set; }
            public override void SerializeImpl(SerializerObject s) {
                PatByte_00 = s.Serialize<byte>(PatByte_00, name: nameof(PatByte_00));
                PatByte_01 = s.Serialize<byte>(PatByte_01, name: nameof(PatByte_01));
                if (PatByte_01 == 0) {
                    if (Rows == null) {
                        List<Row> rows = new List<Row>();
                        bool isEndOfTrack = false;
                        while (!isEndOfTrack) {
                            Row row = s.SerializeObject<Row>(default, name: $"{nameof(Rows)}[{rows.Count}]");
                            rows.Add(row);
                            if (row.Command == 0xFC) {
                                isEndOfTrack = true;
                            }
                        }
                        Rows = rows.ToArray();
                    } else {
                        Rows = s.SerializeObjectArray<Row>(Rows, Rows.Length, name: nameof(Rows));
                    }
                } else {
                    PatByte_02 = s.Serialize<byte>(PatByte_02, name: nameof(PatByte_02));
                }
            }
		}

		public class Row : R1Serializable {
            public byte Command { get; set; }
            public byte[] CommandData { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Command = s.Serialize<byte>(Command, name: nameof(Command));
                if (Command != 0xFC) {
                    CommandData = s.SerializeArray<byte>(CommandData, 4, name: nameof(CommandData));
                }

                if ((s.GameSettings.Game == Game.GBC_DD || s.GameSettings.Game == Game.GBC_Mowgli) && Command == 0xFC) {
                    CommandData = s.SerializeArray<byte>(CommandData, 3, name: nameof(CommandData));
                }
            }
		}
	}
}