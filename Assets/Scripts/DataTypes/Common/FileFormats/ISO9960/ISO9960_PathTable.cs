using Boo.Lang;
using System.Linq;

namespace R1Engine
{
    // See: https://wiki.osdev.org/ISO_9660
    public class ISO9960_PathTable : R1Serializable {
        public Entry[] Entries { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            if (Entries == null) {
                List<Entry> entries = new List<Entry>();
                while (entries.Count == 0 || entries.LastOrDefault().DirectoryIdentifierLength != 0) {
                    entries.Add(s.SerializeObject<Entry>(null, name: $"{nameof(Entries)}[{entries.Count}]"));
                }
                entries.RemoveAt(entries.Count-1);
                Entries = entries.ToArray();
            }
        }

        public class Entry : R1Serializable {
            public byte DirectoryIdentifierLength { get; set; }
            public byte ExtendedAttributeRecordLength { get; set; }
            public uint ExtentLBA { get; set; }
            public ushort ParentDirectoryIndex { get; set; }
            public string DirectoryIdentifier { get; set; }
            public byte Padding { get; set; }


            public override void SerializeImpl(SerializerObject s) {
                DirectoryIdentifierLength = s.Serialize<byte>(DirectoryIdentifierLength, name: nameof(DirectoryIdentifierLength));
                if (DirectoryIdentifierLength == 0) return;
                ExtendedAttributeRecordLength = s.Serialize<byte>(ExtendedAttributeRecordLength, name: nameof(ExtendedAttributeRecordLength));
                ExtentLBA = s.Serialize<uint>(ExtentLBA, name: nameof(ExtentLBA));
                ParentDirectoryIndex = s.Serialize<ushort>(ParentDirectoryIndex, name: nameof(ParentDirectoryIndex));
                DirectoryIdentifier = s.SerializeString(DirectoryIdentifier, length: DirectoryIdentifierLength, name: nameof(DirectoryIdentifier));
                if (DirectoryIdentifierLength % 2 != 0) {
                    Padding = s.Serialize<byte>(Padding, name: nameof(Padding));
                }
            }
        }
    }
}