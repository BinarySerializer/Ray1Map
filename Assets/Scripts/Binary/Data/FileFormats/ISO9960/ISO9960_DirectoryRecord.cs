using System;
using BinarySerializer;

namespace Ray1Map
{
    // See: https://wiki.osdev.org/ISO_9660
    public class ISO9960_DirectoryRecord : BinarySerializable {
        public byte[] Unused { get; set; }
        public byte[] BigEndianValue { get; set; }

        public byte Length { get; set; }
        public byte ExtendedAttributeRecord { get; set; }
        public uint ExtentLBA { get; set; }
        public uint ExtentSize { get; set; }
        public byte[] RecordingDateTime { get; set; }
        public RecordFileFlags FileFlags { get; set; }
        public byte InterleaveFileUnitSize { get; set; }
        public byte InterleaveFileGapSize { get; set; }
        public ushort VolumeSequenceNumber { get; set; }
        public byte FileIdentifierLength { get; set; }
        public string FileIdentifier { get; set; }

        public byte Padding { get; set; }
        public byte[] SystemUse { get; set; }


        public override void SerializeImpl(SerializerObject s) {
            Length = s.Serialize<byte>(Length, name: nameof(Length));
            if (Length == 0) return;
            ExtendedAttributeRecord = s.Serialize<byte>(ExtendedAttributeRecord, name: nameof(ExtendedAttributeRecord));

            ExtentLBA = s.Serialize<uint>(ExtentLBA, name: nameof(ExtentLBA));
            BigEndianValue = s.SerializeArray<byte>(BigEndianValue, 4, name: nameof(BigEndianValue));

            ExtentSize = s.Serialize<uint>(ExtentSize, name: nameof(ExtentSize));
            BigEndianValue = s.SerializeArray<byte>(BigEndianValue, 4, name: nameof(BigEndianValue));

            RecordingDateTime = s.SerializeArray<byte>(RecordingDateTime, 7, name: nameof(RecordingDateTime));
            FileFlags = s.Serialize<RecordFileFlags>(FileFlags, name: nameof(FileFlags));
            InterleaveFileUnitSize = s.Serialize<byte>(InterleaveFileUnitSize, name: nameof(InterleaveFileUnitSize));
            InterleaveFileGapSize = s.Serialize<byte>(InterleaveFileGapSize, name: nameof(InterleaveFileGapSize));

            VolumeSequenceNumber = s.Serialize<ushort>(VolumeSequenceNumber, name: nameof(VolumeSequenceNumber));
            BigEndianValue = s.SerializeArray<byte>(BigEndianValue, 2, name: nameof(BigEndianValue));

            FileIdentifierLength = s.Serialize<byte>(FileIdentifierLength, name: nameof(FileIdentifierLength));
            FileIdentifier = s.SerializeString(FileIdentifier, length: FileIdentifierLength, name: nameof(FileIdentifier));

            if (s.CurrentPointer - Offset < Length && FileIdentifierLength % 2 != 0) {
                Padding = s.Serialize<byte>(Padding, name: nameof(Padding));
            }
            if (s.CurrentPointer - Offset < Length) {
                SystemUse = s.SerializeArray<byte>(SystemUse, Length - (s.CurrentPointer - Offset), name: nameof(SystemUse));
            }

            s.Goto(Offset + Length);
        }

        [Flags]
        public enum RecordFileFlags : byte
        {
            None = 0,
            Hidden = 1 << 0,
            Directory = 1 << 1,
            AssociatedFile = 1 << 2,
            FormatInfo = 1 << 3,
            HasOwnerAndGroupPermissions = 1 << 4,
            Reserved1 = 1 << 5,
            Reserved2 = 1 << 6,
            NotFinalDir = 1 << 7,
        }
    }
}