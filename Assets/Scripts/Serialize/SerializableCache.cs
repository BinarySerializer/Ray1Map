using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;

namespace R1Engine.Serialize {
	public class SerializableCache {
		public Dictionary<Type, Dictionary<Pointer, R1Serializable>> structs = new Dictionary<Type, Dictionary<Pointer, R1Serializable>>();

		public T FromOffset<T>(Pointer pointer) where T : R1Serializable {
			if (pointer == null) return null;
			Type type = typeof(T);
			if (!structs.ContainsKey(type) || !structs[type].ContainsKey(pointer)) return null;
			return structs[type][pointer] as T;
		}

		public void Add<T>(T serializable) where T : R1Serializable {
			Pointer pointer = serializable.Offset;
			System.Type type = typeof(T);
			if (!structs.ContainsKey(type)) {
				structs[type] = new Dictionary<Pointer, R1Serializable>();
			}
			if (!structs[type].ContainsKey(pointer)) {
				if (serializable.Size != 0) {
					structs[type][pointer] = serializable;
				}
			} else {
				UnityEngine.Debug.LogWarning("Duplicate pointer " + pointer + " for type " + type);
			}
		}
	}
}
