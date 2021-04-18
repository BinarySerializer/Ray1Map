using System;
using BinarySerializer;
using UnityEngine;

namespace R1Engine.Jade
{
    public class TEXPRO_Photo : BinarySerializable {
        public uint FileSize { get; set; } // Set in onPreSerialize

        public uint UInt_00_Editor { get; set; }
        public byte Unknown { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            if (FileSize > 0) {
                LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
                if (!Loader.IsBinaryData) UInt_00_Editor = s.Serialize<uint>(UInt_00_Editor, name: nameof(UInt_00_Editor));
                Unknown = s.Serialize<byte>(Unknown, name: nameof(Unknown));
            }
        }
	}
}