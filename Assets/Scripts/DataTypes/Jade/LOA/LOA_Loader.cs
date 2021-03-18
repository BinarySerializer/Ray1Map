using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	public class LOA_Loader {
		public BIG_BigFile[] BigFiles { get; set; }
		public Queue<FileReference> LoadQueue = new Queue<FileReference>();
		public Dictionary<Jade_Key, Pointer> FilePointers { get; private set; }
		public Dictionary<Jade_Key, Jade_File> LoadedFiles { get; private set; }
		public LOA_Loader(BIG_BigFile[] bigFiles) {
			BigFiles = bigFiles;
			CreateSortedFilePointers();

		}

		private void CreateSortedFilePointers() {
			FilePointers = new Dictionary<Jade_Key, Pointer>();
			LoadedFiles = new Dictionary<Jade_Key, Jade_File>();
			for (int b = 0; b < BigFiles.Length; b++) {
				var big = BigFiles[b];
				for (int f = 0; f < big.FatFiles.Length; f++) {
					var fat = big.FatFiles[f];
					foreach (var file in fat.Files) {
						FilePointers.Add(file.Key, file.FileOffset);
					}
				}
			}
		}

		public async UniTask LoadLoop(SerializerObject s) {
			while (LoadQueue.Count > 0) {
				FileReference currentRef = LoadQueue.Dequeue();
				if (currentRef.Key != null && FilePointers.ContainsKey(currentRef.Key)) {
					if (LoadedFiles.ContainsKey(currentRef.Key)) {
						currentRef.AlreadyLoadedCallback(LoadedFiles[currentRef.Key]);
					} else {
						Pointer off_current = s.CurrentPointer;
						Pointer off_target = FilePointers[currentRef.Key];
						s.Goto(off_target);
						await s.FillCacheForRead(4);
						var fileSize = s.Serialize<uint>(default, name: "FileSize");
						await s.FillCacheForRead((int)fileSize);
						currentRef.LoadCallback(s, (f) => {
							f.Key = currentRef.Key;
							f.FileSize = fileSize;
							f.Loader = this;
							if (!LoadedFiles.ContainsKey(f.Key)) LoadedFiles[f.Key] = f;
						});
						s.Goto(off_current);
					}
				}
			}
		}

		public delegate void ResolveAction(SerializerObject s, Action<Jade_File> configureAction);
		public delegate void ResolvedAction(Jade_File f);
		public class FileReference {
			public Jade_Key Key { get; set; }
			public ResolveAction LoadCallback { get; set; }
			public ResolvedAction AlreadyLoadedCallback { get; set; }
		}

		public void RequestFile(Jade_Key key, ResolveAction loadCallback, ResolvedAction alreadyLoadedCallback) {
			LoadQueue.Enqueue(new FileReference() {
				Key = key,
				LoadCallback = loadCallback,
				AlreadyLoadedCallback = alreadyLoadedCallback
			});
		}
	}
}
