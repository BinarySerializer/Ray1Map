using BinarySerializer;

namespace Ray1Map.Jade {
	public abstract class Jade_File : BinarySerializable {
		public LOA_Loader Loader { get; set; }
		public Jade_Key Key { get; set; }
		public uint FileSize { get; set; }
		public bool UnknownFileSize { get; set; }
		public uint ReferencesCount { get; set; } = 1;
		public uint CachedCount { get; set; } = 1;
		public LOA_BinFileHeader BinFileHeader { get; set; }
		public LOA_HeaderBFFile HeaderBFFile { get; set; } // Only for unbinarized files post-TMNT Montreal
		public virtual bool HasHeaderBFFile => false;
		public uint HeaderBFFileSize => (uint)(HeaderBFFile?.Size ?? 0);
		public virtual string Export_Extension => null;
		public virtual string Export_FileBasename => null;
		public string Export_OriginalFilename { get; set; }
		public bool? CurrentIsBinaryData { get; set; }

		protected override void OnPostSerialize(SerializerObject s) {
			base.OnPostSerialize(s);
			CheckFileSize(s);
		}

		protected override void OnPreSerialize(SerializerObject s) {
			base.OnPreSerialize(s);
			if (HasHeaderBFFile && s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_TMNT) && !Loader.IsBinaryData) {
				HeaderBFFile = s.SerializeObject<LOA_HeaderBFFile>(HeaderBFFile, name: nameof(HeaderBFFile));
			}
		}

		public override void SerializeImpl(SerializerObject s) {
			if (FileSize > 0 || UnknownFileSize) {
				SerializeFile(s);
			}
		}

		protected abstract void SerializeFile(SerializerObject s);

		public virtual T ConvertType<T>() where T : Jade_File {
			return (T)this; // Override this method for more complex types
		}
		public void CheckFileSize(SerializerObject s) {
			long readSize = s.CurrentPointer - Offset;
			if (/*Loader.IsBinaryData &&*/ FileSize != readSize && !UnknownFileSize) {
				s.SystemLogger?.LogWarning($"File {Key} with type {GetType()} was not fully serialized: File Size: {FileSize:X8} / Serialized: {readSize:X8}");
			} else if(UnknownFileSize) FileSize = (uint)readSize;
		}

		public void SetIsBinaryData() {
			var newIsBinaryData = Loader.IsBinaryData;
			var lastIsBinaryData = CurrentIsBinaryData;
			CurrentIsBinaryData = newIsBinaryData;
			var isBinaryDataChanged = lastIsBinaryData != null && newIsBinaryData != lastIsBinaryData;
			if (isBinaryDataChanged) {
				OnChangedIsBinaryData();
			}
		}
		protected virtual void OnChangedIsBinaryData() { }
	}
}
