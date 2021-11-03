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
        public byte Velocity { get; set; }

        public byte RestDuration { get; set; }

        public byte Duration {
            get {
                switch (Command) {
                    case Cmd.Note: return 1;
                    case Cmd.NoteCompressed: return 1;
                    case Cmd.RestSingle: return 1;
                    case Cmd.RestMultiple: return RestDuration;
                    case Cmd.StartTrack: return 0;
                    case Cmd.EmptyTrack: return 0;
                    case Cmd.EffectOnly: return 1;
                    default: return 0;
                }
            }
        }
        public bool IsCompressed => BitHelpers.ExtractBits(Flags, 1, 7) == 1;
        public Cmd Command {
            get {
                switch (Flags) {
                    case 0: return Cmd.StartTrack;
                    case 1: return Cmd.EmptyTrack;
                    case 0x80: return Cmd.RestSingle;
                    case 0xFA: return Cmd.EffectOnly;
                    case 0xFF: return Cmd.RestMultiple;
                    default:
                        if (IsCompressed) {
                            return Cmd.NoteCompressed;
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
                    Velocity = s.Serialize<byte>(Velocity, name: nameof(Velocity));
                    break;
                case Cmd.NoteCompressed:
                    Note = (byte)BitHelpers.ExtractBits(Flags, 7, 0);
                    Instrument = s.Serialize<byte>(Instrument, name: nameof(Instrument));
                    break;
                case Cmd.EffectOnly:
                    Effect = s.Serialize<byte>(Effect, name: nameof(Effect));
                    Velocity = s.Serialize<byte>(Velocity, name: nameof(Velocity));
                    break;
                case Cmd.RestMultiple:
                    RestDuration = s.Serialize<byte>(RestDuration, name: nameof(RestDuration));
                    break;
            }
        }

        public enum Cmd {
            StartTrack = 0,
            EmptyTrack = 1,
            Note,
            NoteCompressed,
            Unknown,
            RestSingle = 0x80,
            EffectOnly = 0xFA,
            RestMultiple = 0xFF,
        }
    }
}