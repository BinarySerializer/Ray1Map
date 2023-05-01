using BinarySerializer;
using System.Linq;

namespace Ray1Map.Jade {
	public class AI_System : Jade_File {
		public override bool HasHeaderBFFile => true;

		public int Version { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			Version = s.Serialize<int>(Version, name: nameof(Version));

			s?.SystemLogger?.LogWarning($"{Offset}: TODO: Implement AI_System");
		}
	}
}
