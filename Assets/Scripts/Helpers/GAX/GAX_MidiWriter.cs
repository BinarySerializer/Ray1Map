#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
#define ISWINDOWS
#endif

#if ISWINDOWS
using Sanford.Multimedia.Midi;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using BinarySerializer.GBA.Audio.GAX;
using UnityEngine;

namespace Ray1Map {
    public class GAX_MidiWriter {
        /// <summary>
        /// Export single soundfont.
        /// Advantages: 1 soundfont for all tracks
        /// Disadvantages: Midi seems to only support 128 instruments. Game has more than that (in total)
        /// </summary>
        bool exportSingleSoundfont = false;

        public void Write(IGAX_Song song, string outPath) {
#if ISWINDOWS
            Sequence s = new Sequence();
            Track t = new Track();
            for (int i = 0; i < song.Info.NumChannels; i++) {
                s.Add(CreateTrack(song, i));
            }
            // This plugin doesn't overwrite files
            if (File.Exists(outPath)) {
                File.Delete(outPath);
            }
            s.Save(outPath);
#endif
        }

#if ISWINDOWS
        private Track CreateTrack(IGAX_Song song, int trackNum) {
            Track t = new Track();
            TempoChangeBuilder b = new TempoChangeBuilder();
            b.Tempo = 500000;
            b.Build();
            t.Insert(0, b.Result);
            ChannelMessageBuilder builder = new ChannelMessageBuilder();
            int? lastNoteOn = null;
            int currentTime = 0;
            int timeScale = 5;
            var ch = song.GetChannel(trackNum);
            t.EndOfTrackOffset = (ch.Patterns.Length * song.Info.NumRowsPerPattern) * timeScale;
            for (int trackPiece = 0; trackPiece < ch.Patterns.Length; trackPiece++) {
                GAX_Pattern gaxTrack = ch.Patterns[trackPiece];
                int baseTime = trackPiece * song.Info.NumRowsPerPattern;
                currentTime = baseTime;
                for (int i = 0; i < gaxTrack.Rows.Length; i++) {
                    GAX_PatternRow cmd = gaxTrack.Rows[i];
                    switch (cmd.Command) {
                        case GAX_PatternRow.Cmd.Note:
                        case GAX_PatternRow.Cmd.NoteOnly:
                            if (cmd.Instrument == 250) continue;
                            if (exportSingleSoundfont) {
                                if (song.Info.InstrumentSet[cmd.Instrument]?.Value == null || song.Info.InstrumentSet[cmd.Instrument].Value.SampleIndices[0] >= 128) continue;
                            } else {
                                if (song.Info.InstrumentSet[cmd.Instrument]?.Value == null || Array.IndexOf(song.Info.UsedInstrumentIndices, cmd.Instrument) >= 128) continue;
                            }
                            // Note off
                            if (lastNoteOn.HasValue) {
                                builder.Command = ChannelCommand.NoteOff;
                                builder.MidiChannel = 0;
                                builder.Data1 = lastNoteOn.Value;
                                builder.Data2 = 127;
                                builder.Build();
                                t.Insert(currentTime * timeScale, builder.Result);
                                lastNoteOn = null;
                            }
                            // Program change
                            {
                                int instrument = 0;
                                if (exportSingleSoundfont) {
                                    instrument = song.Info.InstrumentSet[cmd.Instrument].Value.SampleIndices[0];
                                } else {
                                    instrument = Array.IndexOf(song.Info.UsedInstrumentIndices, cmd.Instrument);
                                }
                                builder.MidiChannel = 0;
                                builder.Command = ChannelCommand.ProgramChange;
                                builder.Data1 = instrument;
                                builder.Build();
                                t.Insert(currentTime * timeScale, builder.Result);
                            }
                            // Note on
                            {
                                builder.Command = ChannelCommand.NoteOn;
                                int freq = cmd.Note;
                                int vel = cmd.EffectParameter;
                                builder.Data1 = freq; //GetMidiPitch(GetFrequency(freq));
                                float velf = (vel / 255f); // hack
                                int veli = Mathf.RoundToInt(velf * 127f);
                                builder.Data2 = veli;
                                lastNoteOn = builder.Data1;
                                builder.Build();
                                t.Insert(currentTime * timeScale, builder.Result);
                            }
                            break;
                    }
                    currentTime += cmd.Duration;
                }
            }
            if (lastNoteOn.HasValue) {
                builder.Command = ChannelCommand.NoteOff;
                builder.MidiChannel = 0;
                builder.Data1 = lastNoteOn.Value;
                builder.Data2 = 127;
                builder.Build();
                t.Insert(currentTime * timeScale, builder.Result);
                lastNoteOn = null;
            }

            return t;
        }

        private static Instrument[] GeneralMidiInstruments = new Instrument[] {
            Instrument.AcousticBass,
            Instrument.StringEnsemble1,
            Instrument.TaikoDrum,
            Instrument.Clarinet,
            Instrument.SteelDrums,
            Instrument.Woodblock,
            Instrument.Celesta, // Also good: whistle
			Instrument.Woodblock,
            Instrument.SynthBass2,
            Instrument.AcousticGrandPiano,
            Instrument.Woodblock,
            Instrument.SynthDrum,
            Instrument.DrawbarOrgan,
            Instrument.OrchestralHarp,
            Instrument.Vibraphone,
            Instrument.Xylophone,
            Instrument.Celesta,
            Instrument.Oboe,
            Instrument.OverdrivenGuitar,
            Instrument.AltoSax,
            Instrument.PizzicatoStrings,
            Instrument.DistortionGuitar, // Actually this
			Instrument.OrchestraHit,
            Instrument.FX1Rain,
            Instrument.Cello,
            Instrument.Viola,
            Instrument.Pad1NewAge,
            Instrument.Marimba,
            Instrument.TaikoDrum,
            Instrument.Sitar,
            Instrument.Sitar,
            Instrument.Sitar,
        };
        private static Percussion[] PercussionInstruments = new Percussion[] {
            Percussion.None,
            Percussion.None,
            Percussion.LowTom,
            Percussion.None,
            Percussion.SplashCymbal,
            Percussion.HiBongo,
            Percussion.None,
            Percussion.MuteHiConga,
            Percussion.None,
            Percussion.None,
            Percussion.None,
            Percussion.None,
            Percussion.None,
            Percussion.None,
            Percussion.None,
            Percussion.None,
            Percussion.None,
            Percussion.None,
            Percussion.None,
            Percussion.None,
            Percussion.None,
            Percussion.None,
            Percussion.None,
            Percussion.None,
            Percussion.None,
            Percussion.None,
            Percussion.None,
            Percussion.None,
            Percussion.Cowbell,
            Percussion.None,
            Percussion.None,
            Percussion.None,
        };

        private int GetMidiPitch(int freq, float tuning = 699f) {
            // See https://anotherproducer.com/online-tools-for-musicians/frequency-to-pitch-calculator/
            // default pitch = F5, so 699
            return Mathf.RoundToInt(69 + 12 * Mathf.Log(freq / tuning, 2f));

        }
        private int GetFrequency(int note) {
            return Mathf.RoundToInt(261.7f * Mathf.Pow(1.059463094359f, note));
        }

        #region Instruments
        public enum Instrument {
            None = -1,
            // Piano Family:

            /// <summary>General MIDI instrument 0 ("Acoustic Grand Piano").</summary>
            AcousticGrandPiano = 0,
            /// <summary>General MIDI instrument 1 ("Bright Acoustic Piano").</summary>
            BrightAcousticPiano = 1,
            /// <summary>General MIDI instrument 2 ("Electric Grand Piano").</summary>
            ElectricGrandPiano = 2,
            /// <summary>General MIDI instrument 3 ("Honky Tonk Piano").</summary>
            HonkyTonkPiano = 3,
            /// <summary>General MIDI instrument 4 ("Electric Piano 1").</summary>
            ElectricPiano1 = 4,
            /// <summary>General MIDI instrument 5 ("Electric Piano 2").</summary>
            ElectricPiano2 = 5,
            /// <summary>General MIDI instrument 6 ("Harpsichord").</summary>
            Harpsichord = 6,
            /// <summary>General MIDI instrument 7 ("Clavinet").</summary>
            Clavinet = 7,

            // Chromatic Percussion Family:

            /// <summary>General MIDI instrument 8 ("Celesta").</summary>
            Celesta = 8,
            /// <summary>General MIDI instrument 9 ("Glockenspiel").</summary>
            Glockenspiel = 9,
            /// <summary>General MIDI instrument 10 ("Music Box").</summary>
            MusicBox = 10,
            /// <summary>General MIDI instrument 11 ("Vibraphone").</summary>
            Vibraphone = 11,
            /// <summary>General MIDI instrument 12 ("Marimba").</summary>
            Marimba = 12,
            /// <summary>General MIDI instrument 13 ("Xylophone").</summary>
            Xylophone = 13,
            /// <summary>General MIDI instrument 14 ("Tubular Bells").</summary>
            TubularBells = 14,
            /// <summary>General MIDI instrument 15 ("Dulcimer").</summary>
            Dulcimer = 15,

            // Organ Family:

            /// <summary>General MIDI instrument 16 ("Drawbar Organ").</summary>
            DrawbarOrgan = 16,
            /// <summary>General MIDI instrument 17 ("Percussive Organ").</summary>
            PercussiveOrgan = 17,
            /// <summary>General MIDI instrument 18 ("Rock Organ").</summary>
            RockOrgan = 18,
            /// <summary>General MIDI instrument 19 ("Church Organ").</summary>
            ChurchOrgan = 19,
            /// <summary>General MIDI instrument 20 ("Reed Organ").</summary>
            ReedOrgan = 20,
            /// <summary>General MIDI instrument 21 ("Accordion").</summary>
            Accordion = 21,
            /// <summary>General MIDI instrument 22 ("Harmonica").</summary>
            Harmonica = 22,
            /// <summary>General MIDI instrument 23 ("Tango Accordion").</summary>
            TangoAccordion = 23,

            // Guitar Family:

            /// <summary>General MIDI instrument 24 ("Acoustic Guitar (nylon)").</summary>
            AcousticGuitarNylon = 24,
            /// <summary>General MIDI instrument 25 ("Acoustic Guitar (steel)").</summary>
            AcousticGuitarSteel = 25,
            /// <summary>General MIDI instrument 26 ("Electric Guitar (jazz)").</summary>
            ElectricGuitarJazz = 26,
            /// <summary>General MIDI instrument 27 ("Electric Guitar (clean)").</summary>
            ElectricGuitarClean = 27,
            /// <summary>General MIDI instrument 28 ("Electric Guitar (muted)").</summary>
            ElectricGuitarMuted = 28,
            /// <summary>General MIDI instrument 29 ("Overdriven Guitar").</summary>
            OverdrivenGuitar = 29,
            /// <summary>General MIDI instrument 30 ("Distortion Guitar").</summary>
            DistortionGuitar = 30,
            /// <summary>General MIDI instrument 31 ("Guitar Harmonics").</summary>
            GuitarHarmonics = 31,

            // Bass Family:

            /// <summary>General MIDI instrument 32 ("Acoustic Bass").</summary>
            AcousticBass = 32,
            /// <summary>General MIDI instrument 33 ("Electric Bass (finger)").</summary>
            ElectricBassFinger = 33,
            /// <summary>General MIDI instrument 34 ("Electric Bass (pick)").</summary>
            ElectricBassPick = 34,
            /// <summary>General MIDI instrument 35 ("Fretless Bass").</summary>
            FretlessBass = 35,
            /// <summary>General MIDI instrument 36 ("Slap Bass 1").</summary>
            SlapBass1 = 36,
            /// <summary>General MIDI instrument 37 ("Slap Bass 2").</summary>
            SlapBass2 = 37,
            /// <summary>General MIDI instrument 38 ("Synth Bass 1").</summary>
            SynthBass1 = 38,
            /// <summary>General MIDI instrument 39("Synth Bass 2").</summary>
            SynthBass2 = 39,

            // Strings Family:

            /// <summary>General MIDI instrument 40 ("Violin").</summary>
            Violin = 40,
            /// <summary>General MIDI instrument 41 ("Viola").</summary>
            Viola = 41,
            /// <summary>General MIDI instrument 42 ("Cello").</summary>
            Cello = 42,
            /// <summary>General MIDI instrument 43 ("Contrabass").</summary>
            Contrabass = 43,
            /// <summary>General MIDI instrument 44 ("Tremolo Strings").</summary>
            TremoloStrings = 44,
            /// <summary>General MIDI instrument 45 ("Pizzicato Strings").</summary>
            PizzicatoStrings = 45,
            /// <summary>General MIDI instrument 46 ("Orchestral Harp").</summary>
            OrchestralHarp = 46,
            /// <summary>General MIDI instrument 47 ("Timpani").</summary>
            Timpani = 47,

            // Ensemble Family:

            /// <summary>General MIDI instrument 48 ("String Ensemble 1").</summary>
            StringEnsemble1 = 48,
            /// <summary>General MIDI instrument 49 ("String Ensemble 2").</summary>
            StringEnsemble2 = 49,
            /// <summary>General MIDI instrument 50 ("Synth Strings 1").</summary>
            SynthStrings1 = 50,
            /// <summary>General MIDI instrument 51 ("Synth Strings 2").</summary>
            SynthStrings2 = 51,
            /// <summary>General MIDI instrument 52 ("Choir Aahs").</summary>
            ChoirAahs = 52,
            /// <summary>General MIDI instrument 53 ("Voice oohs").</summary>
            VoiceOohs = 53,
            /// <summary>General MIDI instrument 54 ("Synth Voice").</summary>
            SynthVoice = 54,
            /// <summary>General MIDI instrument 55 ("Orchestra Hit").</summary>
            OrchestraHit = 55,

            // Brass Family:

            /// <summary>General MIDI instrument 56 ("Trumpet").</summary>
            Trumpet = 56,
            /// <summary>General MIDI instrument 57 ("Trombone").</summary>
            Trombone = 57,
            /// <summary>General MIDI instrument 58 ("Tuba").</summary>
            Tuba = 58,
            /// <summary>General MIDI instrument 59 ("Muted Trumpet").</summary>
            MutedTrumpet = 59,
            /// <summary>General MIDI instrument 60 ("French Horn").</summary>
            FrenchHorn = 60,
            /// <summary>General MIDI instrument 61 ("Brass Section").</summary>
            BrassSection = 61,
            /// <summary>General MIDI instrument 62 ("Synth Brass 1").</summary>
            SynthBrass1 = 62,
            /// <summary>General MIDI instrument 63 ("Synth Brass 2").</summary>
            SynthBrass2 = 63,

            // Reed Family:

            /// <summary>General MIDI instrument 64 ("Soprano Sax").</summary>
            SopranoSax = 64,
            /// <summary>General MIDI instrument 65 ("Alto Sax").</summary>
            AltoSax = 65,
            /// <summary>General MIDI instrument 66 ("Tenor Sax").</summary>
            TenorSax = 66,
            /// <summary>General MIDI instrument 67 ("Baritone Sax").</summary>
            BaritoneSax = 67,
            /// <summary>General MIDI instrument 68 ("Oboe").</summary>
            Oboe = 68,
            /// <summary>General MIDI instrument 69 ("English Horn").</summary>
            EnglishHorn = 69,
            /// <summary>General MIDI instrument 70 ("Bassoon").</summary>
            Bassoon = 70,
            /// <summary>General MIDI instrument 71 ("Clarinet").</summary>
            Clarinet = 71,

            // Pipe Family:

            /// <summary>General MIDI instrument 72 ("Piccolo").</summary>
            Piccolo = 72,
            /// <summary>General MIDI instrument 73 ("Flute").</summary>
            Flute = 73,
            /// <summary>General MIDI instrument 74 ("Recorder").</summary>
            Recorder = 74,
            /// <summary>General MIDI instrument 75 ("PanFlute").</summary>
            PanFlute = 75,
            /// <summary>General MIDI instrument 76 ("Blown Bottle").</summary>
            BlownBottle = 76,
            /// <summary>General MIDI instrument 77 ("Shakuhachi").</summary>
            Shakuhachi = 77,
            /// <summary>General MIDI instrument 78 ("Whistle").</summary>
            Whistle = 78,
            /// <summary>General MIDI instrument 79 ("Ocarina").</summary>
            Ocarina = 79,

            // Synth Lead Family:

            /// <summary>General MIDI instrument 80 ("Lead 1 (square)").</summary>
            Lead1Square = 80,
            /// <summary>General MIDI instrument 81 ("Lead 2 (sawtooth)").</summary>
            Lead2Sawtooth = 81,
            /// <summary>General MIDI instrument 82 ("Lead 3 (calliope)").</summary>
            Lead3Calliope = 82,
            /// <summary>General MIDI instrument 83 ("Lead 4 (chiff)").</summary>
            Lead4Chiff = 83,
            /// <summary>General MIDI instrument 84 ("Lead 5 (charang)").</summary>
            Lead5Charang = 84,
            /// <summary>General MIDI instrument 85 ("Lead 6 (voice)").</summary>
            Lead6Voice = 85,
            /// <summary>General MIDI instrument 86 ("Lead 7 (fifths)").</summary>
            Lead7Fifths = 86,
            /// <summary>General MIDI instrument 87 ("Lead 8 (bass + lead)").</summary>
            Lead8BassPlusLead = 87,

            // Synth Pad Family:

            /// <summary>General MIDI instrument 88 ("Pad 1 (new age)").</summary>
            Pad1NewAge = 88,
            /// <summary>General MIDI instrument 89 ("Pad 2 (warm)").</summary>
            Pad2Warm = 89,
            /// <summary>General MIDI instrument 90 ("Pad 3 (polysynth)").</summary>
            Pad3Polysynth = 90,
            /// <summary>General MIDI instrument 91 ("Pad 4 (choir)").</summary>
            Pad4Choir = 91,
            /// <summary>General MIDI instrument 92 ("Pad 5 (bowed)").</summary>
            Pad5Bowed = 92,
            /// <summary>General MIDI instrument 93 ("Pad 6 (metallic)").</summary>
            Pad6Metallic = 93,
            /// <summary>General MIDI instrument 94 ("Pad 7 (halo)").</summary>
            Pad7Halo = 94,
            /// <summary>General MIDI instrument 95 ("Pad 8 (sweep)").</summary>
            Pad8Sweep = 95,

            // Synth Effects Family:

            /// <summary>General MIDI instrument 96 ("FX 1 (rain)").</summary>
            FX1Rain = 96,
            /// <summary>General MIDI instrument 97 ("FX 2 (soundtrack)").</summary>
            FX2Soundtrack = 97,
            /// <summary>General MIDI instrument 98 ("FX 3 (crystal)").</summary>
            FX3Crystal = 98,
            /// <summary>General MIDI instrument 99 ("FX 4 (atmosphere)").</summary>
            FX4Atmosphere = 99,
            /// <summary>General MIDI instrument 100 ("FX 5 (brightness)").</summary>
            FX5Brightness = 100,
            /// <summary>General MIDI instrument 101 ("FX 6 (goblins)").</summary>
            FX6Goblins = 101,
            /// <summary>General MIDI instrument 102 ("FX 7 (echoes)").</summary>
            FX7Echoes = 102,
            /// <summary>General MIDI instrument 103 ("FX 8 (sci-fi)").</summary>
            FX8SciFi = 103,

            // Ethnic Family:

            /// <summary>General MIDI instrument 104 ("Sitar").</summary>
            Sitar = 104,
            /// <summary>General MIDI instrument 105 ("Banjo").</summary>
            Banjo = 105,
            /// <summary>General MIDI instrument 106 ("Shamisen").</summary>
            Shamisen = 106,
            /// <summary>General MIDI instrument 107 ("Koto").</summary>
            Koto = 107,
            /// <summary>General MIDI instrument 108 ("Kalimba").</summary>
            Kalimba = 108,
            /// <summary>General MIDI instrument 109 ("Bagpipe").</summary>
            Bagpipe = 109,
            /// <summary>General MIDI instrument 110 ("Fiddle").</summary>
            Fiddle = 110,
            /// <summary>General MIDI instrument 111 ("Shanai").</summary>
            Shanai = 111,

            // Percussive Family:

            /// <summary>General MIDI instrument 112 ("Tinkle Bell").</summary>
            TinkleBell = 112,
            /// <summary>General MIDI instrument 113 (Agogo"").</summary>
            Agogo = 113,
            /// <summary>General MIDI instrument 114 ("Steel Drums").</summary>
            SteelDrums = 114,
            /// <summary>General MIDI instrument 115 ("Woodblock").</summary>
            Woodblock = 115,
            /// <summary>General MIDI instrument 116 ("Taiko Drum").</summary>
            TaikoDrum = 116,
            /// <summary>General MIDI instrument 117 ("Melodic Tom").</summary>
            MelodicTom = 117,
            /// <summary>General MIDI instrument 118 ("Synth Drum").</summary>
            SynthDrum = 118,
            /// <summary>General MIDI instrument 119 ("Reverse Cymbal").</summary>
            ReverseCymbal = 119,

            // Sound Effects Family:

            /// <summary>General MIDI instrument 120 ("Guitar Fret Noise").</summary>
            GuitarFretNoise = 120,
            /// <summary>General MIDI instrument 121 ("Breath Noise").</summary>
            BreathNoise = 121,
            /// <summary>General MIDI instrument 122 ("Seashore").</summary>
            Seashore = 122,
            /// <summary>General MIDI instrument 123 ("Bird Tweet").</summary>
            BirdTweet = 123,
            /// <summary>General MIDI instrument 124 ("Telephone Ring").</summary>
            TelephoneRing = 124,
            /// <summary>General MIDI instrument 125 ("Helicopter").</summary>
            Helicopter = 125,
            /// <summary>General MIDI instrument 126 ("Applause").</summary>
            Applause = 126,
            /// <summary>General MIDI instrument 127 ("Gunshot").</summary>
            Gunshot = 127,
        };

        public enum Percussion {
            None = -1,
            AcousticBassDrum = 35,
            BassDrum1 = 36,
            SideStick = 37,
            AcousticSnare = 38,
            HandClap = 39,
            ElectricSnare = 40,
            LowFloorTom = 41,
            ClosedHiHat = 42,
            HighFloorTom = 43,
            PedalHiHat = 44,
            LowTom = 45,
            OpenHiHat = 46,
            LowMidTom = 47,
            HiMidTom = 48,
            CrashCymbal1 = 49,
            HighTom = 50,
            RideCymbal1 = 51,
            ChineseCymbal = 52,
            RideBell = 53,
            Tambourine = 54,
            SplashCymbal = 55,
            Cowbell = 56,
            CrashCymbal2 = 57,
            Vibraslap = 58,
            RideCymbal2 = 59,
            HiBongo = 60,
            LowBongo = 61,
            MuteHiConga = 62,
            OpenHiConga = 63,
            LowConga = 64,
            HighTimbale = 65,
            LowTimbale = 66,
            HighAgogo = 67,
            LowAgogo = 68,
            Cabasa = 69,
            Maracas = 70,
            ShortWhistle = 71,
            LongWhistle = 72,
            ShortGuiro = 73,
            LongGuiro = 74,
            Claves = 75,
            HiWoodBlock = 76,
            LowWoodBlock = 77,
            MuteCuica = 78,
            OpenCuica = 79,
            MuteTriangle = 80,
            OpenTriangle = 81,
        }
        #endregion
#endif
    }
}
