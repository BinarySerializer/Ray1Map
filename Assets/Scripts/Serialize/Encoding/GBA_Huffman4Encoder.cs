using DSDecmp.Formats.Nitro;
using System;
using System.IO;
using BinarySerializer;

namespace R1Engine
{
	/// <summary>
	/// Compresses/decompresses data using Huffman4
	/// </summary>
	public class GBA_Huffman4Encoder : IStreamEncoder {
		public Stream DecodeStream(Stream s) {
			Huffman4 huff = new Huffman4();
			MemoryStream outStream = new MemoryStream();
			huff.Decompress(s, s.Length, outStream);
			outStream.Position = 0;
			return outStream;
		}

		public Stream EncodeStream(Stream s) {
			throw new NotImplementedException();
		}
	}
}