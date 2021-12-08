using System.Collections.Generic;
using BinarySerializer;

namespace Ray1Map.GBARRR
{
    public class GAX2_Pattern : BinarySerializable {
        public ushort Duration { get; set; }
        public bool IsEmptyTrack { get; set; }
        public GAX2_PatternRow[] Rows { get; set; }
        public Pointer EndOffset { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            IsEmptyTrack = s.Serialize<bool>(IsEmptyTrack, name: nameof(IsEmptyTrack));
            if (Rows == null) {
                List<GAX2_PatternRow> rows = new List<GAX2_PatternRow>();
                bool isEndOfTrack = IsEmptyTrack;
                int curDuration = 0;
                while (!isEndOfTrack) {
                    GAX2_PatternRow row = s.SerializeObject<GAX2_PatternRow>(default, name: $"{nameof(Rows)}[{rows.Count}]");
                    rows.Add(row);
                    curDuration += row.Duration;
                    if (curDuration >= Duration) {
                        isEndOfTrack = true;
                        EndOffset = s.CurrentPointer;
                        s.Log($"GAX2 Track Duration: {curDuration} - Last Command: {row.Command}");
                    }
                }
                Rows = rows.ToArray();
            } else {
				Rows = s.SerializeObjectArray<GAX2_PatternRow>(Rows, Rows.Length, name: nameof(Rows));
			}
        }
    }
}