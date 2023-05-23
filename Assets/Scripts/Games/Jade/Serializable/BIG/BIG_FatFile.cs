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
				DirectoryInfos = s.SerializeObjectArray<DirectoryInfo>(DirectoryInfos, MaxDir, onPreSerialize: fi => fi.Big = Big, name: nameof(DirectoryInfos));
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
					return 0x3C + NameSize(big); // 0x7C usually, but 0xBC for Avatar
				if (big.Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CloudyWithAChanceOfMeatballs))
					return 0x80;
				return 0x58;
			}
			private static uint NameSize(BIG_BigFile big) => big.Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Avatar) ? (uint)0x80 : 0x40;

			public BIG_BigFile Big { get; set; }

			public string Name { get; set; }
			public uint LengthOnDisk { get; set; }
			public int Previous { get; set; }
			public int Next { get; set; }
			public int ParentDirectory { get; set; }
			public uint DateLastModifiedUInt { get; set; }
			public uint P4RevisionClient { get; set; }
			public string Hash { get; set; }
			public uint V43_UInt { get; set; }

			public DateTime DateLastModified {
				get => new System.DateTime(1970, 1, 1).AddSeconds(DateLastModifiedUInt);
				set => DateLastModifiedUInt = (uint)((value - new System.DateTime(1970, 1, 1)).TotalSeconds);
			}

			public override void SerializeImpl(SerializerObject s) {
				bool hasName = false;
				if (s.GetProcessor<XorProcessor>() is { } xorProcessor) {
                    xorProcessor.DoInactive(() => {
                        hasName = s.Serialize<uint>(default, "NameCheck") != 0;
                    });
					s.Goto(s.CurrentPointer - 4);
				} else {
					hasName = true;
				}
				if (hasName) {
					LengthOnDisk = s.Serialize<uint>(LengthOnDisk, name: nameof(LengthOnDisk));
					Previous = s.Serialize<int>(Previous, name: nameof(Previous));
					Next = s.Serialize<int>(Next, name: nameof(Next));
					ParentDirectory = s.Serialize<int>(ParentDirectory, name: nameof(ParentDirectory));
					DateLastModifiedUInt = s.Serialize<uint>(DateLastModifiedUInt, name: nameof(DateLastModifiedUInt));
					if(s.IsSerializerLoggerEnabled) s.Log("Date: {0:ddd, dd/MM/yyyy - HH:mm:ss}", DateLastModified);
					Name = s.SerializeString(Name, NameSize(Big), encoding: Jade_BaseManager.Encoding, name: nameof(Name));
					if (s.GetR1Settings().EngineVersion == EngineVersion.Jade_BGE_HD || (Big.Version != 34 && Big.Version != 37 && Big.Version != 38)) {
						P4RevisionClient = s.Serialize<uint>(P4RevisionClient, name: nameof(P4RevisionClient));
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
			public static uint StructSize(BIG_BigFile big) => 0x14 + NameSize(big); // 0x54 usually, but 0x94 for Avatar
			private static uint NameSize(BIG_BigFile big) => big.Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Avatar) ? (uint)0x80 : 0x40;

			public BIG_BigFile Big { get; set; }

			public int FirstFile { get; set; }
			public int FirstSubDirectory { get; set; }
			public int Previous { get; set; }
			public int Next { get; set; }
			public int Parent { get; set; }
			public string Name { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				FirstFile = s.Serialize<int>(FirstFile, name: nameof(FirstFile));
				FirstSubDirectory = s.Serialize<int>(FirstSubDirectory, name: nameof(FirstSubDirectory));
				Previous = s.Serialize<int>(Previous, name: nameof(Previous));
				Next = s.Serialize<int>(Next, name: nameof(Next));
				Parent = s.Serialize<int>(Parent, name: nameof(Parent));
				Name = s.SerializeString(Name, NameSize(Big), encoding: Jade_BaseManager.Encoding, name: nameof(Name));
			}
		}

		public BIG_FatFile() { }
		public BIG_FatFile(Pointer offset) 
        {
            Init(offset);
		}

	}
}
