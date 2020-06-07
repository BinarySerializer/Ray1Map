using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace R1Engine {
	public class MidiWriter {
		public void Write(Jaguar_R1_MusicFile file, string outPath) {
			Sequence s = new Sequence();
			Track t = new Track();
			s.Add(CreateTrack(file));
			s.Save(outPath);
		}

		private Track CreateTrack(Jaguar_R1_MusicFile jagFile) {
			Track t = new Track();
			TempoChangeBuilder b = new TempoChangeBuilder();
			b.Tempo = 22500;
			b.Build();
			t.Insert(0, b.Result);
			ChannelMessageBuilder builder = new ChannelMessageBuilder();
			int timeScale = 1;
			for (int i = 0; i < jagFile.Entries.Length; i++) {
				Jaguar_R1_MusicFileEntry e = jagFile.Entries[i];
				if (e.Time != int.MaxValue) {
					int channelByte = BitHelpers.ExtractBits(e.Command, 8, 24);
					if (channelByte == 0x7F) { // special point in the song
						int idByte = BitHelpers.ExtractBits(e.Command, 8, 0);
						if (idByte == 0xFF) { // End point
							t.EndOfTrackOffset = e.Time / timeScale;
							break;
						} else {
							// Loop point, or command to return to loop point
						}
					} else {
						int channel = BitHelpers.ExtractBits(e.Command, 6, 25);
						builder.MidiChannel = channel;
						int command = BitHelpers.ExtractBits(e.Command, 1, 31);
						if (command == 1) {
							builder.Command = ChannelCommand.NoteOn;
							int freq = BitHelpers.ExtractBits(e.Command, 13, 8);
							int vel = BitHelpers.ExtractBits(e.Command, 8, 0);
							builder.Data1 = GetMidiPitch(freq, 699f);
							//builder.Data2 = UnityEngine.Mathf.RoundToInt(127f * (vel / 7f));
							float velf = ((vel + 1) / 256f); // hack
							builder.Data2 = Mathf.RoundToInt((0.75f + velf / 4f) * 127f);
							//builder.Data2 = 127;
						} else {
							builder.Command = ChannelCommand.NoteOff;
						}
						builder.Build();
						t.Insert(e.Time / timeScale, builder.Result);
					}
				}
			}
			return t;
		}

		private int GetMidiPitch(int freq, float tuning = 440f) {
			// See https://anotherproducer.com/online-tools-for-musicians/frequency-to-pitch-calculator/
			// default pitch = F5, so 699
			return Mathf.RoundToInt(69 + 12 * Mathf.Log(freq / tuning, 2f));

		}
	}
}
