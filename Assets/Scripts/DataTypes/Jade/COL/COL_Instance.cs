using System;
using BinarySerializer;

namespace R1Engine.Jade {
    public class COL_Instance : Jade_File {
        public Jade_Reference<COL_ColSet> ColSet { get; set; }
        public byte DesignCount { get; set; }
        public byte Byte_05 { get; set; }
        public byte CobsCount { get; set; }
        public byte Byte_07 { get; set; }
        public byte[] Design { get; set; } // What is this?
        public COL_ColSetStruct[] Cobs { get; set; }
        public short Activation { get; set; }
        public short Short_12 { get; set; }
        public short Short_14_Editor { get; set; }
        public short Short_14 { get; set; }
        public short Short_16 { get; set; }
        public byte Byte_18 { get; set; }
        public byte Byte_19 { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            ColSet = s.SerializeObject<Jade_Reference<COL_ColSet>>(ColSet, name: nameof(ColSet))?.Resolve();
            DesignCount = s.Serialize<byte>(DesignCount, name: nameof(DesignCount));
            Byte_05 = s.Serialize<byte>(Byte_05, name: nameof(Byte_05));
            CobsCount = s.Serialize<byte>(CobsCount, name: nameof(CobsCount));
            if (DesignCount == Byte_05 + CobsCount) {
                Byte_07 = s.Serialize<byte>(Byte_07, name: nameof(Byte_07));
                Design = s.SerializeArray<byte>(Design, DesignCount, name: nameof(Design));
                Cobs = s.SerializeObjectArray<COL_ColSetStruct>(Cobs, CobsCount, onPreSerialize: c => c.IsInstance = true, name: nameof(Cobs));
                Activation = s.Serialize<short>(Activation, name: nameof(Activation));
                Short_12 = s.Serialize<short>(Short_12, name: nameof(Short_12));
                Short_14 = s.Serialize<short>(Short_14, name: nameof(Short_14));
                if(!Loader.IsBinaryData) Short_14_Editor = s.Serialize<short>(Short_14_Editor, name: nameof(Short_14_Editor));
                Short_16 = s.Serialize<short>(Short_16, name: nameof(Short_16));
                Byte_18 = s.Serialize<byte>(Byte_18, name: nameof(Byte_18));
                Byte_19 = s.Serialize<byte>(Byte_19, name: nameof(Byte_19));
            }
        }
	}
}