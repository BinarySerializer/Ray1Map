using BinarySerializer;
using System.Text;
using UnityEngine;

namespace Ray1Map.Jade {
	public class SND_InsertChunk_XAxisVar : SND_InsertChunk {
		public uint VarId { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			VarId = s.Serialize<uint>(VarId, name: nameof(VarId));
		}
	}
}
