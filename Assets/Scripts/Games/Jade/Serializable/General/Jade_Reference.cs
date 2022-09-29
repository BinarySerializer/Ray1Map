
using System;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class Jade_Reference<T> : BinarySerializable, ISerializerShortLog where T : Jade_File, new() {
		public Jade_Key Key { get; set; }
		public T Value { get; set; }
		public bool IsNull => Key.IsNull;
		public bool ForceResolve { get; set; }

		public uint EmbeddedFileSize { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Key = s.SerializeObject<Jade_Key>(Key, name: nameof(Key));
		}

		public Jade_Reference() { }
		public Jade_Reference(Context c, Jade_Key key) {
			Context = c;
			Key = key;
		}

		public Jade_Reference<T> Resolve(
			Action<SerializerObject, T> onPreSerialize = null,
			Action<SerializerObject, T> onPostSerialize = null,
			bool immediate = false,
			LOA_Loader.QueueType queue = LOA_Loader.QueueType.Current,
			LOA_Loader.CacheType cache = LOA_Loader.CacheType.Current,
			LOA_Loader.ReferenceFlags flags = LOA_Loader.ReferenceFlags.Log) {

			if (IsNull && !ForceResolve) return this;
			LOA_Loader loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			loader.RequestFile(Key, Value, (s, configureAction) => {
				SerializeFile(s, configureAction, onPreSerialize, onPostSerialize);
			}, (f) => {
				Value = f?.ConvertType<T>();
			}, immediate: immediate,
			queue: queue,
			cache: cache,
			name: typeof(T).Name,
			flags: flags);
			return this;
		}

		public Jade_Reference<T> ResolveEmbedded(SerializerObject s,
			Action<SerializerObject, T> onPreSerialize = null,
			Action<SerializerObject, T> onPostSerialize = null,
			LOA_Loader.ReferenceFlags flags = LOA_Loader.ReferenceFlags.Log,
			bool unknownFileSize = false) {
			if (IsNull && !ForceResolve) return this;
			LOA_Loader loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			if (loader.Cache.ContainsKey(Key) && !flags.HasFlag(LOA_Loader.ReferenceFlags.DontUseCachedFile)) {
				Value = loader.Cache[Key]?.ConvertType<T>();
			} else {
				if(!unknownFileSize) EmbeddedFileSize = s.Serialize<uint>(EmbeddedFileSize, name: nameof(EmbeddedFileSize));
				SerializeFile(s, f => {
					f.FileSize = EmbeddedFileSize;
					f.Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
					f.Key = Key;
					f.UnknownFileSize = unknownFileSize;
					f.SetIsBinaryData();
				}, onPreSerialize, onPostSerialize);
				if (!flags.HasFlag(LOA_Loader.ReferenceFlags.DontCache)) {
					loader.Cache[Key] = Value;
				}
			}
			return this;
		}


		public void SerializeFile(SerializerObject s, Action<Jade_File> configureAction,
			Action<SerializerObject, T> onPreSerialize = null,
			Action<SerializerObject, T> onPostSerialize = null) {
			Value = s.SerializeObject<T>(Value, onPreSerialize: f => {
				configureAction(f); onPreSerialize?.Invoke(s, f);
			}, name: nameof(Value));
			onPostSerialize?.Invoke(s, Value);
		}

		public string ShortLog => Key.ToString();

		public override string ToString() {
			return $"Reference<{typeof(T)}({Key})>";
		}
	}
}
