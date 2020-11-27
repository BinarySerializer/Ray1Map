using System;
using System.Collections.Generic;

namespace R1Engine
{
    public class LUDI_DataInfo : LUDI_AppInfoBlock {
        public uint DataSize { get; set; }
        public uint NumDataBlocks { get; set; }

        public override void SerializeBlock(SerializerObject s) {
            DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
            NumDataBlocks = s.Serialize<uint>(NumDataBlocks, name: nameof(NumDataBlocks));
        }
	}
}