using System;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBARRR
{
    public class GAX2_PatternRow : BinarySerializable
    {
        public byte Flags { get; set; }
        public byte Note { get; set; }
        public byte Instrument { get; set; }
        public EffectType Effect { get; set; }
        public byte EffectParameter { get; set; }

        public byte RestDuration { get; set; }

        public int Duration {
            get {
                switch (Command) {
                    case Cmd.Note: return 1;
                    case Cmd.NoteOnly: return 1;
                    case Cmd.NoteOff: return 1 + RestDuration;
                    case Cmd.Rest: return 1;
                    case Cmd.RestMulti: return RestDuration;
                    case Cmd.EffectOnly: return 1;
                    default: return 0;
                }
            }
        }
        public bool IsCompressed => BitHelpers.ExtractBits(Flags, 1, 7) == 1;
        public Cmd Command {
            get {
                switch (Flags) {
                    case 0x80: return Cmd.Rest;
                    //case 0x81: return Cmd.NoteOff;
                    case 0xFA: return Cmd.EffectOnly; // 0xFB - 0xFF
                    case 0xFF: return Cmd.RestMulti;
                    default:
                        if (IsCompressed) {
                            return Cmd.NoteOnly;
                        } else {
                            return Cmd.Note;
                        }
                }
            }
        }

        public override void SerializeImpl(SerializerObject s) {
            Flags = s.Serialize<byte>(Flags, name: nameof(Flags));
            switch (Command) {
                case Cmd.Note:
                    Note = (byte)BitHelpers.ExtractBits(Flags, 7, 0);
                    Instrument = s.Serialize<byte>(Instrument, name: nameof(Instrument));
                    Effect = s.Serialize<EffectType>(Effect, name: nameof(Effect));
                    EffectParameter = s.Serialize<byte>(EffectParameter, name: nameof(EffectParameter));
                    break;
                case Cmd.NoteOnly:
                    Note = (byte)BitHelpers.ExtractBits(Flags, 7, 0);
                    Instrument = s.Serialize<byte>(Instrument, name: nameof(Instrument));
                    break;
                case Cmd.NoteOff:
                    RestDuration = s.Serialize<byte>(RestDuration, name: nameof(RestDuration));
                    break;
                case Cmd.EffectOnly:
                    Effect = s.Serialize<EffectType>(Effect, name: nameof(Effect));
                    EffectParameter = s.Serialize<byte>(EffectParameter, name: nameof(EffectParameter));
                    break;
                case Cmd.RestMulti:
                    RestDuration = s.Serialize<byte>(RestDuration, name: nameof(RestDuration));
                    break;
            }
        }

        public enum Cmd {
            Note,
            NoteOnly,
            Unknown,
            Rest = 0x80,
            NoteOff = 0x81,
            EffectOnly = 0xFA,
            RestMulti = 0xFF,
        }

        public enum EffectType : byte { // All effects that are executed every tick include the first tick, so not like XM where tone portamentos and volume slides start on tick 2
            None               = 0,
            PortamentoUp       = 1, // Increases current note pitch by xx units on every tick of the row
            PortamentoDown     = 2, // Decreases current note pitch by xx units on every tick of the row
            TonePortamento     = 3, // Slides the pitch of the previous note towards the current note by xx units on every tick of the row
            Unknown4           = 4,
            Unknown5           = 5,
            Unknown6           = 6,
            SetSpeedModulate   = 7, // Top 4 bits are the speed for now, bottom 4 bits are the speed for next tick. Modulates between them every row
            Unknown8           = 8,
            Unknown9           = 9,
            VolumeSlideUp     = 10, // Increases note volume by xx units on every tick of the row (end volume clamped between 0 and 0xFF)
            VolumeSlideDown   = 11, // Decreases note volume by xx units on every tick of the row
            SetVolume         = 12, // Set volume directly
            PatternBreak      = 13, // Jumps to row xx of the next pattern in the Order List
            NoteDelay         = 14, // (Only if MSB of Parameter == 0xD, like XM) Delays the note or instrument change in the current pattern cell by x ticks
            SetSpeed          = 15, // Sets the module Speed (ticks per row)
        }

		public override bool UseShortLog => true;
		public override string ShortLog => ToString();

		public override string ToString() {
			string str = $"[Flags:{Flags:X2}] {Command,10}(";
            bool hasNote = false;
            bool hasEffect = false;
            switch (Command) {
                case Cmd.Note:
                    hasNote = true;
                    hasEffect = true;
                    break;
                case Cmd.NoteOnly:
                    hasNote = true;
                    break;
                case Cmd.EffectOnly:
                    hasEffect = true;
                    break;
                case Cmd.RestMulti:
                case Cmd.NoteOff:
                    str += $"Duration: {RestDuration})";
                    return str;
                default:
                    str += $")";
                    return str;
            }
            if (hasNote) {
                str += $"Note: {Note,3}, Instrument: {Instrument,3}";
                if (hasEffect) str += ", ";
            }
            if (hasEffect) {
                str += $"Effect: {Effect}(0x{EffectParameter:X2})";
            }
            str += $")";
            return str;
		}
	}
}