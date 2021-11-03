using System;
using BinarySerializer;

namespace Ray1Map.Jade 
{
	public class TEX_Palette_RRR2_Unknown : Jade_File 
    {
        public uint UInt_00 { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
		}
	}
}