using System.Collections.Generic;
using BinarySerializer;

namespace Ray1Map.GBC
{
    public class LUDI_OffsetTable : LUDI_AppInfoBlock {
        public uint NumEntries { get; set; }
        public Entry[] Entries { get; set; }

        private Dictionary<ushort, int> _entriesDictionary { get; set; }
        public Dictionary<ushort, int> EntriesDictionary {
            get {
                if (_entriesDictionary == null) {
                    _entriesDictionary = new Dictionary<ushort, int>();
                    for (int i = 0; i < Entries.Length; i++) {
                        _entriesDictionary[Entries[i].BlockID] = i;
                    }
                }
                return _entriesDictionary;
            }
        }

		public override void SerializeBlock(SerializerObject s) {
		    NumEntries = s.Serialize<uint>(NumEntries, name: nameof(NumEntries));
            Entries = s.SerializeObjectArray<Entry>(Entries, NumEntries, name: nameof(Entries));
        }

		public class Entry : BinarySerializable {
            public ushort BlockID { get; set; }
            public ushort Padding { get; set; }

            public uint RecordID { get; set; } // Palm OS
            public uint BlockOffset { get; set; } // Pocket PC

            public override void SerializeImpl(SerializerObject s) {
                BlockID = s.Serialize<ushort>(BlockID, name: nameof(BlockID));
                Padding = s.Serialize<ushort>(Padding, name: nameof(Padding));

                if (s.GetR1Settings().EngineVersion == EngineVersion.GBC_R1_Palm) {
                    RecordID = s.Serialize<uint>(RecordID, name: nameof(RecordID));
                } else if (s.GetR1Settings().EngineVersion == EngineVersion.GBC_R1_PocketPC) {
                    BlockOffset = s.Serialize<uint>(BlockOffset, name: nameof(BlockOffset));
                }
            }
		}
	}
}