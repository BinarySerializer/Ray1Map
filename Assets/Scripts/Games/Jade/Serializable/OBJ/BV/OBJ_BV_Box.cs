using BinarySerializer;

namespace Ray1Map.Jade {
	public class OBJ_BV_Box : BinarySerializable {
		public Jade_Vector Min { get; set; }
		public Jade_Vector Max { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Min = s.SerializeObject<Jade_Vector>(Min, name: nameof(Min));
			Max = s.SerializeObject<Jade_Vector>(Max, name: nameof(Max));
		}
	}
}
