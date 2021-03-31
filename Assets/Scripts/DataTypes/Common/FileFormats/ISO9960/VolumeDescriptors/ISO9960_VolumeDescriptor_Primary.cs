using BinarySerializer;

namespace R1Engine
{
    // See: https://wiki.osdev.org/ISO_9660
    public class ISO9960_VolumeDescriptor_Primary : ISO9960_VolumeDescriptor
    {
        public byte[] Unused { get; set; }
        public byte[] BigEndianValue { get; set; }

        public string SystemIdentifier { get; set; }
        public string VolumeIdentifier { get; set; }
        public uint VolumeSpaceSize { get; set; }
        public ushort VolumeSetSize { get; set; }
        public ushort VolumeSequenceNumber { get; set; }
        public ushort LogicalBlockSize { get; set; }
        public uint PathTableSize { get; set; }
        public uint PathTableLBA { get; set; }
        public uint OptionalPathTableLBA { get; set; }
        public ISO9960_DirectoryRecord Root { get; set; }

        public string VolumeSetIdentifier { get; set; }
        public string PublisherIdentifier { get; set; }
        public string DataPreparerIdentifier { get; set; }
        public string ApplicationIdentifier { get; set; }
        public string CopyrightFileIdentifier { get; set; }
        public string AbstractFileIdentifier { get; set; }
        public string BibliographicFileIdentifier { get; set; }

        public string VolumeCreationDateTime { get; set; }
        public string VolumeModificationDateTime { get; set; }
        public string VolumeExpirationDateTime { get; set; }
        public string VolumeEffectiveDateTime { get; set; }

        public byte FileStructureVersion { get; set; }


        public override void SerializeImpl(SerializerObject s) {
            base.SerializeImpl(s);
            Unused = s.SerializeArray<byte>(Unused, 1, nameof(Unused));
            SystemIdentifier = s.SerializeString(SystemIdentifier, length: 0x20, name: nameof(SystemIdentifier));
            VolumeIdentifier = s.SerializeString(VolumeIdentifier, length: 0x20, name: nameof(VolumeIdentifier));
            Unused = s.SerializeArray<byte>(Unused, 8, nameof(Unused));

            VolumeSpaceSize = s.Serialize<uint>(VolumeSpaceSize, name: nameof(VolumeSpaceSize));
            BigEndianValue = s.SerializeArray<byte>(BigEndianValue, 4, name: nameof(BigEndianValue));
            Unused = s.SerializeArray<byte>(Unused, 0x20, nameof(Unused));

            VolumeSetSize = s.Serialize<ushort>(VolumeSetSize, name: nameof(VolumeSetSize));
            BigEndianValue = s.SerializeArray<byte>(BigEndianValue, 2, name: nameof(BigEndianValue));

            VolumeSequenceNumber = s.Serialize<ushort>(VolumeSequenceNumber, name: nameof(VolumeSequenceNumber));
            BigEndianValue = s.SerializeArray<byte>(BigEndianValue, 2, name: nameof(BigEndianValue));

            LogicalBlockSize = s.Serialize<ushort>(LogicalBlockSize, name: nameof(LogicalBlockSize));
            BigEndianValue = s.SerializeArray<byte>(BigEndianValue, 2, name: nameof(BigEndianValue));

            PathTableSize = s.Serialize<uint>(PathTableSize, name: nameof(PathTableSize));
            BigEndianValue = s.SerializeArray<byte>(BigEndianValue, 4, name: nameof(BigEndianValue));

            PathTableLBA = s.Serialize<uint>(PathTableLBA, name: nameof(PathTableLBA));
            OptionalPathTableLBA = s.Serialize<uint>(OptionalPathTableLBA, name: nameof(OptionalPathTableLBA));
            BigEndianValue = s.SerializeArray<byte>(BigEndianValue, 4, name: nameof(BigEndianValue));
            BigEndianValue = s.SerializeArray<byte>(BigEndianValue, 4, name: nameof(BigEndianValue));

            Root = s.SerializeObject<ISO9960_DirectoryRecord>(Root, name: nameof(Root));

            VolumeSetIdentifier = s.SerializeString(VolumeSetIdentifier, 128, name: nameof(VolumeSetIdentifier));
            PublisherIdentifier = s.SerializeString(PublisherIdentifier, 128, name: nameof(PublisherIdentifier));
            DataPreparerIdentifier = s.SerializeString(DataPreparerIdentifier, 128, name: nameof(DataPreparerIdentifier));
            ApplicationIdentifier = s.SerializeString(ApplicationIdentifier, 128, name: nameof(ApplicationIdentifier));
            CopyrightFileIdentifier = s.SerializeString(CopyrightFileIdentifier, 38, name: nameof(CopyrightFileIdentifier));
            AbstractFileIdentifier = s.SerializeString(AbstractFileIdentifier, 36, name: nameof(AbstractFileIdentifier));
            BibliographicFileIdentifier = s.SerializeString(BibliographicFileIdentifier, 37, name: nameof(BibliographicFileIdentifier));

            VolumeCreationDateTime = s.SerializeString(VolumeCreationDateTime, 17, name: nameof(VolumeCreationDateTime));
            VolumeModificationDateTime = s.SerializeString(VolumeModificationDateTime, 17, name: nameof(VolumeModificationDateTime));
            VolumeExpirationDateTime = s.SerializeString(VolumeExpirationDateTime, 17, name: nameof(VolumeExpirationDateTime));
            VolumeEffectiveDateTime = s.SerializeString(VolumeEffectiveDateTime, 17, name: nameof(VolumeEffectiveDateTime));

            FileStructureVersion = s.Serialize<byte>(FileStructureVersion, name: nameof(FileStructureVersion));
        }
    }
}