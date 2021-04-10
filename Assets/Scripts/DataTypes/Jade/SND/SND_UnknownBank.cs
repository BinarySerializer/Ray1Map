using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class SND_UnknownBank : Jade_File {
		public Jade_FileType Type { get; set; }
		public uint ContainedFileSize { get; set; }
		public Jade_File File { get; set; }

		// Irregular Jade_File format. The filesize is only read after the type
		protected override void OnPreSerialize(SerializerObject s) {
			base.OnPreSerialize(s);
			Type = s.SerializeObject<Jade_FileType>(Type, name: nameof(Type));
			ContainedFileSize = s.Serialize<uint>(ContainedFileSize, name: nameof(ContainedFileSize));
			var ptr = s.CurrentPointer;
			FileSize = ContainedFileSize + (uint)(ptr - Offset);
			ptr.File.AddRegion(ptr.FileOffset, ContainedFileSize, $"{GetType().Name}_{Key:X8}");
		}

		public override void SerializeImpl(SerializerObject s) {
			// Serialize file inline
			if (Type.Type == Jade_FileType.FileType.SND_Metabank) {
				File = s.SerializeObject<SND_Metabank>((SND_Metabank)File, onPreSerialize: f => {
					f.Key = Key;
					f.FileSize = ContainedFileSize;
					f.Loader = Loader;
				}, name: nameof(File));
			} else {
				File = s.SerializeObject<SND_Bank>((SND_Bank)File, onPreSerialize: f => {
					f.Key = Key;
					f.FileSize = ContainedFileSize;
					f.Loader = Loader;
				}, name: nameof(File));
			}
        }

		// Irregular Jade_File format. Make sure to jump to end of file
		protected override void OnPostSerialize(SerializerObject s) {
			base.OnPostSerialize(s);
			s.Goto(Offset + FileSize);
		}
	}
}