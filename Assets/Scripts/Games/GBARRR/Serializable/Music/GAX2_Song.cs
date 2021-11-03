using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBARRR
{
    public class GAX2_Song : BinarySerializable
    {
        public int? PredefinedSampleCount { get; set; } // Set in onPreSerialize

        public ushort NumChannels { get; set; }
        public ushort NumRowsPerPattern { get; set; }
        public ushort NumPatternsPerChannel { get; set; }
        public ushort UShort_06 { get; set; }
        public ushort Volume { get; set; }
        public ushort UShort_0A { get; set; }
        public Pointer SequenceDataPointer { get; set; }
        public Pointer InstrumentSetPointer { get; set; }
        public Pointer SampleSetPointer { get; set; }
        public ushort SampleRate { get; set; } // 0x3D99
        public ushort UShort_1A { get; set; }
        public ushort UShort_1C { get; set; }
        public ushort UShort_1E { get; set; }
        public Pointer[] PatternTablePointers { get; set; }
        public uint[] Reserved { get; set; }
        public string Name { get; set; }
        public string ParsedName { get; set; }
        public string ParsedArtist { get; set; }

        public GAX2_PatternHeader[][] PatternTable { get; set; }
        public GAX2_Pattern[][] Patterns { get; set; }
        public Pointer<GAX2_Instrument>[] InstrumentSet { get; set; }
        public int[] InstrumentIndices { get; set; }
        public GAX2_Sample[] Samples { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            NumChannels = s.Serialize<ushort>(NumChannels, name: nameof(NumChannels));
            NumRowsPerPattern = s.Serialize<ushort>(NumRowsPerPattern, name: nameof(NumRowsPerPattern));
            NumPatternsPerChannel = s.Serialize<ushort>(NumPatternsPerChannel, name: nameof(NumPatternsPerChannel));
            UShort_06 = s.Serialize<ushort>(UShort_06, name: nameof(UShort_06));
            Volume = s.Serialize<ushort>(Volume, name: nameof(Volume));
            UShort_0A = s.Serialize<ushort>(UShort_0A, name: nameof(UShort_0A));
            SequenceDataPointer = s.SerializePointer(SequenceDataPointer, name: nameof(SequenceDataPointer));
            InstrumentSetPointer = s.SerializePointer(InstrumentSetPointer, name: nameof(InstrumentSetPointer));
            SampleSetPointer = s.SerializePointer(SampleSetPointer, name: nameof(SampleSetPointer));
            SampleRate = s.Serialize<ushort>(SampleRate, name: nameof(SampleRate));
            UShort_1A = s.Serialize<ushort>(UShort_1A, name: nameof(UShort_1A));
            UShort_1C = s.Serialize<ushort>(UShort_1C, name: nameof(UShort_1C));
            UShort_1E = s.Serialize<ushort>(UShort_1E, name: nameof(UShort_1E));
            PatternTablePointers = s.SerializePointerArray(PatternTablePointers, NumChannels, name: nameof(PatternTablePointers));
            Reserved = s.SerializeArray<uint>(Reserved, NumChannels, name: nameof(Reserved));

            List<int> instruments = new List<int>();
            if (PatternTable == null) {
                int instrumentCount = 0;
                PatternTable = new GAX2_PatternHeader[PatternTablePointers.Length][];
                Patterns = new GAX2_Pattern[PatternTablePointers.Length][];
                for (int i = 0; i < PatternTablePointers.Length; i++) {
                    s.DoAt(PatternTablePointers[i], () => {
                        PatternTable[i] = s.SerializeObjectArray<GAX2_PatternHeader>(PatternTable[i], NumPatternsPerChannel, name: $"{nameof(PatternTable)}[{i}]");
                        if (Patterns[i] == null) {
                            Patterns[i] = new GAX2_Pattern[PatternTable[i].Length];
                            for (int j = 0; j < Patterns[i].Length; j++) {
                                s.DoAt(SequenceDataPointer + PatternTable[i][j].SequenceOffset, () => {
                                    Patterns[i][j] = s.SerializeObject<GAX2_Pattern>(Patterns[i][j], onPreSerialize: t => t.Duration = NumRowsPerPattern, name: $"{nameof(Patterns)}[{i}][{j}]");
                                    instrumentCount = Math.Max(instrumentCount, Patterns[i][j].Rows
                                        .Max(cmd => (cmd.Command == GAX2_PatternRow.Cmd.Note || cmd.Command == GAX2_PatternRow.Cmd.NoteCompressed) ? cmd.Instrument + 1 : 0));
                                    instruments.AddRange(Patterns[i][j].Rows
                                        .Where(cmd => cmd.Command == GAX2_PatternRow.Cmd.Note || cmd.Command == GAX2_PatternRow.Cmd.NoteCompressed)
                                        .Select(cmd => (int)cmd.Instrument));
                                });
                            }
                        }
                    });
                }
                InstrumentIndices = instruments.Distinct().Where(i => i != 250).ToArray();
                s.Log("Instrument Count: " + InstrumentIndices.Length);
                Pointer endOffset = Patterns.Max(ta => ta.Max(t => t.EndOffset));
                s.DoAt(endOffset, () => {
                    Name = s.Serialize<string>(Name, name: nameof(Name));
                    string[] parse = Name.Split('"');
                    ParsedName = parse[1];
                    ParsedArtist = parse[2].Substring(3, 0xF);
                    s.Log(ParsedName + " - " + ParsedArtist);
                });
                s.DoAt(InstrumentSetPointer, () => {
                    InstrumentSet = s.SerializePointerArray<GAX2_Instrument>(InstrumentSet, PredefinedSampleCount ?? instrumentCount, resolve: true, name: nameof(InstrumentSet));
                });
                Samples = new GAX2_Sample[PredefinedSampleCount ?? InstrumentIndices.Length];
                for (int i = 0; i < Samples.Length; i++) {
                    int ind = PredefinedSampleCount.HasValue ? i : InstrumentIndices[i];
                    var instr = InstrumentSet[ind].Value;
                    if (instr != null) {
                        s.DoAt(SampleSetPointer + (instr.Sample) * 8, () => {
                            Samples[i] = s.SerializeObject<GAX2_Sample>(Samples[i], name: $"{nameof(Samples)}[{i}]");
                        });
                    }
                }
            }
        }
    }
}