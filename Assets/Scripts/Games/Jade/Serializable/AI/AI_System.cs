using BinarySerializer;
using System.Linq;

namespace Ray1Map.Jade {
	public class AI_System : Jade_File {
		public override bool HasHeaderBFFile => true;

		protected override void SerializeFile(SerializerObject s) {
			s?.SystemLogger?.LogWarning($"{Offset}: TODO: Implement AI_System");
		}
	}
}
