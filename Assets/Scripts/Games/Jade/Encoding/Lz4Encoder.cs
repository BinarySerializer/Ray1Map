using System.IO;
using BinarySerializer;

namespace Ray1Map {
	public class Lz4Encoder : IStreamEncoder {
		public Lz4Encoder(uint compressedSize, uint decompressedSize) {
			CompressedSize = compressedSize;
			DecompressedSize = decompressedSize;
		}

		public string Name => "Lz4";

		public uint CompressedSize { get; set; }
		public uint DecompressedSize { get; }

		public void DecodeStream(Stream input, Stream output) {
			using Reader reader = new Reader(input, isLittleEndian: true, leaveOpen: true);

			byte[] compressedData = reader.ReadBytes((int)CompressedSize);
			byte[] decompressedData = LZ4.LZ4Codec.Decode(compressedData, 0, compressedData.Length, (int)DecompressedSize);

			output.Write(decompressedData, 0, decompressedData.Length);
		}

		public void EncodeStream(Stream input, Stream output) {
			using Reader reader = new Reader(input, isLittleEndian: true, leaveOpen: true);

			byte[] uncompressedData = reader.ReadBytes((int)DecompressedSize);
			byte[] compressedData = LZ4.LZ4Codec.Encode(uncompressedData, 0, uncompressedData.Length);
			CompressedSize = (uint)compressedData.Length;

			output.Write(compressedData, 0, compressedData.Length);
		}
	}
}