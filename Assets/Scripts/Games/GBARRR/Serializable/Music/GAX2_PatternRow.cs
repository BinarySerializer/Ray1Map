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
        public byte Effect { get; set; } // ? always 0xC ?
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
                    case 0xFA: return Cmd.EffectOnly;
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
                    Effect = s.Serialize<byte>(Effect, name: nameof(Effect));
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
                    Effect = s.Serialize<byte>(Effect, name: nameof(Effect));
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
                str += $"Effect: {Effect:X2}, EffectParam: {EffectParameter:X2}";
            }
            str += $")";
            return str;
		}
	}
}