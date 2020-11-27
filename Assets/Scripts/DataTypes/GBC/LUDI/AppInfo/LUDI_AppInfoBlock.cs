using System;
using System.Collections.Generic;

namespace R1Engine
{
    public abstract class LUDI_AppInfoBlock : R1Serializable {
        public uint LengthInBytes { get; set; }
        public byte[] UnknownData { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            LengthInBytes = s.Serialize<uint>(LengthInBytes, name: nameof(LengthInBytes));
            SerializeBlock(s);
            UnknownData = s.SerializeArray<byte>(UnknownData, LengthInBytes - (s.CurrentPointer - Offset - 4), name: nameof(UnknownData));
        }

        public abstract void SerializeBlock(SerializerObject s);
	}
}