using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    // See: https://wiki.osdev.org/ISO_9660
    public class ISO9960_Directory : R1Serializable {
        public ISO9960_DirectoryRecord[] Entries { get; set; }
        // Note: Every directory will start with 2 special entries: an empty string, describing the "." entry, and the string "\1" describing the ".." entry.

        public override void SerializeImpl(SerializerObject s) {
            if (Entries == null) {
                List<ISO9960_DirectoryRecord> entries = new List<ISO9960_DirectoryRecord>();
                while (((s.CurrentPointer - Offset) < ISO9960_BinFile.SectorDataSize) && (entries.Count == 0 || entries.LastOrDefault().Length != 0)) {
                    entries.Add(s.SerializeObject<ISO9960_DirectoryRecord>(null, name: $"{nameof(Entries)}[{entries.Count}]"));
                }
                if (entries.LastOrDefault().Length == 0) {
                    entries.RemoveAt(entries.Count - 1);
                }
                Entries = entries.ToArray();
            }
        }
    }
}