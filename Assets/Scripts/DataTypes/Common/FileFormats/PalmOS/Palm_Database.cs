using System.Collections.Generic;
using BinarySerializer;

namespace R1Engine
{
    // Documentation: 
    // https://web.archive.org/web/20090315213538/http://membres.lycos.fr/microfirst/palm/pdb.html
    // http://web.mit.edu/tytso/www/pilot/prc-format.html
    // https://en.wikipedia.org/wiki/PRC_(Palm_OS)

    /// <summary>
    /// Base data for a Palm database
    /// </summary>
    public class Palm_Database : BinarySerializable
    {
        public DatabaseType Type { get; set; } // Set before serializing

        public string Name { get; set; }
        public ushort Attributes { get; set; }
        public ushort Version { get; set; }

        public Palm_DateTime CreationTime { get; set; }
        public Palm_DateTime ModificationTime { get; set; }
        public Palm_DateTime BackupTime { get; set; }

        public uint ModificationNumber { get; set; }
        public Pointer AppInfoAreaPointer { get; set; }
        public Pointer SortInfoAreaPointer { get; set; }

        public string DataBaseType { get; set; }
        public string CreatorID { get; set; }
        public uint UniqueID { get; set; }

        public uint NextRecordListID { get; set; } // Runtime only
        public ushort RecordsCount { get; set; }

        public Palm_DatabaseRecord[] Records { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Name = s.SerializeString(Name, 32, name: nameof(Name));
            Attributes = s.Serialize<ushort>(Attributes, name: nameof(Attributes));
            Version = s.Serialize<ushort>(Version, name: nameof(Version));

            CreationTime = s.SerializeObject<Palm_DateTime>(CreationTime, name: nameof(CreationTime));
            ModificationTime = s.SerializeObject<Palm_DateTime>(ModificationTime, name: nameof(ModificationTime));
            BackupTime = s.SerializeObject<Palm_DateTime>(BackupTime, name: nameof(BackupTime));

            ModificationNumber = s.Serialize<uint>(ModificationNumber, name: nameof(ModificationNumber));
            AppInfoAreaPointer = s.SerializePointer(AppInfoAreaPointer, name: nameof(AppInfoAreaPointer));
            SortInfoAreaPointer = s.SerializePointer(SortInfoAreaPointer, name: nameof(SortInfoAreaPointer));

            DataBaseType = s.SerializeString(DataBaseType, 4, name: nameof(DataBaseType));
            CreatorID = s.SerializeString(CreatorID, 4, name: nameof(CreatorID));
            UniqueID = s.Serialize<uint>(UniqueID, name: nameof(UniqueID));

            NextRecordListID = s.Serialize<uint>(NextRecordListID, name: nameof(NextRecordListID));
            RecordsCount = s.Serialize<ushort>(RecordsCount, name: nameof(RecordsCount));

            Records = s.SerializeObjectArray<Palm_DatabaseRecord>(Records, RecordsCount, onPreSerialize: x => x.Type = Type, name: nameof(Records));

            // Set the length of every record
            for (int i = 0; i < RecordsCount; i++)
                Records[i].Length = (i == RecordsCount - 1 ? s.CurrentLength : Records[i + 1].DataPointer.FileOffset) - Records[i].DataPointer.FileOffset;

            // TODO: Serialize sort info
        }

        public enum DatabaseType
        {
            PRC, // Palm Resource Code,
            PDB, // Pilot Database
        }


        private Dictionary<uint, Palm_DatabaseRecord> _recordsDictionary { get; set; }
        public Dictionary<uint, Palm_DatabaseRecord> RecordsDictionary {
            get {
                if (_recordsDictionary == null) {
                    _recordsDictionary = new Dictionary<uint, Palm_DatabaseRecord>();
                    foreach (var e in Records) {
                        _recordsDictionary[e.UniqueID] = e;
                    }
                }
                return _recordsDictionary;
            }
        }
    }
}