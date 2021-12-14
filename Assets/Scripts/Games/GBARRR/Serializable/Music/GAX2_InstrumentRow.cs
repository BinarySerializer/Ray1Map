using System;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBARRR
{
    public class GAX2_InstrumentRow : BinarySerializable {
        public sbyte RelativeNoteNumber { get; set; }
        public bool DontUseNotePitch { get; set; } // Is this a sign?
        public byte SampleIndex { get; set; } // Starting from 1
        public byte Byte_03 { get; set; }
        public Effect[] Effects { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            RelativeNoteNumber = s.Serialize<sbyte>(RelativeNoteNumber, name: nameof(RelativeNoteNumber));
            DontUseNotePitch = s.Serialize<bool>(DontUseNotePitch, name: nameof(DontUseNotePitch));
			SampleIndex = s.Serialize<byte>(SampleIndex, name: nameof(SampleIndex));
			Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
			Effects = s.SerializeObjectArray<Effect>(Effects, 2, name: nameof(Effects));
		}

		public class Effect : BinarySerializable {
            public EffectType Type { get; set; }
            public byte Parameter { get; set; }
			public override void SerializeImpl(SerializerObject s) {
                s.DoBits<ushort>(b => {
                    Parameter = b.SerializeBits<byte>(Parameter, 8, name: nameof(Parameter));
                    Type = b.SerializeBits<EffectType>(Type, 8, name: nameof(Type));
                });
			}

            public enum EffectType : byte { // All effects that are executed every tick include the first tick, so not like XM where tone portamentos and volume slides start on tick 2
                None               = 0,
                PortamentoUp       = 1, // Increases current note pitch by xx units on every tick of the row
                PortamentoDown     = 2, // Decreases current note pitch by xx units on every tick of the row
                Unknown3           = 3,
                Unknown4           = 4,
                RowSwitchCheck     = 5, // Checks row switch timer. If zero, it switches to instrument row (param), else it decreases row switch timer
                RowSwitchTimer     = 6, // Sets row switch timer
                Unknown7           = 7,
                Unknown8           = 8,
                Unknown9           = 9,
                VolumeSlideUp     = 10, // Increases instrument volume by xx units on every tick of the row (this is a volume scale factor that defaults at 0xFF)
                VolumeSlideDown   = 11, // Decreases instrument volume by xx units on every tick of the row
                SetVolume         = 12, // Set instrument volume directly
                Unknown13         = 13,
                Unknown14         = 14,
                SetSpeed          = 15, // Sets ticks per instrument row
            }
		}
	}
}