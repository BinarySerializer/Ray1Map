using BinarySerializer;

namespace Ray1Map.Jade {
	public class GRID_GridGroup : Jade_File {
		public Jade_GenericReference[] References { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			References = s.SerializeObjectArray<Jade_GenericReference>(References, FileSize / 8, name: nameof(References));
			foreach (var reference in References) {
				reference?.Resolve();
			}
		}
	}
}
