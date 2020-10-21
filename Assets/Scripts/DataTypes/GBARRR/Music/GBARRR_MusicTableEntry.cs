using System;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBARRR_MusicTableEntry : R1Serializable
    {
        public ushort NumTracks { get; set; }
        public ushort TrackLength { get; set; }
        public uint NumPieces { get; set; }
        public uint UInt_08 { get; set; }
        public Pointer MusicData { get; set; }
        public Pointer InstrumentDescriptionTable { get; set; }
        public Pointer InstrumentSampleTable { get; set; }
        public uint UInt_18 { get; set; }
        public uint UInt_1C { get; set; }
        public Pointer[] TrackOffsetTables { get; set; }
        public string Name { get; set; }
        public string ParsedName { get; set; }
        public string ParsedArtist { get; set; }

        public int[][] TrackOffsets { get; set; }
        public GAX2_MusicTrack[][] Tracks { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            NumTracks = s.Serialize<ushort>(NumTracks, name: nameof(NumTracks));
            TrackLength = s.Serialize<ushort>(TrackLength, name: nameof(TrackLength)); // In frames
            NumPieces = s.Serialize<uint>(NumPieces, name: nameof(NumPieces));
            UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
            MusicData = s.SerializePointer(MusicData, name: nameof(MusicData));
            InstrumentDescriptionTable = s.SerializePointer(InstrumentDescriptionTable, name: nameof(InstrumentDescriptionTable));
            InstrumentSampleTable = s.SerializePointer(InstrumentSampleTable, name: nameof(InstrumentSampleTable));
            UInt_18 = s.Serialize<uint>(UInt_18, name: nameof(UInt_18));
            UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));
            TrackOffsetTables = s.SerializePointerArray(TrackOffsetTables, NumTracks, name: nameof(TrackOffsetTables));

            if (TrackOffsets == null) {
                TrackOffsets = new int[TrackOffsetTables.Length][];
                Tracks = new GAX2_MusicTrack[TrackOffsetTables.Length][];
                for (int i = 0; i < TrackOffsetTables.Length; i++) {
                    s.DoAt(TrackOffsetTables[i], () => {
                        TrackOffsets[i] = s.SerializeArray<int>(TrackOffsets[i], NumPieces, name: $"{nameof(TrackOffsets)}[{i}]");
                        if (Tracks[i] == null) {
                            Tracks[i] = new GAX2_MusicTrack[TrackOffsets[i].Length];
                            for (int j = 0; j < Tracks[i].Length; j++) {
                                s.DoAt(MusicData + TrackOffsets[i][j], () => {
                                    Tracks[i][j] = s.SerializeObject<GAX2_MusicTrack>(Tracks[i][j], onPreSerialize: t => t.Duration = TrackLength, name: $"{nameof(Tracks)}[{i}][{j}]");
                                });
                            }
                        }
                    });
                }
                Pointer endOffset = Tracks.Max(ta => ta.Max(t => t.EndOffset));
                s.DoAt(endOffset, () => {
                    Name = s.Serialize<string>(Name, name: nameof(Name));
                    string[] parse = Name.Split('"');
                    ParsedName = parse[1];
                    ParsedArtist = parse[2].Substring(3, 0xF);
                    s.Log(ParsedName + " - " + ParsedArtist);
                });
            }
        }
    }
}