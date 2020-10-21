using System;
using UnityEngine;

namespace R1Engine
{
    public class GAX2_MusicCommand : R1Serializable
    {
        public byte Note { get; set; }
        public byte Instrument { get; set; }
        public byte Effect { get; set; } // ? always 0xC ?
        public byte Velocity { get; set; }

        public byte RestDuration { get; set; }
        public byte Unknown81Arg { get; set; }

        public byte Duration {
            get {
                switch (Command) {
                    case Cmd.Note: return 1;
                    case Cmd.RestSingle: return 1;
                    case Cmd.RestMultiple: return RestDuration;
                    case Cmd.StartTrack: return 0;
                    case Cmd.EmptyTrack: return 0;
                    case Cmd.Unknown81: return 1;
                    case Cmd.ChangeLoudness: return 1;
                    default: return 0;
                }
            }
        }
        public Cmd Command {
            get {
                switch (Note) {
                    case 0: return Cmd.StartTrack;
                    case 1: return Cmd.EmptyTrack;
                    case 0x80: return Cmd.RestSingle;
                    case 0x81: return Cmd.Unknown81;
                    case 0xFA: return Cmd.ChangeLoudness;
                    case 0xFF: return Cmd.RestMultiple;
                    default:
                        if ((Note & 0x80) == 0x80) {
                            return Cmd.Unknown;
                        } else {
                            return Cmd.Note;
                        }
                }
            }
        }

        public override void SerializeImpl(SerializerObject s) {
            Note = s.Serialize<byte>(Note, name: nameof(Note));
            switch (Command) {
                case Cmd.Note:
                    Instrument = s.Serialize<byte>(Instrument, name: nameof(Instrument));
                    Effect = s.Serialize<byte>(Effect, name: nameof(Effect));
                    Velocity = s.Serialize<byte>(Velocity, name: nameof(Velocity));
                    break;
                case Cmd.Unknown81:
                    Unknown81Arg = s.Serialize<byte>(Unknown81Arg, name: nameof(Unknown81Arg));
                    break;
                case Cmd.ChangeLoudness:
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
            Unknown,
            RestSingle = 0x80,
            Unknown81 = 0x81,
            ChangeLoudness = 0xFA,
            RestMultiple = 0xFF
        }
    }
}