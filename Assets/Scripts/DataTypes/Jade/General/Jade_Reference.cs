using R1Engine.Serialize;
using System;

namespace R1Engine.Jade {
	public class Jade_Reference<T> : R1Serializable where T : Jade_File, new() {
		public Jade_Key Key { get; set; }
		public T Value { get; set; }
		public bool IsNull => Key.IsNull;

		public override void SerializeImpl(SerializerObject s) {
			Key = s.SerializeObject<Jade_Key>(Key, name: nameof(Key));
		}

		public Jade_Reference() { }
		public Jade_Reference(Context c, Jade_Key key) {
			Context = c;
			Key = key;
		}

		public void Resolve(Action<SerializerObject, T> onPreSerialize = null, Action<SerializerObject, T> onPostSerialize = null, bool immediate = false, LOA_Loader.QueueType queue = LOA_Loader.QueueType.Current) {
			if (IsNull) return;
			LOA_Loader loader = Context.GetStoredObject<LOA_Loader>("loader");
			loader.RequestFile(Key, (s, configureAction) => {
				Value = s.SerializeObject<T>(Value, onPreSerialize: f => {
					configureAction(f); onPreSerialize?.Invoke(s, f);
				}, name: nameof(Value));
				onPostSerialize?.Invoke(s, Value);
			}, (f) => {
				Value = (T)f;
			}, immediate: immediate,
			queue: queue);
		}
	}
}
