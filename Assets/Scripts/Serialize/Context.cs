using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Serialize {
	public class Context : IDisposable {
		public MemoryMap MemoryMap { get; } = new MemoryMap();

		public GameSettings Settings { get; }

		public SerializableCache Cache { get; } = new SerializableCache();

		public SerializerLog Log { get; } = new SerializerLog();

		public string BasePath { get; }

		protected Dictionary<string, object> ObjectStorage { get; } = new Dictionary<string, object>();

		/// <summary>
		/// Creates an empty serialization context
		/// </summary>
		/// <param name="basePath">The base directory that all files are relative to</param>
		/// <param name="settings">The game settings</param>
		public Context(string basePath, GameSettings settings) {
			this.BasePath = Util.NormalizePath(basePath, true);
			this.Settings = settings;
		}
		public Context(GameSettings settings) : this(settings.GameDirectory, settings) {
		}

		public Stream GetFileStream(string relativePath) {
			Stream str = FileSystem.GetFileReadStream(BasePath + Util.NormalizePath(relativePath, false));
			return str;
		}

		public BinaryFile GetFile(string relativePath) {
			string path = Util.NormalizePath(relativePath, false);
			return MemoryMap.Files.FirstOrDefault<BinaryFile>(f => f.filePath.ToLower() == path.ToLower() || f.alias?.ToLower() == relativePath.ToLower());
		}
		public void AddFile(BinaryFile file) {
			MemoryMap.Files.Add(file);
		}
		public Pointer<T> FilePointer<T>(string relativePath) where T : R1Serializable, new() {
			Pointer p = FilePointer(relativePath);
			if (p == null) return null;
			return new Pointer<T>(p);
		}
		public Pointer FilePointer(string relativePath) {
			BinaryFile f = GetFile(relativePath);
			if (f == null) {
				throw new Exception("File with path " + relativePath + " is not loaded in this Context!");
			}
			//if (f == null) return null;
			return f.StartPointer;
		}
		public bool FileExists(string relativePath) {
			BinaryFile f = GetFile(relativePath);
			return f != null;
		}

		public T GetMainFileObject<T>(string relativePath) where T : R1Serializable {
			return GetMainFileObject<T>(GetFile(relativePath));
		}
		public T GetMainFileObject<T>(BinaryFile file) where T : R1Serializable {
			if (file == null) return default;
			Pointer ptr = file.StartPointer;
			return Cache.FromOffset<T>(ptr);
		}

		public T GetStoredObject<T>(string id) {
			if (ObjectStorage.ContainsKey(id)) return (T)ObjectStorage[id];
			return default;
		}

		public void StoreObject<T>(string id, T obj) {
			ObjectStorage[id] = obj;
		}

		private BinaryDeserializer deserializer;
		public BinaryDeserializer Deserializer {
			get {
				if (deserializer == null) {
					if (serializer != null) {
						serializer.Dispose();
						serializer = null;
					}
					deserializer = new BinaryDeserializer(this);
				}
				return deserializer;
			}
		}

		private BinarySerializer serializer;
		public BinarySerializer Serializer {
			get {
				if (serializer == null) {
					if (deserializer != null) {
						deserializer.Dispose();
						deserializer = null;
					}
					serializer = new BinarySerializer(this);
				}
				return serializer;
			}
		}

		public void Close() {
			deserializer?.Dispose();
			deserializer = null;
			serializer?.Dispose();
			serializer = null;
			Log.WriteLog();
		}

		public void Dispose() {
			Close();
		}
	}
}
