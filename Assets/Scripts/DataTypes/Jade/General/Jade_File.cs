using BinarySerializer;
using System;

namespace R1Engine.Jade {
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

		public virtual T ConvertType<T>() where T : Jade_File {
			return (T)this; // Override this method for more complex types
		}
		public void CheckFileSize(SerializerObject s) {
			long readSize = s.CurrentPointer - Offset;
			if (Loader.IsBinaryData && FileSize != readSize && !UnknownFileSize) {
				UnityEngine.Debug.LogWarning($"File {Key} with type {GetType()} was not fully serialized: File Size: {FileSize:X8} / Serialized: {readSize:X8}");
			} else if(UnknownFileSize) FileSize = (uint)readSize;
		}
	}
}
