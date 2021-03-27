using R1Engine.Serialize;
using System;

namespace R1Engine.Jade {
	public class Jade_GenericReference : R1Serializable {
		public Jade_Key Key { get; set; }
		public Jade_FileType FileType { get; set; }
		public Jade_File Value { get; set; }
		public Jade_FileType.FileType Type => FileType.Type;
		public bool IsNull => Type == Jade_FileType.FileType.None || Key.IsNull;

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

		public void Resolve(Action<SerializerObject, Jade_File> onPreSerialize = null, Action<SerializerObject, Jade_File> onPostSerialize = null, bool immediate = false, LOA_Loader.QueueType queue = LOA_Loader.QueueType.Current) {
			if (IsNull) return;
			LOA_Loader loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			loader.RequestFile(Key, (s, configureAction) => {
				switch(Type) {
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
				}
				onPostSerialize?.Invoke(s, Value);
			}, (f) => {
				Value = f;
			}, immediate: immediate,
			queue: queue);
		}
	}
}
