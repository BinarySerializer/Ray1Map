using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class SND_UnknownBank : Jade_File {
		public Jade_FileType Type { get; set; }
		public uint HeaderFileSize { get; set; } = 4;
		public uint ContainedFileSize { get; set; }
		public Jade_File File { get; set; }

		// Irregular Jade_File format. The filesize is only read after the type
		protected override void OnPreSerialize(SerializerObject s) {
			base.OnPreSerialize(s);
			if (!(s is BinarySerializer.BinarySerializer)) {
				if (Loader.IsBinaryData) {
					if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRR2)) {
						HeaderFileSize = s.Serialize<uint>(HeaderFileSize, name: nameof(HeaderFileSize));
					}
					Type = s.SerializeObject<Jade_FileType>(Type, name: nameof(Type));
					ContainedFileSize = s.Serialize<uint>(ContainedFileSize, name: nameof(ContainedFileSize));
					var ptr = s.CurrentPointer;
					FileSize = ContainedFileSize + (uint)(ptr - Offset);
					ptr.File.AddRegion(ptr.FileOffset, ContainedFileSize, $"{GetType().Name}_{Key:X8}");
				} else {
					s.DoAt(Offset + 4, () => {
						Type = s.SerializeObject<Jade_FileType>(Type, name: nameof(Type));
					});
					ContainedFileSize = FileSize;
				}
			}
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

		/*public override T ConvertType<T>() {
			var type = typeof(T);
			if(type == typeof(SND_Bank) || type == typeof(SND_Metabank))
				return File?.ConvertType<T>() ?? base.ConvertType<T>();
			return base.ConvertType<T>();
		}*/
	}
}