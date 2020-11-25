using System;
using System.Collections.Generic;

namespace R1Engine
{
    public class LUDI_OffsetTable : LUDI_Block {
        public uint NumEntries { get; set; }
        public Entry[] Entries { get; set; }

        private Dictionary<ushort, Entry> _entriesDictionary { get; set; }
        public Dictionary<ushort, Entry> EntriesDictionary {
            get {
                if (_entriesDictionary == null) {
                    _entriesDictionary = new Dictionary<ushort, Entry>();
                    foreach (var e in Entries) {
                        _entriesDictionary[e.BlockID] = e;
                    }
                }
                return _entriesDictionary;
            }
        }

		public override void SerializeBlock(SerializerObject s) {
		    NumEntries = s.Serialize<uint>(NumEntries, name: nameof(NumEntries));
            Entries = s.SerializeObjectArray<Entry>(Entries, NumEntries, name: nameof(Entries));
        }

		public class Entry : R1Serializable {
            public ushort BlockID { get; set; }
            public ushort Padding { get; set; }

            public uint RecordID { get; set; } // Palm OS
            public uint BlockOffset { get; set; } // Pocket PC

            public override void SerializeImpl(SerializerObject s) {
                BlockID = s.Serialize<ushort>(BlockID, name: nameof(BlockID));
                Padding = s.Serialize<ushort>(Padding, name: nameof(Padding));

                if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1_Palm) {
                    RecordID = s.Serialize<uint>(RecordID, name: nameof(RecordID));
                } else if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1_PocketPC) {
                    BlockOffset = s.Serialize<uint>(BlockOffset, name: nameof(BlockOffset));
                }
            }
		}
	}
}