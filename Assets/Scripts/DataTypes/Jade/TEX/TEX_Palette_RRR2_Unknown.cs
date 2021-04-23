using System;
using BinarySerializer;

namespace R1Engine.Jade 
{
	public class TEX_Palette_RRR2_Unknown : Jade_File 
    {
        public uint UInt_00 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
		}
	}
}