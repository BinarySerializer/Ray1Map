using System;
using BinarySerializer;
using UnityEngine;

namespace R1Engine.Jade
{
    public class TEXPRO_Mpeg : BinarySerializable {
        public uint FileSize { get; set; } // Set in onPreSerialize

        public uint SerializedFileSize { get; set; }
        public Jade_Reference<TEXPRO_Mpeg_Content> Content { get; set; }
        public uint UInt_0 { get; set; }
        public uint UInt_1 { get; set; }
        public uint UInt_2 { get; set; }
        public uint UInt_3 { get; set; }
        public uint UInt_4 { get; set; }
        public uint UInt_5 { get; set; }
        public uint UInt_6 { get; set; }
        public uint UInt_7 { get; set; }
        public uint UInt_8 { get; set; }
        public uint UInt_9 { get; set; }
        public uint UInt_10 { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            if (FileSize > 0) {
                SerializedFileSize = s.Serialize<uint>(SerializedFileSize, name: nameof(SerializedFileSize));
                if (FileSize != SerializedFileSize) return;
                Content = s.SerializeObject<Jade_Reference<TEXPRO_Mpeg_Content>>(Content, name: nameof(Content))?.Resolve(immediate: true, flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.IsIrregularFileFormat);
                if (SerializedFileSize >= 0x20) {
                    UInt_0 = s.Serialize<uint>(UInt_0, name: nameof(UInt_0));
                    UInt_1 = s.Serialize<uint>(UInt_1, name: nameof(UInt_1));
                    UInt_2 = s.Serialize<uint>(UInt_2, name: nameof(UInt_2));
                    UInt_3 = s.Serialize<uint>(UInt_3, name: nameof(UInt_3));
                    UInt_4 = s.Serialize<uint>(UInt_4, name: nameof(UInt_4));
                    UInt_5 = s.Serialize<uint>(UInt_5, name: nameof(UInt_5));
                }
                if (SerializedFileSize >= 0x30) {
                    UInt_6 = s.Serialize<uint>(UInt_6, name: nameof(UInt_6));
                    UInt_7 = s.Serialize<uint>(UInt_7, name: nameof(UInt_7));
                    UInt_8 = s.Serialize<uint>(UInt_8, name: nameof(UInt_8));
                    UInt_9 = s.Serialize<uint>(UInt_9, name: nameof(UInt_9));
                }
                if (SerializedFileSize > 0x30) {
                    UInt_10 = s.Serialize<uint>(UInt_10, name: nameof(UInt_10));
                }
            }
        }
	}
}