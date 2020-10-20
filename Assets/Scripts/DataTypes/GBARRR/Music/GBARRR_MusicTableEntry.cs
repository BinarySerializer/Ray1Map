using System;
using UnityEngine;

namespace R1Engine
{
    public class GBARRR_MusicTableEntry : R1Serializable
    {
        public ushort NumMarkerTables { get; set; }
        public ushort UShort_02 { get; set; }
        public uint NumMarkers { get; set; }
        public uint UInt_08 { get; set; }
        public Pointer MusicData { get; set; }
        public Pointer GlobalOffset_10 { get; set; }
        public Pointer SampleTable { get; set; }
        public uint UInt_18 { get; set; }
        public uint UInt_1C { get; set; }
        public Pointer[] MarkerTables { get; set; }

        public int[][] Markers { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            NumMarkerTables = s.Serialize<ushort>(NumMarkerTables, name: nameof(NumMarkerTables));
            UShort_02 = s.Serialize<ushort>(UShort_02, name: nameof(UShort_02));
            NumMarkers = s.Serialize<uint>(NumMarkers, name: nameof(NumMarkers));
            UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
            MusicData = s.SerializePointer(MusicData, name: nameof(MusicData));
            GlobalOffset_10 = s.SerializePointer(GlobalOffset_10, name: nameof(GlobalOffset_10));
            SampleTable = s.SerializePointer(SampleTable, name: nameof(SampleTable));
            UInt_18 = s.Serialize<uint>(UInt_18, name: nameof(UInt_18));
            UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));
            MarkerTables = s.SerializePointerArray(MarkerTables, NumMarkerTables, name: nameof(MarkerTables));

            if (Markers == null) {
                Markers = new int[MarkerTables.Length][];
                for (int i = 0; i < MarkerTables.Length; i++) {
                    s.DoAt(MarkerTables[i], () => {
                        Markers[i] = s.SerializeArray<int>(Markers[i], NumMarkers, name: $"{nameof(Markers)}[{i}]");
                    });
                }
            }
        }
    }
}