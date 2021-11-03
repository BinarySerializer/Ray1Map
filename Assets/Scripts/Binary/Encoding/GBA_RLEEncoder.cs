using DSDecmp.Formats.Nitro;
using System;
using System.IO;
using BinarySerializer;

namespace Ray1Map
{
	/// <summary>
	/// Compresses/decompresses data using Huffman4
	/// </summary>
	public class GBA_RLEEncoder : IStreamEncoder {
		public string Name => "GBA_RLE";

		public Stream DecodeStream(Stream s) {
			RLE rle = new RLE();
			MemoryStream outStream = new MemoryStream();
			rle.Decompress(s, s.Length, outStream);
			outStream.Position = 0;
			return outStream;
		}

		public Stream EncodeStream(Stream s) {
			throw new NotImplementedException();
		}
	}
}