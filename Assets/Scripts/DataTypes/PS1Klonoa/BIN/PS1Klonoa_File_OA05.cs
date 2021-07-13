using System;
using BinarySerializer;

namespace R1Engine
{
    // Some sound related pack
    public class PS1Klonoa_File_OA05 : PS1Klonoa_BaseFile
    {
        public string Header { get; set; }
        public Pointer Pointer_04 { get; set; }
        public Pointer VABPointer { get; set; } // Points to a VAB file
        public Pointer SEQPointer { get; set; } // Points to the SEQ files
        public Pointer Pointer_10 { get; set; }
        public Pointer Pointer_14 { get; set; }
        public Pointer Pointer_18 { get; set; }

        public byte DataType { get; set; }
        public byte Byte_01 { get; set; }
        public short Short_02 { get; set; }
        public byte Byte_04 { get; set; } // Count
        public byte Byte_05 { get; set; } // Padding?

        // Type 1
        public short SEQCount { get; set; }
        public uint VABFileSize { get; set; } // Offset
        public uint[] SEQFileSizes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Header = s.SerializeString(Header, 4, name: nameof(Header));

            if (Header != "OA05")
                throw new Exception($"Invalid OA05 header {Header}");

            Pointer_04 = s.SerializePointer(Pointer_04, anchor: Offset, name: nameof(Pointer_04));
            VABPointer = s.SerializePointer(VABPointer, anchor: Offset, name: nameof(VABPointer));
            SEQPointer = s.SerializePointer(SEQPointer, anchor: Offset, name: nameof(SEQPointer));
            Pointer_10 = s.SerializePointer(Pointer_10, anchor: Offset, name: nameof(Pointer_10));
            Pointer_14 = s.SerializePointer(Pointer_14, anchor: Offset, name: nameof(Pointer_14));
            Pointer_18 = s.SerializePointer(Pointer_18, anchor: Offset, name: nameof(Pointer_18));

            s.SerializePadding(20, logIfNotNull: true);

            s.DoAt(Pointer_04, () =>
            {
                DataType = s.Serialize<byte>(DataType, name: nameof(DataType));
                Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
                Short_02 = s.Serialize<short>(Short_02, name: nameof(Short_02));
                Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
                Byte_05 = s.Serialize<byte>(Byte_05, name: nameof(Byte_05));

                if (DataType == 0)
                {
                    // TODO: Implement
                }
                else
                {
                    SEQCount = s.Serialize<short>(SEQCount, name: nameof(SEQCount));
                    s.SerializeArray<byte>(null, 8);
                    VABFileSize = s.Serialize<uint>(VABFileSize, name: nameof(VABFileSize));
                    s.SerializeArray<byte>(null, 28);
                    SEQFileSizes = s.SerializeArray<uint>(SEQFileSizes, SEQCount, name: nameof(SEQFileSizes));

                    s.DoAt(Pointer_18, () =>
                    {
                        s.Serialize<int>(default);
                        var length = s.Serialize<int>(default);
                        var data = s.SerializeObjectArray<UnknownStruct>(null, length);
                    });
                }

                // Byte_04 is the count, Byte_01 is the initial offset. Lengths to what Pointer_14 points to.
                s.DoAt(Offset + 0x430 + Byte_01 * 4, () => s.SerializeArray<int>(null, Byte_04 - Byte_01));
            });
        }

        public class UnknownStruct : BinarySerializable
        {
            public byte[] Bytes_00 { get; set; }
            public int Int_04 { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Bytes_00 = s.SerializeArray<byte>(Bytes_00, 4, name: nameof(Bytes_00));
                Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));
            }
        }
    }
}