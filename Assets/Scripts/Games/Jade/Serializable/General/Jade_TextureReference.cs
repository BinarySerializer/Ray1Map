
using System;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class Jade_TextureReference : BinarySerializable {
		public Jade_Key Key { get; set; }
		public bool IsNull => Key.IsNull;
		public TEX_File Info { get; set; }
		public TEX_File Content { get; set; }

		public bool RRR2_Bool { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Key = s.SerializeObject<Jade_Key>(Key, name: nameof(Key));
		}

		public Jade_TextureReference() { }
		public Jade_TextureReference(Context c, Jade_Key key) {
			Context = c;
			Key = key;
		}

		// Dummy resolve method for now
		public Jade_TextureReference Resolve(SerializerObject s = null, bool RRR2_readBool = false) {
			if (IsNull) return this;
			TEX_GlobalList lists = Context.GetStoredObject<TEX_GlobalList>(Jade_BaseManager.TextureListKey);
			if (Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRR2) && RRR2_readBool && s != null) {
				if (!lists.ContainsTexture(this)) {
					RRR2_Bool = s.Serialize<bool>(RRR2_Bool, name: nameof(RRR2_Bool));
					if (!RRR2_Bool) return this;
				}
			}
			lists.AddTexture(this);
			LOA_Loader loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			if (!loader.IsBinaryData) LoadEditor();
			return this;
		}

		public Jade_TextureReference LoadEditor(
			Action<SerializerObject, TEX_File> onPreSerialize = null,
			Action<SerializerObject, TEX_File> onPostSerialize = null) {
			if (IsNull) return this;
			LOA_Loader loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			if (Info != null && Info.ContentKey != Key) {
				loader.RequestFile(Key, Info, (s, configureAction) => {
					Info = s.SerializeObject<TEX_File>(Info, onPreSerialize: f => {
						configureAction(f); f.IsContent = false; onPreSerialize?.Invoke(s, f);
					}, name: nameof(Info));
					onPostSerialize?.Invoke(s, Info);
				}, (f) => {
					Info = f?.ConvertType<TEX_File>();
				}, immediate: false,
				queue: LOA_Loader.QueueType.Current,
				cache: LOA_Loader.CacheType.TextureInfo,
				flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.Montreal_AllowSkip,
				name: nameof(TEX_File));

				loader.RequestFile(Info.ContentKey, Content, (s, configureAction) => {
					Content = s.SerializeObject<TEX_File>(Content, onPreSerialize: f => {
						configureAction(f); f.IsContent = true; onPreSerialize?.Invoke(s, f);
					}, name: nameof(Content));
					onPostSerialize?.Invoke(s, Content);
				}, (f) => {
					Content = f?.ConvertType<TEX_File>();
				}, immediate: false,
				queue: LOA_Loader.QueueType.Current,
				cache: LOA_Loader.CacheType.TextureContent,
				flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.Montreal_AllowSkip,
				name: nameof(TEX_File));
			} else {
				loader.RequestFile(Key, Content ?? Info, (s, configureAction) => {
					if (Content == null) Content = Info;
					Content = s.SerializeObject<TEX_File>(Content, onPreSerialize: f => {
						configureAction(f); f.IsContent = true; onPreSerialize?.Invoke(s, f);
					}, name: nameof(Content));
					Info = Content;
					onPostSerialize?.Invoke(s, Content);
				}, (f) => {
					Content = f?.ConvertType<TEX_File>();
					Info = Content;
				}, immediate: false,
				queue: LOA_Loader.QueueType.Current,
				cache: LOA_Loader.CacheType.TextureInfo,
				flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.Montreal_AllowSkip,
				name: nameof(TEX_File));
			}
			return this;
		}

		public Jade_TextureReference LoadInfo(
			Action<SerializerObject, TEX_File> onPreSerialize = null,
			Action<SerializerObject, TEX_File> onPostSerialize = null) {
			if (IsNull) return this;
			LOA_Loader loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			loader.RequestFile(Key, Info, (s, configureAction) => {
				if (s is BinarySerializer.BinarySerializer && Info == null) return;
				Info = s.SerializeObject<TEX_File>(Info, onPreSerialize: f => {
					configureAction(f); f.IsContent = false; onPreSerialize?.Invoke(s, f);
				}, name: nameof(Info));
				onPostSerialize?.Invoke(s, Info);
			}, (f) => {
				Info = f?.ConvertType<TEX_File>();
			}, immediate: false,
			queue: LOA_Loader.QueueType.Current,
			cache: LOA_Loader.CacheType.TextureInfo,
			flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.Montreal_AllowSkip,
			name: nameof(TEX_File));
			return this;
		}

		public Jade_TextureReference LoadContent(
			Action<SerializerObject, TEX_File> onPreSerialize = null,
			Action<SerializerObject, TEX_File> onPostSerialize = null) {
			if (IsNull) return this;
			if (Info == null) return this;
			if (!Info.HasContent) return this;

			LOA_Loader loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			loader.RequestFile(Key, Content, (s, configureAction) => {
				if (s is BinarySerializer.BinarySerializer && Content == null) return;
				Content = s.SerializeObject<TEX_File>(Content, onPreSerialize: f => {
					f.Info = Info;
					configureAction(f); f.IsContent = true; onPreSerialize?.Invoke(s, f);
				}, name: nameof(Content));
				onPostSerialize?.Invoke(s, Content);
			}, (f) => {
				Content = f?.ConvertType<TEX_File>(); ;
			}, immediate: false,
			queue: LOA_Loader.QueueType.Current,
			cache: LOA_Loader.CacheType.TextureContent,
			flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.Montreal_AllowSkip,
			name: nameof(TEX_File));
			return this;
		}

		public override bool UseShortLog => true;
		public override string ShortLog => RRR2_Bool ? $"{Key},{RRR2_Bool}" : Key.ToString();
	}
}
