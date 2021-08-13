
using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class Jade_GenericReference : BinarySerializable {
		public Jade_Key Key { get; set; }
		public Jade_FileType FileType { get; set; }
		public Jade_File Value { get; set; }
		public Jade_FileType.FileType Type => FileType.Type;
		public bool IsNull => Type == Jade_FileType.FileType.None || Key.IsNull;
		
		public uint EmbeddedFileSize { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Key = s.SerializeObject<Jade_Key>(Key, name: nameof(Key));
			FileType = s.SerializeObject<Jade_FileType>(FileType, name: nameof(FileType));
		}

		public Jade_GenericReference() { }
		public Jade_GenericReference(Context c, Jade_Key key, Jade_FileType fileType) {
			Context = c;
			Key = key;
			FileType = fileType;
		}

		public Jade_GenericReference Resolve(
			Action<SerializerObject, Jade_File> onPreSerialize = null,
			Action<SerializerObject, Jade_File> onPostSerialize = null,
			bool immediate = false,
			LOA_Loader.QueueType queue = LOA_Loader.QueueType.Current,
			LOA_Loader.CacheType cache = LOA_Loader.CacheType.Current,
			LOA_Loader.ReferenceFlags flags = LOA_Loader.ReferenceFlags.Log) {

			if (IsNull) return this;
			LOA_Loader loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			loader.RequestFile(Key, Value, (s, configureAction) => {
				SerializeFile(s, configureAction, onPreSerialize, onPostSerialize);
			}, (f) => {
				Value = f;
			}, immediate: immediate,
			queue: queue,
			name: Type.ToString(),
			cache: cache,
			flags: flags);
			return this;
		}

		public Jade_GenericReference ResolveEmbedded(SerializerObject s,
			Action<SerializerObject, Jade_File> onPreSerialize = null,
			Action<SerializerObject, Jade_File> onPostSerialize = null,
			LOA_Loader.ReferenceFlags flags = LOA_Loader.ReferenceFlags.Log,
			bool unknownFileSize = false) {
			if (IsNull) return this;
			LOA_Loader loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			if (loader.Cache.ContainsKey(Key) && !flags.HasFlag(LOA_Loader.ReferenceFlags.DontUseCachedFile)) {
				Value = loader.Cache[Key];
			} else {
				if (!unknownFileSize) EmbeddedFileSize = s.Serialize<uint>(EmbeddedFileSize, name: nameof(EmbeddedFileSize));
				SerializeFile(s, f => {
					f.FileSize = EmbeddedFileSize;
					f.Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
					f.Key = Key;
					f.UnknownFileSize = unknownFileSize;
					f.SetEditorMode();
				}, onPreSerialize, onPostSerialize);
				if (!flags.HasFlag(LOA_Loader.ReferenceFlags.DontCache)) {
					loader.Cache[Key] = Value;
				}
			}
			return this;
		}

		public void SerializeFile(SerializerObject s, Action<Jade_File> configureAction,
			Action<SerializerObject, Jade_File> onPreSerialize = null,
			Action<SerializerObject, Jade_File> onPostSerialize = null) {
			switch (Type) {
				case Jade_FileType.FileType.AI_Instance:
					Value = s.SerializeObject<AI_Instance>((AI_Instance)Value, onPreSerialize: f => {
						configureAction(f); onPreSerialize?.Invoke(s, f);
					}, name: nameof(Value));
					break;
				case Jade_FileType.FileType.AI_Model:
					Value = s.SerializeObject<AI_Model>((AI_Model)Value, onPreSerialize: f => {
						configureAction(f); onPreSerialize?.Invoke(s, f);
					}, name: nameof(Value));
					break;
				case Jade_FileType.FileType.AI_Vars:
					Value = s.SerializeObject<AI_Vars>((AI_Vars)Value, onPreSerialize: f => {
						configureAction(f); onPreSerialize?.Invoke(s, f);
					}, name: nameof(Value));
					break;
				case Jade_FileType.FileType.AI_Function:
					Value = s.SerializeObject<AI_Function>((AI_Function)Value, onPreSerialize: f => {
						configureAction(f); onPreSerialize?.Invoke(s, f);
					}, name: nameof(Value));
					break;
				case Jade_FileType.FileType.AI_ProcList:
					Value = s.SerializeObject<AI_ProcList>((AI_ProcList)Value, onPreSerialize: f => {
						configureAction(f); onPreSerialize?.Invoke(s, f);
					}, name: nameof(Value));
					break;
				case Jade_FileType.FileType.AI_TT:
					Value = s.SerializeObject<AI_TT>((AI_TT)Value, onPreSerialize: f => {
						configureAction(f); onPreSerialize?.Invoke(s, f);
					}, name: nameof(Value));
					break;
				case Jade_FileType.FileType.WOR_WorldList:
					Value = s.SerializeObject<WOR_WorldList>((WOR_WorldList)Value, onPreSerialize: f => {
						configureAction(f); onPreSerialize?.Invoke(s, f);
					}, name: nameof(Value));
					break;
				case Jade_FileType.FileType.WOR_World:
					Value = s.SerializeObject<WOR_World>((WOR_World)Value, onPreSerialize: f => {
						configureAction(f); onPreSerialize?.Invoke(s, f);
					}, name: nameof(Value));
					break;
				case Jade_FileType.FileType.SND_Ambience:
				case Jade_FileType.FileType.SND_Dialog:
				case Jade_FileType.FileType.SND_LoadingSound:
				case Jade_FileType.FileType.SND_Music:
				case Jade_FileType.FileType.SND_Sound:
					var soundType = Type switch
					{
						Jade_FileType.FileType.SND_Ambience => SND_Wave.Type.Ambience,
						Jade_FileType.FileType.SND_Dialog => SND_Wave.Type.Dialog,
						Jade_FileType.FileType.SND_LoadingSound => SND_Wave.Type.LoadingSound,
						Jade_FileType.FileType.SND_Music => SND_Wave.Type.Music,
						Jade_FileType.FileType.SND_Sound => SND_Wave.Type.Sound,
						_ => SND_Wave.Type.Sound
					};
					Value = s.SerializeObject<SND_Wave>((SND_Wave)Value, onPreSerialize: f => {
						f.SoundType = soundType; configureAction(f); onPreSerialize?.Invoke(s, f);
					}, name: nameof(Value));
					break;
				case Jade_FileType.FileType.SND_SModifier:
					Value = s.SerializeObject<SND_SModifier>((SND_SModifier)Value, onPreSerialize: f => {
						configureAction(f); onPreSerialize?.Invoke(s, f);
					}, name: nameof(Value));
					break;
				case Jade_FileType.FileType.GRID_WorldGrid:
					Value = s.SerializeObject<GRID_WorldGrid>((GRID_WorldGrid)Value, onPreSerialize: f => {
						configureAction(f); onPreSerialize?.Invoke(s, f);
					}, name: nameof(Value));
					break;
				case Jade_FileType.FileType.TEXT_TextGroup:
					Value = s.SerializeObject<TEXT_TextGroup>((TEXT_TextGroup)Value, onPreSerialize: f => {
						configureAction(f); onPreSerialize?.Invoke(s, f);
					}, name: nameof(Value));
					break;
				case Jade_FileType.FileType.TEXT_TextList:
					Value = s.SerializeObject<TEXT_TextList>((TEXT_TextList)Value, onPreSerialize: f => {
						configureAction(f); onPreSerialize?.Invoke(s, f);
					}, name: nameof(Value));
					break;
				case Jade_FileType.FileType.OBJ_GameObject:
					Value = s.SerializeObject<OBJ_GameObject>((OBJ_GameObject)Value, onPreSerialize: f => {
						configureAction(f); onPreSerialize?.Invoke(s, f);
					}, name: nameof(Value));
					break;
				default:
					throw new NotImplementedException($"GenericReference: Could not resolve key {Key} of type {Type} ({FileType.Extension})");
			}
			onPostSerialize?.Invoke(s, Value);
		}

		public override bool IsShortLog => true;
		public override string ShortLog => $"GenericReference({Key}{FileType.Extension} - {Type})";
		public override string ToString() {
			return ShortLog;
		}
	}
}
