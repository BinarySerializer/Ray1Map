
using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class Jade_TextureReference : BinarySerializable {
		public Jade_Key Key { get; set; }
		public bool IsNull => Key.IsNull;
		public TEX_File Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Key = s.SerializeObject<Jade_Key>(Key, name: nameof(Key));
		}

		public Jade_TextureReference() { }
		public Jade_TextureReference(Context c, Jade_Key key) {
			Context = c;
			Key = key;
		}

		// Dummy resolve method for now
		public Jade_TextureReference Resolve(
			Action<SerializerObject, TEX_File> onPreSerialize = null,
			Action<SerializerObject, TEX_File> onPostSerialize = null,
			bool immediate = false,
			LOA_Loader.QueueType queue = LOA_Loader.QueueType.Textures,
			LOA_Loader.ReferenceFlags flags = LOA_Loader.ReferenceFlags.Log) {

			if (IsNull) return this;
			LOA_Loader loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			loader.RequestFile(Key, (s, configureAction) => {
				Value = s.SerializeObject<TEX_File>(Value, onPreSerialize: f => {
					configureAction(f); onPreSerialize?.Invoke(s, f);
				}, name: nameof(Value));
				onPostSerialize?.Invoke(s, Value);
			}, (f) => {
				Value = (TEX_File)f;
			}, immediate: immediate,
			queue: queue,
			name: typeof(TEX_File).Name,
			flags: flags);
			return this;
		}
	}
}
