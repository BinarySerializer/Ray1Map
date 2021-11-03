using System.Collections.Generic;
using BinarySerializer;

namespace Ray1Map.GBARRR
{
    public class GAX2_Pattern : BinarySerializable {
        public ushort Duration { get; set; }
        public GAX2_PatternRow[] Rows { get; set; }
        public Pointer EndOffset { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            if (Rows == null) {
                List<GAX2_PatternRow> rows = new List<GAX2_PatternRow>();
                bool isEndOfTrack = false;
                int curDuration = 0;
                while (!isEndOfTrack) {
                    GAX2_PatternRow row = s.SerializeObject<GAX2_PatternRow>(default, name: $"{nameof(Rows)}[{rows.Count}]");
                    rows.Add(row);
                    curDuration += row.Duration;
                    if (row.Command == GAX2_PatternRow.Cmd.EmptyTrack || curDuration >= Duration) {
                        isEndOfTrack = true;
                        EndOffset = s.CurrentPointer;
                        s.Log($"GAX2 Track Duration: {curDuration} - Last Command: {row.Command}");
                    }
                }
                Rows = rows.ToArray();
            }
        }
    }
}