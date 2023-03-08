using BinarySerializer;
using System.Text;
using UnityEngine;

namespace Ray1Map.Jade {
	public class SND_InsertChunk_XAxisDistance : SND_InsertChunk {
		public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
		public uint Axis { get; set; }
		public float Min { get; set; }
		public float Max { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject));
			Axis = s.Serialize<uint>(Axis, name: nameof(Axis));
			Min = s.Serialize<float>(Min, name: nameof(Min));
			Max = s.Serialize<float>(Max, name: nameof(Max));
		}
	}
}
