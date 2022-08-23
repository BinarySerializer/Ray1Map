using BinarySerializer;
using BinarySerializer.PSP;
using System.IO;

namespace Ray1Map.Jade {
    public class PSP_GEData : BinarySerializable {
		public uint Flags { get; set; }
		public uint DataSize { get; set; }
		public byte[] Bytes { get; set; }
		// TODO: PSP's data is in GE commands format http://hitmen.c02.at/files/yapspd/psp_doc/chap11.html#sec11.1

		public bool Pre_IsInstance { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (!Pre_IsInstance) {
				Flags = s.Serialize<ushort>((ushort)Flags, name: nameof(Flags));
			} else {
				Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			}
			DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
			Bytes = s.SerializeArray<byte>(Bytes, DataSize, name: nameof(Bytes));
		}

		public void Execute(Context context) {
			var progKey = $"GEData_{Offset}_{Flags:X8}";
			using (Context c = new Context("", serializerLog: new ParentContextSerializerLog(context.SerializerLog), systemLog: context.SystemLog)) {
				// Parse GE program
				var file = c.AddStreamFile(progKey, new MemoryStream(Bytes));
				try {
					var s = c.Deserializer;
					var parser = new GE_Parser();
					parser.Parse(s, file.StartPointer);
				} finally {
					c.RemoveFile(file);
				}
			}
		}
	}
}