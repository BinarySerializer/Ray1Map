using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	// Found in GEO_ul_ModifierRLICarte_Load
	public class GEO_ModifierRLICarte : MDF_Modifier {
		public uint UInt_Editor_00 { get; set; }
		public uint[] UInts { get; set; }
		public byte Byte_00 { get; set; }
		public byte Byte_01 { get; set; }
		public byte Byte_02 { get; set; }
		public byte Byte_03 { get; set; }
		public uint Count { get; set; }
		public byte[] Bytes { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
			UInts = s.SerializeArray<uint>(UInts, 64, name: nameof(UInts));
			Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
			Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
			Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
			Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
			Count = s.Serialize<uint>(Count, name: nameof(Count));
			Bytes = s.SerializeArray<byte>(Bytes, Count, name: nameof(Bytes));
		}
	}
}
