
using System;
using System.Collections.Generic;
using BinarySerializer;

namespace R1Engine.Jade {
	public class Jade_CubeMapReference : BinarySerializable {
		public Jade_Key Key { get; set; }
		public bool IsNull => Key.IsNull;
		public TEX_CubeMap Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Key = s.SerializeObject<Jade_Key>(Key, name: nameof(Key));
		}

		public Jade_CubeMapReference() { }
		public Jade_CubeMapReference(Context c, Jade_Key key) {
			Context = c;
			Key = key;
		}

		// Dummy resolve method for now
		public Jade_CubeMapReference Resolve() {
			if (IsNull) return this;
			TEX_GlobalList lists = Context.GetStoredObject<TEX_GlobalList>(Jade_BaseManager.TextureListKey);
			lists.AddCubeMap(this);
			return this;
		}

		public Jade_CubeMapReference Load(
			Action<SerializerObject, TEX_CubeMap> onPreSerialize = null,
			Action<SerializerObject, TEX_CubeMap> onPostSerialize = null) {
			if (IsNull) return this;
			LOA_Loader loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			loader.RequestFile(Key, (s, configureAction) => {
				Value = s.SerializeObject<TEX_CubeMap>(Value, onPreSerialize: f => {
					configureAction(f); onPreSerialize?.Invoke(s, f);
				}, name: nameof(Value));
				onPostSerialize?.Invoke(s, Value);
			}, (f) => {
				Value = f?.ConvertType<TEX_CubeMap>();
			}, immediate: false,
			queue: LOA_Loader.QueueType.Textures,
			name: typeof(TEX_CubeMap).Name,
			flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.DontCache);
			return this;
		}

		public override bool IsShortLog => true;
		public override string ShortLog => Key.ToString();
	}
}
