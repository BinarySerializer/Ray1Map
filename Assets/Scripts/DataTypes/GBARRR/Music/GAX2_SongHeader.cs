using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GAX2_SongHeader : R1Serializable
    {
        public ushort NumChannels { get; set; }
        public ushort TrackLength { get; set; }
        public ushort NumPatterns { get; set; }
        public ushort UShort_06 { get; set; }
        public ushort Volume { get; set; }
        public ushort UShort_0A { get; set; }
        public Pointer SequenceDataPointer { get; set; }
        public Pointer InstrumentSetPointer { get; set; }
        public Pointer SampleSetPointer { get; set; }
        public ushort UShort_18 { get; set; }
        public ushort UShort_1A { get; set; }
        public ushort UShort_1C { get; set; }
        public ushort UShort_1E { get; set; }
        public Pointer[] PatternTablePointers { get; set; }
        public uint[] Reserved { get; set; }
        public string Name { get; set; }
        public string ParsedName { get; set; }
        public string ParsedArtist { get; set; }

        public GAX2_PatternEntry[][] PatternTable { get; set; }
        public GAX2_MusicTrack[][] Tracks { get; set; }
        public Pointer<GAX2_Instrument>[] InstrumentSet { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            NumChannels = s.Serialize<ushort>(NumChannels, name: nameof(NumChannels));
            TrackLength = s.Serialize<ushort>(TrackLength, name: nameof(TrackLength)); // In frames
            NumPatterns = s.Serialize<ushort>(NumPatterns, name: nameof(NumPatterns));
            UShort_06 = s.Serialize<ushort>(UShort_06, name: nameof(UShort_06));
            Volume = s.Serialize<ushort>(Volume, name: nameof(Volume));
            UShort_0A = s.Serialize<ushort>(UShort_0A, name: nameof(UShort_0A));
            SequenceDataPointer = s.SerializePointer(SequenceDataPointer, name: nameof(SequenceDataPointer));
            InstrumentSetPointer = s.SerializePointer(InstrumentSetPointer, name: nameof(InstrumentSetPointer));
            SampleSetPointer = s.SerializePointer(SampleSetPointer, name: nameof(SampleSetPointer));
            UShort_18 = s.Serialize<ushort>(UShort_18, name: nameof(UShort_18));
            UShort_1A = s.Serialize<ushort>(UShort_1A, name: nameof(UShort_1A));
            UShort_1C = s.Serialize<ushort>(UShort_1C, name: nameof(UShort_1C));
            UShort_1E = s.Serialize<ushort>(UShort_1E, name: nameof(UShort_1E));
            PatternTablePointers = s.SerializePointerArray(PatternTablePointers, NumChannels, name: nameof(PatternTablePointers));
            Reserved = s.SerializeArray<uint>(Reserved, NumChannels, name: nameof(Reserved));

            List<int> instruments = new List<int>();
            if (PatternTable == null) {
                int instrumentCount = 0;
                PatternTable = new GAX2_PatternEntry[PatternTablePointers.Length][];
                Tracks = new GAX2_MusicTrack[PatternTablePointers.Length][];
                for (int i = 0; i < PatternTablePointers.Length; i++) {
                    s.DoAt(PatternTablePointers[i], () => {
                        PatternTable[i] = s.SerializeObjectArray<GAX2_PatternEntry>(PatternTable[i], NumPatterns, name: $"{nameof(PatternTable)}[{i}]");
                        if (Tracks[i] == null) {
                            Tracks[i] = new GAX2_MusicTrack[PatternTable[i].Length];
                            for (int j = 0; j < Tracks[i].Length; j++) {
                                s.DoAt(SequenceDataPointer + PatternTable[i][j].SequenceOffset, () => {
                                    Tracks[i][j] = s.SerializeObject<GAX2_MusicTrack>(Tracks[i][j], onPreSerialize: t => t.Duration = TrackLength, name: $"{nameof(Tracks)}[{i}][{j}]");
                                    instrumentCount = Math.Max(instrumentCount, Tracks[i][j].Commands.Max(cmd => cmd.Command == GAX2_MusicCommand.Cmd.Note ? cmd.Instrument + 1 : 0));
                                    instruments.AddRange(Tracks[i][j].Commands.Where(cmd => cmd.Command == GAX2_MusicCommand.Cmd.Note).Select(cmd => (int)cmd.Instrument));
                                });
                            }
                        }
                    });
                }
                instruments = instruments.Distinct().ToList();
                s.Log("Instrument Count: " + instruments.Count);
                Pointer endOffset = Tracks.Max(ta => ta.Max(t => t.EndOffset));
                s.DoAt(endOffset, () => {
                    Name = s.Serialize<string>(Name, name: nameof(Name));
                    string[] parse = Name.Split('"');
                    ParsedName = parse[1];
                    ParsedArtist = parse[2].Substring(3, 0xF);
                    s.Log(ParsedName + " - " + ParsedArtist);
                });
                s.DoAt(InstrumentSetPointer, () => {
                    InstrumentSet = s.SerializePointerArray<GAX2_Instrument>(InstrumentSet, instrumentCount, resolve: true, name: nameof(InstrumentSet));
                });
            }
        }
    }
}