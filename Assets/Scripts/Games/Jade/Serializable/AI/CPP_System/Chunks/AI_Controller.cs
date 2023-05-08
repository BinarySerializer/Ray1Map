using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_Controller : AI_Chunk {

		public override void SerializeImpl(SerializerObject s) {
			if(Version >= 5) // Don't serialize base chunk otherwise
				base.SerializeImpl(s);
		}
	}
}
