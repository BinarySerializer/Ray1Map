using System;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierCharacterFXRef : MDF_Modifier {
		public uint Version { get; set; }
		public Jade_Reference<OBJ_GameObject> Source { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			Source = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Source, name: nameof(Source))?.Resolve();
		}
	}
}
