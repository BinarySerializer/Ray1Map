﻿#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
#define ISWINDOWS
#endif

#if ISWINDOWS
using Sanford.Multimedia.Midi;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using BinarySerializer;
using BinarySerializer.Ray1.Jaguar;

namespace Ray1Map.Rayman1_Jaguar {
    public class R1Jaguar_MidiWriter {
        public void Write(JAG_MusicDescriptor file, string outPath) {
#if ISWINDOWS
            Sequence s = new Sequence();
            Track t = new Track();
            s.Add(CreateTrack(file));
            // This plugin doesn't overwrite files
            if (File.Exists(outPath)) {
                File.Delete(outPath);
            }
            s.Save(outPath);
#endif
        }

#if ISWINDOWS
        private Track CreateTrack(JAG_MusicDescriptor jagFile) {
            Track t = new Track();
            TempoChangeBuilder b = new TempoChangeBuilder();
            b.Tempo = 22000;
            b.Build();
            t.Insert(0, b.Result);
            ChannelMessageBuilder builder = new ChannelMessageBuilder();
            Dictionary<int, int> curNoteOnChannel = new Dictionary<int, int>();
            int timeScale = 1;
            for (int i = 0; i < jagFile.MusicData.Length; i++) {
                JAG_MusicData e = jagFile.MusicData[i];
                if (e.Time != int.MaxValue) {
                    int channelByte = BitHelpers.ExtractBits(e.Command, 8, 24);
                    if (channelByte == 0x7F) { // special point in the song
                        int idByte = BitHelpers.ExtractBits(e.Command, 8, 0);
                        if (idByte == 0xFF && i >= jagFile.MusicData.Length - 2) { // End point
                            t.EndOfTrackOffset = e.Time / timeScale;
                            break;
                        } else {
                            // Loop point, or command to return to loop point
                        }
                    } else {
                        int channel = BitHelpers.ExtractBits(e.Command, 4, 26);
                        int command = BitHelpers.ExtractBits(e.Command, 1, 31);
                        if (command == 1) {
                            // Note off
                            if (curNoteOnChannel.ContainsKey(channel)) {
                                builder.Command = ChannelCommand.NoteOff;
                                int note = curNoteOnChannel.TryGetValue(channel, out int val) ? val : 0;
                                if (note >= 128) {
                                    builder.MidiChannel = 9;
                                    builder.Data1 = note - 128;
                                } else {
                                    builder.MidiChannel = channel;
                                    builder.Data1 = note;
                                }
                                builder.Data2 = 127;
                                builder.Build();
                                t.Insert(e.Time / timeScale, builder.Result);
                            }

                            // Program change
                            int instrument = BitHelpers.ExtractBits(e.Command, 5, 21);
                            if (Patches[instrument].GetType() == typeof(Percussion)) {
                                builder.MidiChannel = 9;
                            } else {
                                builder.MidiChannel = channel;
                                builder.Command = ChannelCommand.ProgramChange;
                                builder.Data1 = (Instrument)Patches[instrument] == Instrument.None ? 0 : (int)(Instrument)Patches[instrument];
                                //if (GeneralMidiInstruments[instrument] == Instrument.Sitar) Controller.print("unknown @ " + jagFile.MusicDataPointer);
                                builder.Build();
                                t.Insert(e.Time / timeScale, builder.Result);
                            }

                            // Note on
                            builder.Command = ChannelCommand.NoteOn;
                            //int freq = BitHelpers.ExtractBits(e.Command, 13, 8);
                            int freq = BitHelpers.ExtractBits(e.Command, 14, 7);
                            int vel = BitHelpers.ExtractBits(e.Command, 7, 0);
                            //bool hasVelocity = BitHelpers.ExtractBits(e.Command, 1, 7) == 1;
                            //builder.Data1 = GetMidiPitch(freq, 349f);
                            builder.Data1 = GetMidiPitch(freq, 699);
                            //builder.Data2 = UnityEngine.Mathf.RoundToInt(127f * (vel / 7f));
                            //float velf = ((vel + 1) / 128f); // hack
                            /*float velf = (vel / 127f);
                            int veli = Mathf.RoundToInt(velf * 127f);*/
                            int veli = vel;
                            /*if (!hasVelocity) {
                                veli = 127;
                            }*/
                            /*if (PercussionInstruments[instrument] != Percussion.None) {
                                builder.Data1 = (int)PercussionInstruments[instrument];
                                builder.Data2 = PercussionInstruments[instrument] == Percussion.None ? 0 : veli;
                                curNoteOnChannel[channel] = builder.Data1 + 128;
                            } else {
                                builder.Data2 = GeneralMidiInstruments[instrument] == Instrument.None ? 0 : veli;
                                curNoteOnChannel[channel] = builder.Data1;
                            }*/
                            if (Patches[instrument].GetType() == typeof(Percussion)) {
                                builder.Data1 = (int)(Percussion)Patches[instrument];
                                builder.Data2 = (Percussion)Patches[instrument] == Percussion.None ? 0 : veli;
                                curNoteOnChannel[channel] = builder.Data1 + 128;
                            } else {
                                builder.Data2 = (Instrument)Patches[instrument] == Instrument.None ? 0 : veli;
                                curNoteOnChannel[channel] = builder.Data1;
                            }

                            builder.Build();
                            t.Insert(e.Time / timeScale, builder.Result);
                        } else {
                            builder.Command = ChannelCommand.NoteOff;
                            int note = curNoteOnChannel.TryGetValue(channel, out int val) ? val : 0;
                            if (note >= 128) {
                                builder.MidiChannel = 9;
                                builder.Data1 = note - 128;
                            } else {
                                builder.MidiChannel = channel;
                                builder.Data1 = note;
                            }
                            builder.Data2 = 127;
                            curNoteOnChannel.Remove(channel);

                            builder.Build();
                            t.Insert(e.Time / timeScale, builder.Result);
                        }
                    }
                }
            }
            return t;
        }

        private static Enum[] Patches = new Enum[] {
            Instrument.AcousticBass,
            Instrument.Clarinet,
            Instrument.TaikoDrum, // Kick2
            Instrument.AcousticGrandPiano,
            Percussion.ElectricSnare,
            Instrument.Woodblock, // cng1
            Percussion.OpenHiConga, // perkcng2
			Instrument.Woodblock, // cng3
            Instrument.FretlessBass,
            Instrument.ElectricPiano1,
            Percussion.PedalHiHat, // hihato2
            Percussion.ClosedHiHat,
            Instrument.PercussiveOrgan,
            Instrument.OrchestralHarp, // acnlngtr //Instrument.AcousticGuitarNylon,
            Instrument.RockOrgan,
            Instrument.Xylophone, //116fa103
            Instrument.Kalimba,
            Instrument.Contrabass,
            Instrument.Vibraphone,
            Instrument.Oboe,
            Instrument.PizzicatoStrings,
            Instrument.DistortionGuitar,
            Instrument.SynthBrass1,
            Instrument.Vibraphone, // vibe
            Instrument.Cello,
            Instrument.SopranoSax,
            Instrument.Sitar,
            Instrument.Xylophone,
            Instrument.MelodicTom,
            Instrument.TaikoDrum, // hulotte
        };
        private int GetMidiPitch(int freq, float tuning = 440f) {
			// See https://anotherproducer.com/online-tools-for-musicians/frequency-to-pitch-calculator/
			// default pitch = F5, so 699
			return Mathf.RoundToInt(69 + 12 * Mathf.Log(freq / tuning, 2f));

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
