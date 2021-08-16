using System;
using BinarySerializer;

namespace R1Engine.Jade {
	/// <summary>
	/// Special Geometric object format for Xbox 360 (and King Kong Gamer's Edition)
	/// </summary>
	public class GEO_XenonPack : Jade_File {
		public byte[] Bytes { get; set; } // Game reads this as a byte array, then parses the buffer. Seems to be big endian

		protected override void SerializeFile(SerializerObject s) {
			Bytes = s.SerializeArray<byte>(Bytes, FileSize, name: nameof(Bytes));
		}
	}
}
