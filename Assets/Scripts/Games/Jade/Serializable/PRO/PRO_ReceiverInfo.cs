using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in PRO_p_TextureProjector_CreateFromBuffer
	public class PRO_ReceiverInfo : BinarySerializable {
		public PRO_TextureProjector Pre_TextureProjector { get; set; }

		public int IsExcluded { get; set; } // Boolean
		public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
		public GEO_CPP_IndexBuffer Indices { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			IsExcluded = s.Serialize<int>(IsExcluded, name: nameof(IsExcluded));
			GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject));
			if (IsExcluded != 0 && !GameObject.IsNull) {
				if(Pre_TextureProjector.ObjectVersion >= 2 && Pre_TextureProjector.IsStatic != 0)
					Indices = s.SerializeObject<GEO_CPP_IndexBuffer>(Indices, name: nameof(Indices));
				GameObject?.Resolve();
			}
		}
	}
}
