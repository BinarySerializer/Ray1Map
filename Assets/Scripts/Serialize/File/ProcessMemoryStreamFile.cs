namespace R1Engine.Serialize
{
    public class ProcessMemoryStreamFile : BinaryFile {
		private string filename; // Keep filename so we can reopen stream later
		private ProcessMemoryStream stream;

		public ProcessMemoryStreamFile(string name, string filename, Context context) : base(context) {
			this.filename = filename;
			filePath = name;
			stream = null;
        }

        public override Pointer StartPointer => new Pointer((uint)baseAddress, this);
		public override Reader CreateReader() {
			if(stream == null) stream = new ProcessMemoryStream(filename, ProcessMemoryStream.Mode.AllAccess);
			Reader reader = new Reader(new NonClosingStreamWrapper(stream), isLittleEndian: Endianness == Endian.Little);
			return reader;
		}

		public override Writer CreateWriter() {
			if (stream == null) stream = new ProcessMemoryStream(filename, ProcessMemoryStream.Mode.AllAccess);
			Writer writer = new Writer(new NonClosingStreamWrapper(stream), isLittleEndian: Endianness == Endian.Little);
			return writer;
		}

		public override Pointer GetPointer(uint serializedValue, Pointer anchor = null) {
			return new Pointer(serializedValue, this, anchor: anchor);
		}

		public override void Dispose() {
			stream?.Dispose();
			stream = null;
			base.Dispose();
		}
	}
}