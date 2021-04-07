using System;
using BinarySerializer;

namespace R1Engine.Jade {
    public class COL_ColSet : Jade_File {
        public byte CobsCount { get; set; }
        public byte Byte_01 { get; set; }
        public short Short_Editor { get; set; }
        public byte[] DesignIndices { get; set; } // Indices for COL_Instance.Design
        public COL_ColSetStruct[] Cobs { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            CobsCount = s.Serialize<byte>(CobsCount, name: nameof(CobsCount));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            if(!Loader.IsBinaryData) Short_Editor = s.Serialize<short>(Short_Editor, name: nameof(Short_Editor));
            DesignIndices = s.SerializeArray<byte>(DesignIndices, 16, name: nameof(DesignIndices));
            Cobs = s.SerializeObjectArray<COL_ColSetStruct>(Cobs, CobsCount, onPreSerialize: c => c.IsInstance = false, name: nameof(Cobs));

        }
    }
}