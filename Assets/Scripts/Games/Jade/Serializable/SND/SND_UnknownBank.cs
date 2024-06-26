using BinarySerializer;

namespace Ray1Map.Jade {
	public class SND_UnknownBank : Jade_File {
		public Jade_FileType ElementType { get; set; }
		public uint HeaderFileSize { get; set; } = 4;
		public uint ContainedFileSize { get; set; }
		public Jade_File File { get; set; }
		public override string Export_Extension => File?.Export_Extension;

		// Irregular Jade_File format. The filesize is only read after the type
		protected override void OnPreSerialize(SerializerObject s) {
			base.OnPreSerialize(s);
			if (!(s is BinarySerializer.BinarySerializer)) {
				if (Loader.IsBinaryData) {
					if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRR2)) {
						HeaderFileSize = s.Serialize<uint>(HeaderFileSize, name: nameof(HeaderFileSize));
					}
					ElementType = s.SerializeObject<Jade_FileType>(ElementType, name: nameof(ElementType));
					ContainedFileSize = s.Serialize<uint>(ContainedFileSize, name: nameof(ContainedFileSize));
					var ptr = s.CurrentPointer;
					FileSize = ContainedFileSize + (uint)(ptr - Offset);
					ptr.File.AddRegion(ptr.FileOffset, ContainedFileSize, $"{GetType().Name}_{Key:X8}");
				} else {
					s.DoAt(Offset + 4, () => {
						ElementType = s.SerializeObject<Jade_FileType>(ElementType, name: nameof(ElementType));
					});
					ContainedFileSize = FileSize;
				}
			}
		}

		protected override void SerializeFile(SerializerObject s) {
			// Serialize file inline
			if (ElementType.Type == Jade_FileType.FileType.SND_Bank) {
				File = s.SerializeObject<SND_MetaBank>((SND_MetaBank)File, onPreSerialize: f => {
					f.Key = Key;
					f.FileSize = ContainedFileSize;
					f.Loader = Loader;
					f.SetIsBinaryData();
				}, name: nameof(File));
			} else {
				File = s.SerializeObject<SND_Bank>((SND_Bank)File, onPreSerialize: f => {
					f.Key = Key;
					f.FileSize = ContainedFileSize;
					f.Loader = Loader;
					f.SetIsBinaryData();
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