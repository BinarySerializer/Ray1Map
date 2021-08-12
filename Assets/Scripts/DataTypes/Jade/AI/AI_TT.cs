using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace R1Engine.Jade {
	public class AI_TT : Jade_File {
		public override string Export_Extension => "ttt";
		public override bool HasHeaderBFFile => true;

		public uint UInt_00 { get; set; }
		public byte[] Bytes { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if(Loader.IsBinaryData)
				UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			Bytes = s.SerializeArray<byte>(Bytes, FileSize - (Loader.IsBinaryData ? 4 : 0) - HeaderBFFileSize, name: nameof(Bytes));
		}
	}
}
