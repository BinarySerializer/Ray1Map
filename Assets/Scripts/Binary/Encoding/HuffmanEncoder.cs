using DSDecmp.Formats.Nitro;
using System;
using System.IO;

namespace R1Engine
{
	/// <summary>
	/// Compresses/decompresses data using LZSS
	/// </summary>
	public class HuffmanEncoder : IStreamEncoder {
		public Stream DecodeStream(Stream s) {
			Huffman4 huff = new Huffman4();
			MemoryStream outStream = new MemoryStream();
			huff.Decompress(s, s.Length, outStream);
			outStream.Position = 0;
			Util.ByteArrayToFile("D:/BART/RIPPING/R1/PoP_GBA/lol.bin",outStream.ToArray());
			return outStream;
		}

		public Stream EncodeStream(Stream s) {
			throw new NotImplementedException();
		}
	}
}