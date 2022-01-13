using BinarySerializer;
using System;

namespace Ray1Map.Jade {
	public class BIG_FatFile : BinarySerializable {
		public static uint HeaderLength => 0x18;
		
		public uint MaxFile { get; set; } // File count
		public uint MaxDir { get; set; } // Directory count
		public Pointer PosFat { get; set; } // offset of file list
		public int NextPosFat { get; set; }
		public uint FirstIndex { get; set; }
		public uint LastIndex { get; set; }

		public FileReference[] Files { get; set; }
		public FileInfo[] FileInfos { get; set; }
		public DirectoryInfo[] DirectoryInfos { get; set; }

		public BIG_BigFile Big { get; set; } // Set in OnPreSerialize

		public override void SerializeImpl(SerializerObject s) {
			MaxFile = s.Serialize<uint>(MaxFile, name: nameof(MaxFile));
			MaxDir = s.Serialize<uint>(MaxDir, name: nameof(MaxDir));
			PosFat = s.SerializePointer(PosFat, name: nameof(PosFat));
			NextPosFat = s.Serialize<int>(NextPosFat, name: nameof(NextPosFat));
			FirstIndex = s.Serialize<uint>(FirstIndex, name: nameof(FirstIndex));
			LastIndex = s.Serialize<uint>(LastIndex, name: nameof(LastIndex));

			s.DoAt(PosFat, () => {
				Files = s.SerializeObjectArray<FileReference>(Files, MaxFile, name: nameof(Files));
			});
			s.DoAt(PosFat + Big.SizeOfFat * FileReference.StructSize, () => {
				FileInfos = s.SerializeObjectArray<FileInfo>(FileInfos, MaxFile, onPreSerialize: fi => fi.Big = Big, name: nameof(FileInfos));
			});
			s.DoAt(PosFat + Big.SizeOfFat * FileReference.StructSize + Big.SizeOfFat * FileInfo.StructSize(Big), () => {
				DirectoryInfos = s.SerializeObjectArray<DirectoryInfo>(DirectoryInfos, MaxDir, name: nameof(DirectoryInfos));
			});
			if (NextPosFat != -1) {
				s.Goto(s.CurrentPointer.File.StartPointer + NextPosFat - HeaderLength);
			}
		}

		public class FileReference : BinarySerializable {
			public static uint StructSize => 0x8;
			public Pointer FileOffset { get; set; }
			public Jade_Key Key { get; set; }
			public bool IsCompressed  => Key.IsCompressed;

			public override void SerializeImpl(SerializerObject s) {
				FileOffset = s.SerializePointer(FileOffset, name: nameof(FileOffset));
				Key = s.SerializeObject<Jade_Key>(Key, name: nameof(Key));
			}
		}
		/// <summary>
		/// File names. Not read by the engine
		/// </summary>
		public class FileInfo : BinarySerializable {
			public static uint StructSize(BIG_BigFile big) {
				if(big.Context.GetR1Settings().EngineVersion == EngineVersion.Jade_BGE_HD)
					return 0x58;
				if (big.Version == 34 || big.Version == 37 || big.Version == 38)
					return 0x54;
				if (big.Version >= 42)
					return 0x7C;
				return 0x58;
			}
			public BIG_BigFile Big { get; set; }

			public string Name { get; set; }
			public uint FileSize { get; set; }
			public int NextFile { get; set; }
			public int PreviousFile { get; set; }
			public int ParentDirectory { get; set; }
			public uint DateLastModifiedUInt { get; set; }
			public uint UInt_14 { get; set; }
			public uint UInt_54 { get; set; }
			public string Hash { get; set; }
			public uint V43_UInt { get; set; }

			public DateTime DateLastModified => new System.DateTime(1970,1,1).AddSeconds(DateLastModifiedUInt);

			public override void SerializeImpl(SerializerObject s) {
				bool hasName = false;
				if (s.GetXOR() != null) {
					s.DoXOR(null, () => {
						hasName = s.Serialize<uint>(default, "NameCheck") != 0;
					});
					s.Goto(s.CurrentPointer - 4);
				} else {
					hasName = true;
				}
				if (hasName) {
					FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));
					NextFile = s.Serialize<int>(NextFile, name: nameof(NextFile));
					PreviousFile = s.Serialize<int>(PreviousFile, name: nameof(PreviousFile));
					ParentDirectory = s.Serialize<int>(ParentDirectory, name: nameof(ParentDirectory));
					DateLastModifiedUInt = s.Serialize<uint>(DateLastModifiedUInt, name: nameof(DateLastModifiedUInt));
					if(s.IsLogEnabled) s.Log($"Date: {DateLastModified:ddd, dd/MM/yyyy - HH:mm:ss}");
					Name = s.SerializeString(Name, 0x40, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
					if (s.GetR1Settings().EngineVersion == EngineVersion.Jade_BGE_HD || (Big.Version != 34 && Big.Version != 37 && Big.Version != 38)) {
						UInt_54 = s.Serialize<uint>(UInt_54, name: nameof(UInt_54));
					}
					if (Big.Version >= 42) {
						Hash = s.SerializeString(Hash, 0x20, encoding: Jade_BaseManager.Encoding, name: nameof(Hash));
						V43_UInt = s.Serialize<uint>(V43_UInt, name: nameof(V43_UInt));
					}
				} else {
					s.Goto(s.CurrentPointer + StructSize(Big));
				}
			}
		}


		/// <summary>
		/// Directories. Not read by the engine
		/// </summary>
		public class DirectoryInfo : BinarySerializable {
			public static uint StructSize => 0x54;

			public int FirstFileID { get; set; }
			public int FirstDirectoryID { get; set; }
			public int NextDirectory { get; set; }
			public int PreviousDirectory { get; set; }
			public int ParentDirectory { get; set; }
			public string Name { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				FirstFileID = s.Serialize<int>(FirstFileID, name: nameof(FirstFileID));
				FirstDirectoryID = s.Serialize<int>(FirstDirectoryID, name: nameof(FirstDirectoryID));
				NextDirectory = s.Serialize<int>(NextDirectory, name: nameof(NextDirectory));
				PreviousDirectory = s.Serialize<int>(PreviousDirectory, name: nameof(PreviousDirectory));
				ParentDirectory = s.Serialize<int>(ParentDirectory, name: nameof(ParentDirectory));
				Name = s.SerializeString(Name, 0x40, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
			}
		}

		public BIG_FatFile() { }
		public BIG_FatFile(Pointer offset) {
			Offset = offset;
			Context = offset.Context;
		}

	}
}
