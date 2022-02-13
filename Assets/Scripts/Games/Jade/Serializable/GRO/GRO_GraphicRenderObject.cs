using BinarySerializer;

namespace Ray1Map.Jade {
	public abstract class GRO_GraphicRenderObject : BinarySerializable {
		public GRO_Struct GRO { get; set; }
		public uint ObjectVersion => GRO.ObjectVersion;
	}
}
