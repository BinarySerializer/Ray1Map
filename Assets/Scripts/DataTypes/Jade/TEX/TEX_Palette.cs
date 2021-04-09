using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class TEX_Palette : Jade_File {
		public byte[] Bytes { get; set; }
		public override void SerializeImpl(SerializerObject s) {
			// See TEX_File_LoadPalette
			Bytes = s.SerializeArray<byte>(Bytes, FileSize, name: nameof(Bytes));
		}
	}
}
