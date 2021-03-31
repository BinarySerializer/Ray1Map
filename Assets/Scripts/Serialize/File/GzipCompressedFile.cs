using BinarySerializer;
using System.IO;
using System.IO.Compression;

namespace R1Engine
{
    public class GzipCompressedFile : LinearSerializedFile {
		public GzipCompressedFile(Context context) : base(context) {
		}

		public override Pointer StartPointer => new Pointer((uint)BaseAddress, this);

		public override Reader CreateReader() {
			Stream s = FileSystem.GetFileReadStream(AbsolutePath);
			// Create a memory stream to write to so we can get the position
			var memStream = new MemoryStream();

			// Decompress to the memory stream
			using (var gZipStream = new GZipStream(s, CompressionMode.Decompress))
				gZipStream.CopyTo(memStream);

			// Set the position to the beginning
			memStream.Position = 0;
			s.Close();

			Length = (uint)memStream.Length;
			Reader reader = new Reader(memStream, isLittleEndian: Endianness == Endian.Little);
			return reader;
		}

		public override Writer CreateWriter() {
			Stream memStream = new MemoryStream();
			memStream.SetLength(Length);
			Writer writer = new Writer(memStream, isLittleEndian: Endianness == Endian.Little);
			return writer;
		}

		public override void EndWrite(Writer writer) {
			if (writer != null) {
				CreateBackupFile();
				using (Stream s = FileSystem.GetFileWriteStream(AbsolutePath, RecreateOnWrite)) {
					using (GZipStream compressionStream = new GZipStream(s, CompressionMode.Compress)) {
						writer.BaseStream.CopyTo(compressionStream);
					}
				}
			}
			base.EndWrite(writer);
		}

		public override Pointer GetPointer(uint serializedValue, Pointer anchor = null) {
			if (Length == 0) {
				Stream s = FileSystem.GetFileReadStream(AbsolutePath);
				// Create a memory stream to write to so we can get the position
				var memStream = new MemoryStream();

				// Decompress to the memory stream
				using (var gZipStream = new GZipStream(s, CompressionMode.Decompress))
					gZipStream.CopyTo(memStream);

				// Set the position to the beginning
				memStream.Position = 0;
				Length = (uint)memStream.Length;
				s.Close();
				memStream.Close();
			}
			uint anchorOffset = anchor?.AbsoluteOffset ?? 0;
			if (serializedValue + anchorOffset >= BaseAddress && serializedValue + anchorOffset <= BaseAddress + Length) {
				return new Pointer(serializedValue, this, anchor: anchor);
			}
			return null;
		}
	}
}
