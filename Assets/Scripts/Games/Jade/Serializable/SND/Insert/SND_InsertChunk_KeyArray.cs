using BinarySerializer;
using System.Text;
using UnityEngine;

namespace Ray1Map.Jade {
	public class SND_InsertChunk_KeyArray : SND_InsertChunk {
		public uint Count { get; set; }
		public Key[] Keys { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Count = s.Serialize<uint>(Count, name: nameof(Count));
			Keys = s.SerializeObjectArray<Key>(Keys, Count, name: nameof(Keys));
		}

		public class Key : BinarySerializable {
			public float TX { get; set; }
			public float TY { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				TX = s.Serialize<float>(TX, name: nameof(TX));
				TY = s.Serialize<float>(TY, name: nameof(TY));
			}
		}
	}
}
